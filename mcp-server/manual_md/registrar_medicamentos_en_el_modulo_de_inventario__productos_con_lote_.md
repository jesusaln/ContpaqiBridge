## Registrar medicamentos en el módulo de inventario (productos con lote)

Una empresa distribuidora de medicamentos trabaja con productos farmacéuticos que requieren un estricto control por lote y fecha de caducidad, conforme a las normativas sanitarias vigentes.

Con la integración del SDK del sistema administrativo, se busca automatizar el proceso de registro de entradas al inventario, asegurando que cada medicamento esté correctamente vinculado a su lote, fecha de fabricación y caducidad, facilitando de esta forma la trazabilidad de cada uno.

Para lograrlo, se hace uso de tres funciones principales del SDK:

- fAltaDocumento () que permite generar un documento de entrada con los datos básicos de la operación.
- fAltaMovimiento () que registra el movimiento del producto dentro del documento.
- fAltaMovimientoSeriesCapas () que asocia el producto con su lote y fechas relevantes.

Configuración en el sistema de CONTPAQi Comercial Premium®

| Paso | Acción |  |
|---|---|---|
|  | Ingresa al catálogo de Productos, para activar la opción de control por lote:  | Nota Esto permite que el sistema registre y gestione los movimientos del producto. |

****

**Implementación C#**

private void button_lote_Click(object sender, EventArgs e)

{

int lError = 0;

SDK.tDocumento ltDocumento = new SDK.tDocumento();

SDK.tMovimiento ltMovimiento = new SDK.tMovimiento();

SDK.tSeriesCapas ltSeriesCapas = new SDK.tSeriesCapas();

int lIdDocumento = 0;

int lIdMovimiento = 0;

ltDocumento.aCodConcepto = "34"; //CONCEPTO DEL DOCUMENTO

ltDocumento.aFecha = DateTime.Today.ToString("MM/dd/yyyy");

ltDocumento.aSerie = "";// INDICAR SERIE PARA IDENTIFICACION DEL DOCUMENTO

ltDocumento.aSistemaOrigen = 205; //205=COMERCIAL

ltDocumento.aNumMoneda = 1; // INDICAR TIPO DE MONEDA

ltDocumento.aTipoCambio = 1; // INDICAR TIPO DE CAMBIO

ltDocumento.aFolio = 0;

lError = SDK.fAltaDocumento(ref lIdDocumento, ref ltDocumento);

if (lError != 0)

{

MessageBox.Show(SDK.rError(lError));

}

ltMovimiento.aCodProdSer = "PR08"; //CODIGO DEL PRODUCTO

ltMovimiento.aCodAlmacen = "1";

ltMovimiento.aCosto = 100;

ltMovimiento.aReferencia = "";

lError = SDK.fAltaMovimiento(lIdDocumento, ref lIdMovimiento, ref ltMovimiento);

if (lError != 0)

{

MessageBox.Show(SDK.rError(lError));

}

else

{

MessageBox.Show("Documento guardado con exito");

}

ltSeriesCapas.aTipoCambio = 1.0000; //INDICAR EL TIPO DE CAMBIO

ltSeriesCapas.aNumeroLote = "AB250715B"; //INDICAR EL NOMBRE DEL LOTE ltSeriesCapas.aFechaFabricacion = "06/10/2025";

ltSeriesCapas.aFechaCaducidad = "07/10/2025";

ltSeriesCapas.aUnidades = 100; // INDICAR LA CANTIDAD DE UNIDADES

lError = SDK.fAltaMovimientoSeriesCapas(lIdMovimiento, ref ltSeriesCapas); //ALTA DEL MOVIMIENTO SERIES CAPAS

if (lError != 0)

{

MessageBox.Show(SDK.rError(lError));

}

else

{

MessageBox.Show("Documento SeriesCapa creado con exito");

}

}

}

****

Al generar el documento de entrada en el sistema, este queda debidamente registrado:

Y el medicamento queda vinculado al lote con fechas de fabricación y caducidad:

|  | Recuerda Todas las fechas que se ingresen por medio del SDK, tanto en funciones de alto como de bajo nivel, deberán capturarse en formato MM/DD/YYYY. Por ejemplo: el día 16 de marzo de 2025 se representa como "03/16/2025". |
|---|---|

|  | Importante La función fAltaMovimientoSeriesCapas agrega la información de lote y/o pedimento asociado a un movimiento de entrada. En documentos de salida, se buscarán y tomarán automáticamente los pedimentos/lotes registrados, respetando las fechas de elaboración o importación. Se soluciona el problema común de registros con fecha errónea "12/30/1899", asignando correctamente las fechas ingresadas. Cuando existan movimientos de salida sin fecha asignada, pero con el mismo lote o pedimento, se tomará la existencia correcta de las capas disponibles. |
|---|---|