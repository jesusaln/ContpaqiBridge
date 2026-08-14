## Bajo nivel - Búsqueda/Navegación

fBuscaCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaCteProv (aCodCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodCteProv | Cadena | Por valor | Código del Cliente/Proveedor. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un Cliente/Proveedor por su código.

**Ejemplo**

CANCELAR EDICION

{

VAR aCodCteProv CADENA

VAR error ENTERO

VAR aCampo CADENA

VAR aValor CADENA

VAR aLong ENTERO

VAR aEditCampos ENTERO

Error = ejecutar fBuscaCteProv recibe PARAMETRO aCodCteProv

SI

error = 0

ENTONCES

ejecutar fEditaCteProv

MIENTRAS aEditCampos > 0

HACER

aCampo = nuevo campo

aValor = nuevo valor

error = ejecutar fSetDatoCteProv recibe PARAMETRO aCampo,

PARAMETRO aValor

SI

error <> 0

ENTONCES

ejecutar fCancelarModificacionCteProv

FIN ENTONCES

FIN HACER

FIN MIENTRAS

FIN ENTONCES

}

**Comentarios**

Se puede consultar el nombre de cada campo utilizable para las funciones **fLeeDatoCteProv**y **fSetDatoCteProv**en el documento estructura de la BDD comercial (COM_BDD) tabla **admClientes**.

aCampo = Nombre del campo, aValor = Valor del campo

Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables.

fBuscaIdCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdCteProv (aIdCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdCteProv | Entero | Por valor | Identificador del Cliente/Proveedor. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un Cliente/Proveedor por su Identificador.

**Ejemplo**

BORRAR CLIENTE-PROVEEDOR (recibe aIdCteProv)

{

VAR error ENTERO

error ejecutar fBuscaIdCteProv recibe PARAMETRO aIdCteProv

SI

error = 0

ENTONCES

ejecutar fBorrarCteProv

FIN ENTONCES

}

fPosPrimerCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdCteProv (aIdCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdCteProv | Entero | Por valor | Identificador del Cliente/Proveedor. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un Cliente/Proveedor por su Identificador.

**Ejemplo**

RECORRIDO CLIENTE-PROVEEDOR

{

VAR error ENTERO

VAR aCampo CADENA

VAR aValor CADENA

VAR aLong ENTERO

error = ejecutar fPosPrimerCteProv

SI

error = 0

ENTONCES

HACER

ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo,

PARAMETRO aValor, PARAMETRO aLong

SI

ejecutar fPosEOFteProv = VERDADERO

ENTONCES

Cortar

FIN ENTONCES

Ejecutar fPosSiguienteProv

MIENTRAS ejecutar fPosEOFteProv = FALSO

FIN ENTONCES

}

fPosUltimoCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimoCteProv() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el último registro de la tabla de Clientes/Proveedores. |
| Ejemplo | RECORRIDO CLIENTE-PROVEEDOR { VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO error = ejecutar fPosUltimoCteProv SI error = 0 ENTONCES HACER ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong SI ejecutar fPosBOFCteProv = VERDADERO ENTONCES Cortar FIN ENTONCES Ejecutar fPosSiguienteProv MIENTRAS ejecutar fPosBOFCteProv = FALSO FIN ENTONCES } |

fPosSiguienteCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteCteProv() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Clientes / Proveedores. |
| Ejemplo | RECORRIDO CLIENTE-PROVEEDOR { VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO error = ejecutar fPosPrimerCteProv SI error = 0 ENTONCES HACER ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong SI ejecutar fPosEOFteProv = VERDADERO ENTONCES Cortar FIN ENTONCES Ejecutar fPosSiguienteProv MIENTRAS ejecutar fPosEOFteProv = FALSO FIN ENTONCES } |

fPosAnteriorCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorCteProv() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de Clientes / Proveedores. |
| Ejemplo | RECORRIDO CLIENTE-PROVEEDOR { VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO error = ejecutar fPosUltimoCteProv SI error = 0 ENTONCES HACER ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong SI ejecutar fPosBOFCteProv = VERDADERO ENTONCES Cortar FIN ENTONCES Ejecutar fPosSiguienteProv MIENTRAS ejecutar fPosBOFCteProv = FALSO FIN ENTONCES } |

fPosBOFCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOFCteProv() |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Documentos. |
| Ejemplo | RECORRIDO CLIENTE-PROVEEDOR { VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO error = ejecutar fPosUltimoCteProv SI error = 0 ENTONCES HACER ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong SI ejecutar fPosBOFCteProv = VERDADERO ENTONCES Cortar FIN ENTONCES Ejecutar fPosSiguienteProv MIENTRAS ejecutar fPosBOFCteProv = FALSO FIN ENTONCES } |

fPosEOFCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFCteProv() |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Documentos. |
| Ejemplo | RECORRIDO CLIENTE-PROVEEDOR { VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO error = ejecutar fPosPrimerCteProv SI error = 0 ENTONCES HACER ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong SI ejecutar fPosEOFCteProv = VERDADERO ENTONCES Cortar FIN ENTONCES Ejecutar fPosSiguienteProv MIENTRAS ejecutar fPosEOFCteProv = FALSO FIN ENTONCES } |