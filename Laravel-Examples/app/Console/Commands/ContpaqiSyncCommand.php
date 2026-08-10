<?php
/**
 * Comando Artisan para sincronización bidireccional Laravel ↔ CONTPAQi.
 * Colocar en: app/Console/Commands/ContpaqiSyncCommand.php
 *
 * Uso:
 *   php artisan contpaqi:sync --direction=push   (Laravel → CONTPAQi)
 *   php artisan contpaqi:sync --direction=pull   (CONTPAQi → Laravel)
 *   php artisan contpaqi:sync --direction=both   (ambas direcciones)
 *   php artisan contpaqi:sync --entidades=clientes,productos,facturas
 *   php artisan contpaqi:sync --full              (snapshot completo, ignora timestamp)
 */

namespace App\Console\Commands;

use Illuminate\Console\Command;
use App\Services\ContpaqiBridgeService;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Log;

class ContpaqiSyncCommand extends Command
{
    protected $signature = 'contpaqi:sync
                            {--direction=both : push (Laravel→CONTPAQi), pull (CONTPAQi→Laravel), both}
                            {--entidades=clientes,productos,facturas : entidades a sincronizar}
                            {--full : sincronización completa, ignora last_sync_at}
                            {--batch=100 : tamaño de lote para batch sync}';

    protected $description = 'Sincroniza datos entre Laravel (MySQL) y CONTPAQi Comercial via el bridge API';

    protected ContpaqiBridgeService $bridge;

    public function __construct(ContpaqiBridgeService $bridge)
    {
        parent::__construct();
        $this->bridge = $bridge;
    }

    public function handle(): int
    {
        $direction = $this->option('direction');
        $entidades = explode(',', $this->option('entidades'));
        $full = $this->option('full');
        $batch = (int) $this->option('batch');

        $this->info("═══════════════════════════════════════════════════════════");
        $this->info(" Sincronización Laravel ↔ CONTPAQi");
        $this->info(" Dirección: $direction | Entidades: " . implode(', ', $entidades) . " | Full: " . ($full ? 'sí' : 'no'));
        $this->info("═══════════════════════════════════════════════════════════");

        $resultados = [];

        if (in_array('clientes', $entidades)) {
            if (in_array($direction, ['push', 'both'])) {
                $resultados['clientes_push'] = $this->pushClientes($batch);
            }
            if (in_array($direction, ['pull', 'both'])) {
                $resultados['clientes_pull'] = $this->pullClientes($full);
            }
        }

        if (in_array('productos', $entidades)) {
            if (in_array($direction, ['push', 'both'])) {
                $resultados['productos_push'] = $this->pushProductos($batch);
            }
            if (in_array($direction, ['pull', 'both'])) {
                $resultados['productos_pull'] = $this->pullProductos($full);
            }
        }

        if (in_array('facturas', $entidades)) {
            // Solo pull: las facturas nacen en CONTPAQi (o vía bridge)
            if (in_array($direction, ['pull', 'both'])) {
                $resultados['facturas_pull'] = $this->pullFacturas($full);
            }
        }

        $this->info("\n═══════════════════════════════════════════════════════════");
        $this->info(" Resumen:");
        foreach ($resultados as $k => $r) {
            $this->line("  $k: $r");
        }
        $this->info("═══════════════════════════════════════════════════════════\n");

        return 0;
    }

    // ========================================================================
    // CLIENTES: Laravel → CONTPAQi
    // ========================================================================
    protected function pushClientes(int $batch): string
    {
        $this->info("\n→ Push CLIENTES: Laravel → CONTPAQi");

        $clientes = DB::table('clientes')
            ->where('sync_status', '!=', 'synced')
            ->orWhereNull('sync_status')
            ->limit($batch)
            ->get();

        if ($clientes->isEmpty()) {
            return "0 clientes pendientes";
        }

        $items = $clientes->map(function ($c) {
            return [
                'codigo' => $c->codigo,
                'razonSocial' => $c->razon_social,
                'rfc' => $c->rfc,
                'email' => $c->email ?? '',
                'calle' => $c->calle ?? '',
                'colonia' => $c->colonia ?? '',
                'codigoPostal' => $c->codigo_postal ?? '',
                'ciudad' => $c->ciudad ?? '',
                'estado' => $c->estado ?? '',
                'pais' => $c->pais ?? 'México',
                'regimenFiscal' => $c->regimen_fiscal ?? '',
                'usoCFDI' => $c->uso_cfdi ?? '',
                'formaPago' => $c->forma_pago ?? '',
            ];
        })->toArray();

        $r = $this->bridge->pushClientes($items);
        $ok = $r['ok'] ?? 0;
        $error = $r['error'] ?? 0;

        // Marcar como sincronizados los que tuvieron éxito
        foreach ($r['resultados'] ?? [] as $res) {
            if ($res['exito'] ?? false) {
                DB::table('clientes')->where('codigo', $res['codigo'])->update([
                    'sync_status' => 'synced',
                    'contpaqi_id' => $res['idCliente'] ?? null,
                    'last_sync_at' => now(),
                    'updated_at' => now()
                ]);
            } else {
                DB::table('clientes')->where('codigo', $res['codigo'])->update([
                    'sync_status' => 'error',
                    'sync_error' => $res['mensaje'] ?? 'Error desconocido',
                    'updated_at' => now()
                ]);
            }
        }

        return "$ok OK / $error ERROR (de {$clientes->count()} clientes)";
    }

