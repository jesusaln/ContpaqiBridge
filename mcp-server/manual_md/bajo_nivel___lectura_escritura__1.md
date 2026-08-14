## Bajo nivel – Lectura/Escritura

fInsertarDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertarDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Adiciona un nuevo registro en la tabla de Documentos en modo de inserción. |
| Ejemplo | fInsertarDocumento { VAR Error: ENTERO Error = fInsertarDocumento SI Error <> 0 ENTONCES Error SI NO fSetDatoDocumento recibe VAR aCampo: CADENA, VAR aValor: CADENA fGuardaDocumento FIN SI } |
| Comentarios: | Para que la función fInsertarDocumento pueda establecer un nuevo registro a la tabla de documentos, es necesario indicar mediante la función fSetDatoDocumento los registros de la tabla Documentos a afectar; por ejemplo: fSetDatoDocumento recibe VAR aCampo: CADENA, VAR aValor: CADENA Después de la inserción de los valores a afectar, se utiliza la función fGuardaDocumento, la cual no lleva parámetros; si no se utiliza esta función, no se agregará el nuevo registro a la tabla de documentos. fGuardaDocumento |

fEditarDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditarDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Activa el modo de edición de un registro en la tabla de Documentos. |
| Ejemplo | fEditarDocumento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fEditarDocumento fSetDatoDocumento recibe VAR aCampo: CADENA, VAR aValor: CADENA fGuardaDocumento FIN SI } |
| Comentarios | Para poder editar un documento, es necesario posicionarnos sobre él y esto se consigue llevando a cabo una búsqueda del documento. En la documentación se observa que utilizan la función fBuscaDocumento con el parámetro lLlaveDocto: fBuscaDocumento recibe lLlaveDocto Pero un método utilizado actualmente que realiza la misma funcionalidad, es la función fBuscarDocumento, que recibe 3 parámetros directamente; como se describe en el ejemplo a continuación: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fGuardaDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Guarda los cambios realizados a un documento. |
| Ejemplo | Guarda Documento { VAR Error: ENTERO fBuscarDocumento fEditarDocumento fSetDatoDocumento Error = fGuardaDocumento SI Error <> 0 ENTONCES Error SI NO fGuardaDocumento FIN SI } |
| Comentarios | Esta función no recibe parámetros; es utilizada cuando un documento recibe algún tipo de edición. Si no se utiliza la función fGuardaDocumento, no se aplicarán las modificaciones que se hayan realizado. |

fCancelarModificacionDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función cancela las modificaciones al registro actual de documentos. El registro debe estar en modo de edición o inserción. |
| Ejemplo | fCancelarModificacionDocumento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fCancelarModificacionDocumento FIN SI } |
| Comentarios | Es necesario realizar una búsqueda del documento, y si lo encuentra, aplica el procedimiento de cancelación a las modificaciones al registro actual de documentos: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fBorraDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBorraDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Borra un registro en la tabla de Documentos. |
| Ejemplo | fBorraDocumento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fBorraDocumento FIN SI } |
| Comentarios | Para poder borrar un documento, es necesario llevar a cabo una búsqueda del documento para posicionarse sobre él. En la documentación se observa que utilizan la función fBuscaDocumento con el parámetro lLlaveDocto fBuscaDocumento recibe lLlaveDocto Pero un método utilizado actualmente que realiza lo mismo, es la función fBuscarDocumento, que recibe 3 parámetros directamente, como se describe en el ejemplo a continuación: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fCancelaDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelaDocumento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función cancela documentos de CONTPAQi Comercial Premium®®. |
| Ejemplo | Cancela Documento { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fEditarDocumento fCancelaDocumento fGuardaDocumento FIN SI } |
| Comentarios | Para posicionarnos sobre el documento a cancelar, utilizamos la función fBuscarDocumento por sus parámetros aCodConcepto, aSerie, aFolio: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fBorraDocumento_CW ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBorraDocumento_CW () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Borra un documento de CONTPAQi Comercial Premium®® y si este estuviera contabilizado, también borra la póliza correspondiente en CONTPAQi® Contabilidad. |
| Ejemplo | Borra Documento CW { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fBorraDocumento_CW FIN SI } |
| Comentarios | Para posicionarnos sobre el documento a borrar, utilizamos la función fBuscarDocumento por sus parámetros aCodConcepto, aSerie, aFolio: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fCancelaDocumento_CW ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelaDocumento_CW () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función cancela un documento de CONTPAQi Comercial Premium®® y borra la poliza correspondiente en CONTPAQi® Contabilidad. |
| Ejemplo | Cancela Documento CW { VAR Error: ENTERO Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA SI Error <> 0 ENTONCES Error SI NO fCancelaDocumento_CW FIN SI } |
| Comentarios | Para posicionarnos sobre el documento a cancelar utilizamos la función fBuscarDocumento por sus parámetros aCodConcepto, aSerie, aFolio: fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA |

