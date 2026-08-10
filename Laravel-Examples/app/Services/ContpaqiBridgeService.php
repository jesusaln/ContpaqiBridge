<?php
/**
 * Cliente HTTP para consumir el ContpaqiBridge API.
 * Colocar en: app/Services/ContpaqiBridgeService.php
 */

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Cache;

class ContpaqiBridgeService
{
    protected string $baseUrl;
    protected string $apiKey;
    protected string $empresaPath;
    protected int $timeout = 30;

    public function __construct()
    {
        $this->baseUrl = rtrim(config('contpaqi.bridge_url'), '/');
        $this->apiKey = config('contpaqi.api_key');
        $this->empresaPath = config('contpaqi.empresa_path');
    }

    /**
     * Request genérico al bridge.
     */
    protected function request(string $method, string $endpoint, array $payload = [], array $query = [])
    {
        $url = $this->baseUrl . $endpoint;
        $headers = ['X-Api-Key' => $this->apiKey, 'Accept' => 'application/json'];

        $request = Http::withHeaders($headers)->timeout($this->timeout);

        if (in_array(strtoupper($method), ['GET', 'HEAD'])) {
            $response = $request->get($url, array_merge($query, $payload));
        } else {
            $response = $request->{$method}($url, $payload);
        }

        if (!$response->successful()) {
            Log::error("ContpaqiBridge {$method} {$endpoint} falló", [
                'status' => $response->status(),
                'body' => $response->body(),
                'payload' => $payload
            ]);
        }

        return $response->json() ?? ['success' => false, 'message' => 'Respuesta inválida del bridge'];
    }

    // ====================================================================
    // CLIENTES
    // ====================================================================

    public function getClientes(bool $fullSnapshot = false, ?\DateTime $desde = null, int $limite = 500): array
    {
        if ($fullSnapshot || $desde === null) {
            return $this->request('GET', '/api/Sync/clientes', [], [
                'rutaEmpresa' => $this->empresaPath,
                'limite' => $limite
            ]);
        }
        return $this->request('GET', '/api/Sync/clientes/modificados', [], [
            'rutaEmpresa' => $this->empresaPath,
            'desde' => $desde->format('Y-m-d\TH:i:s'),
            'limite' => $limite
        ]);
    }

    public function pushClientes(array $clientes): array
    {
        return $this->request('POST', '/api/Sync/clientes/batch', [
            'rutaEmpresa' => $this->empresaPath,
            'items' => $clientes
        ]);
    }

    public function buscarCliente(string $codigo): ?array
    {
        $r = $this->request('GET', "/api/Clientes/{$codigo}", [], [
            'rutaEmpresa' => $this->empresaPath
        ]);
        return ($r['success'] ?? false) ? $r['cliente'] : null;
    }

    // ====================================================================
    // PRODUCTOS
    // ====================================================================

    public function getProductos(bool $fullSnapshot = false, ?\DateTime $desde = null, int $limite = 500): array
    {
        if ($fullSnapshot || $desde === null) {
            return $this->request('GET', '/api/Sync/productos', [], [
                'rutaEmpresa' => $this->empresaPath,
                'limite' => $limite
            ]);
        }
        return $this->request('GET', '/api/Sync/productos/modificados', [], [
            'rutaEmpresa' => $this->empresaPath,
            'desde' => $desde->format('Y-m-d\TH:i:s'),
            'limite' => $limite
        ]);
    }

    public function pushProductos(array $productos): array
    {
        return $this->request('POST', '/api/Sync/productos/batch', [
            'rutaEmpresa' => $this->empresaPath,
            'items' => $productos
        ]);
    }

    public function buscarProducto(string $codigo): ?array
    {
        $r = $this->request('GET', "/api/Productos/{$codigo}", [], [
            'rutaEmpresa' => $this->empresaPath
        ]);
        return ($r['success'] ?? false) ? $r['producto'] : null;
    }

    // ====================================================================
    // DOCUMENTOS (FACTURAS)
    // ====================================================================

    /**
     * Obtiene documentos (facturas/notas) modificados en CONTPAQi desde una fecha.
     * Este es el método principal para sincronizar ventas desde CONTPAQi hacia Laravel.
     */
    public function getDocumentosModificados(\DateTime $desde, int $limite = 500): array
    {
        return $this->request('GET', '/api/Sync/documentos/modificados', [], [
            'rutaEmpresa' => $this->empresaPath,
            'desde' => $desde->format('Y-m-d\TH:i:s'),
            'limite' => $limite
        ]);
    }

    // ====================================================================
    // REPORTES
    // ====================================================================

    public function reporteVentas(\DateTime $desde, \DateTime $hasta): array
    {
        return $this->request('GET', '/api/Reportes/ventas', [], [
            'rutaEmpresa' => $this->empresaPath,
            'desde' => $desde->format('Y-m-d'),
            'hasta' => $hasta->format('Y-m-d')
        ]);
    }

    public function topClientes(\DateTime $desde, \DateTime $hasta, int $top = 10): array
    {
        return $this->request('GET', '/api/Reportes/top-clientes', [], [
            'rutaEmpresa' => $this->empresaPath,
            'desde' => $desde->format('Y-m-d'),
            'hasta' => $hasta->format('Y-m-d'),
            'top' => $top
        ]);
    }

    // ====================================================================
    // WEBHOOKS
    // ====================================================================

    public function registrarWebhook(string $evento, string $url): array
    {
        return $this->request('POST', '/api/Webhooks', [
            'evento' => $evento,
            'url' => $url
        ]);
    }

    public function listarWebhooks(): array
    {
        return $this->request('GET', '/api/Webhooks');
    }

    // ====================================================================
    // FACTURACIÓN
    // ====================================================================

    public function facturar(array $data): array
    {
        $data['rutaEmpresa'] = $this->empresaPath;
        return $this->request('POST', '/api/Integracion/flujo-completo', $data);
    }

    public function timbrar(string $concepto, string $serie, float $folio, string $passCSD): array
    {
        return $this->request('POST', '/api/Documentos/timbrar', [
            'rutaEmpresa' => $this->empresaPath,
            'codigoConcepto' => $concepto,
            'serie' => $serie,
            'folio' => $folio,
            'passCSD' => $passCSD
        ]);
    }
}