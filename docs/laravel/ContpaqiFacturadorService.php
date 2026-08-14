<?php
/**
 * ContpaqiBridge Laravel - Facturador (versión final con sync)
 *
 * Antes de facturar, sincroniza el cliente y los productos a CONTPAQi
 * para que existan con los datos fiscales correctos. Después crea la
 * factura y la timbra.
 *
 * Ubicación: app/Services/ContpaqiFacturadorService.php
 */

namespace App\Services;

use App\Models\Cliente;
use App\Models\Producto;
use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;

class ContpaqiFacturadorService
{
    public function __construct(
        protected ContpaqiSyncService $sync,
        protected ContpaqiBridgeClient $client,
        protected string $rutaEmpresa,
    ) {}

    public static function fromConfig(): self
    {
        $client = ContpaqiBridgeClient::fromConfig();
        $sync = new ContpaqiSyncService(
            $client->baseUrl,
            $client->apiKey,
            (string) config('contpaqi.ruta_empresa', env('CONTPAQI_RUTA_EMPRESA', 'C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA')),
        );

        return new self(
            $sync,
            $client,
            (string) config('contpaqi.ruta_empresa', env('CONTPAQI_RUTA_EMPRESA', 'C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA')),
        );
    }

    /**
     * Flujo completo end-to-end:
     *  1. Asegurar que el cliente existe en CONTPAQi (sync o crear)
     *  2. Asegurar que cada producto existe en CONTPAQi
     *  3. Crear la factura en CONTPAQi (devuelve folio)
     *  4. Timbrar la factura (devuelve UUID via webhook)
     *
     * Acepta:
     *   - Cliente y array de Productos (Eloquent)
     *   - O todo como array plano
     *
     * @return array{
     *   ok: bool,
     *   mensaje: string,
     *   serie?: string,
     *   folio?: int,
     *   id_documento?: int,
     *   uuid?: string,
     *   error?: string,
     *   cliente_sync?: array,
     *   productos_sync?: array,
     * }
     */
    public function facturarDesdeModelos(
        Cliente $cliente,
        array $productos, // [['producto' => Producto, 'cantidad' => 1, 'precio' => 100], ...]
        string $concepto = '4CLIMAS',
        array $opciones = [],  // uso_cfdi, forma_pago, metodo_pago, etc.
    ): array {
        $log = [];

        // Paso 1: Sync cliente
        Log::info("Contpaqi: sincronizando cliente {$cliente->codigo}");
        $clienteSync = $this->sync->asegurarCliente($cliente);
        $log[] = "cliente: {$clienteSync['mensaje']}";

        if (!$clienteSync['ok']) {
            return [
                'ok' => false,
                'error' => "Sync cliente fallo: {$clienteSync['mensaje']}",
                'cliente_sync' => $clienteSync,
            ];
        }

        // Paso 2: Sync productos
        $productosSync = [];
        foreach ($productos as $item) {
            $producto = $item['producto'] ?? null;
            if (!$producto instanceof Producto) {
                continue;
            }
            $sync = $this->sync->asegurarProducto($producto);
            $log[] = "producto {$producto->codigo}: {$sync['mensaje']}";
            $productosSync[$producto->codigo] = $sync;
        }

        // Paso 3: Crear la factura via POST al bridge
        $productosParaBody = array_map(fn ($item) => [
            'Codigo'       => $item['producto']->codigo,
            'Cantidad'     => $item['cantidad'] ?? 1,
            'Precio'       => $item['precio'] ?? $item['producto']->precio ?? 0,
            'Nombre'       => $item['producto']->nombre,
            'UnidadMedida' => $item['producto']->unidad ?? 'H87',
            'ClaveSAT'     => $item['producto']->clave_sat ?? '',
        ], $productos);

        $body = [
            'RutaEmpresa'    => $this->rutaEmpresa,
            'CodigoConcepto' => $concepto,
            'CodigoCliente'   => $cliente->codigo,
            'Productos'       => $productosParaBody,
            'ClienteRazonSocial' => $cliente->nombre ?? $cliente->razon_social,
            'ClienteRFC'         => $cliente->rfc,
            'ClienteRegimenFiscal' => $cliente->regimen_fiscal ?? '',
            'UsoCFDI'         => $opciones['uso_cfdi'] ?? 'G03',
            'FormaPago'       => $opciones['forma_pago'] ?? '03',
            'MetodoPago'      => $opciones['metodo_pago'] ?? 'PUE',
        ];

        $resp = $this->call('POST', '/api/Documentos/factura', $body, 90);

        if (!$resp['ok']) {
            Log::error("Contpaqi: fallo crear factura - {$resp['error']}");
            return [
                'ok' => false,
                'error' => "Fallo crear factura: {$resp['error']}",
                'cliente_sync' => $clienteSync,
                'productos_sync' => $productosSync,
            ];
        }

        $data = $resp['data'];
        if (!($data['success'] ?? false)) {
            return [
                'ok' => false,
                'error' => $data['message'] ?? 'Error al crear factura',
                'cliente_sync' => $clienteSync,
                'productos_sync' => $productosSync,
            ];
        }

        return [
            'ok' => true,
            'mensaje' => $data['message'] ?? 'Factura creada',
            'serie' => $data['serie'] ?? '',
            'folio' => $data['folio'] ?? 0,
            'id_documento' => $data['idDocumento'] ?? null,
            'cliente_sync' => $clienteSync,
            'productos_sync' => $productosSync,
            'log' => $log,
        ];
    }

