## Bajo nivel - Búsqueda/Navegación

fBuscaDireccionEmpresa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBuscaDireccionEmpresa () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función busca la dirección de la empresa. |
| Ejemplo | Busca Dirección Empresa{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fBuscaDireccionEmpresa SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |
| Comentario | Para agregar datos en la función fLeeDatoDireccionse se puede consultar el documento de referencia de las bases de datos del sistema CONTPAQi® que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato. |

fBuscaDireccionCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaDireccionCteProv (aCodCteProv, aTipoDireccion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Código del cliente/proveedor. |  |
| aValor | Cadena | Por referencia | Tipo de dirección 0 = Fiscal, 1 = Envío |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca una dirección de un cliente/proveedor.

**Ejemplo**

Busca Dirección Cliente Proveedor{

VAR Error: ENTERO

Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaDireccionCteProv

FIN SI

}

fBuscaDireccionDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaDireccionDocumento (aIdDocumento, aTipoDireccion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDocumento | Entero largo | Por valor | Identificador del documento. |  |
| aValor | Cadena | Por valor | Tipo de dirección 0 = Fiscal, 1 = Envío |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca una dirección de un documento.

**Ejemplo**

Busca Dirección Documento{

VAR Error: ENTERO

Ejecuta fBuscaDireccionDocumento recibe PARAMETRO aIdDocumento: ENTERO,

PARAMETRO aTipoDireccion: BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaDireccionDocumento

FIN SI

}

fPosPrimerDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el primer registro de la tabla de Direcciones. |
| Ejemplo | Posicionar Primer Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosPrimerDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |
| Comentario | Para leer datos utilizando la función fLeeDatoDireccion se puede consultar el documento de referencia de las bases de datos del sistema CONTPAQi® que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato. |

fPosUltimaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimaDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el ultimo registro de la tabla de Direcciones. |
| Ejemplo | Posicionar Ultima Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosUltimaDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |

fPosSiguienteDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Direcciones. |
| Ejemplo | Posicionar Siguiente Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosSiguienteDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |

fPosAnteriorDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de Direcciones. |
| Ejemplo | Posicionar Anterior Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosAnteriorDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |

fPosBOFDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosBOFDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Esta función informa si el registro activo se encuentra en el inicio de la tabla de Direcciones. |
| Ejemplo | Posicionar BOF Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosBOFDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |

fPosEOFDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Esta función informa si el registro activo se encuentra en el fin de la tabla de Direcciones |
| Ejemplo | Posicionar EOF Dirección{ VAR aValor: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fPosEOFDireccion SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Regresa aValor FIN SI FIN SI } |