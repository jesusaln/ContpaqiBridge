## Alto nivel – Lectura/Escritura

fAltaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaProducto (aIdProducto, astProducto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdProducto | Entero | Por referencia | Identificador del producto. |  |
| astProducto | tProducto | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdProducto: Al finalizar la función este parámetro contiene el identificador del nuevo producto.

**Descripción**

Esta función da de alta un nuevo producto.

**Ejemplo**

Alta_producto(){

VAR IdProducto: ENTERO

OBJETO aProducto: tProducto

VAR cCodigoProducto : aProducto

VAR cNombreProducto : aProducto

VAR cDescripcionProducto : aProducto

VAR cTipoProducto : aProducto

VAR cFechaAltaProducto : aProducto

VAR cFechaBajaProducto : aProducto

VAR cStatusProducto : aProducto

VAR cControlExistencia : aProducto

VAR cMetodoCosteo : aProducto

VAR cCodigoUnidadBase : aProducto

VAR cCodigoUnidadNoConvertible : aProducto

VAR cPrecio1 : aProducto

VAR cPrecio2 : aProducto

VAR cPrecio3 : aProducto

VAR cPrecio4 : aProducto

VAR cPrecio5 : aProducto

VAR cPrecio6 : aProducto

VAR cPrecio7 : aProducto

VAR cPrecio8 : aProducto

VAR cPrecio9 : aProducto

VAR cPrecio10 : aProducto

VAR cImpuesto1 : aProducto

VAR cImpuesto2 : aProducto

VAR cImpuesto3 : aProducto

VAR cRetencion1 : aProducto

VAR cRetencion2 : aProducto

VAR cNombreCaracteristica1 : aProducto

VAR cNombreCaracteristica2 : aProducto

VAR cNombreCaracteristica3 : aProducto

VAR cCodigoValorCaracterisitica1 : aProducto

VAR cCodigoValorCaracterisitica2 : aProducto

VAR cCodigoValorCaracterisitica3 : aProducto

VAR cCodigoValorCaracterisitica4 : aProducto

VAR cCodigoValorCaracterisitica5 : aProducto

VAR cCodigoValorCaracterisitica6 : aProducto

VAR cTextoExtra1 : aProducto

VAR cTextoExtra2 : aProducto

VAR cTextoExtra3 : aProducto

VAR cFechaExtra : aProducto

VAR cImporteExtra1 : aProducto

VAR cImporteExtra2 : aProducto

VAR cImporteExtra3 : aProducto

VAR cImporteExtra4 : aProducto

Ejecutar fAltaProducto (recibe REFERENCIA IdProducto, REFERENCIA Producto);

}

fActualizaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fActualizaProducto (aCodigoProducto, astCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Entero largo | Por referencia | Código del producto. |  |
| astProducto | tProducto | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función actualiza un producto.

**Ejemplo**

Actualizar_producto(){

VAR codigoProducto: CADENA

OBJETO Producto: tProducto

VAR cCodigoProducto : aProducto

VAR cNombreProducto : aProducto

VAR cDescripcionProducto : aProducto

VAR cTipoProducto : aProducto

VAR cFechaAltaProducto : aProducto

VAR cFechaBajaProducto : aProducto

VAR cStatusProducto : aProducto

VAR cControlExistencia : aProducto

VAR cMetodoCosteo : aProducto

VAR cCodigoUnidadBase : aProducto

VAR cCodigoUnidadNoConvertible : aProducto

VAR cPrecio1 : aProducto

VAR cPrecio2 : aProducto

VAR cPrecio3 : aProducto

VAR cPrecio4 : aProducto

VAR cPrecio5 : aProducto

VAR cPrecio6 : aProducto

VAR cPrecio7 : aProducto

VAR cPrecio8 : aProducto

VAR cPrecio9 : aProducto

VAR cPrecio10 : aProducto

VAR cImpuesto1 : aProducto

VAR cImpuesto2 : aProducto

VAR cImpuesto3 : aProducto

VAR cRetencion1 : aProducto

VAR cRetencion2 : aProducto

VAR cNombreCaracteristica1 : aProducto

VAR cNombreCaracteristica2 : aProducto

VAR cNombreCaracteristica3 : aProducto

VAR cCodigoValorCaracterisitica1 : aProducto

VAR cCodigoValorCaracterisitica2 : aProducto

VAR cCodigoValorCaracterisitica3 : aProducto

VAR cCodigoValorCaracterisitica4 : aProducto

VAR cCodigoValorCaracterisitica5 : aProducto

VAR cCodigoValorCaracterisitica6 : aProducto

VAR cTextoExtra1 : aProducto

VAR cTextoExtra2 : aProducto

VAR cTextoExtra3 : aProducto

VAR cFechaExtra : aProducto

VAR cImporteExtra1 : aProducto

VAR cImporteExtra2 : aProducto

VAR cImporteExtra3 : aProducto

VAR cImporteExtra4 : aProducto

Ejecutar fActualizaProducto (recibe PARAMETRO codigoProducto,

REFERENCIA Producto);

}

fLlenaRegistroProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLlenaRegistroCteProv (astProducto, aEsAlta ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| astProducto | tProducto | Por valor | Tipo de dato abstracto. |  |
| aEsAlta | Entero | Por valor | 1 = Nuevo Producto. 2 = Actualizacion Producto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función asigna al registro de la tabla de productos los valores de la estructura de datos astCteProv.

**Ejemplo**

LlenaRegistro_producto(){

VAR aEsAlta: ENTERO

OBJETO Producto: tProducto

VAR cCodigoProducto : aProducto

VAR cNombreProducto : aProducto

VAR cDescripcionProducto : aProducto

VAR cTipoProducto : aProducto

VAR cFechaAltaProducto : aProducto

VAR cFechaBajaProducto : aProducto

VAR cStatusProducto : aProducto

VAR cControlExistencia : aProducto

VAR cMetodoCosteo : aProducto

VAR cCodigoUnidadBase : aProducto

VAR cCodigoUnidadNoConvertible : aProducto

VAR cPrecio1 : aProducto

VAR cPrecio2 : aProducto

VAR cPrecio3 : aProducto

VAR cPrecio4 : aProducto

VAR cPrecio5 : aProducto

VAR cPrecio6 : aProducto

VAR cPrecio7 : aProducto

VAR cPrecio8 : aProducto

VAR cPrecio9 : aProducto

VAR cPrecio10 : aProducto

VAR cImpuesto1 : aProducto

VAR cImpuesto2 : aProducto

VAR cImpuesto3 : aProducto

VAR cRetencion1 : aProducto

VAR cRetencion2 : aProducto

VAR cNombreCaracteristica1 : aProducto

VAR cNombreCaracteristica2 : aProducto

VAR cNombreCaracteristica3 : aProducto

VAR cCodigoValorCaracterisitica1 : aProducto

VAR cCodigoValorCaracterisitica2 : aProducto

VAR cCodigoValorCaracterisitica3 : aProducto

VAR cCodigoValorCaracterisitica4 : aProducto

VAR cCodigoValorCaracterisitica5 : aProducto

VAR cCodigoValorCaracterisitica6 : aProducto

VAR cTextoExtra1 : aProducto

VAR cTextoExtra2 : aProducto

VAR cTextoExtra3 : aProducto

VAR cFechaExtra : aProducto

VAR cImporteExtra1 : aProducto

VAR cImporteExtra2 : aProducto

VAR cImporteExtra3 : aProducto

VAR cImporteExtra4 : aProducto

Ejecutar fLlenaRegistroProducto(recibe REFERENCIA Producto,

PARAMETRO aEsAlta);

}