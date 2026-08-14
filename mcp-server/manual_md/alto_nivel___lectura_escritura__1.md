## Alto nivel – Lectura/Escritura

fAltaDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fAltaDocumento (aIdDocumento, aDocumento ) |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. aIdDocumento: Al finalizar la función este parámetro contiene el identificador del nuevo documento. |
| Descripción | Esta función da de alta documentos de cargo o abono. |
| Ejemplo | AltaDocumento { REFERENCIA tDocumento: SDK VAR Error: ENTERO Error = fSiguienteFolio recibe VAR aCodigoConcepto: CADENA, REFERENCIA aSerie: CADENA, REFERENCIA aFolio: DOUBLE VAR aCodConcepto: tDocumento VAR aSerie: tDocumento VAR aCodClienteProveedor: tDocumento VAR aFecha: tDocumento Error = fAltaDocumento recibe REFERENCIA aIdDocumento: ENTERO, REFERENCIA tDocumento SI Error <> 0 ENTONCES Error SI NO fAltaDocumento FIN SI } |

fAltaDocumentoCargoAbono ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fAltaDocumentoCargoAbono (aDocumento) |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función da de alta documentos de cargo o abono. |
| Ejemplo | Documento Cargo Abono { REFERENCIA tDocumento: SDK VAR Error: ENTERO VAR aCodConcepto: tDocumento VAR aFolio: tDocumento VAR aSerie: tDocumento VAR aFecha: tDocumento VAR aCodClienteProveedor: tDocumento VAR aNumMoneda: tDocumento VAR aTipoCambio: tDocumento VAR aImporte: tDocumento Error = fAltaDocumentoCargoAbono recibe REFERENCIA tDocumento SI Error <> 0 ENTONCES Error SI NO fAltaDocumentoCargoAbono FIN SI } |

fAfectaDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAfectaDocto (aLlaveDocto, aAfecta) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aLlaveDocto | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |
| aAfecta | Lógico (Bool) | Por valor | Verdadero o falso. Afectar o desafectar. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función utiliza aLlaveDocto como llave del documento y aAfecta para afectar o desafectarlo.

**Ejemplo**

Afecta Documento

{

VAR Error: ENTERO

REFERENCIA lLlaveDocto: SDK

VAR aCodConcepto: tLlaveDocto

VAR aSerie: tLlaveDocto

VAR aFolio: tLlaveDocto

Error = fAfectaDocto recibe REFERENCIA tLlaveDocto, VAR aAfecta: BOOL)

SI

Error <> 0

ENTONCES

Error

SI NO

fAfectaDocto

FIN SI

}

fSaldarDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSaldarDocumento (aDoctoaPagar, aDoctoPago, aImporte, aIdMoneda, aFecha) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aDoctoaPagar | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |
| aDoctoPago | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |
| aImporte | Doble | Por valor | Importe del pago. |  |
| aIdMoneda | Entero | Por valor | Moneda del pago. |  |
| aFecha | Cadena | Por valor | Fecha del pago. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función asocia documentos y salda sus importes.

**Ejemplo**

Saldar Documento

{

REFERENCIA astDoctoaPagar : SDK.RegLlavedoc

REFERENCIA astDoctoPago : SDK.RegLlavedoc

VAR Error: ENTERO

Error = fSaldarDocumento recibe REFERENCIA astDoctoaPagar,

REFERENCIA astDoctoPago, VAR aImporte: DOUBLE, VAR aMoneda:

ENTERO, VAR aFecha: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fSaldarDocumento

FIN SI

}

fBorrarAsociacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBorrarAsociacion (aDoctoaPagar, aDoctoPago) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aDoctoaPagar | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |
| aDoctoPago | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función la asociación de documentos.

**Ejemplo**

Borrar Asociación

{

REFERENCIA astDoctoaPagar : SDK.RegLlavedoc

REFERENCIA astDoctoPago : SDK.RegLlavedoc

VAR Error: ENTERO

Error = fBorrarAsociacion recibe REFERENCIA astDocAPagar, REFERENCIA

astDocPago

SI

Error <> 0

ENTONCES

Error

SI NO

fBorrarAsociacion

FIN SI

}

fRegresaIVACargo ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaIVACargo (aLlaveDocto, aNetoTasa15, aNetoTasa10, aNetoTasaCero, aNetoTasaExcenta, aNetoOtrasTasas, aIVATasa15, aIVATasa10, aIVAOtrasTasas) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aLlaveDocto | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |
| aNetoTasa15 | Doble | Por referencia | Base de la tasa de 15% |  |
| aNetoTasa10 | Doble | Por referencia | Base de la tasa de 10% |  |
| aNetoTasaCero | Doble | Por referencia | Base de la tasa cero |  |
| aNetoTasaExcenta | Doble | Por referencia | Base de productos exentos |  |
| aNetoOtrasTasas | Doble | Por referencia | Base de otras tasas |  |
| aIVATasa15 | Doble | Por referencia | IVA de la tasa de 15% |  |
| aIVATasa10 | Doble | Por referencia | IVA de la tasa de 10% |  |
| aIVAOtrasTasas | Doble | Por referencia | IVA de otras tasas |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función regresa el desglose de IVA de un documento.

