## Emitir factura de gasolina (producto con IEPS)

Una empresa dedicada al transporte o distribución requiere emitir facturas por la venta de gasolina, un producto que está gravado con el IEPS (Impuesto Especial sobre Producción y Servicios). Este tipo de operaciones deben cumplir con los requisitos fiscales del SAT y con los criterios contables internos, especialmente por el manejo del IEPS.

La empresa utiliza el SDK para automatizar la emisión de las facturas, asegurando que el IEPS sea desglosado correctamente, y que la operación quede registrada como una cuenta por cobrar a favor de la empresa.

Para lograrlo, se hace uso de dos funciones principales del SDK:

- fAltaDocumento () que permite generar un documento con encabezado.
- fAltaMovimiento () que registra el movimiento del producto dentro del documento.

Configuración en el sistema de CONTPAQi Comercial Premium®

| Paso | Acción |
|---|---|
|  | Ingresa al menú Redefinir Empresa y selecciona la pestaña 6. Impuestos y retenciones. |
|  | Configura el impuesto IEPS como primer impuesto y en segundo lugar el IVA: |
|  | Ve al menú Configuración y elige la opción Conceptos. Selecciona el concepto que se utilizará para realizar las siguientes modificaciones: |
|  | Ingresa al catálogo de Clientes, para seleccionar la opción Desglosar IEPS en CFD: |
|  | Por último ingresa al catálogo de Productos para asignar el porcentaje correspondiente a cada uno de los impuestos aplicables: |
|  | Implementación C# private void factura_Click(object sender, EventArgs e) { int nError = 0; SDK.tDocumento ltDocumento = new SDK.tDocumento(); SDK.tMovimiento ltMovimiento = new SDK.tMovimiento(); int lIdDocumento = 0; int lIdMovimiento = 0; ltDocumento.aCodConcepto = "4"; //Concepto del documento ltDocumento.aCodigoCteProv = "003"; //Codigo del cliente ltDocumento.aSerie = ""; ltDocumento.aFolio = 0; ltDocumento.aFecha = DateTime.Today.ToString("MM/dd/yyyy"); ltDocumento.aSistemaOrigen = 205; ltDocumento.aNumMoneda = 1; ltDocumento.aTipoCambio = 1; ltDocumento.aReferencia = "Documento SDK"; nError = SDK.fAltaDocumento(ref lIdDocumento, ref ltDocumento); if (nError != 0) { MessageBox.Show(SDK.rError(nError)); } ltMovimiento.aCodAlmacen = "1"; ltMovimiento.aCodProdSer = "GAS "; //Código delproducto ltMovimiento.aUnidades = 5; ltMovimiento.aPrecio = 200; nError = SDK.fAltaMovimiento(lIdDocumento, ref lIdMovimiento, ref ltMovimiento); if (nError != 0) { MessageBox.Show(SDK.rError(nError)); } else { MessageBox.Show("Documento guardado con exito con folio "); } } |
|  | Al emitir la factura, los impuestos se muestran desglosados en el archivo XML: |