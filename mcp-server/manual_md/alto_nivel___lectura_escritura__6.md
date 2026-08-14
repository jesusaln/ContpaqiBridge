## Alto nivel – Lectura/Escritura

fAltaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaValorClasif (aIdValorClasif, astValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdValorClasif | Entero | Por referencia | Identificador de la dirección. |  |
| astValorClasif | tValorClasif | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aIdValorClasif: Al finalizar la función este parámetro contiene el identificador del nuevo valor de clasificación.

**Descripción**

Esta función da de alta un nuevo valor de clasificación.

**Ejemplo**

Alta Valor Clasificacion

{

VAR Error: ENTERO

VAR tValor: tValorClasificacion

Error = fAltaValorClasif REFERENCIA aValor: ENTERO, REFERENCIA tValor

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaValorClasif

FIN SI

}

fActualizaValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaClasificacion (aClasificacionDe, aNumClasificacion, aCodValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoValorClasif | Cadena | Por valor | Código del valor de clasificación. |  |
| astValorClasif | tValorClasif | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función actualiza el valor de clasificación del registro especificado por el parametro

aCodigoValorClasif.

**Ejemplo**

Actualiza Valor Clasificacion

{

VAR Error: ENTERO

VAR tValor: tValorClasificacion

Error = fActualizaValorClasif PARAMETRO aCodigoValorClasificacion: CADENA, REFERENCIA tValor

SI

Error <> 0

ENTONCES

Error

SI NO

fActualizaValorClasif

FIN SI

}

fLlenaRegistroValorClasif ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLlenaRegistroValorClasif (astValorClasif) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| astValorClasif | tValorClasif | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función asigna al registro de la base de datos los valores de la estructura de datos del valor de clasificación.

**Ejemplo**

Llena Registro Valor Clasificacion

{

VAR Error: ENTERO

VAR tValor: tValorClasificacion

Error = fLlenaRegistroValorClasif REFERENCIA tValor

SI

Error <> 0

ENTONCES

Error

SI NO

fLlenaRegistroValorClasif

FIN SI

}