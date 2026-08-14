<?php
/**
 * ContpaqiBridge Laravel - Controller HTTP
 *
 * Expone endpoints REST en Laravel para que tu aplicacion facture y timbre
 * via el bridge. Tu frontend (o cualquier cliente HTTP) llama a estos
 * endpoints; Laravel se encarga de orquestar bridge.
 *
 * Ubicacion sugerida: app/Http/Controllers/Api/FacturaController.php
 *
 * Rutas (en routes/api.php):
 *   Route::middleware(['auth:sanctum'])->prefix('api/facturas')->group(function () {
 *       Route::post('/', [FacturaController::class, 'store']);          // POST /api/facturas
 *       Route::post('/{id}/timbrar', [FacturaController::class, 'timbrar']); // POST /api/facturas/{id}/timbrar
 *   });
 */

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Services\ContpaqiFacturadorService;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Validator;

class FacturaController extends Controller
{
    public function __construct(protected ContpaqiFacturadorService $facturador) {}

    /**
     * POST /api/facturas
     * Body JSON: { ruta_empresa, concepto, cliente, productos[], cliente_datos{}, uso_cfdi?, forma_pago?, metodo_pago?, timbrar?: bool, pass_csd?: string }
     *
     * Si timbrar=true (default), crea y timbra en una sola llamada (requiere pass_csd).
     * Si timbrar=false, solo crea la cabecera y movimientos (folio sin timbrar).
     */
    public function store(Request $request): JsonResponse
    {
        $validator = Validator::make($request->all(), [
            'ruta_empresa'                  => 'required|string',
            'concepto'                      => 'required|string',
            'cliente'                       => 'required|string',
            'productos'                     => 'required|array|min:1',
            'productos.*.codigo'            => 'required|string',
            'productos.*.cantidad'          => 'required|numeric|min:0.01',
            'productos.*.precio'            => 'required|numeric|min:0',
            'productos.*.unidad_medida'     => 'sometimes|string',
            'productos.*.clave_sat'         => 'sometimes|string',
            'cliente_datos.razon_social'     => 'sometimes|string',
            'cliente_datos.rfc'              => 'sometimes|string',
            'cliente_datos.regimen_fiscal'   => 'sometimes|string',
            'uso_cfdi'                      => 'sometimes|string',
            'forma_pago'                    => 'sometimes|string',
            'metodo_pago'                   => 'sometimes|string',
            'timbrar'                       => 'sometimes|boolean',
            'pass_csd'                      => 'required_if:timbrar,true|string',
        ]);

        if ($validator->fails()) {
            return response()->json([
                'ok' => false,
                'error' => 'Validacion',
                'detalles' => $validator->errors()->toArray(),
            ], 422);
        }

        $datos = $request->all();
        $timbrar = $datos['timbrar'] ?? true;
        unset($datos['timbrar']);

        try {
            if ($timbrar) {
                $passCSD = $datos['pass_csd'];
                unset($datos['pass_csd']);

                $resultado = $this->facturador->crearYTimbrar($datos, $passCSD);
            } else {
                $resultado = $this->facturador->facturar($datos);
            }

            if (!$resultado['ok']) {
                return response()->json($resultado, 500);
            }

            return response()->json($resultado, 201);
        }
        catch (\Throwable $e) {
            Log::error("FacturaController: excepcion no controlada: " . $e->getMessage(), [
                'exception' => $e,
                'request'   => $request->except(['pass_csd']),
            ]);
            return response()->json([
                'ok' => false,
                'error' => 'Error interno del servidor',
            ], 500);
        }
    }

    /**
     * POST /api/facturas/{id}/timbrar
     * Body JSON: { pass_csd: "..." }
     * {id} es el contpaqi_id de la factura creada.
     */
    public function timbrar(Request $request, int $id): JsonResponse
    {
        $validator = Validator::make($request->all(), [
            'pass_csd'        => 'required|string',
            'ruta_empresa'    => 'required|string',
            'concepto'        => 'required|string',
            'serie'           => 'required|string',
            'folio'           => 'required|numeric',
        ]);

        if ($validator->fails()) {
            return response()->json([
                'ok' => false,
                'error' => 'Validacion',
                'detalles' => $validator->errors()->toArray(),
            ], 422);
        }

        $datos = $request->all();
        $resultado = $this->facturador->timbrar(
            $datos['ruta_empresa'],
            $datos['concepto'],
            $datos['serie'],
            (int) $datos['folio'],
            $datos['pass_csd']
        );

        return response()->json($resultado, $resultado['ok'] ? 200 : 500);
    }

    /**
     * GET /api/facturas/validar
     * Query params: ruta_empresa, concepto, serie, folio
     * Devuelve issues sin timbrar (validacion pre-timbrado).
     */
    public function validar(Request $request): JsonResponse
    {
        $validator = Validator::make($request->all(), [
            'ruta_empresa' => 'required|string',
            'concepto'     => 'required|string',
            'serie'        => 'sometimes|string',
            'folio'        => 'sometimes|numeric',
        ]);

        if ($validator->fails()) {
            return response()->json(['ok' => false, 'error' => 'Validacion', 'detalles' => $validator->errors()], 422);
        }

        // Redirigir al endpoint del bridge
        $client = \App\Services\ContpaqiBridgeClient::fromConfig();
        $r = $client->call('POST', '/api/Documentos/validar', $request->all());

        if (!$r['ok']) {
            return response()->json(['ok' => false, 'error' => $r['error']], 502);
        }

        return response()->json($r['data']);
    }
}