fAfectaDocto_Param ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAfectaDocto (aCodConcepto, aSerie, aFolio, aAfecta) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por valor | Código del concepto del documento. |  |
| aSerie | Cadena | Por valor | Serie del documento |  |
| aFolio | Doble | Por valor | Folio del documento |  |
| aAfecta | Lógico (Bool) | Por valor | Verdadero o falso. Afectar o desafectar. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función utiliza aCodConcepto, aSerie, y aFolio como llave del documento y aAfecta para afectar o desafectarlo.

**Ejemplo**

Afecta Documento Parámetros

{

VAR Error: ENTERO

Error = fAfectaDocto_Param recibe VAR aCodConcepto: CADENA, VAR aSerie:

CADENA, VAR aFolio: DOUBLE, VAR aAfecta: BOOL

SI

Error <> 0

ENTONCES

Error

SI NO

fAfectaDocto_Param

FIN SI

}

fSaldarDocumento_Param ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSaldarDocumento_Param (aCodConcepto_Pagar, aSerie_Pagar, aFolio_Pagar aCodConcepto_Pago, aSerie_Pago, aFolio_Pago, aImporte, aIdMoneda, aFecha) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto_Pagar | Cadena | Por valor | Código del concepto del documento a pagar. |  |
| aSerie_Pagar | Cadena | Por valor | Serie del documento a pagar. |  |
| aFolio_Pagar | Doble | Por valor | Folio del documento a pagar. |  |
| aCodConcepto_Pago | Cadena | Por valor | Código del concepto del documento que paga. |  |
| aSerie_Pago | Cadena | Por valor | Serie del documento que paga. |  |
| aFolio_Pago | Cadena | Por valor | Folio del documento que paga. |  |
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

Saldar Documento Parámetros

{

VAR Error: ENTERO

Error = fSaldarDocumento_Param recibe VAR aCodConcepto: CADENA,

VAR aSerie_Parar:

CADENA, VAR aFolio_Pagar: DOUBLE, VAR aCodConcepto_Pago:

CADENA, VAR aSerie_Pago: CADENA, VAR aFolio_Pago: DOUBLE, VAR aImporte:

DOUBLE, VAR aIdMoneda: INT, VAR aFecha: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fSaldarDocumento_Param

FIN SI

}

**Comentarios**

El parámetro aFolio_Pago está marcado como tipo CADENA: | Parámetros | Nombre | Tipo | Uso | Descripción |
|---|---|---|---|---|
|  | aFolio_Pago | Cadena | Por valor | Folio del documento que paga. |

Pero la situación real es que este tipo de dato es de tipo DOUBLE según el código fuente de la librería del SDK:

| Parámetros | Nombre | Tipo | Uso | Descripción |
|---|---|---|---|---|
|  | aFolio_Pago | Double | Por valor | Folio del documento que paga. |

fSaldarDocumento_Param recibe VAR aCodConcepto_Pagar: CADENA, VAR

aSerie_Parar: CADENA, VAR aFolio_Pagar: DOUBLE, VAR aCodConcepto_Pago:

CADENA, VAR aSerie_Pago: CADENA, VAR aFolio_Pago: DOUBLE

fBorrarAsociacion_Param ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBorrarAsociacion (aCodConcepto_Pagar, aSerie_Pagar, aFolio_Pagar CodConcepto_Pago, aSerie_Pago, aFolio_Pago) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto_Pagar | Cadena | Por valor | Código del concepto del documento pagado. |  |
| aSerie_Pagar | Cadena | Por valor | Serie del documento pagado. |  |
| aFolio_Pagar | Doble | Por valor | Folio del documento pagado. |  |
| aCodConcepto_Pago | Cadena | Por valor | Código del concepto del documento que pagó. |  |
| aSerie_Pago | Cadena | Por valor | Serie del documento que pagó. |  |
| aFolio_Pago | Cadena | Por valor | Folio del documento que pagó. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función la asociación de documentos.

**Ejemplo**

Borrar Asociación Parámetros

{

VAR Error: ENTERO

Error = fBorrarAsociacion_Param recibe VAR aCodConcepto_Pagar: CADENA, VAR

aSerie_Parar: CADENA, VAR aFolio_Pagar: DOUBLE, VAR aCodConcepto_Pago:

CADENA, VAR aSerie_Pago: CADENA, VAR aFolio_Pago: DOUBLE

SI

Error <> 0

ENTONCES

Error

SI NO

fSaldarDocumento_Param

FIN SI

}

**Comentarios**

El parámetro aFolio_Pago está marcado como tipo CADENA:

| Parámetros | Nombre | Tipo | Uso | Descripción |
|---|---|---|---|---|
|  | aFolio_Pago | Cadena | Por valor | Folio del documento que pagó. |

Pero la situación real es que este tipo de dato es de tipo DOUBLE según el código fuente de la librería del SDK:

| Parámetros | Nombre | Tipo | Uso | Descripción |
|---|---|---|---|---|
|  | aFolio_Pago | Double | Por valor | Folio del documento que pagó. |

