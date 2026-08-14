## Registrar una relación CFDI mediante UUID

En algunos escenarios, el documento con el que se desea relacionar un CFDI no existe dentro de la empresa o fue generado en otro sistema. En estos casos, la autoridad permite establecer la relación utilizando directamente el UUID del CFDI previamente timbrado.

La función**fAgregarRelacionCFDI2** facilita este proceso, ya que permite registrar una o varias relaciones mediante los UUID de los comprobantes fiscales, sin necesidad de que estos existan como documentos dentro del sistema administrativo.

Para lograrlo, se hace uso de dos funciones principales del SDK:

- fAgregarRelacionCFDI2() que registra la relación CFDI utilizando directamente el UUID del comprobante relacionado.
- fEmitirDocumento() que timbra el documento incorporando las relaciones CFDI previamente registradas.

Configuración en el sistema de CONTPAQi Comercial Premium®

El siguiente ejemplo parte de un documento previamente creado dentro del sistema, en este caso una **nota de crédito**, sobre la cual se agregará una relación CFDI.

La función **fAgregarRelacionCFDI2** recibe el concepto, serie y folio del documento que será timbrado, el tipo de relación SAT y el UUID del CFDI con el que se desea establecer la relación.

Una vez registrada correctamente la relación, se utiliza la función **fEmitirDocumento** para realizar el timbrado del documento.

Durante este proceso el SDK incorpora automáticamente el UUID relacionado dentro del nodo **CfdiRelacionados** del XML generado.

| Paso | Acción |
|---|---|
|  | CFDI a relacionar: |
|  | Al ejecutar fAgregarRelacionCFDI2 se relaciona el UUID: |
|  | Se emite documento con fEmitirDocumento de la nota de crédito: |
|  | La relación está completa: |
|  | Implementación C# private void btnAgregaRelacionCFDI2_Click(object sender, EventArgs e) { int nError = 0; //Datos del documento al que relacionaré el UUID string aCodConceptoR = "8";//Nota de crédito string aSerieR = ""; string aFolioR = "14"; string aTipoRelacion = "01"; string aUUID = "00000000-3357-4eca-922a-20bacc0431f5";//UUID a relacionar del documento timbrado timbrado //Se agrega la relación nError = SDK.fAgregarRelacionCFDI2(aCodConceptoR, aSerieR, aFolioR, aTipoRelacion, aUUID); if (nError != 0) { MessageBox.Show("Error: " + SDK.rError(nError)); } else { MessageBox.Show("UUID: " + aUUID + " relacionado"); //Se timbra la nota de crédito nError = SDK.fEmitirDocumento(aCodConceptoR, aSerieR, Convert.ToDouble(aFolioR), "12345678a", ""); if (nError != 0) { MessageBox.Show("Error: " + SDK.rError(nError)); } else { MessageBox.Show("Documento timbrado exitosamente"); } } } |