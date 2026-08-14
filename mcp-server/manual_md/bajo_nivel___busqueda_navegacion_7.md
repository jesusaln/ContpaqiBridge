## Bajo nivel - Búsqueda/Navegación

fBuscaClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaClasificacion (aClasificacionDe, aNumClasificacion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aClasificacionDe | Entero | Por valor | Clasificación de 1 – Agente 2 – Cliente 3 – Proveedor 4 – Almacén 5 – Producto. |  |
| aNumClasificacion | Entero | Por valor | Número de la lasificacion (1-6) |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca una clasificación de acuerdo a los parámetros recibidos y se posiciona en el registro correspondiente.

**Ejemplo**

Busca clasificacion

{

VAR Error: ENTERO

Error = fBuscaClasificacion recibe PARAMETRO aClasificacionDe: ENTERO,PARAMETRO aNumClasificacion: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaClasificacion

FIN SI

}

fBuscaIdConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdConceptoDocto (aIdConcepto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdConcepto | Entero | Por valor | Identificador del concepto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca un concepto por su Identificador.

**Ejemplo**

Busca Id Concepto Documento

{

VAR Error: ENTERO

Error = fBuscaIdConceptoDocto recibe PARAMETRO aIdConcepto: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaIdConceptoDocto

FIN SI

}

fPosPrimerClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el primer registro de la tabla Clasificaciones. |
| Ejemplo | Posicion Primer Clasificacion { VAR Error: ENTERO Error = fPosPrimerClasificacion SI Error <> 0 ENTONCES Error SI NO fPosPrimerClasificacion FIN SI } |

fPosUltimoClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el último registro de la tabla Clasificaciones. |
| Ejemplo | Posicion Primer Clasificacion { VAR Error: ENTERO Error = fPosPrimerClasificacion SI Error <> 0 ENTONCES Error SI NO fPosPrimerClasificacion FIN SI } |

fPosSiguienteClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla Clasificaciones. |
| Ejemplo | Posicion Siguiente Clasificacion { VAR Error: ENTERO Error = fPosSiguienteClasificacion SI Error <> 0 ENTONCES Error SI NO fPosSiguienteClasificacion FIN SI } |

fPosAnteriorClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla Clasificaciones. |
| Ejemplo | Posicion Anterior Clasificacion { VAR Error: ENTERO Error = fPosAnteriorClasificacion SI Error <> 0 ENTONCES Error SI NO fPosAnteriorClasificacion FIN SI } |

fPosBOFClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOFClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla Clasificaciones. |
| Ejemplo | Posicion BOF Clasificacion { VAR Error: ENTERO Error = fPosBOFClasificacion SI Error <> 0 ENTONCES Error SI NO fPosBOFClasificacion FIN SI } |

fPosEOFClasificacion ()

| Disponibilidad | ACONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFClasificacion() |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla Clasificaciones. |
| Ejemplo | Posicion EOF Clasificacion { VAR Error: ENTERO Error = fPosEOFClasificacion SI Error <> 0 ENTONCES Error SI NO fPosEOFClasificacion FIN SI } |