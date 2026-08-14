## Alto nivel – Lectura/Escritura

fAltaMovimiento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimiento (aIdDocumento, aIdMovimiento, astMovimiento) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDocumento | Entero largo | Por valor | Identificador del movimiento. |  |
| aIdMovimiento | Entero largo | Por referencia | Identificador del documento. |  |
| astMovimiento | tMovimiento | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdMovimiento: Al finalizar la función este parámetro contiene el identificador del nuevo movimiento.

**Descripción**

Esta función da de alta un nuevo registro en la tabla de Movimientos.

**Ejemplo**

Crear_movimiento( recibe ENTERO idDocumento )

{

VAR idMovimiento: ENTERO

OBJETO aMovimiento: tMovimiento

VAR aConsecutivo : aMovimiento

VAR aUnidades : aMovimiento

VAR aPrecio : aMovimiento

VAR aCosto : aMovimiento

VAR aCodProdSer : aMovimiento

VAR aCodAlmacen : aMovimiento

VAR aReferencia : aMovimiento

VAR aCodClasificacion : aMovimiento

regresar fAltaMovimiento( recibe PARAMETRO idDocumento, REFERENCIA idMovimiento,

REFERENCIA aMovimiento);

}

fAltaMovimientoEx ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoEx (aIdMovimiento, aTipoProducto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Entero largo | Por referencia | Identificador del documento. |  |
| aTipoProducto | tTipoProducto | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función da de alta de un movimiento los datos adicionales de un producto con series, lotes, pedimientos o características.

**Ejemplo**

Crear_movimiento_Ex ( recibe ENTERO idMovimiento )

{

OBJETO aTipoProducto: tTipoProducto

VAR aSeriesCapas : aTipoProducto

VAR aCaracteristicas : aTipoProducto

regresar fAltaMovimientoEx( recibe PARAMETRO idMovimiento,

REFERENCIA aTipoProducto);

}

fAltaMovimientoCDesct ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoCDesct (aIdDocumento, aIdMovimiento, astMovimiento) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDocumento | Entero largo | Por valor | Identificador del documento. |  |
| aIdMovimiento | Entero largo | Por Referencia | Identificador del movimiento |  |
| astMovimiento | tMovmientoDesc | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función da de alta un nuevo registro en la tabla de Movimientos.

Esta función incluye Importes y Porcentajes de Descuentos, a diferencia de la función fAltaMovimiento.

**Ejemplo**

Crear_movimiento_descuento( recibe ENTERO idDocumento )

{

VAR idMovimientoCDesc: ENTERO

OBJETO MovimientoCDesc: tMovimientoDesc

VAR aConsecutivo : MovimientoCDesc

VAR aUnidades : MovimientoCDesc

VAR aPrecio : MovimientoCDesc

VAR aCosto : MovimientoCDesc

VAR aPorcDesct1 : MovimientoCDesc

VAR aImporteDesc1 : MovimientoCDesc

VAR aPorcDesct2 : MovimientoCDesc

VAR aImporteDesc2 : MovimientoCDesc

VAR aPorcDesct3 : MovimientoCDesc

VAR aImporteDesc3 : MovimientoCDesc

VAR aPorcDesct4 : MovimientoCDesc

VAR aImporteDesc4 : MovimientoCDesc

VAR aPorcDesct5 : MovimientoCDesc

VAR aImporteDesc5 : MovimientoCDesc

VAR aCodProdSer : MovimientoCDesc

VAR aCodAlmacen : MovimientoCDesc

VAR aReferencia : MovimientoCDesc

VAR aCodClasificacion : MovimientoCDesc

regresar fAltaMovimientoCDesct( recibe PARAMETRO idDocumento,

REFERENCIA idMovimientoCDesc, REFERENCIA MovimientoCDesc);

}

fAltaMovimientoCaracteristicas ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoCaracteristicas (aIdMovimiento, aIdMovtoCaracteristicas, aCaracteristicas) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Entero largo | Por valor | Identificador del movimiento. |  |
| aIdMovtoCaracteristicas | Entero largo | Por referencia | Identificador del documento. |  |
| aCaracteristicas | tCaracteristicas | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdMovtoCaracteristicas: Al finalizar la función este parámetro contiene el identificador del nuevo movimiento.

**Descripción**

Esta función inserta un movimiento con características.

**Ejemplo**

Crear_movimiento_caracteristicas( recibe ENTERO idMovimiento )

{

VAR idMovtoCaracteristicas: ENTERO

OBJETO aCaracteristicas : tCaracterisiticas

VAR aUnidades : aCaracteristicas

VAR aValorCaracteristica1 : aCaracteristicas

VAR aValorCaracteristica2 : aCaracteristicas

VAR aValorCaracterisitica3 : aCaracteristicas

regresar fAltaMovimientoCaracteristicas( recibe PARAMETRO idMovimiento,

REFERENCIA idMovtoCaracteristicas, REFERENCIA aCaracteristicas);

}

fAltaMovtoCaracteristicasUnidades ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovtoCaracteristicasUnidades (aIdMovimiento, aIdMovtoCaracteristicas, aCaracteristicasUnidades) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | Entero largo | Por valor | Identificador del movimiento. |  |
| aIdMovtoCaracteristicas | Entero largo | Por referencia | Identificador del documento. |  |
| aCaracteristicasUnidades | tCaracteristicasUnidades | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdMovtoCaracteristicas: Al finalizar la función este parámetro contiene el identificador del nuevo movimiento.

**Descripción**

Esta función da de alta movimiento de características con unidades de compra venta.

**Ejemplo**

Crear_movimiento_caracteristicasUnidades( recibe ENTERO idMovimiento )

{

VAR idMovtoCaracteristicas: ENTERO

OBJETO aCaracteristicasUnidades: tCaracteristicasUnidades

VAR aUnidad : aCaracteristicasUnidades

VAR aUnidades : aCaracteristicasUnidades

VAR aUnidadesNC : aCaracteristicasUnidades

VAR aValorCaracteristica1 : aCaracteristicasUnidades

VAR aValorCaracteristica2 : aCaracteristicasUnidades

VAR aValorCaracteristica3 : aCaracteristicasUnidades

regresar fAltaMovimientoCaracteristicasUnidades( recibe PARAMETRO idMovimiento,

PARAMETRO idMovtoCaracteristicas, PARAMETRO aCaracteristicasUnidades);

}

fAltaMovimientoSeriesCapas ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaMovimientoSeriesCapas (aIdMovimiento, aSeriesCapas) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdMovimiento | long | Por valor | Identificador del movimiento. |  |
| aSeriesCapas | tSeriesCapas | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función agrega el movimiento de número de serie, lote y/o pedimento asociados a un movimiento cuyo producto maneje cualquiera de estas posibles configuraciones.

**Ejemplo**

Crear_movimiento_SeriesCapas ( recibe ENTERO idMovimiento )

{

OBJETO aSerieCapa: tSeriesCapas

VAR aUnidades : aSerieCapa

VAR aTipoCambio : aSerieCapa

VAR aSeries : aSerieCapa

VAR aPedimento : aSerieCapa

VAR aAgencia : aSerieCapa

VAR aFechaPedimiento : aSerieCapa

VAR aNumeroLote : aSerieCapa

VAR aFechaFabricacion : aSerieCapa

VAR aFechaCaducidad : aSerieCapa

regresar fAltaMovimientoSeriesCapas( recibe PARAMETRO idMovimiento,

REFERENCIA aSerieCapa);

}