    // ========================================================================
    // CLIENTES: CONTPAQi → Laravel
    // ========================================================================
    protected function pullClientes(bool $full): string
    {
        $this->info("\n← Pull CLIENTES: CONTPAQi → Laravel");

        $desde = $full ? null : $this->getLastSync('clientes');
        $r = $this->bridge->getClientes($full, $desde);
        $items = $r['clientes'] ?? [];

        $insertados = 0;
        $actualizados = 0;

        foreach ($items as $cli) {
            $existe = DB::table('clientes')->where('codigo', $cli['codigo'])->exists();
            $data = [
                'codigo' => $cli['codigo'],
                'razon_social' => $cli['razon_social'],
                'rfc' => $cli['rfc'],
                'email' => $cli['email'],
                'calle' => $cli['calle'],
                'colonia' => $cli['colonia'],
                'codigo_postal' => $cli['codigo_postal'],
                'ciudad' => $cli['ciudad'],
                'estado' => $cli['estado'],
                'pais' => $cli['pais'],
                'regimen_fiscal' => $cli['regimen_fiscal'],
                'uso_cfdi' => $cli['uso_cfdi'],
                'forma_pago' => $cli['forma_pago'],
                'telefono' => $cli['telefono'] ?? '',
                'contpaqi_id' => $cli['contpaqi_id'],
                'sync_status' => 'synced',
                'sync_source' => 'contpaqi',
                'last_sync_at' => now(),
                'updated_at' => now()
            ];

            if ($existe) {
                DB::table('clientes')->where('codigo', $cli['codigo'])->update($data);
                $actualizados++;
            } else {
                $data['created_at'] = now();
                DB::table('clientes')->insert($data);
                $insertados++;
            }
        }

        $this->saveLastSync('clientes');

        return "$insertados nuevos / $actualizados actualizados (de {$r['count']} en CONTPAQi)";
    }

    // ========================================================================
    // PRODUCTOS: Laravel → CONTPAQi
    // ========================================================================
    protected function pushProductos(int $batch): string
    {
        $this->info("\n→ Push PRODUCTOS: Laravel → CONTPAQi");

        $productos = DB::table('productos')
            ->where('sync_status', '!=', 'synced')
            ->orWhereNull('sync_status')
            ->limit($batch)
            ->get();

        if ($productos->isEmpty()) {
            return "0 productos pendientes";
        }

        $items = $productos->map(fn($p) => [
            'codigo' => $p->codigo,
            'nombre' => $p->nombre,
            'descripcion' => $p->descripcion ?? '',
            'precio' => (float) $p->precio,
            'tipoProducto' => $p->tipo_producto ?? 1,
            'unidadMedida' => $p->unidad_medida ?? 'H87',
            'claveSAT' => $p->clave_sat ?? '',
        ])->toArray();

        $r = $this->bridge->pushProductos($items);
        $ok = $r['ok'] ?? 0;
        $error = $r['error'] ?? 0;

        foreach ($r['resultados'] ?? [] as $res) {
            DB::table('productos')->where('codigo', $res['codigo'])->update([
                'sync_status' => ($res['exito'] ?? false) ? 'synced' : 'error',
                'sync_error' => $res['mensaje'] ?? null,
                'last_sync_at' => now(),
                'updated_at' => now()
            ]);
        }

        return "$ok OK / $error ERROR (de {$productos->count()} productos)";
    }

