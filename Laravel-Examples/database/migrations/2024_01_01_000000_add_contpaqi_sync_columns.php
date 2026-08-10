<?php
/**
 * Migración: agregar columnas de sincronización a tablas existentes.
 *
 * Uso:
 *   php artisan make:migration add_contpaqi_sync_columns_to_tables --table=clientes
 *   Luego pega este contenido adaptado.
 */

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        // CLIENTES
        Schema::table('clientes', function (Blueprint $t) {
            $t->string('contpaqi_id')->nullable()->index()->after('id');
            $t->string('sync_status', 20)->default('pending')->index(); // pending, synced, error
            $t->string('sync_source', 20)->nullable(); // 'laravel' (origen), 'contpaqi' (recibido)
            $t->text('sync_error')->nullable();
            $t->timestamp('last_sync_at')->nullable();
        });

        // PRODUCTOS
        Schema::table('productos', function (Blueprint $t) {
            $t->string('contpaqi_id')->nullable()->index()->after('id');
            $t->string('sync_status', 20)->default('pending')->index();
            $t->string('sync_source', 20)->nullable();
            $t->text('sync_error')->nullable();
            $t->timestamp('last_sync_at')->nullable();
        });

        // FACTURAS (nueva tabla si no existe, o agregar columnas)
        if (!Schema::hasTable('facturas')) {
            Schema::create('facturas', function (Blueprint $t) {
                $t->id();
                $t->string('contpaqi_id')->nullable()->index();
                $t->string('serie', 20)->default('');
                $t->double('folio');
                $t->string('uuid', 50)->nullable()->index();
                $t->date('fecha');
                $t->string('cliente_codigo', 30)->nullable()->index();
                $t->string('cliente_razon_social', 200)->nullable();
                $t->double('importe')->default(0);
                $t->double('iva')->default(0);
                $t->double('total')->default(0);
                $t->string('metodo_pago', 10)->nullable();
                $t->string('forma_pago', 10)->nullable();
                $t->string('uso_cfdi', 10)->nullable();
                $t->boolean('cancelado')->default(false);
                $t->boolean('timbrada')->default(false);
                $t->string('motivo_cancelacion', 5)->nullable();
                $t->string('uuid_sustitucion', 50)->nullable();
                $t->timestamp('fecha_timbrado')->nullable();
                $t->timestamp('fecha_cancelacion')->nullable();
                $t->string('sync_source', 20)->nullable();
                $t->timestamp('last_sync_at')->nullable();
                $t->timestamps();

                $t->unique(['serie', 'folio']);
            });
        }

        // LOG de eventos
        Schema::create('facturas_log', function (Blueprint $t) {
            $t->id();
            $t->string('tipo', 50);
            $t->string('serie', 20)->nullable();
            $t->double('folio')->nullable();
            $t->text('error')->nullable();
            $t->timestamps();
        });
    }

    public function down(): void
    {
        Schema::table('clientes', function (Blueprint $t) {
            $t->dropColumn(['contpaqi_id', 'sync_status', 'sync_source', 'sync_error', 'last_sync_at']);
        });
        Schema::table('productos', function (Blueprint $t) {
            $t->dropColumn(['contpaqi_id', 'sync_status', 'sync_source', 'sync_error', 'last_sync_at']);
        });
        Schema::dropIfExists('facturas_log');
    }
};