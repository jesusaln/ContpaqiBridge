## Bajo nivel – Lectura/Escritura

fInsertaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertaDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Adiciona un nuevo registro en la tabla de Direcciones en modo de inserción. |
| Ejemplo | Inserta Direccion{ VAR idCte: CADENA(StringBuilder) VAR Error: ENTERO Ejecuta fBuscaCteProv recibe PARAMETRO aCodCteProv: CADENA SI Error <> 0 ENTONCES Error SI NO Ejecuta fLeeDatoCteProv recibe PARAMETRO aCampo: CADENA, PARAMETRO idCte, PARAMETRO aLen: ENTERO SI Error <> 0 ENTONCES Error SI NO Ejecuta fInsertaDireccion Ejecuta fSetDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaDireccion SI Error <> 0 ENTONCES Error SI NO fGuardaDireccion FIN SI FIN SI FIN SI } |
| Comentarios | Para agregar datos en la función fSetDatoDireccion se deberán agregar por lo menos los campos obligatorios para insertar una dirección. En el documento de referencia de las bases de datos del sistema CONTPAQi® que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato. |

fEditaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Direcciones. |
| Ejemplo | Edita Dirección{ VAR Error: ENTERO Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE SI Error <> 0 ENTONCES Error SI NO Ejecuta fEditaDireccion Ejecuta fSetDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaDireccion SI Error <> 0 ENTONCES Error SI NO fGuardaDireccion FIN SI FIN SI } |
| Comentarios | Para agregar datos en la función fSetDatoDireccion se deberán agregar por lo menos los campos obligatorios para insertar una dirección. En el documento de referencia de las bases de datos del sistema CONTPAQi® que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato. |

fGuardaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Guarda los cambios realizados a un registro de productos. |
| Ejemplo | Guarda Dirección{ VAR Error: ENTERO Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE SI Error <> 0 ENTONCES Error SI NO Ejecuta fEditaDireccion Ejecuta fSetDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fGuardaDireccion SI Error <> 0 ENTONCES Error SI NO fGuardaDireccion FIN SI FIN SI } |

fCancelarModificacionDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionDireccion () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función cancela las modificaciones al registro actual de direcciones. El registro debe estar en modo de edición o inserción. |
| Ejemplo | Cancela Modificación Dirección{ VAR Error: ENTERO Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE SI Error <> 0 ENTONCES Error SI NO Ejecuta fEditaDireccion Ejecuta fSetDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA Ejecuta fCancelarModificacionDireccion SI Error <> 0 ENTONCES Error SI NO fCancelarModificacionDireccion FIN SI FIN SI } |
| Comentario | Para agregar datos en la función fSetDatoDireccion se deberán agregar por lo menos los campos obligatorios para insertar una dirección. En el documento de referencia de las bases de datos del sistema CONTPAQi® que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato. |

fLeeDatoDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoDireccion (aCampo, aValr, aLen) |  |  |  |
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

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de Direcciones.

**Ejemplo**

Lee Dato Dirección{

VAR Error: ENTERO

VAR aValor: CADENA(StringBuilder)

Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

Ejecuta fLeeDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Regresa aValor

FIN SI

FIN SI

}

**Comentario**

Para agregar datos en la función **fLeeDatoDireccion**se puede consultar el documento de referencia de las bases de datos del sistema **CONTPAQi®** que se esté trabajando, se pueden consultar todos los campos, descripción y tipo de dato.

fSetDatoDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoDireccion (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino. |  |
| aValor | Cadena | Por referencia | Valor de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de Cliente / Proveedor.

**Ejemplo**

Set Dato Dirección{

VAR Error: ENTERO

Ejecuta fBuscaDireccionCteProv recibe PARAMETRO aCodCteProv: CADENA, PARAMETRO aTipoDireccion: BYTE

SI

Error <> 0

ENTONCES

Error

SI NO

Ejecuta fEditaDireccion

Ejecuta fSetDatoDireccion recibe PARAMETRO aCampo: CADENA, PARAMETRO aValor: CADENA

Ejecuta fGuardaDireccion

SI

Error <> 0

ENTONCES

Error

SI NO

fGuardaDireccion

FIN SI

FIN SI

}