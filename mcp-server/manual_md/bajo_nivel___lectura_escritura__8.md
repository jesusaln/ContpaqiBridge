## Bajo nivel – Lectura/Escritura

fRegresaCostoPromedio ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaCostoPromedio (aCodigoProducto, aCodigoAlmacen, aAnio, aMes, aDia, aCostoPromedio) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén. 0 (cero) – Todos los almacenes. |  |
| aAnio | Cadena | Por valor | Año. |  |
| aMes | Cadena | Por valor | Mes. |  |
| aDia | Cadena | Por valor | Día. |  |
| aCostoPromedio | Cadena | Por referencia | Costo promedio |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aCostoPromedio: Al finalizar la función este parámetro contiene el costo promedio del producto requerido.

**Descripción**

Esta función se encarga de obtener el costo promedio de un producto en determinada fecha para todos los almacenes o para uno solo.

**Ejemplo**

Regresa Costo Promedio

{

VAR Error: ENTERO

Error = fRegresaCostoPromedio recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCodigoAlmacen: CADENA, PARAMETRO aAnio: CADENA, PARAMETRO aMes: CADENA, PARAMETRO aDia: CADENA, PARAMETRO aCostoPromedio: STRINGBUILDER

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaCostoPromedio

FIN SI

}

fRegresaUltimoCosto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaUltimoCosto (aCodigoProducto, aCodigoAlmacen, aAnio, aMes, aDia, aUltimoCosto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén. 0 (cero) – Todos los almacenes. |  |
| aAnio | Cadena | Por valor | Año. |  |
| aMes | Cadena | Por valor | Mes. |  |
| aDia | Cadena | Por valor | Día. |  |
| aUltimoCosto | Cadena | Por referencia | Último costo. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aUltimoCosto: Al finalizar la función este parámetro contiene el ultimo costo del producto requerido.

**Descripción**

Esta función se encarga de obtener el último costo de un producto en determinada fecha para todos los almacenes o para uno solo.

**Ejemplo**

Regresa Ultimo Costo

{

VAR Error: ENTERO

Error = fRegresaUltimoCosto recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCodigoAlmacen: CADENA, PARAMETRO aAnio: CADENA, PARAMETRO aMes: CADENA, PARAMETRO aDia: CADENA, PARAMETRO aUltmoCosto: STRINGBUILDER

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaUltimoCosto

FIN SI

}

fRegresaCostoEstandar ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaCostoEstandar (aCodigoProducto, aCostoEstandar) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCostoEstandar | Cadena | Por referencia | Costo estándar. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aCostoEstandar: Al finalizar la función este parámetro contiene el costo estándar del producto requerido.

**Descripción**

Esta función se encarga de obtener el costo estándar de un producto.

**Ejemplo**

Regresa Costo Estandar

{

VAR Error: ENTERO

Error = fRegresaCostoEstandar recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCostoEstandar: STRINGBUILDER

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaCostoEstandar

FIN SI

}

fRegresaCostoCapa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresaCostoCapa (aCodigoProducto, aCodigoAlmacen, aUnidades, aImporteCosto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aCodigoAlmacen | Cadena | Por valor | Código del almacén. |  |
| aUnidades | Doble | Por valor | Unidades a costear. |  |
| aImporteCosto | Cadena | Por referencia | Importe del costo de la unidades recibidas. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aImporteCosto: Al finalizar la función este parámetro contiene el costo UEPS o PEPS del producto requerido.

**Descripción**

Esta función obtiene el costo UEPS o PEPS de un producto en un almacén en base a una cantidad de unidades proporcionadas.

**Ejemplo**

Regresa Costo Capa

{

VAR Error: ENTERO

Error = fRegresaCostoCapa recibe PARAMETRO aCodigoProducto: CADENA, PARAMETRO aCodigoAlmacen: CADENA, PARAMETRO aUnidades: DOBLE, PARAMETRO aImporteCosto: STRINGBUILDER

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaCostoCapa

FIN SI

}