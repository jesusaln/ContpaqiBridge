## Bajo nivel - Búsqueda/Navegación

fLeeDatoValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoValorClasif (aCampo, aValor, aLen) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |
| aLen | Entero | Por valor | Longitud del dato de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de Valores de Clasificación.

**Ejemplo**

Lee Dato Valor Clasificacion

{

VAR Error: ENTERO

VAR aValor: STRINGBUILDER

Error = fLeeDatoValorClasif recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoValorClasif

FIN SI

}

fBuscaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaClasificacion (aClasificacionDe, aNumClasificacion, aCodValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aClasificacionDe | Entero | Por valor | Clasificación de 1 – Agente 2 – Cliente 3 – Proveedor 4 – Almacen 5 – Producto. |  |
| aNumClasificacion | Entero | Por valor | Numero de la clasificacion (1-6) |  |
| aCodValorClasif | Cadena | Por valor | Código del Valor Clasificacion Producto |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca una clasificacion de acuerdo a los parámetros recibidos y se posiciona en el registro correspondiente.

**Ejemplo**

Busca Valor Clasificacion

{

VAR Error: ENTERO

Error = fBuscaValorClasif recibe PARAMETRO aClasificacionDe: ENTERO, PARAMETRO aNumClasificacion: ENTERO, PARAMETRO aCodValorClasif: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaValorClasif

FIN SI

}

fBuscaIdValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdValorClasif (aIdValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdValorClasif | Entero | Por valor | Identificador del valor de clasificación. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca un valor de clasificación por su Identificador y se posiciona en el registro correspondiente.

**Ejemplo**

Busca Id Valor Clasificacion

{

VAR Error: ENTERO

Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaIdValorClasif

FIN SI

}

fPosPrimerValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el primer registro de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion Primer Valor Clasificacion { VAR Error: ENTERO Error = fPosPrimerValorClasif SI Error <> 0 ENTONCES Error SI NO fPosPrimerValorClasif FIN SI } |

fPosUltimoValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el último registro de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion Ultimo Valor Clasificacion { VAR Error: ENTERO Error = fPosUltimoValorClasif SI Error <> 0 ENTONCES Error SI NO fPosUltimoValorClasif FIN SI } |

fPosUltimoValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el último registro de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion Siguiente Valor Clasificacion { VAR Error: ENTERO Error = fPosSiguienteValorClasif SI Error <> 0 ENTONCES Error SI NO fPosSiguienteValorClasif FIN SI } |

fPosSiguienteValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion Siguiente Valor Clasificacion { VAR Error: ENTERO Error = fPosSiguienteValorClasif SI Error <> 0 ENTONCES Error SI NO fPosSiguienteValorClasif FIN SI } |

fPosAnteriorValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion Anterior Valor Clasificacion { VAR Error: ENTERO Error = fPosAnteriorValorClasif SI Error <> 0 ENTONCES Error SI NO fPosAnteriorValorClasif FIN SI } |

fPosBOFValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOFValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion BOF Valor Clasificacion { VAR Error: ENTERO Error = fPosBOFValorClasif SI Error <> 0 ENTONCES Error SI NO fPosBOFValorClasif FIN SI } |

fPosEOFValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Valores de Clasificación. |
| Ejemplo | Posicion EOF Valor Clasificacion { VAR Error: ENTERO Error = fPosEOFValorClasif SI Error <> 0 ENTONCES Error SI NO fPosEOFValorClasif FIN SI } |