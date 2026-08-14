## Bajo nivel – Lectura/Escritura

fInsertaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertaValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Adiciona un nuevo registro en la tabla de Valores de Clasificación en modo de inserción. |
| Ejemplo | Inserta Valor Clasificacion { VAR Error: ENTERO Error = fInsertaValorClasif SI Error <> 0 ENTONCES Error SI NO fInsertaValorClasif FIN SI } |

fEditaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Valores de Clasificación. |
| Ejemplo | Edita Valor Clasificacion { VAR Error: ENTERO Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fEditaValorClasif SI Error <> 0 ENTONCES Error SI NO Error = fEditaValorClasif FIN SI FIN SI } |

fGuardaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Guarda los cambios realizados a un registro de Valores de Clasificación. |
| Ejemplo | Guarda Valor Clasificacion { VAR Error: ENTERO Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fEditaValorClasif SI Error <> 0 ENTONCES Error SI NO fEditaValorClasif Error = fSetDatoValorClasif recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA SI Error <> 0 ENTONCES Error SI NO fSetDatoValorClasif Error = fGuardaValorClasif SI Error <> 0 ENTONCES Error SI NO fGuardaValorClasif FIN SI FIN SI FIN SI FIN SI } |

fBorraValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBorraValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Borra un registro en la tabla de Valores de Clasificación. |
| Ejemplo | Borra Valor Clasificacion { VAR Error: ENTERO Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fBorraValorClasif SI Error <> 0 ENTONCES Error SI NO fBorraValorClasif FIN SI FIN SI } |

fCancelarModificacionValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionValorClasif () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función cancela las modificaciones al registro actual de Valores de Clasificación. El registro debe estar en modo de edición o inserción. |
| Ejemplo | Cancela Modificacion Clasificacion { VAR Error: ENTERO Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fEditaValorClasif SI Error <> 0 ENTONCES Error SI NO fEditaValorClasif Error = fCancelarModificacionClasificacion SI Error <> 0 ENTONCES Error SI NO fCancelarModificacionClasificacion FIN SI FIN SI FIN SI } |

fEliminarValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fEliminarValorClasif (aClasificacionDe, aNumClasificacion, aCodValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aClasificacionDe | Entero | Por valor | Clasificación de 1 – Agente 2 – Cliente 3 – Proveedor 4 – Almacen 5 – Producto. |  |
| aNumClasificacion | Entero | Por valor | Numero de la clasificacion (1-6) |  |
| aCodValorClasif | Cadena | Por valor | Código del valor clasificación producto |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función elimina un registro de la tabla Valores de Clasificación usando su código.

**Ejemplo**

Eliminar Valor Clasificacion

{

VAR Error: ENTERO

Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fEliminarValorClasif recibe PARAMETRO aClasificacionDe: ENTERO, PARAMETRO aNumClasificacion: ENTERO, PARAMETRO aCodigoValorClasificacion: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEliminarValorClasif

FIN SI

FIN SI

}

fSetDatoValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoValorClasif (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de Valores de Clasificación.

**Ejemplo**

Set Dato Valor Clasificacion

{

VAR Error: ENTERO

Error = fBuscaIdValorClasif recibe PARAMETRO aIdValorClasif: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fEditaValorClasif

SI

Error <> 0

ENTONCES

Error

SI NO

fEditaValorClasif

Error = fSetDatoValorClasif recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

FSetDatoValorClasif

FIN SI

FIN SI

FIN SI

}