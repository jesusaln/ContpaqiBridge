## Bajo nivel – Lectura/Escritura

fInsertarMovimiento () | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertarMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Adiciona un nuevo registro en la tabla de Movimientos en modo de inserción. |
| Ejemplo | Insertar Movimiento { VAR Error: ENTERO Error = fInsertarDocumento Error = fInsertarMovimiento SI Error <> 0 ENTONCES Error SI NO fSetDatoMovimiento recibe VAR aCampo: CADENA, VAR aValor:CADENA fGuardaMovimiento FIN SI } |

fEditarMovimiento () | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditarMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Movimientos. |
| Ejemplo | Editar Movimiento { VAR Error: ENTERO fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie:CADENA, VAR aFolio: CADENA Error = fBuscarMovimiento recibe VAR aIdMovimiento: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fEditarMovimiento SI Error <> 0 ENTONCES Error SI NO fSetDatoMovimiento recibe VAR aCampo: CADENA, VAR aValor: CADENA fGuardaMovimiento FIN SI FIN SI } |

fGuardaMovimiento ()**** | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Guarda los cambios realizados a un movimiento. |
| Ejemplo | El siguiente código indica a la aplicación que guarde cierto registro en la tabla de Documentos. Esta función se llama después de que se utiliza la función fInsertarMovimiento() o fEditarMovimiento() y se graban los valores en los campos correspondientes. Guarda Movimiento{ VAR Error: ENTERO fBuscarDocumento fBuscarIdMovimiento SI Error <> 0 ENTONCES Error SI NO fEditarMovimiento fSetDatoMovimiento fGuardaMovimiento FIN SI } |

fCancelaCambiosMovimiento ()**** | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelaCambiosMovimiento () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función cancela las modificaciones al registro actual de movimientos. El registro debe estar en modo de edición o inserción. |
| Ejemplo | Cancela Cambios Movimiento { VAR Error: ENTERO fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie: CADENA, VAR aFolio: CADENA Error = fBuscarMovimiento recibe VAR aIdMovimiento: ENTERO SI Error <> 0 ENTONCES Error SI NO Error = fEditarIdMovimiento SI Error <> 0 ENTONCES Error SI NO fCancelaCambiosMovimiento FIN SI FIN SI } |

fAltaMovimientoCaracteristicas_Param ()**** | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoCaracteristicas_Param (aIdMovimiento, aIdMovtoCaracteristicas, aUnidades, aValorCaracteristica1, aValorCaracteristica2, aValorCaracteristica3) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Cadena | Por valor | Identificador del movimiento. |  |
| aIdMovtoCaracteristicas | Cadena | Por valor | Identificador del movimiento con características. |  |
| aUnidades | Cadena | Por valor | Unidades. |  |
| aValorCaracteristica1 | Cadena | Por valor | Valor de la característica 1. |  |
| aValorCaracteristica2 | Cadena | Por valor | Valor de la característica 2. |  |
| aValorCaracteristica3 | Cadena | Por valor | Valor de la característica 3. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función inserta un movimiento con características.

**Ejemplo**

Alta Movimiento Caracteristicas Parametros

{

VAR Error: ENTERO

fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie:

CADENA, VAR aFolio: CADENA

Error = fAltaMovimientoCaracteristicas_Param recibe VAR aIdMovimiento:

CADENA, VAR aIdMovtoCaracteristicas: CADENA, VAR aUnidades: CADENA,

VAR aValorCaracteristica1: CADENA, VAR aValorCaracteristica2: CADENA,

VAR aValorCaracteristica3: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaMovimientoCaracteristicas_Param

FIN SI

}

