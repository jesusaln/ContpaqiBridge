## Registrar y saldar pagos (REP)

Una empresa necesita automatizar la gestión de pagos de sus clientes para emitir los Recibos Electrónicos de Pago (REP) conforme a la normativa fiscal vigente. La integración del SDK del sistema administrativo permite agilizar el proceso de conciliación bancaria y garantizar la correcta aplicación de los pagos a las facturas correspondientes.

Para lograrlo, se utilizan las siguientes funciones clave del SDK:

- fAltaDocumento: Genera un documento de factura a crédito con los datos de la operación.
- fAltaMovimiento: Agrega los movimientos del documento generado.
- fAltaDocumentoCargoAbono: Permite registrar un documento de pago que afecte el saldo de un cliente.
- fSaldarDocumento_Param: Vincula el pago con la factura, saldando la deuda total o parcialmente.
- fSiguienteFolio: Obtiene el siguiente folio disponible para un concepto de documento, evitando duplicidades.
- fLeeDatoDocumento: Función para leer el dato del documento según se especifique el campo de la BD que quiera obtener. Funciones de bajo nivel:
- fEditarDocumento: Activa el documento en modo de edición.
- fSetDatoDocumento: Escribe el contenido de la variable valor en el campo de la tabla de documentos.
- fGuardaDocumento: Esta función se llama después de que se hace un set a un campo de la tabla de documentos.

Configuración en el sistema de CONTPAQi Factura Electrónica®

El siguiente código crea un documento de factura a crédito, tras darlo de alta por bajo nivel le indicamos que serán pagos en parcialidades mediante SDK utilizando las funciones de documentos de bajo nivel. Al haber indicado que el documento será a parcialidades se continúa y se da de alta el movimiento del documento.

Habiendo generado el documento de tipo factura a crédito, se crea un documento de cargo abono. Este documento de cargo abono será un pago del cliente con concepto de documento 10. Este documento nos entrega un folio y para poder extraerlo se emplea la función **fLeeDatoDocumento**, que obtiene el valor de **CFOLIO** del documento de cargo abono.

Una vez que se creó el documento cargo abono y obtuvimos el valor del folio, se realiza el saldado del documento inicial que es nuestra factura a crédito mediante la función **fSaldarDocumento_Param**.

| Paso | Acción |
|---|---|
|  | Se crea el documento factura Crédito concepto 4 |
|  | Se edita el método de pago: |
|  | Se crea documento de pago del cliente: |
|  | Al aplicar la función fSaldarDocumento_Param se asocia el documento de pago a la factura a crédito: |
|  | Implementación C# private void btnCargoAbono_Click(object sender, EventArgs e) { SDK.tDocumento lDocto = new SDK.tDocumento(); SDK.tMovimiento lMovto = new SDK.tMovimiento(); StringBuilder serie = new StringBuilder(); int nError = 0; double folio = 0; int idDocto = 0; int idMovto = 0; nError = SDK.fSiguienteFolio("4", serie, ref folio); if (nError != 0) { MessageBox.Show("Error 1: " + SDK.rError(nError)); } else { //Llenamos datos de estructura tDocumento para el documento de factura a credito lDocto.aCodConcepto = "4";//Factura credito lDocto.aFolio = folio; lDocto.aSerie = ""; lDocto.aFecha = DateTime.Today.ToString("MM/dd/yyyy"); lDocto.aCodigoCteProv = "CL01"; lDocto.aTipoCambio = 1; lDocto.aNumMoneda = 1; lDocto.aSistemaOrigen = 202;//202 factura, 205 comercial lDocto.aAfecta = 1; //Función para dar de alta de documento tipo factura a credito nError = SDK.fAltaDocumento(ref idDocto, ref lDocto); if (nError != 0) { MessageBox.Show("Error fAltaDocumento: " + SDK.rError(nError)); } else { MessageBox.Show("Documento creado con folio " + folio.ToString()); //Por bajo nivel le asigno el metodo de pago con el campo CCANTPARCI al documento credo SDK.fEditarDocumento(); SDK.fSetDatoDocumento("CCANTPARCI", "2");//Método de pago, el valor 1 = Pago en una sola exhibición y 2 = Pago en parcialidades o diferido SDK.fGuardaDocumento(); //Agregamos movimiento a mi factura utilizando la esctructura tMovimiento lMovto.aCodProdSer = "SER001"; lMovto.aPrecio = 1000; lMovto.aUnidades = 1; lMovto.aCodAlmacen = "1"; //Doy de alta el movimiento de mi documento nError = SDK.fAltaMovimiento(idDocto, ref idMovto, ref lMovto); if (nError != 0) { MessageBox.Show("Error fAltaMovimiento: " + SDK.rError(nError)); } else { MessageBox.Show("Movimiento creado"); //Crear documento de pago (cargo abono) del cliente concepto 10 con la función fAltaDocumentoCargoAbono double folioDos = 0; StringBuilder serieDos = new StringBuilder(); SDK.fSiguienteFolio("10", serieDos, ref folioDos);//Concepto 10 = Pago del cliente lDocto.aCodConcepto = "10"; lDocto.aFecha = DateTime.Today.ToString("MM/dd/yyyy"); lDocto.aCodigoCteProv = "CL01"; lDocto.aNumMoneda = 1; lDocto.aTipoCambio = 1; lDocto.aSistemaOrigen = 202;//202 factura, 205 comercial lDocto.aFolio = folioDos; lDocto.aImporte = 299; //Con los campos de la estructura para e concepto 10 se crea el documento cargo abono nError = SDK.fAltaDocumentoCargoAbono(ref lDocto); if (nError != 0) { MessageBox.Show("Error fAltaDocumento: " + SDK.rError(nError)); } else { //Obtenemos el folio del documento creado con funcion de bajo nivel fLeeDatoDocumento StringBuilder aValor = new StringBuilder(""); //Leemos el folio del documento cargo abono para poder utilizarlo al saldar el documento SDK.fLeeDatoDocumento("CFOLIO", aValor, 64); MessageBox.Show("Documento pago del cliente creado con folio " + aValor); double folioPago = Double.Parse(aValor.ToString()); string fechaPago = DateTime.Today.ToString("MM/dd/yyyy"); /*Utilizamos la función fSaldarDocumento_Param para saldar factura concepto 4 con el documento cargo abono creado concepto 10 En la función principalmente van los datos del documento a pagar y enseguida los del documento con el que voy a pagar*/ nError = SDK.fSaldarDocumento_Param("4", "", folio, "10", "", folioPago, 299, 1, fechaPago); if (nError != 0) { MessageBox.Show("Error fSaldarDocumento_Param: " + SDK.rError(nError)); } else { MessageBox.Show("Documento saldado"); } } } } } } |