**Ejemplo**

Regresa IVA Cargo

{

REFERENCIA RegLlavedoc: SDK

VAR Error: ENTERO

Error = fRegresaIVACargo recibe REFERENCIA RegLlavedoc,

VAR aNetoTasa15: DOUBLE, VAR aNetoTasa10: DOUBLE, VAR

aNetoTasaCero: DOUBLE, VAR aNetoTasaExcenta: DOUBLE, VAR

aNetoOtrasTasas: DOUBLE, VAR aIVATasa15: DOUBLE, VAR aIVATasa10:

DOUBLE, VAR aIVAOtrasTasas: DOUBLE

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaIVACargo

FIN SI

}

fGetTamSelloDigitalYCadena ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fGetTamSelloDigitalYCadena (atPtrPassword, aEspSelloDig, aEspCadOrig) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| atPtrPassword | Cadena | Por referencia | Contraseña del certificado. |  |
| aEspSelloDig | Entero | Por referencia | Tamaño del Sello digital. |  |
| aEspCadOrig | Entero | Por referencia | Tamaño de la Cadena original. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Con esta función se obtiene el tamaño de la cadena original y el sello digital, mismas que se guardarán en las variables **aEspSelloDig** y **aEspCadOrig**.

**Ejemplo**

Get Tamaño Sello Digital y Cadena

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fGetTamSelloDigitalYCadena recibe REFERENCIA atPtrPassword:

CADENA, REFERENCIA aEspSelloDig: ENTERO, REFERENCIA

aEspCadOrig: ENTERO

FIN SI

}

fGetSelloDigitalYCadena ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fGetSelloDigitalYCadena (char *atPtrPassword, char* atPtrSelloDigital, char* atPtrCadenaOriginal) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| atPtrPassword | Cadena | Por referencia | Contraseña del certificado. |  |
| atPtrSelloDigital | Cadena | Por referencia | Sello digital. |  |
| atPtrCadenaOriginal | Cadena | Por referencia | Cadena original. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Con esta función se obtiene el sello digital y la cadena original de un CFD.

**Ejemplo**

Get Sello Digital y Cadena

{

VAR Error: ENTERO

Error = fBuscarDocumen recibe REFERENCIA atPtrPassword:

CADENA, REFERENCIA aPtrSelloDigital: CADENA, REFERENCIA

aPtrCadenaOriginal: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fGetSelloDigitalYCadena

FIN SI

}

fInicializaLicenseInfo()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fInicializaLicenseInfo (aSistema) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aSistema | Unsigned char | Por valor | Sistema: 1 = CONTPAQi Factura Electrónica® |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = • 0 (cero) que significa que se pudo conectar y obtener información del Servidor de Licencias, aunque la verificación del número de usuarios se hace hasta el uso de la función fEmitirDocumento.

!kSIN_ERRORES = • -1 que significa que hubo un error al intentar obtener información del Servidor de Licencias del sistema especificado.

**Descripción**

Esta función verifica que el sistema esté activado y tenga una licencia válida.

**Ejemplo**

Inicializa Licence Info****

{

VAR Error: ENTERO

Error = fInicializaLicenseInfo recibe VAR aSistema:BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

fInicializaLicenseInfo

FIN SI

}

fEmitirDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fEmitirDocumento (aCodConcepto, aSerie, aFolio, aPassword, aArchivoAdicional) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por referencia | Código del concepto |  |
| aSerie | Cadena | Por referencia | Serie del documento |  |
| aFolio | Doble | Por valor | Folio del documento |  |
| aPassword | Cadena | Por referencia | Contraseña del certificado de sello digital |  |
| aArchivoAdicional | Cadena | Por referencia | Nombre del archivo con el complemento, este archivo ya debe existir en la carpeta “Adicionales” dentro de la empresa. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) si no hubo error.

!kSIN_ERRORES = -1 que significa que hubo un error con la Licencia (la licencia es para menos de 10 usuarios, es temporal, de evaluación, no está activada, etc.)

!kSIN_ERRORES = Un número de error positivo del que se puede obtener la descripción con la función fError.

**Descripción**

Para poder utilizar la función fEmitirDocumento, se deberá ejecutar primero la función fInicializaLicenseInfo.

Esta función requiere una liciencia monousuario. Si cuentas con un licenciamiento anual además se requeire que la licencia sea multiempresa.

Esta función solo soporta las divisas, EstadoDeCuentaBancario, EstadoDeCuentaCombustible, PrestadoresDeServiciosDeCFD y la combinacion de estos.

**Ejemplo**

Emitir Documento****

{

VAR Error: ENTERO

Error = fInicializaLicenseInfo recibe VAR aSistema:BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

fEmitirDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: DOUBLE, VAR aPassword: CADENA,

VAR aArchivoAdicional: CADENA

FIN SI

}