fAltaMovtoCaracteristicasUnidades_Param ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovtoCaracteristicasUnidades_Param (aIdMovimiento, aIdMovtoCaracteristicas, aUnidad, aUnidades, aUnidadesNC, aValorCaracteristica1, aValorCaracteristica2, aValorCaracteristica3) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Cadena | Por valor | Identificador del movimiento. |  |
| aIdMovtoCaracteristicas | Cadena | Por valor | Identificador del movimiento con características. |  |
| aUnidad | Cadena | Por valor | Abreviatura de la unidad de compra venta |  |
| aUnidades | Cadena | Por valor | Las unidades del movimiento de características. |  |
| aUnidadesNC | Cadena | Por valor | Abreviatura de la unidad de compra venta no convertible. |  |
| aValorCaracteristica1 | Cadena | Por valor | Valor de la característica 1. |  |
| aValorCaracteristica2 | Cadena | Por valor | Valor de la característica 2. |  |
| aValorCaracteristica3 | Cadena | Por valor | Valor de la característica 3. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función da de alta movimiento de características con unidades de compra venta.

**Ejemplo**

Alta Movimiento Caracteristicas Parametros

{

VAR Error: ENTERO

fBuscarDocumento recibe VAR aCodConcepto: CADENA, VAR aSerie

CADENA, VAR aFolio: CADENA

Error = fAltaMovtoCaracteristicasUnidades_Param recibe VAR

aIdMovimiento: CADENA, VAR aIdMovtoCaracteristicas: CADENA, VAR

aUnidad: CADENA, VAR aUnidades: CADENA, VAR aUnidadesNC: CADENA,

VAR aValorCaracteristica1: CADENA, VAR aValorCaracteristica2: CADENA,

VAR aValorCaracteristica3: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaMovtoCaracteristicasUnidades_Param

FIN SI

}

fAltaMovimientoSeriesCapas_Param ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoSeriesCapas _Param (aIdMovimiento, aUnidades, aTipoCambio, aSeries, aPedimento, aAgencia, aFechaPedimento, aNumeroLote, aFechaFabricacion, aFechaCaducidad) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Cadena | Por valor | Identificador del movimiento. |  |
| aUnidades | Cadena | Por valor | Unidad de peso y medida. |  |
| aTipoCambio | Cadena | Por valor | Tipo de cambio. |  |
| aSeries | Cadena | Por valor | Series. |  |
| aPedimento | Cadena | Por valor | Referencia del pedimento. |  |
| aAgencia | Cadena | Por valor | Referencia de la agencia. |  |
| aFechaPedimento | Cadena | Por valor | Fecha del pedimento. |  |
| aNumeroLote | Cadena | Por valor | Número de lote. |  |
| aFechaFabricacion | Cadena | Por valor | Fecha de fabricación. |  |
| aFechaCaducidad | Cadena | Por valor | Fecha de caducidad. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función agrega el movimiento de numero de serie, lote y/o pedimento asociados un movimiento cuyo producto maneje cualquiera de estas posibles configuraciones.

**Ejemplo**

Alta Movimiento Series Capas Parametros

{

VAR Error: ENTERO

fAltaDocumento

fAltaMovimiento

Error = fAltaMovimientoSeriesCapas_Param recibe VAR aIdMovimiento:

CADENA, VAR aUnidades: CADENA, VAR aTipoCambio: CADENA, VAR

aSeries: CADENA, VAR aPedimento: CADENA, VAR aAgencia: CADENA, VAR

aFechaPedimento: CADENA, VAR aNumeroLote: CADENA, VAR

aFechaFabricacion: CADENA, VAR aFechaCaducidad: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaMovimientoSeriesCapas_Param

FIN SI

}

fCalculaMovtoSerieCapa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fCalculaMovtoSerieCapa (aIdMovimiento) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Eentero largo | Por valor | Identificador del movimiento a recalcular. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función recalcula el movimiento cuando este pertenece a un producto con series, lotes o pedimentos.

**Ejemplo**

Calcula Movimiento Series Capas

{

VAR Error: ENTERO

fAltaDocumento

fAltaMovimiento

fAltaMovimientoSeriesCapas_Param

Error = fCalculaMovtoSerieCapa recibe VAR lIdMovimiento: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fCalculaMovtoSerieCapa

FIN SI

}

