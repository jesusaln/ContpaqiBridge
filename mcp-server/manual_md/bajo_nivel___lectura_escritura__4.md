## Bajo nivel – Lectura/Escritura

fInsertaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertaProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Adiciona un nuevo registro en la tabla de productos en modo de inserción. |
| Ejemplo | Inserta Producto { VAR Error: ENTERO Error = Ejecuta fInsertaProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fSetDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaProducto FIN SI } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para la función fSetDatoProducto en el documento de base de datos, tabla Productos del sistema CONTPAQi Factura Electrónica® y tabla admProductos del sistema CONTPAQi Comercial Premium®. Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fEditaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Productos. |
| Ejemplo | Edita Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Error = Ejecuta fEditaProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fSetDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaProducto FIN SI FIN SI } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para la función fSetDatoProducto en el documento de base de datos, tabla Productos del sistema CONTPAQi Factura Electrónica® y tabla admProductos del sistema CONTPAQi Comercial Premium®. Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fGuardaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Guarda los cambios realizados a un registro de productos. |
| Ejemplo | Guarda Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Error = Ejecuta fEditaProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fSetDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaProducto FIN SI FIN SI } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para la función fSetDatoProducto en el documento de base de datos, tabla Productos del sistema CONTPAQi Factura Electrónica® y tabla admProductos del sistema CONTPAQi Comercial Premium®. Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fBorraProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBorraProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Borra un registro en la tabla de productos. |
| Ejemplo | Borra Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Ejecuta fBorraProducto FIN SI } |

fCancelarModificacionProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función cancela las modificaciones al registro actual de productos. El registro debe estar en modo de edición o inserción. |
| Ejemplo | Cancela Modificación Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Error = Ejecuta fEditaProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fCancelarModificacionProducto FIN SI FIN SI } |

fEliminarProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fEliminarProducto (aCodigoProducto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoProducto | Cadena | Por valor | Código del producto |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función elimina un producto usando su código.

**Ejemplo**

Eliminar Producto

{

VAR Error: ENTERO

Error = Ejecuta fEliminarProducto recibe PARAMETRO aCodigoProducto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEliminarProducto

FIN SI

}

fSetDatoProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoProducto (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla Productos.

**Ejemplo**

Set Dato Producto

{

VAR Error: ENTERO

Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = Ejecuta fEditaProducto

SI

Error <> 0

ENTONCES

Error

SI NO

Ejecuta fSetDatoProducto recibe PARAMETRO aCampo: CADENA,

PARAMETRO aValor: CADENA

Ejecuta fGuardaProducto

FIN SI

FIN SI

}

**Comentarios**

Se puede consultar el nombre de cada campo utilizable para la función **fSetDatoProducto**en el documento de base de datos, tabla **Productos**del sistema **CONTPAQi Factura Electrónica®** y tabla **admProductos**del sistema **CONTPAQi Comercial Premium®**.

Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables.

fLeeDatoProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoProducto (aCampo, aValr, aLen) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por referencia | Valor de lectura |  |
| aLen | Entero | Por valor | Longitud del dato de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de productos.

**Ejemplo**

Set Dato Producto

{

VAR Error: ENTERO

VAR aValor: CADENA(StringBuilder)

Error = Ejecuta fBuscaProducto recibe VAR aCodProducto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = Ejecuta fLeeDatoProducto recibe PARAMETRO aCampo: CADENA, aValor,

PARAMETRO aLong: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoProducto

FIN SI

FIN SI

}

fRecuperaTipoProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRecuperaTipoProducto(aUnidades, aSerie, aLote, aPedimento, aCaracteristicas) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aUnidades | Lógico (bool) | Por referencia | Valor lógico. Verdadero o Falso. Maneja unidades o no. |  |
| aSerie | Lógico (bool) | Por referencia | Valor lógico. Verdadero o Falso. Maneja series o no. |  |
| aLote | Lógico (bool) | Por referencia | Valor lógico. Verdadero o Falso. Maneja lotes o no. |  |
| aPedimento | Lógico (bool) | Por referencia | Valor lógico. Verdadero o Falso. Maneja pedimentos o no. |  |
| aCaracteristicas | Lógico (bool) | Por referencia | Valor lógico. Verdadero o Falso. Maneja caracterisricas o no. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aUnidades: Al finalizar la función este parámetro indica si el producto maneja unidades o no.

aSerie: Al finalizar la función este parámetro indica si el producto maneja series o no.

aLote: Al finalizar la función este parámetro indica si el producto maneja lotes o no.

aPedimento: Al finalizar la función este parámetro indica si el producto maneja pedimentos o no.

aCaracteristicas: Al finalizar la función este parámetro indica si el producto maneja características o no.

**Descripción**

Esta función define el tipo de producto, indicando si maneja series, lotes, pedimentos, unidades y/o características.

**Ejemplo**

Recupera Tipo Producto

{

VAR Error: ENTERO

VAR aUnidades, aSerie, aLote, aPedimento, aCaracteristica: BOOL

Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

Error = Ejecuta fRecuperaTipoProducto recibe REFERENCIA aUnidades,

REFERENCIA aSerie, REFERENCIA aLote, REFERENCIA aPedimento, REFERENCIA

aCaracteristica

SI

Error <> 0

ENTONCES

Error

SI NO

fRecuperaTipoProducto

FIN SI

FIN SI

}

fRegresaPrecioVenta ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRecosteoProducto (aCodigoProducto, aEjercicio, aPeriodo, aCodigoClasificacion1, aCodigoClasificacion2, aCodigoClasificacion3, aCodigoClasificacion4, aCodigoClasificacion5, aCodigoClasificacion6, aNombreBitacora, aSobreEscribirBitacora , aEsCalculoArimetico) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoConcepto | Cadena | Por valor | Código del concepto. |  |
| aCodigoCliente | Cadena | Por valor | Código del cliente. |  |
| aCodigoProducto | Cadena | Por valor | Código del producto. |  |
| aPrecioVenta | Cadena | Por referencia | Precio de venta. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aPrecioVenta: Al finalizar la función este parámetro contiene el precio de venta del producto solicitado.

**Descripción**

Esta función obtiene el precio de venta de un producto de un determinado cliente para un concepto de documento en especifico.

**Ejemplo**

Regresa Precio Venta

{

VAR Error: ENTERO

VAR aPrecioVenta: CADENA(StringBuilder)

Error = Ejecuta fRegresaPrecioVenta recibe PARAMETRO aCodigoConcepto:

CADENA, PARAMETRO aCodigoCliente: CADENA, PARAMETRO aCodigoProducto:

CADENA, PARAMETRO aPrecioVenta

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresaPrecioVenta

FIN SI

}