fDocumentoUUID()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fDocumentoUUID (aCodigoConcepto, aSerie, aFolio, atPtrCFDIUUID) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por referencia | Código del concepto |  |
| aSerie | Cadena | Por referencia | Serie del documento |  |
| aFolio | Doble | Por valor | Folio del documento |  |
| atPtrCFDIUUID | Cadena | Por referencia | Cadena para colocar el valor de UUID |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función despliega el UUID de un documento.

**Ejemplo**

Documento UUID

{

VAR Error: ENTERO

Error = fDocumentoUUID recibe REFERENCIA aCodConcepto: CADENA,

REFERENCIA aSerie: CADENA, VAR aFolio: DOUBLE, REFERENCIA

atPtrCFDIUUID: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fDocumentoUUID

FIN SI

}

fGetSerieCertificado ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fGetSerieCertificado (atPtrPassword, aPtrSerieCertificado) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| atPtrPassword | Cadena | Por referencia | Contraseña del certificado |  |
| aPtrSerieCertificado | Cadena | Por referencia | Serie del certificado |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función regresa la serie de un certificado utilizado por una factura electrónica.

**Ejemplo**

Get Serie Certificado

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fGetSerieCertificado recibe REFERENCIA atPtrPassword: CADENA,

REFERENCIA aSerieCertificado: CADENA

FIN SI

}

fActivarPrecioCompra ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fActivarPrecioCompra (aActivar) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aActivar | Entero | Por valor | 0 = No busca el precio 1 = Valor asumido (busca el precio) |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función determina si al momento de registrar una compra vía SDK se ejecutará la función que busca el último precio de compra registrado en caso de que el precio sea igual a cero.

fDocumentoDevuelto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fDocumentoDevuelto (aDevuelto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aDevuelto | Entero | Por valor | 0 = No busca el precio 1 = Valor asumido (busca el precio) |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función ajusta el estado de un documento en devuelto o no devuelto.

**Ejemplo**

Documento Devuelto

{

VAR Error: ENTERO

Error = fDocumentoDevuelto recibe REFERENCIA aDevuelto: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fDocumentoDevuelto

FIN SI

}

fEntregEnDiscoXML ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fEntregEnDiscoXML (aCodConcepto, aSerie, aFolio, aFormato, aFormatoAmig) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por referencia | Código del concepto |  |
| aSerie | Cadena | Por referencia | Serie del documento |  |
| aFolio | Doble | Por valor | Folio del documento |  |
| aFormato | Entero | Por valor | Formato de entrega (0 = XML, 1 = PDF) Nota: Al seleccionar la opción de entrega 1= PDF, por disposición fiscal también se generará el XML. |  |
| aFormatoAmig | Cadena | Por referencia | Plantilla de impresión |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función entrega el XML en un archivo.

**Ejemplo**

Entrega en Disco XML

{

VAR Error: ENTERO

Error = fEntregEnDiscoXML recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: DOUBLE, VAR aFormato: ENTERO, VAR

aFormatoAmigable: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEntregEnDiscoXML

FIN SI

}

fObtieneDatosCFDI ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fDocumentoDevuelto (aDevuelto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| atPtrPassword | Cadena | Por referencia | Contraseña del certificado |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

La función **fObtieneDatosCFDI** obtiene los datos del CFDI del documento previamente definido con la función **fBuscarDocumento**. Esta función almacena en variables globales los datos del CFDI dentro del mismo SDK para posteriormente ser leídos con la función **fLeeDatoCFDI**.

**Ejemplo**

Obtiene y Lee Dato CFDI

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fObtieneDatosCFDI recibe VAR atPtrPassword

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fLeeDatoCFDI recibe REFERENCIA aValor: CADENA,

VAR aDato: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoCFDI

FIN SI

FIN SI

FIN SI

}

fLeeDatoCFDI ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoCFDI (aValor, aDato) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aValor | Cadena | Por referencia | Cadena donde se regresará el dato requerido |  |
| aDato | Entero | Por valor | 1 = Serie del Certificado del Emisor 2 = Folio Fiscal (UUID) 3 = Número de Serie del Certificado del SAT 4 = Fecha y Hora de Certificación 5 = Sello Digital del CFDI 6 = Sello SAT 7 = Cadena Original del Complemento de Certificación Digital del SAT 8 = Método de Pago 9 = Lugar de expedición 10 = Régimen Fiscal 11 = Folio Fiscal de origen* 12 = Serie del Folio Fiscal de origen* 13 = Fecha del Folio Fiscal de origen* 14 = Monto del Folio Fiscal de origen* * Para documentación de Deuda o Pago en Parcialidades |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

La función fLeeDatoCFDI lee los datos previamente accedidos con la función fObtieneDatosCFDI.

La función recibe como parámetros, la cadena donde copiará el dato requerido y un entero donde se indica qué dato se desea y regresará un número de error en caso de existir alguno.

**Ejemplo**

Obtiene y Lee Dato CFDI

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fObtieneDatosCFDI recibe VAR atPtrPassword

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fLeeDatoCFDI recibe REFERENCIA aValor: CADENA,

VAR aDato: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoCFDI

FIN SI

FIN SI

FIN SI

}