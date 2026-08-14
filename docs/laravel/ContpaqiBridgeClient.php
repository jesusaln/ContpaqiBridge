<?php
/**
 * ContpaqiBridge Laravel Sync Client
 *
 * Cliente que consume los endpoints de sync del bridge.
 * Incluye retry con backoff exponencial y logging.
 *
 * Ubicación sugerida: app/Services/ContpaqiBridgeClient.php
 *
 * Dependencias: configurar en config/contpaqi.php:
 *   return [
 *       'base_url' => env('CONTPAQI_BRIDGE_URL', 'http://192.168.191.226:5000'),
 *       'api_key'  => env('CONTPAQI_BRIDGE_KEY', 'BRIDGE_API_KEY_SECRETA_...'),
 *       'webhook_secret' => env('CONTPAQI_WEBHOOK_SECRET'), // el que generó Registrar()
 *   ];
 */

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;

class ContpaqiBridgeClient
{
    public function __construct(
        public string $baseUrl,
        public string $apiKey,
    ) {}

    public static function fromConfig(): self
    {
        return new self(
            (string) config('contpaqi.base_url'),
            (string) config('contpaqi.api_key'),
        );
    }

    /**
     * Llama un endpoint del bridge con retry + backoff exponencial.
     * $maxAttempts = 4 → delays aprox: 0s, 30s, 2min, 8min
     */
    protected function call(string $method, string $path, array $query = [], ?array $body = null): ?array
    {
        $url = rtrim($this->baseUrl, '/') . $path;
        $maxAttempts = (int) config('contpaqi.max_attempts', 4);

        $delays = [0, 30, 120, 480]; // segundos

        for ($attempt = 1; $attempt <= $maxAttempts; $attempt++) {
            $delay = $delays[$attempt - 1] ?? 480;
            if ($delay > 0) {
                Log::info("Contpaqi sync retry {$attempt} en {$delay}s: {$method} {$url}");
                sleep($delay);
            }

            try {
                $req = Http::withHeaders(['X-Api-Key' => $this->apiKey])
                    ->timeout(120)
                    ->connectTimeout(15);

                $resp = match (strtoupper($method)) {
                    'GET'    => $req->get($url, $query),
                    'POST'   => $req->post($url, $body ?? []),
                    default  => throw new \InvalidArgumentException("Método no soportado: $method"),
                };

                if ($resp->successful()) {
                    $json = $resp->json();
                    if (is_array($json)) return $json;
                    Log::warning("Contpaqi sync respuesta no-JSON: " . $resp->status());
                    return null;
                }

                if ($resp->status() === 404) {
                    Log::warning("Contpaqi sync 404 (no existe): {$url}");
                    return null;
                }

                Log::warning("Contpaqi sync HTTP {$resp->status()} intento {$attempt}", [
                    'url' => $url, 'body' => $resp->body(),
                ]);
            }
            catch (\Throwable $e) {
                Log::warning("Contpaqi sync error intento {$attempt}: " . $e->getMessage(), [
                    'url' => $url, 'exception' => $e,
                ]);
            }
        }

        Log::error("Contpaqi sync AGOTÓ los {$maxAttempts} intentos: {$method} {$url}");
        return null;
    }

    // ============ ENDPOINTS DE SYNC ============

    /**
     * Trae todos los clientes (snapshot completo).
     * Útil para la primera sincronización.
     */
    public function pullClientes(string $rutaEmpresa, int $limite = 500): array
    {
        $r = $this->call('GET', '/api/Sync/clientes', [
            'rutaEmpresa' => $rutaEmpresa, 'limite' => $limite,
        ]);
        return $r['clientes'] ?? [];
    }

    /**
     * Trae clientes modificados desde una fecha.
     */
    public function pullClientesModificados(string $rutaEmpresa, \DateTimeInterface $desde, int $limite = 500): array
    {
        $r = $this->call('GET', '/api/Sync/clientes/modificados', [
            'rutaEmpresa' => $rutaEmpresa,
            'desde'       => $desde->format('Y-m-d\TH:i:s'),
            'limite'      => $limite,
        ]);
        return $r['clientes'] ?? [];
    }

    public function pullProductos(string $rutaEmpresa, int $limite = 500): array
    {
        $r = $this->call('GET', '/api/Sync/productos', [
            'rutaEmpresa' => $rutaEmpresa, 'limite' => $limite,
        ]);
        return $r['productos'] ?? [];
    }

    public function pullProductosModificados(string $rutaEmpresa, \DateTimeInterface $desde, int $limite = 500): array
    {
        $r = $this->call('GET', '/api/Sync/productos/modificados', [
            'rutaEmpresa' => $rEmpresa = $rutaEmpresa,
            'desde'       => $desde->format('Y-m-d\TH:i:s'),
            'limite'      => $limite,
        ]);
        return $r['productos'] ?? [];
    }

    public function pullDocumentosModificados(string $rutaEmpresa, \DateTimeInterface $desde, int $limite = 500): array
    {
        $r = $this->call('GET', '/api/Sync/documentos/modificados', [
            'rutaEmpresa' => $rutaEmpresa,
            'desde'       => $desde->format('Y-m-d\TH:i:s'),
            'limite'      => $limite,
        ]);
        return $r['documentos'] ?? [];
    }

    public function pullReporteVentas(string $rutaEmpresa, \DateTimeInterface $desde, \DateTimeInterface $hasta): array
    {
        $r = $this->call('GET', '/api/Reportes/ventas', [
            'rutaEmpresa' => $rutaEmpresa,
            'desde'       => $desde->format('Y-m-d'),
            'hasta'       => $hasta->format('Y-m-d'),
        ]);
        return $r['ventas'] ?? [];
    }

    /**
     * Devuelve las últimas N entradas del sync log del bridge.
     * Útil para debugging desde Laravel.
     */
    public function pullSyncLog(int $ultimas = 100): array
    {
        $r = $this->call('GET', '/api/Sync/log', ['ultimas' => $ultimas]);
        return $r['entries'] ?? [];
    }
}