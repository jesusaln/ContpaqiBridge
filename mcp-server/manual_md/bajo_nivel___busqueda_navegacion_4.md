## Bajo nivel - Búsqueda/Navegación

fBuscaProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaProducto (aCodProducto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodProducto | Cadena | Por valor | Código del producto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un producto por su código.

**Ejemplo**

Busca Producto

{

VAR Error: ENTERO

Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaProducto

FIN SI

}

fBuscaIdProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdProducto (aIdProducto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdProducto | Entero | Por valor | Identificador del producto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un producto por su Identificador.

**Ejemplo**

Busca Id Producto

{

VAR Error: ENTERO

Error = Ejecuta fBuscaIdProducto recibe PARAMETRO aIdProducto: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaIdProducto

FIN SI

}

fPosPrimerProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el primer registro de la tabla de Productos. |
| Ejemplo | Posicionar Primer Producto { VAR Error: ENTERO VAR aNomProducto: CADENA(StringBuilder) Error = Ejecuta fPosPrimerProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aNomProducto, PARAMETRO aLong: ENTERO FIN SI } |

fPosUltimoProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el último registro de la tabla de Productos. |
| Ejemplo | Posicionar Ultimo Producto { VAR Error: ENTERO VAR aNomProducto: CADENA(StringBuilder) Error = Ejecuta fPosUltimoProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aNomProducto, PARAMETRO aLong: ENTERO FIN SI } |

fPosSiguienteProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Productos. |
| Ejemplo | Posicionar Siguiente Producto { VAR Error: ENTERO VAR aNomProducto: CADENA(StringBuilder) Error = Ejecuta fPosSiguienteProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aNomProducto, PARAMETRO aLong: ENTERO FIN SI } |

fPosAnteriorProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Productos. |
| Ejemplo | Posicionar Anterior Producto { VAR Error: ENTERO VAR aNomProducto: CADENA(StringBuilder) Error = Ejecuta fPosAnteriorProducto SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoProducto recibe PARAMETRO aCampo: CADENA, PARAMETRO aNomProducto, PARAMETRO aLong: ENTERO FIN SI } |

fPosBOFProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOFProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Productos. |
| Ejemplo | Posicionar BOF Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Error = Ejecuta fPosBOFProducto SI Error <> 0 ENTONCES Error SI NO fPosBOFProducto FIN SI FIN SI } |

fPosEOFProducto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFProducto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Productos. |
| Ejemplo | Posicionar EOF Producto { VAR Error: ENTERO Error = Ejecuta fBuscaProducto recibe PARAMETRO aCodProducto: CADENA SI Error <> 0 ENTONCES Error SI NO Error = Ejecuta fPosEOFProducto SI Error <> 0 ENTONCES Error SI NO fPosEOFProducto FIN SI FIN SI } |