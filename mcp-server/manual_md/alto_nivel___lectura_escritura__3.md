## Alto nivel – Lectura/Escritura

fAltaCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaCteProv (aIdCteProv, astCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdCteProv | Entero | Por referencia | Identificador del Cliente/Proveedor. |  |
| astCteProv | tCteProv | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdCteProv: Al finalizar la función este parámetro contiene el identificador del nuevo Cliente/Proveedor.

**Descripción**

Esta función da de alta un nuevo Cliente / Proveedor.

**Ejemplo**

Alta Cliente Proveedor

{

OBJ tCteProv: SDK

VAR Error, idClienteProveedor: ENTERO

VAR aCodigoCliente: tCteProv

VAR aRazonSocial: tCteProv

VAR cRFC: tCteProv

VAR cFechaAlta: tCteProv

Error = Ejecuta fAltaCteProv recibe REFERENCIA idClienteProveedor,

REFERENCIA tCteProv

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaCteProv

FIN SI

}

**Comentarios**

Para la referencia de **tCteProv**los campos utilizables dependerán de la necesidad del desarrollador para el alta del cliente-proveedor.

fActualizaCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fActualizaCteProv (aCodigoCteProv, astCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoCteProv | Cadena | Por referencia | Identificador del Cliente/Proveedor. |  |
| astCteProv | tCteProv | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función actualiza un Cliente / Proveedor por medio su código.

**Ejemplo**

Actualiza Cliente Proveedor

{

OBJ tCteProv: SDK

VAR Error: ENTERO

VAR aCodigoCliente: tCteProv

VAR aRazonSocial: tCteProv

Error = Ejecuta fActualizaCteProv recibe PARAMETRO aCodigoCteProv: CADENA,

REFERENCIA tCteProv

SI

Error <> 0

ENTONCES

Error

SI NO

fActualizaCteProv

FIN SI

}

**Comentarios**

Para la referencia de **tCteProv**los campos utilizables dependerán de la necesidad del desarrollador para la actualización del cliente-proveedor.

fLlenaRegistroCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLlenaRegistroCteProv (astCteProv, aEsAlta ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| astCteProv | tCteProv | Por valor | Tipo de dato abstracto |  |
| aEsAlta | Entero | Por valor | 1 = Nuevo Cliente / Proveedor. 2 = Actualizacion Cliente / Proveedor. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función asigna al registro de la tabla de Clientes / Proveedores los valores de la estructura de datos astCteProv.

**Ejemplo**

Llena Registro Cliente Proveedor

{

OBJ tCteProv: SDK

VAR Error: ENTERO

VAR aCodigoCliente: tCteProv

VAR aRazonSocial: tCteProv

VAR aNombreMoneda: tCteProv

VAR aFechaAlta: tCteProv

VAR aRFC: tCteProv

Error = Ejecuta fInsertaCteProv

SI

Error <> 0

ENTONCES

Error

SI NO

Error = Ejecuta fLlenaRegistroCteProv recibe REFERENCIA tCteProv,

PARAMETRO aEsAlta: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLlenaRegistroCteProv

FIN SI

FIN SI

}