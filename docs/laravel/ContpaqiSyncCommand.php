<?php
/**
 * Comando Artisan para sincronizar catálogos y documentos con el bridge de CONTPAQi.
 *
 * Ubicación sugerida: app/Console/Commands/ContpaqiSyncCommand.php
 *
 * Uso:
 *   php artisan contpaqi:sync clientes --empresa=adJESUS_LOPEZ_NORIEGA
 *   php artisan contpaqi:sync productos --empresa=adJESUS_LOPEZ_NORIEGA
 *   php artisan contpaqi:sync documentos --empresa=adJESUS_LOPEZ_NORIEGA --desde="2026-08-01"
 *   php artisan contpaqi:sync todo --empresa=adJESUS_LOPEZ_NORIEGA
 *
 * Programar en routes/console.php:
 *   Schedule::command('contpaqi:sync clientes')->everyFiveMinutes();
 *   Schedule::command('contpaqi:sync documentos')->everyTenMinutes();
 */

namespace App\Console\Commands;

use Illuminate\Console\Command;
use App\Services\ContpaqiBridgeClient;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Log;

class ContpaqiSyncCommand extends Command
{
    protected $signature = 'contpaqi:sync
        {tipo : clientes|productos|documentos|todo}
        {--empresa= : Nombre de la carpeta de empresa (ej. adJESUS_LOPEZ_NORIEGA)}
        {--desde= : Fecha desde para sync incremental (YYYY-MM-DD)}
        {--limite=500 : Máximo de registros}';

    protected $description = 'Sincroniza catálogos y documentos desde el bridge de CONTPAQi a Laravel';

    public function handle(): int
    {
        $tipo = $this->argument('tipo');
        $empresa = $this->option('empresa') ?: config('contpaqi.empresa_default');
        if (!$empresa) {
            $this->error('Debes especificar --empresa=... o configurar contpaqi.empresa_default');
            return self::FAILURE;
        }
        $rutaEmpresa = "C:\\Compac\\Empresas\\{$empresa}";
        $limite = (int) $this->option('limite');
        $desde = $this->option('desde')
            ? new \DateTime($this->option('desde'))
            : null;

        $client = ContpaqiBridgeClient::fromConfig();

        try {
            match ($tipo) {
                'clientes'  => $this->syncClientes($client, $rutaEmpresa, $limite, $desde),
                'productos' => $this->syncProductos($client, $rutaEmpresa, $limite, $desde),
                'documentos' => $this->syncDocumentos($client, $rutaEmpresa, $limite, $desde),
                'todo' => $this->syncTodo($client, $rutaEmpresa, $limite, $desde),
                default => null,
            };
            return self::SUCCESS;
        }
        catch (\Throwable $e) {
            Log::error('contpaqi:sync error: ' . $e->getMessage(), ['exception' => $e]);
            $this->error('Error: ' . $e->getMessage());
            return self::FAILURE;
        }
    }

    protected function syncClientes(ContpaqiBridgeClient $client, string $rutaEmpresa, int $limite, ?\DateTime $desde): void
    {
        $items = $desde
            ? $client->pullClientesModificados($rutaEmpresa, $desde, $limite)
            : $client->pullClientes($rutaEmpresa, $limite);

        $this->info("Clientes recibidos: " . count($items));
        foreach ($items as $c) {
            DB::table('clientes')->updateOrInsert(
                ['contpaqi_id' => $c['CIDCLIENTEPROVEEDOR'] ?? $c['contpaqi_id'] ?? null],
                [
                    'codigo'        => $c['CCODIGOCLIENTE'] ?? $c['codigo'] ?? '',
                    'razon_social'  => $c['CRAZONSOCIAL'] ?? $c['razon_social'] ?? '',
                    'rfc'           => $c['CRFC'] ?? $c['rfc'] ?? '',
                    'email'         => $c['CEMAIL1'] ?? $c['email'] ?? null,
                    'uso_cfdi'      => $c['CUSOCFDI'] ?? $c['uso_cfdi'] ?? null,
                    'regimen_fiscal'=> $c['CREGIMFISC'] ?? $c['regimen_fiscal'] ?? null,
                    'estatus'       => $c['CESTATUS'] ?? $c['estatus'] ?? null,
                    'updated_at'    => now(),
                    'created_at'    => now(),
                ]
            );
        }
        $this->info("Sincronizados: " . count($items) . " clientes");
    }

    protected function syncProductos(ContpaqiBridgeClient $client, string $rutaEmpresa, int $limite, ?\DateTime $desde): void
    {
        $items = $desde
            ? $client->pullProductosModificados($rutaEmpresa, $desde, $limite)
            : $client->pullProductos($rutaEmpresa, $limite);

        $this->info("Productos recibidos: " . count($items));
        foreach ($items as $p) {
            DB::table('productos')->updateOrInsert(
                ['contpaqi_id' => $p['CIDPRODUCTO'] ?? $p['contpaqi_id'] ?? null],
                [
                    'codigo'   => $p['CCODIGOPRODUCTO'] ?? $p['codigo'] ?? '',
                    'nombre'   => $p['CNOMBREPRODUCTO'] ?? $p['nombre'] ?? '',
                    'precio'   => $p['CPRECIO1'] ?? $p['precio'] ?? 0,
                    'clave_sat'=> $p['CCLAVESAT'] ?? $p['clave_sat'] ?? null,
                    'updated_at'=> now(),
                    'created_at'=> now(),
                ]
            );
        }
        $this->info("Sincronizados: " . count($items) . " productos");
    }

    protected function syncDocumentos(ContpaqiBridgeClient $client, string $rutaEmpresa, int $limite, ?\DateTime $desde): void
    {
        $desde = $desde ?? new \DateTime('-7 days');
        $items = $client->pullDocumentosModificados($rutaEmpresa, $desde, $limite);

        $this->info("Documentos recibidos: " . count($items));
        foreach ($items as $d) {
            DB::table('documentos')->updateOrInsert(
                ['contpaqi_id' => $d['CIDDOCUMENTO'] ?? null],
                [
                    'serie'        => $d['CSERIEDOCUMENTO'] ?? '',
                    'folio'        => $d['CFOLIO'] ?? 0,
                    'fecha'        => $d['CFECHA'] ?? null,
                    'cliente_id'   => $d['CRAZONSOCIAL'] ?? null,
                    'total'        => $d['CTOTAL'] ?? 0,
                    'timbrado'     => $d['CTIMBRADO'] ?? 0,
                    'cancelado'    => $d['CCANCELADO'] ?? 0,
                    'updated_at'   => now(),
                ]
            );
        }
        $this->info("Sincronizados: " . count($items) . " documentos");
    }

    protected function syncTodo(ContpaqiBridgeClient $client, string $rutaEmpresa, int $limite, ?\DateTime $desde): void
    {
        $this->syncClientes($client, $rutaEmpresa, $limite, $desde);
        $this->syncProductos($client, $rutaEmpresa, $limite, $desde);
        $this->syncDocumentos($client, $rutaEmpresa, $limite, $desde);
    }
}