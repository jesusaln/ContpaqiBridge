<?php
/**
 * ContpaqiBridge Laravel - Test E2E Command
 *
 * Ubicación sugerida: app/Console/Commands/ContpaqiTestE2ECommand.php
 *
 * Simula el flujo Laravel → bridge → CONTPAQi sin necesidad de modelos Eloquent.
 * Útil para verificar que toda la cadena funciona end-to-end.
 *
 * Uso:
 *   php artisan contpaqi:test-e2e                       # usa datos de muestra
 *   php artisan contpaqi:test-e2e --timbrar             # incluye timbrado
 *   php artisan contpaqi:test-e2e --cliente=CLI001     # usa código personalizado
 */

namespace App\Console\Commands;

use App\Services\ContpaqiFacturadorService;
use Illuminate\Console\Command;

class ContpaqiTestE2ECommand extends Command
{
    protected $signature = 'contpaqi:test-e2e
        {--cliente=CLIE2ETEST : Codigo del cliente de prueba}
        {--producto=PRODE2ETEST : Codigo del producto de prueba}
        {--timbrar : Incluir paso de timbrado (requiere PassCSD real)}
        {--pass-csd= : Password del CSD (si --timbrar)}';

    protected $description = 'Prueba end-to-end del flujo Laravel -> bridge -> CONTPAQi';

    public function handle(): int
    {
        $facturador = ContpaqiFacturadorService::fromConfig();
        $codigoCliente = $this->option('cliente');
        $codigoProducto = $this->option('producto');

        $this->info("=== Test E2E ContpaqiBridge ===");
        $this->info("Cliente: {$codigoCliente}");
        $this->info("Producto: {$codigoProducto}");
        $this->info("");

        // 1) Datos de muestra (simula clientes/productos de Laravel)
        $datos = [
            'ruta_empresa' => 'C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA',
            'concepto' => '4CLIMAS',
            'cliente' => $codigoCliente,
            'cliente_datos' => [
                'codigo' => $codigoCliente,
                'nombre' => 'CLIENTE DE PRUEBA E2E',
                'rfc' => 'E2E010101AAA',
                'email' => 'prueba@example.com',
                'regimen_fiscal' => '601',
                'uso_cfdi' => 'G03',
                'codigo_postal' => '64000',
            ],
            'productos' => [
                [
                    'codigo' => $codigoProducto,
                    'nombre' => 'PRODUCTO DE PRUEBA E2E',
                    'cantidad' => 2,
                    'precio' => 100.00,
                    'unidad_medida' => 'H87',
                    'clave_sat' => '01010101',
                ],
            ],
        ];

        // 2) Llamar al facturador (que internamente hace sync + crear [+ timbrar])
        $passCSD = null;
        if ($this->option('timbrar')) {
            $passCSD = $this->option('pass-csd') ?: '';
            if (empty($passCSD)) {
                $this->warn('--timbrar requiere --pass-csd=<password> o se enviara vacio (timbrado fallara por PAC)');
            }
        }

        $resultado = $facturador->crearYTimbrar($datos, $passCSD);

        $this->info("");
        $this->info("=== Resultado ===");
        if ($resultado['ok'] ?? false) {
            $this->info("OK:");
            $this->line("  Mensaje:      " . ($resultado['mensaje'] ?? ''));
            $this->line("  Serie:        " . ($resultado['serie'] ?? ''));
            $this->line("  Folio:        " . ($resultado['folio'] ?? ''));
            $this->line("  ID documento: " . ($resultado['id_documento'] ?? ''));
            if (!empty($resultado['timbrado'])) {
                $this->line("  Timbrado:     " . ($resultado['timbrado']['mensaje'] ?? ''));
            }
            return self::SUCCESS;
        }

        $this->error("FALLO:");
        $this->line("  Error: " . ($resultado['error'] ?? 'desconocido'));
        if (!empty($resultado['id_documento'])) {
            $this->line("  (Factura creada con ID {$resultado['id_documento']} pero algo mas fallo)");
        }
        return self::FAILURE;
    }
}