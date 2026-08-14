Escenario 1: Documento con complemento previamente creado en el sistema

El documento ya existe en el sistema **CONTPAQi Comercial Premium®** y cuenta con el complemento Carta Porte 3.1 previamente generado y correctamente llenado.

El primer paso consiste en identificar los datos del documento que se desea emitir. Esta información puede obtenerse directamente desde el sistema **CONTPAQi Comercial Premium®** o mediante una implementación personalizada que consulte los datos a través del SDK.

|  | Nota La contraseña del CSD y el archivo adicional son datos personalizados; por lo tanto, no pueden consultarse mediante SDK y deben proporcionarse manualmente. |
|---|---|

| Paso | Acción |
|---|---|
|  | Documento asociado: |
|  | Complemento Carta Porte: |
|  | Emitir el documento utilizando la función fEmitirDocumento: #region EMITIR DOCUMENTO CARTA PORTE public static void EmitirDocumentoCP() { //variables //Datos correspondientes al documento que se va intentar emitir y que deben ser datos existentes dentro de su empresa. string aCodigoConcepto = "Factura4.0"; double aFolio = 21; string aSerie = "CP"; string aContraseña = "12345678a";//Contraseña del CSD configurado dentro del concepto asignado en aCodigoConcepto string aArchivoAdicional = ""; codigoDeError = MGWServicios.fEmitirDocumento(aCodigoConcepto, aSerie, aFolio, aContraseña, aArchivoAdicional); //en caso de que la función retorne un código diferente de 0 indicara que no se ejecutó con éxito if (codigoDeError != 0) { Console.WriteLine("Se genero el error " + codigoDeError); Console.WriteLine("Descripción: " + MGWServicios.rError(codigoDeError)); } else { Console.WriteLine("Documento emitido"); } } #endregion |

|  | Nota En el ejemplo los datos se asignan a variables locales, pero también pueden enviarse como parámetros a la función EmitirDocumentoCP. Si los parámetros enviados a la función fEmitirDocumento son correctos y cumplen con las reglas de llenado, el documento será emitido exitosamente. En caso contrario, se retornará un mensaje de error que indicará el motivo por el cual no pudo completarse la emisión. |
|---|---|