    /**
     * Versión con arrays (sin modelos Eloquent) — útil para tests o para
     * sistemas donde los datos vienen de otra fuente.
     */
    public function crearYTimbrar(array $datos, ?string $passCSD = null): array
    {
        // Sync cliente si nos dan datos del cliente
        if (!empty($datos['cliente_datos'])) {
            $this->sync->asegurarCliente($datos['cliente_datos']);
        }

        // Sync productos
        foreach ($datos['productos'] ?? [] as $prod) {
            if (!empty($prod['codigo']) && !empty($prod['nombre'])) {
                $this->sync->asegurarProducto($prod);
            }
        }

        // Crear
        $body = [
            'RutaEmpresa'    => $datos['ruta_empresa'] ?? $this->rutaEmpresa,
            'CodigoConcepto' => $datos['concepto'] ?? '4CLIMAS',
            'CodigoCliente'   => $datos['cliente'],
            'Productos'       => array_map(fn ($p) => [
                'Codigo'       => $p['codigo'],
                'Cantidad'     => $p['cantidad'] ?? 1,
                'Precio'       => $p['precio'] ?? 0,
                'Nombre'       => $p['nombre'] ?? '',
                'UnidadMedida' => $p['unidad_medida'] ?? 'H87',
                'ClaveSAT'     => $p['clave_sat'] ?? '',
            ], $datos['productos']),
        ];

        if (!empty($datos['cliente_datos'])) {
            $body['ClienteRazonSocial'] = $datos['cliente_datos']['nombre'] ?? $datos['cliente_datos']['razon_social'] ?? '';
            $body['ClienteRFC'] = $datos['cliente_datos']['rfc'] ?? '';
            $body['ClienteRegimenFiscal'] = $datos['cliente_datos']['regimen_fiscal'] ?? '';
        }

        $body['UsoCFDI'] = $datos['uso_cfdi'] ?? 'G03';
        $body['FormaPago'] = $datos['forma_pago'] ?? '03';
        $body['MetodoPago'] = $datos['metodo_pago'] ?? 'PUE';

        $resp = $this->call('POST', '/api/Documentos/factura', $body, 90);
        if (!$resp['ok'] || !($resp['data']['success'] ?? false)) {
            return [
                'ok' => false,
                'error' => $resp['error'] ?? $resp['data']['message'] ?? 'Error al crear',
            ];
        }

        $resultado = [
            'ok' => true,
            'mensaje' => $resp['data']['message'] ?? '',
            'serie' => $resp['data']['serie'] ?? '',
            'folio' => $resp['data']['folio'] ?? 0,
            'id_documento' => $resp['data']['idDocumento'] ?? null,
        ];

        // Timbrar si nos dieron passCSD
        if ($passCSD) {
            $timb = $this->timbrar(
                $body['RutaEmpresa'],
                $body['CodigoConcepto'],
                $resultado['serie'],
                $resultado['folio'],
                $passCSD
            );
            $resultado['timbrado'] = $timb;
            if (!$timb['ok']) {
                $resultado['ok'] = false;
                $resultado['error'] = "Creada pero timbrado fallo: " . $timb['error'];
            }
        }

        return $resultado;
    }

    public function timbrar(string $rutaEmpresa, string $concepto, string $serie, int $folio, string $passCSD): array
    {
        $resp = $this->call('POST', '/api/Documentos/timbrar', [
            'RutaEmpresa'    => $rutaEmpresa,
            'CodigoConcepto' => $concepto,
            'Serie'          => $serie,
            'Folio'          => $folio,
            'PassCSD'        => $passCSD,
        ], 120);

        if (!$resp['ok']) {
            return ['ok' => false, 'error' => $resp['error']];
        }

        return [
            'ok' => $resp['data']['success'] ?? false,
            'mensaje' => $resp['data']['message'] ?? '',
        ];
    }

    protected function call(string $method, string $path, array $params = [], int $timeoutSec = 60): array
    {
        return $this->client->call($method, $path, $params, $timeoutSec);
    }
}