fBorrarAsociacion_Param recibe VAR aCodConcepto_Pagar: CADENA, VAR

aSerie_Parar: CADENA, VAR aFolio_Pagar: DOUBLE, VAR aCodConcepto_Pago:

CADENA, VAR aSerie_Pago: CADENA, VAR aFolio_Pago: DOUBLE, VAR aImporte:

DOUBLE, VAR aIdMoneda: INT, VAR aFecha: CADENA

fSetDatoDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoDocumento (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de documentos.

**Ejemplo**

Set Dato Documento

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie:

CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEditarDocumento

fSetDatoDocumento recibe VAR aCampo: CADENA, VAR aValor: CADENA

fGuardaDocumento

FIN SI

}

**Comentarios**

Para poder setear un documento, es necesario posicionarnos sobre él y esto es llevándose a cabo una búsqueda del documento. En la documentación se observa que utilizan la función fBuscaDocumento con el parámetro lLlaveDocto:

fBuscaDocumento recibe lLlaveDocto

Pero un método utilizado actualmente que realiza la misma funcionalidad, es la función fBuscarDocumento que recibe 3 parámetros directamente, como se describe en el ejemplo y a continuación:

fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie:

CADENA, VAR aFolio: CADENA

Una vez encontrado, lo ponemos en modo de edición con la función fEditarDocumento para poder aplicar los cambios o actualizaciones correspondientes en modo de programación de bajo nivel.

Realizado lo anterior, se guardan las modificaciones realizadas empleando la función fGuardaDocumento:

fEditarDocumento

fSetDatoDocumento recibe VAR aCampo: CADENA, VAR aValor: CADENA

fGuardaDocumento

fLeeDatoDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoDocumento (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por referencia | Valor de escritura |  |
| alen | Entero | Por valor | Longitud del dato de lectura |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de documentos.

**Ejemplo**

Lee Dato Documento

{

VAR Error: ENTERO

Error = fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie:

CADENA, VAR aFolio: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoDocumento recibe VAR aCampo: CADENA, REFERENCIA aValor: CADENA,

VAR aLongitud: ENTERO

FIN SI

}

fSiguienteFolio ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSiguienteFolio(aCodigoConcepto, aSerie, aFolio ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoConcepto | Cadena | Por valor | Código del concepto del documento. |  |
| aSerie | Cadena | Por referencia | Serie del documento. |  |
| aFolio | Doble | Por referencia | Folio del documento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aSerie: Al finalizar la función este parámetro contiene el valor de la serie del documento especificado.

aFolio: Al finalizar la función este parámetro contiene el siguiente folio del documento especificado.

**Descripción**

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de documentos.

**Ejemplo**

Siguiente Folio

{

VAR Error: ENTERO

Error = fSiguienteFolio recibe VAR aCodConcepto: CADENA, REFERENCIA

aSerie:

CADENA, REFERENCIA aFolio: DOUBLE

SI

Error <> 0

ENTONCES

Error

SI NO

fSiguienteFolio

FIN SI

}

fSetFiltroDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetFiltroDocumento(aFechaInicio, aFechaFin, aCodigoConcepto, aCodigoCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aFechaInicio | Cadena | Por valor | Fecha inicial del rango. |  |
| aFechaFin | Cadena | Por valor | Fecha final del rango. |  |
| aCodigoConcepto | Cadena | Por valor | Código del concepto a filtrar. |  |
| aCodigoCteProv | Cadena | Por valor | Código del Cliente/Proveedor a filtrar. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función aplica un filtro a los documentos de acuerdo a su código y al código del cliente/proveedor en un rango de fechas especificados.

**Ejemplo**

Set Filtro Documento

{

VAR Error: ENTERO

Error fSetFiltroDocumento recibe VAR aFechaInicio: CADENA, VAR aFechaFin:

CADENA, VAR aCodigoConcepto: CADENA, VAR aCodigoCteProv: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fSetFiltroDocumento

FIN SI

}

fCancelaFiltroDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelaFiltroDocumento () |
| Parámetros | No usa |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función cancela el ultimo filtro activo de documentos. |
| Ejemplo | Cancela Filtro Documento { VAR Error: ENTERO Error = fCancelaFiltroDocumento SI Error <> 0 ENTONCES Error SI NO fCancelaFiltroDocumento FIN SI } |

fDocumentoImpreso ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fDocumentoImpreso (aImpreso) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aImpreso | Lógico (bool) | Por referencia | Valor lógico. Verdadero o falso. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función cambia la bandera de documento impreso.

Es necesario estar en el registro del documento que se quiere actualizar la bandera.

**Ejemplo**

Documento Impreso

{

VAR Error: ENTERO

Error = fDocumentoImpreso recibe REFERENCIA aImpreso: BOOL

SI

Error <> 0

ENTONCES

Error

SI NO

fDocumentoImpreso

FIN SI

}