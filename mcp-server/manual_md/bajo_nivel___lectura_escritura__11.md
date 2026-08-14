## Bajo nivel – Lectura/Escritura

fEditaClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaClasificacion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Clasificaciones. |
| Ejemplo | Edita Clasificacion { VAR Error: ENTERO Error = fBuscaIdClasificacion recibe PARAMETRO aIdClasificacion: ENTERO SI Error <> 0 ENTONCES Error SI NO fEditaClasificacion FIN SI } |

fGuardaClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaClasificacion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Guarda los cambios realizados a un registro de clasificaciones. |
| Ejemplo | Guarda Clasificacion { VAR Error: ENTERO Error = fBuscaIdClasificacion recibe PARAMETRO aIdClasificacion: ENTERO SI Error <> 0 ENTONCES Error SI NO fEditaClasificacion SI Error <> 0 ENTONCES Error SI NO Error = fSetDatoClasificacion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA SI Error <> 0 ENTONCES Error SI NO fSetDatoClasificacion Error = fGuardaClasificacion SI Error <> 0 ENTONCES Error SI NO fGuardaClasificacion FIN SI FIN SI FIN SI FIN SI } |

fCancelarModificacionClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionClasificacion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función cancela las modificaciones al registro actual de clasificaciones. El registro debe estar en modo de edición o inserción. |
| Ejemplo | Cancelar Modificacion Clasificacion { VAR Error: ENTERO Error = fBuscaIdClasificacion recibe PARAMETRO aIdClasificacion: ENTERO SI Error <> 0 ENTONCES Error SI NO fEditaClasificacion Error = fCancelarModificacionClasificacion SI Error <> 0 ENTONCES Error SI NO fCancelarModificacionClasificacion FIN SI FIN SI } |

fActualizaClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fActualizaClasificacion (aClasificacionDe, aNumClasificacion, aNombreClasificacion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aClasificacionDe | Entero | Por valor | Clasificación de: 1 – Agente 2 – Cliente 3 – Proveedor 4 – Almacén 5 – Producto. |  |
| aNumClasificacion | Entero | Por valor | Número de la clasificación (1-6) |  |
| aNombreClasificacion | Cadena | Por valor | Texto a actualizar en la clasificación. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función actualiza la dirección del registro de Cliente/Proveedor activo.

**Ejemplo**

Actualiza Clasificacion

{

VAR Error: ENTERO

Error = fActualizaClasificacion recibe PARAMETRO aClasificacionDe: ENTERO, PARAMETRO aNumClasificacion: ENTERO, PARAMETRO aNombreClasificacion: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fActualizaClasificacion

FIN SI

}

fLeeDatoClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoClasificacion (aCampo, aValr, aLen) |  |  |  |
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

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de Clasificaciones.

**Ejemplo**

Lee Dato Clasificacion

{

VAR Error: ENTERO

VAR aValor: STRINGBUILDER

Error = fBuscaIdClasificacion recibe PARAMETRO aIdClasificacion: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fLeeDatoClasificacion recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoClasificacion

FIN SI

FIN SI

}

fSetDatoClasificacion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoClasificacion (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino. |  |
| aValor | Cadena | Por referencia | Valor de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de Clasificaciones.

**Ejemplo**

Set Dato Clasificacion

{

VAR Error: ENTERO

Error = fBuscaIdClasificacion recibe PARAMETRO aIdClasificacion: ENTERO

SI

Error <> 0

ENTONCES

Error = fEditaClasificacion

SI

Error <> 0

ENTONCES

Error

SI NO

Error = fSetDatoClasificacion recibe PARAMETRO aCampo: CADENA, aValor: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fSetDatoClasificacion

FIN SI

FIN SI

FIN SI

}