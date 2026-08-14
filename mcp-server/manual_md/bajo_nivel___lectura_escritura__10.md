## Bajo nivel – Lectura/Escritura

fLeeDatoParametros ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoParametros (aCampo, aValor, aLen) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino. |  |
| aValor | Cadena | Por referencia | Valor de lectura. |  |
| aLen | Entero | Por valor | Longitud del dato de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función lee un campo del registro actual de parámetros.

**Ejemplo**

Lee Dato Parametros

{

VAR Error: ENTERO

VAR aValor: STRINGBUILDER

Error = fLeeDatoParametros recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoParametros

FIN SI

}

fEditaParametros ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaParametros () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función activa el modo de edición de un registro de los Parámetros. |
| Ejemplo | Edita Parametros { VAR Error: ENTERO VAR aValor: STRINGBUILDER Error = fLeeDatoParametros recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO fEditaParametros FIN SI } |

fSetDatoParametros ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoParametros(aCampo, aValor ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por referencia | Nombre del campo |  |
| aValor | Cadena | Por referencia | Valor del campo |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla Parámetros.

**Ejemplo**

SetDatoParametros

{

VAR Error: ENTERO

VAR aValor: STRINGBUILDER

Error = fLeeDatoParametros recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Error=fEditaParametros

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fSetDatoParametros

SI

Error <> 0

ENTONCES

Error

SI NO

fGuardaParametros

FIN SI

FIN SI

FIN SI

}

fGuardaParametros ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaParametros() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función guarda los cambios efectuados al registro de la tabla Parámetros. |