fObtieneUnidadesPendientes ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fObtieneUnidadesPendientes (aConceptoDocto, aCodigoProducto, aCodigoAlmacen, Unidades) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aConceptoDocto | Cadena | Por valor | Código del concepto del documento a buscar. |  |
| aCodigoProducto | Cadena | Por valor | Código del producto a buscar su unidades pendientes. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén a buscar si es igual a 0 (cero) busca en todos los almacenes. |  |
| aUnidades | Cadena | Por referencia | Valor de retorno con las unidades pendientes. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aUnidades: Al finalizar la función este parámetro contiene las unidades pendientes.

**Descripción**

Esta función obtiene la cantidad de unidades pendientes de cierto concepto de documento para un almacén/almacenes de un determinado producto en toda la historia del sistema.

**Ejemplo**

Obtiene Unidades Pendientes

{

VAR Error: ENTERO

Error = fObtieneUnidadesPendientes recibe VAR aConceptoDocto: CADENA,

VAR aCodigoProducto: CADENA, VAR aCodigoAlmacen: CADENA,

REFERENCIA: aUnidades: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fObtieneUnidadesPendientes

FIN SI

}

fObtieneUnidadesPendientesCarac ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fObtieneUnidadesPendientesCarac (aConceptoDocto, aCodigoProducto, aCodigoAlmacen, aValorCaracteristica1, aValorCaracteristica2, aValorCaracteristica3, aUnidades) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aConceptoDocto | Cadena | Por valor | Código del concepto del documento a buscar. |  |
| aCodigoProducto | Cadena | Por valor | Código del producto a buscar su unidades pendientes. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén a buscar si es igual a 0 (cero) busca en todos los almacenes. |  |
| aValorCaracteristica1 | Cadena | Por valor | Valor característica 1 |  |
| aValorCaracteristica2 | Cadena | Por valor | Valor característica 2 |  |
| aValorCaracteristica3 | Cadena | Por valor | Valor característica 3 |  |
| aUnidades | Cadena | Por referencia | Valor de retorno con las unidades pendientes. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aUnidades: Al finalizar la función este parámetro contiene las unidades pendientes.

**Descripción**

Esta función obtiene la cantidad de unidades pendientes de cierto concepto de documento para un almacén/almacenes de un determinado producto con caracteristicas en toda la historia del sistema.

**Ejemplo**

Obtiene Unidades Pendientes Caracteristicas

{

VAR Error: ENTERO

Error = fObtieneUnidadesPendientesCarac recibe VAR aConceptoDocto:

CADENA, VAR aCodigoProducto: CADENA, VAR aCodigoAlmacen: CADENA,

VAR aValorCaracteristica1: CADENA, VAR aValorCaracteristica2: CADENA,

VAR aValorCaracteristica3: CADENA, REFERENCIA: aUnidades: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fObtieneUnidadesPendientesCarac

FIN SI

}

fModificaCostoEntrada ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fModificaCostoEntrada (aIdMovimiento, aCostoEntrada) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Cadena | Por valor | Identificador del movimiento a modificar. |  |
| aCostoEntrada | Cadena | Por valor | Valor del costo a asignar al movimiento. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función modifica el costo de una entrada de inventario.

**Ejemplo**

Modifica Costo Entrada

{

VAR Error: ENTERO

Error = fModificaCostoEntrada recibe VAR aIdMovimiento: CADENA, VAR

aCostoEntrada: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fModificaCostoEntrada

FIN SI

}

fSetDatoMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoMovimiento (aCampo, aValor) |  |  |  |
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

Set Dato Movimiento

{

VAR Error: ENTERO

fBuscarDocumento

fBuscarIdMovimiento

Error = fEditarMovimiento

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fSetDatoMovimiento recibe VAR aCampo: CADENA, VAR aValor: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fGuardaMovimiento

FIN SI

FIN SI

}

fLeeDatoMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoMovimiento (aCampo, aValr, aLen) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por referencia | Valor de escritura |  |
| aLen | Entero | Por valor | Longitud del dato de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de documentos.

**Ejemplo**

Lee Dato Movimiento

{

VAR Error: ENTERO

fBuscarDocumento

fBuscarIdMovimiento

Error = fLeeDatoMovimiento recibe VAR aCampo: CADENA, REFERENCIA

aValor: CADENA, VAR aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoMovimiento

FIN SI

}