    // ========================================================================
    // PRODUCTOS: CONTPAQi → Laravel
    // ========================================================================
    protected function pullProductos(bool $full): string
    {
        $this->info("\n← Pull PRODUCTOS: CONTPAQi → Laravel");

        $desde = $full ? null : $this->getLastSync('productos');
        $r = $this->bridge->getProductos($full, $desde);
        $items = $r['productos'] ?? [];

        $insertados = 0;
        $actualizados = 0;

        foreach ($items as $p) {
            $existe = DB::table('productos')->where('codigo', $p['codigo'])->exists();
            $data = [
                'codigo' => $p['codigo'],
                'nombre' => $p['nombre'],
                'descripcion' => $p['descripcion'] ?? '',
                'precio' => $p['precio1'] ?? 0,
                'tipo_producto' => $p['tipo_producto'] ?? 1,
                'unidad_medida' => $p['unidad_sat'] ?? 'H87',
                'clave_sat' => $p['clave_sat'] ?? '',
                'contpaqi_id' => $p['contpaqi_id'],
                'existencia' => $p['existencia'] ?? 0,
                'sync_status' => 'synced',
                'sync_source' => 'contpaqi',
                'last_sync_at' => now(),
                'updated_at' => now()
            ];

            if ($existe) {
                DB::table('productos')->where('codigo', $p['codigo'])->update($data);
                $actualizados++;
            } else {
                $data['created_at'] = now();
                DB::table('productos')->insert($data);
                $insertados++;
            }
        }

        $this->saveLastSync('productos');
        return "$insertados nuevos / $actualizados actualizados (de {$r['count']} en CONTPAQi)";
    }

    // ========================================================================
    // FACTURAS: CONTPAQi → Laravel
    // ========================================================================
    protected function pullFacturas(bool $full): string
    {
        $this->info("\n← Pull FACTURAS: CONTPAQi → Laravel");

        $desde = $full ? new \DateTime('2020-01-01') : $this->getLastSync('facturas', new \DateTime('2020-01-01'));
        $r = $this->bridge->getDocumentosModificados($desde);

        $items = $r['documentos'] ?? [];
        $insertados = 0;
        $actualizados = 0;

        foreach ($items as $doc) {
            // Solo facturas (tipo 4)
            if (!in_array($doc['id_concepto'] ?? '', ['4', 'FAC', 'FACTURA'])) continue;

            $existe = DB::table('facturas')
                ->where('serie', $doc['serie'])
                ->where('folio', $doc['folio'])
                ->exists();

            $data = [
                'contpaqi_id' => $doc['contpaqi_id'],
                'serie' => $doc['serie'],
                'folio' => $doc['folio'],
                'uuid' => $doc['uuid'] ?? null,
                'fecha' => $doc['fecha'],
                'cliente_codigo' => $doc['cliente_codigo'],
                'cliente_razon_social' => $doc['cliente_razon_social'],
                'importe' => $doc['importe'] ?? 0,
                'iva' => $doc['iva'] ?? 0,
                'total' => $doc['total'] ?? 0,
                'metodo_pago' => $doc['metodo_pago'] ?? null,
                'forma_pago' => $doc['forma_pago'] ?? null,
                'uso_cfdi' => $doc['uso_cfdi'] ?? null,
                'cancelado' => ($doc['cancelado'] ?? '0') == '1',
                'sync_source' => 'contpaqi',
                'last_sync_at' => now(),
                'updated_at' => now()
            ];

            if ($existe) {
                DB::table('facturas')->where('serie', $doc['serie'])->where('folio', $doc['folio'])->update($data);
                $actualizados++;
            } else {
                $data['created_at'] = now();
                DB::table('facturas')->insert($data);
                $insertados++;
            }
        }

        $this->saveLastSync('facturas');
        return "$insertados nuevas / $actualizados actualizadas (de {$r['count']} documentos en CONTPAQi)";
    }

    // ========================================================================
    // TRACKING DE SINCRONIZACIÓN
    // ========================================================================
    protected function getLastSync(string $entidad, ?\DateTime $default = null): ?\DateTime
    {
        $val = Cache::get("contpaqi_last_sync_{$entidad}");
        if ($val) return new \DateTime($val);
        return $default;
    }

    protected function saveLastSync(string $entidad): void
    {
        Cache::put("contpaqi_last_sync_{$entidad}", now()->toDateTimeString(), 86400 * 365);
    }
}