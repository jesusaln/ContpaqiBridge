<?php
/**
 * ContpaqiBridge Laravel - Sync Service
 *
 * Sincroniza clientes y productos de la BD local de Laravel hacia CONTPAQi Comercial
 * a través del bridge. Antes de facturar, asegura que el cliente y los productos
 * existan en CONTPAQi.
 *
 * Ubicación: app/Services/ContpaqiSyncService.php
 *
 * Flujo:
 *   $sync = app(ContpaqiSyncService::class);
 *   $sync->asegurarCliente($clienteModel);
 *   $sync->asegurarProducto($productoModel);
 *
 * Para usar antes de facturar, el Facturador ya llama a estos métodos
 * automáticamente si le pasas el modelo Eloquent en vez de un array.
 */

namespace App\Services;

use App\Models\Cliente;
use App\Models\Producto;
use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;

class ContpaqiSyncService
{
    public function __construct(
        protected string $baseUrl,
        protected string $apiKey,
        protected string $rutaEmpresa,
    ) {}

    public static function fromConfig(): self
    {
        return new self(
            (string) config('contpaqi.base_url', env('CONTPAQI_BRIDGE_URL')),
            (string) config('contpaqi.api_key', env('CONTPAQI_BRIDGE_KEY')),
            (string) config('contpaqi.ruta_empresa', env('CONTPAQI_RUTA_EMPRESA', 'C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA')),
        );
    }

    /**
     * Asegura que el cliente exista en CONTPAQi. Si no existe, el bridge lo crea
     * con los datos fiscales del modelo de Laravel.
     *
     * Acepta:
     *   - App\Models\Cliente (Eloquent)
     *   - array con keys: codigo, nombre, rfc, email?, cp?, regimen_fiscal?
     *
     * @param Cliente|array $cliente
     * @return array{ok: bool, mensaje: string, codigo: string}
     */
    public function asegurarCliente(Cliente|array $cliente): array
    {
        $datos = $cliente instanceof Cliente
            ? $this->clienteModelToArray($cliente)
            : $cliente;

        // Validar mínimos
        if (empty($datos['codigo'])) {
            return ['ok' => false, 'mensaje' => 'Falta codigo del cliente', 'codigo' => ''];
        }
        if (empty($datos['nombre'])) {
            return ['ok' => false, 'mensaje' => 'Falta nombre del cliente', 'codigo' => $datos['codigo']];
        }
        if (empty($datos['rfc'])) {
            return ['ok' => false, 'mensaje' => 'Falta RFC del cliente', 'codigo' => $datos['codigo']];
        }

        $codigo = $datos['codigo'];

        // Verificar primero si ya existe en CONTPAQi (cacheable en el futuro)
        $existente = $this->obtenerCliente($codigo);
        if ($existente && !empty($existente['contpaqi_id'])) {
            Log::info("Contpaqi: cliente {$codigo} ya existe (ID {$existente['contpaqi_id']})");
            return [
                'ok' => true,
                'mensaje' => 'Cliente ya existe en CONTPAQi',
                'codigo' => $codigo,
                'id' => $existente['contpaqi_id'],
                'nuevo' => false,
            ];
        }

        // No existe: crearlo via bridge (el bridge lo crea si no existe)
        // Pero como el bridge auto-crea dentro de CrearFactura, también podemos
        // hacerlo explícitamente via CrearCliente del bridge para que exista
        // antes de facturar.
        Log::info("Contpaqi: creando cliente {$codigo} ({$datos['nombre']})");
        $resp = $this->call('POST', '/api/Clientes', [
            'RutaEmpresa'    => $this->rutaEmpresa,
            'Codigo'         => $codigo,
            'RazonSocial'    => $datos['nombre'],
            'RFC'            => $datos['rfc'],
            'Email'          => $datos['email'] ?? '',
            'RegimenFiscal'  => $datos['regimen_fiscal'] ?? '',
            'UsoCFDI'        => $datos['uso_cfdi'] ?? '',
            'Calle'          => $datos['calle'] ?? '',
            'Colonia'        => $datos['colonia'] ?? '',
            'CodigoPostal'   => $datos['codigo_postal'] ?? '',
            'Ciudad'         => $datos['ciudad'] ?? '',
            'Estado'         => $datos['estado'] ?? '',
            'Pais'           => $datos['pais'] ?? 'México',
        ]);

        if (!$resp['ok']) {
            Log::warning("Contpaqi: no se pudo crear cliente via API, el bridge lo hara al facturar: {$resp['error']}");
            // No es fatal: el bridge auto-crea el cliente al recibir CrearFactura
            return ['ok' => true, 'mensaje' => 'Cliente se creara al facturar (auto-create)', 'codigo' => $codigo, 'nuevo' => null];
        }

        return [
            'ok' => true,
            'mensaje' => 'Cliente creado en CONTPAQi',
            'codigo' => $codigo,
            'id' => $resp['data']['idCliente'] ?? null,
            'nuevo' => true,
        ];
    }

