## Bajo nivel - Búsqueda/Navegación

fSetFiltroMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetFiltroMovimiento(aIdDocumento ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDocumento | Long | Por valor | Identificador del documento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función aplica un filtro de movimientos de acuerdo al documento indicado.

**Ejemplo**

Set Filtro Movimiento

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA,

VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fSetFiltroMovimiento recibe VAR aIdDocumento: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fSetFiltroMovimiento

FIN SI

FIN SI

}

fCancelaFiltroMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelaFiltroMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función aplica un filtro de movimientos de acuerdo al documento indicado. |
| Ejemplo | Cancela Filtro Movimiento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fSetFiltroMovimiento recibe VAR aIdDocumento: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fCancelaFiltroMovimiento SI Error <> 0 ENTONCES Error SI NO fCancelaFiltroMovimiento FIN SI FIN SI FIN SI } |

fBuscarIdMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscarIdMovimiento (aIdMovimiento) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aaIdMovimiento | Entero largo | Por valor | Identificador del documento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un movimiento por su identificador. Si lo encuentra se posiciona en el registro correspondiente.

**Ejemplo**

Buscar Id Movimiento

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA,

VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fBuscarIdMovimiento recibe VAR aIdMovimiento: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscarIdMovimiento

FIN SI

FIN SI

}

fPosPrimerMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el primer registro de la tabla de movimientos. |
| Ejemplo | Posicionar Primer Movimiento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosPrimerMovimiento SI Error <> 0 ENTONCES Error SI NO Error = fLeeDatoMovimiento recibe VAR aCampo: CADENA, REFERENCIA aValor, VAR aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO fLeeDatoMovimiento FIN SI FIN SI FIN SI } |

fPosUltimoMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el último registro de la tabla de documentos. |
| Ejemplo | Posicionar Ultimo Movimiento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosUltimoMovimiento SI Error <> 0 ENTONCES Error SI NO Error = fLeeDatoMovimiento recibe VAR aCampo: CADENA, REFERENCIA aValor, VAR aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO fLeeDatoMovimiento FIN SI FIN SI FIN SI } |

fPosSiguienteMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de documentos. |
| Ejemplo | Posicionar Siguiente Movimiento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosSiguienteMovimiento SI Error <> 0 ENTONCES Error SI NO Error = fLeeDatoMovimiento recibe VAR aCampo: CADENA, REFERENCIA aValor, VAR aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO fLeeDatoMovimiento FIN SI FIN SI FIN SI } |

fPosAnteriorMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de documentos. |
| Ejemplo | Posicionar Anterior Movimiento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosAnteriorMovimiento SI Error <> 0 ENTONCES Error SI NO Error = fLeeDatoMovimiento recibe VAR aCampo: CADENA, REFERENCIA aValor, VAR aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO fLeeDatoMovimiento FIN SI FIN SI FIN SI } |

fPosMovimientoBOF ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosMovimientoBOF () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Movimientos. |
| Ejemplo | Posicionar Movimiento BOF { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosMovimientoBOF SI Error <> 0 ENTONCES Error SI NO fPosMovimientoBOF FIN SI FIN SI } |

fPosMovimientoEOF ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosMovimientoEOF () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Documentos. |
| Ejemplo | Posicionar Movimiento EOF{ VAR Error: ENTERO Error = fBuscarDocumento recibe PARAMETRO aCodConcepto: CADENA, PARAMETRO aSerie: CADENA, PARAMETRO aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO Error = fPosMovimientoEOF SI Error <> 0 ENTONCES Error SI NO fPosMovimientoEOF FIN SI FIN SI } |