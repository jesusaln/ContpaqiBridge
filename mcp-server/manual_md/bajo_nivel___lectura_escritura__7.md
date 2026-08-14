## Bajo nivel – Lectura/Escritura

fRegresaExistencia ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaExistencia (aCodigoProducto, aCodigoAlmacen, aAnio, aMes, aDia, aExistencia) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén. |  |
| aAnio | Cadena | Por valor | Año. |  |
| aMes | Cadena | Por valor | Mes. |  |
| aDia | Cadena | Por valor | Día. |  |
| aExistencia | Doble | Por referencia | Existencia |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aExistencia: Al finalizar la función este parámetro contiene la existencia del producto requerido.

**Descripción**

Esta función regresa la existencia de un producto en un almacén a una determinada fecha.

**Ejemplo**

Regresa Existencia

{

VAR Error: ENTERO

Error = fRegresaExistencia recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCodigoAlmacen: CADENA, PARAMETRO aAnio: CADENA, PARAMETRO aMes: CADENA, PARAMETRO aDia: CADENA, REFERENCIA aExistencia: DOUBLE

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaExistencia

FIN SI

}

fRegresaExistenciaCaracteristicas ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaExistenciaCaracteristicas (aCodigoProducto, aCodigoAlmacen, aAnio, aMes, aDia, aValorCaracteristica1, aValorCaracteristica2, aValorCaracteristica3, aExistencia) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén. |  |
| aAnio | Cadena | Por valor | Año. |  |
| aMes | Cadena | Por valor | Mes. |  |
| aDia | Cadena | Por valor | Día. |  |
| aValorCaracteristica1 | Cadena | Por valor | Valor característica 1. |  |
| aValorCaracteristica2 | Cadena | Por valor | Valor característica 2. |  |
| aValorCaracteristica3 | Cadena | Por valor | Valor característica 3. |  |
| aExistencia | Doble | Por referencia | Existencia |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aExistencia: Al finalizar la función este parámetro contiene la existencia del producto requerido.

**Descripción**

Esta función regresa la existencia de un producto con características en un almacén a una determinada fecha.

**Ejemplo**

Regresa Existencia Caracteristicas

{

VAR Error: ENTERO

Error = fRegresaExistenciaCaracteristicas recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCodigoAlmacen: CADENA, PARAMETRO aAnio: CADENA, PARAMETRO aMes: CADENA, PARAMETRO aDia: CADENA, PARAMETRO aValorCaracteristica1: CADENA, PARAMETRO aValorCaracteristica2: CADENA, PARAMETRO aValorCaracteristica3: CADENA, REFERENCIA aExistencia: DOUBLE

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaExistenciaCaracteristicas

FIN SI

}