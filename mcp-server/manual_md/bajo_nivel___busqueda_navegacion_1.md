## Bajo nivel - Búsqueda/Navegación

fBuscarDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscarDocumento (aCodConcepto, aSerie, aFolio) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por valor | Código del concepto del documento. |  |
| aSerie | Cadena | Por valor | Serie del documento. |  |
| aFolio | Cadena | Por valor | Folio del documento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un documento por su llave, si lo encuentra se posiciona en el registro correspondiente.

**Ejemplo**

Buscar Documento

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR

aSerie: CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscarDocumento

FIN SI

}

fBuscarIdDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscarIdDocumento (aIdDocumento) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDocumento | Entero | Por valor | Identificador del documento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un documento por su identificador.

**Ejemplo**

Buscar Id Documento

{

VAR Error: ENTERO

Error = fBuscarIdDocumento recibe VAR aIdDocumento: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscarIdDocumento

FIN SI

}

fPosPrimerDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerDocumento () |
| Parámetros | No usa |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el primer registro de la tabla de documentos. |
| Ejemplo | Posicionar Primer Documento { VAR Error: ENTERO Error = fPosPrimerDocumento SI Error <> 0 ENTONCES Error SI NO fPosPrimerDocumento FIN SI } |

fPosUltimoDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoDocumento () |
| Parámetros | No usa |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el último registro de la tabla de documentos. |
| Ejemplo | Posicionar Ultimo Documento { VAR Error: ENTERO Error = fPosUltimoDocumento SI Error <> 0 ENTONCES Error SI NO fPosUltimoDocumento FIN SI } |

fPosSiguienteDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteDocumento () |
| Parámetros | No usa |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de documentos. |
| Ejemplo | Posicionar Siguiente Documento { VAR Error: ENTERO Error = fPosSiguienteDocumento SI Error <> 0 ENTONCES Error SI NO fPosSiguienteDocumento FIN SI } |

fPosAnteriorDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorDocumento () |
| Parámetros | No usa |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de documentos. |
| Ejemplo | Posicionar Anterior Documento { VAR Error: ENTERO Error = fPosAnteriorDocumento SI Error <> 0 ENTONCES Error SI NO fPosAnteriorDocumento FIN SI } |

fPosBOF ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOF () |
| Parámetros | No usa |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Documentos. |
| Ejemplo | Posicionar BOF { VAR Error: ENTERO Error = fPosBOF SI Error <> 0 ENTONCES Error SI NO fPosBOF FIN SI } |

fPosEOF ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOF () |
| Parámetros | No usa |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Documentos. |
| Ejemplo | Posicionar EOF { VAR Error: ENTERO Error = fPosEOF SI Error <> 0 ENTONCES Error SI NO fPosEOF FIN SI } |