    /**
     * Asegura que el producto exista en CONTPAQi.
     *
     * @param Producto|array $producto
     * @return array{ok: bool, mensaje: string, codigo: string}
     */
    public function asegurarProducto(Producto|array $producto): array
    {
        $datos = $producto instanceof Producto
            ? $this->productoModelToArray($producto)
            : $producto;

        if (empty($datos['codigo'])) {
            return ['ok' => false, 'mensaje' => 'Falta codigo del producto', 'codigo' => ''];
        }
        if (empty($datos['nombre'])) {
            return ['ok' => false, 'mensaje' => 'Falta nombre del producto', 'codigo' => $datos['codigo']];
        }

        $codigo = $datos['codigo'];

        $existente = $this->obtenerProducto($codigo);
        if ($existente && !empty($existente['contpaqi_id'])) {
            Log::info("Contpaqi: producto {$codigo} ya existe (ID {$existente['contpaqi_id']})");
            return [
                'ok' => true,
                'mensaje' => 'Producto ya existe en CONTPAQi',
                'codigo' => $codigo,
                'id' => $existente['contpaqi_id'],
                'nuevo' => false,
            ];
        }

        Log::info("Contpaqi: creando producto {$codigo} ({$datos['nombre']})");
        $resp = $this->call('POST', '/api/Productos', [
            'RutaEmpresa'    => $this->rutaEmpresa,
            'Codigo'         => $codigo,
            'Nombre'         => $datos['nombre'],
            'Descripcion'    => $datos['descripcion'] ?? '',
            'Precio'         => $datos['precio'] ?? 0,
            'TipoProducto'   => $datos['tipo'] ?? 1,  // 1=Producto, 2=Paquete, 3=Servicio
            'UnidadMedida'   => $datos['unidad'] ?? 'H87',
            'ClaveSAT'       => $datos['clave_sat'] ?? '',
        ]);

        if (!$resp['ok']) {
            Log::warning("Contpaqi: no se pudo crear producto via API, el bridge lo hara al facturar: {$resp['error']}");
            return ['ok' => true, 'mensaje' => 'Producto se creara al facturar (auto-create)', 'codigo' => $codigo, 'nuevo' => null];
        }

        return [
            'ok' => true,
            'mensaje' => 'Producto creado en CONTPAQi',
            'codigo' => $codigo,
            'id' => $resp['data']['idProducto'] ?? null,
            'nuevo' => true,
        ];
    }

    /**
     * Convierte un modelo Cliente de Laravel a array con los campos que necesita CONTPAQi.
     * Adapta los nombres de campo a TU modelo real si difieren.
     */
    protected function clienteModelToArray(Cliente $c): array
    {
        return [
            'codigo'        => $c->codigo ?? $c->code ?? $c->id,
            'nombre'        => $c->nombre ?? $c->razon_social ?? $c->name ?? '',
            'rfc'           => $c->rfc ?? $c->RFC ?? '',
            'email'         => $c->email ?? '',
            'calle'         => $c->calle ?? $c->direccion ?? '',
            'colonia'       => $c->colonia ?? '',
            'codigo_postal' => $c->codigo_postal ?? $c->cp ?? '',
            'ciudad'        => $c->ciudad ?? $c->municipio ?? '',
            'estado'        => $c->estado ?? '',
            'pais'          => $c->pais ?? 'México',
            'regimen_fiscal'=> $c->regimen_fiscal ?? $c->regimenFiscal ?? '',
            'uso_cfdi'      => $c->uso_cfdi ?? $c->usoCFDI ?? '',
        ];
    }

    protected function productoModelToArray(Producto $p): array
    {
        return [
            'codigo'       => $p->codigo ?? $p->code ?? $p->id,
            'nombre'       => $p->nombre ?? $p->descripcion ?? $p->name ?? '',
            'descripcion'  => $p->descripcion ?? '',
            'precio'       => $p->precio ?? $p->price ?? 0,
            'unidad'       => $p->unidad ?? $p->unidad_medida ?? 'H87',
            'clave_sat'    => $p->clave_sat ?? $p->claveSAT ?? '',
            'tipo'         => $p->tipo ?? 1,
        ];
    }

    /**
     * Busca un cliente en CONTPAQi por su código.
     * Usa el endpoint GET /api/Sync/cliente?codigo=X
     */
    protected function obtenerCliente(string $codigo): ?array
    {
        $resp = $this->call('GET', '/api/Sync/cliente', [
            'rutaEmpresa' => $this->rutaEmpresa,
            'codigo'      => $codigo,
        ]);

        if (!$resp['ok'] || empty($resp['data'])) {
            return null;
        }

        return is_array($resp['data']) ? $resp['data'] : null;
    }

    protected function obtenerProducto(string $codigo): ?array
    {
        $resp = $this->call('GET', '/api/Sync/producto', [
            'rutaEmpresa' => $this->rutaEmpresa,
            'codigo'      => $codigo,
        ]);

        if (!$resp['ok'] || empty($resp['data'])) {
            return null;
        }

        return is_array($resp['data']) ? $resp['data'] : null;
    }

    protected function call(string $method, string $path, array $params = [], int $timeoutSec = 60): array
    {
        $url = rtrim($this->baseUrl, '/') . $path;
        try {
            $req = Http::withHeaders(['X-Api-Key' => $this->apiKey])
                ->timeout($timeoutSec)
                ->connectTimeout(15);

            $resp = match (strtoupper($method)) {
                'GET'  => $req->get($url, $params),
                'POST' => $req->post($url, $params),
                default => throw new \InvalidArgumentException("Metodo no soportado: $method"),
            };

            if ($resp->successful()) {
                return ['ok' => true, 'data' => $resp->json() ?? [], 'error' => null];
            }

            if ($resp->status() === 404) {
                return ['ok' => false, 'data' => null, 'error' => "No encontrado"];
            }

            return ['ok' => false, 'data' => null, 'error' => "HTTP {$resp->status()}: {$resp->body()}"];
        }
        catch (\Throwable $e) {
            return ['ok' => false, 'data' => null, 'error' => $e->getMessage()];
        }
    }
}