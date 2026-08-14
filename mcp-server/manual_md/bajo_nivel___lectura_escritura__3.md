## Bajo nivel – Lectura/Escritura

fInsertaCteProv () | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInsertaCteProv () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Adiciona un nuevo registro en la tabla de Clientes / Proveedores en modo de inserción. |
| Ejemplo | INSERTAR CLIENTE-PROVEEDOR { VAR idCteProv CADENA(StringBuilder) VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO VAR aInserCampos ENTERO error = ejecutar fInsertaCteProv SI error = 0 ENTONCES ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong ejecutar fEditaCteProv MIENTRAS aInserCampos > 0 HACER aCampo = nuevo campo aValor = nuevo valor ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor FIN HACER FIN MIENTRAS ejecutar fGuardaCteProv FIN ENTONCES } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para las funciones fLeeDatoCteProv y fSetDatoCteProv en el documento estructura de la BDD comercial (COM_BDD) tabla admClientes. aCampo = Nombre del campo, aValor = Valor del campo Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fEditaCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaCteProv () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Activa el modo de Edición de un registro en la tabla de Clientes/Proveedores. |
| Ejemplo | INSERTAR CLIENTE-PROVEEDOR { VAR idCteProv CADENA(StringBuilder) VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO VAR aInserCampos ENTERO error = ejecutar fInsertaCteProv SI error = 0 ENTONCES ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong ejecutar fEditaCteProv MIENTRAS aInserCampos > 0 HACER aCampo = nuevo campo aValor = nuevo valor ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor FIN HACER FIN MIENTRAS ejecutar fGuardaCteProv FIN ENTONCES } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para las funciones fLeeDatoCteProv y fSetDatoCteProv en el documento estructura de la BDD comercial (COM_BDD) tabla admClientes. aCampo = Nombre del campo, aValor = Valor del campo Se puede asignar un valor a la gran mayoría de campos; algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fGuardaCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaCteProv () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Guarda los cambios realizados a un registro de cliente/proveedor. |
| Ejemplo | INSERTAR CLIENTE-PROVEEDOR { VAR idCteProv CADENA(StringBuilder) VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO VAR aInserCampos ENTERO error = ejecutar fInsertaCteProv SI error = 0 ENTONCES ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor, PARAMETRO aLong ejecutar fEditaCteProv MIENTRAS aInserCampos > 0 HACER aCampo = nuevo campo aValor = nuevo valor ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor FIN HACER FIN MIENTRAS ejecutar fGuardaCteProv FIN ENTONCES } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para las funciones fLeeDatoCteProv y fSetDatoCteProv en el documento estructura de la BDD comercial (COM_BDD) tabla admClientes. aCampo = Nombre del campo, aValor = Valor del campo Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fBorraCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fBorraCteProv () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Borra un registro en la tabla de Clientes / Proveedores. |
| Ejemplo | BORRAR CLIENTE-PROVEEDOR (recibe aIdCteProv) { VAR error ENTERO error ejecutar fBuscaIdCteProv recibe PARAMETRO aIdCteProv SI error = 0 ENTONCES ejecutar fBorrarCteProv FIN ENTONCES } |

fCancelarModificacionCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCancelarModificacionCteProv () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función cancela las modificaciones al registro actual de Clientes / Proveedores. El registro debe estar en modo de edición o inserción. |
| Ejemplo | CANCELAR EDICION { VAR aCodCteProv CADENA VAR error ENTERO VAR aCampo CADENA VAR aValor CADENA VAR aLong ENTERO VAR aEditCampos ENTERO Error = ejecutar fBuscaCteProv recibe PARAMETRO aCodCteProv SI error = 0 ENTONCES ejecutar fEditaCteProv MIENTRAS aEditCampos > 0 HACER aCampo = nuevo campo aValor = nuevo valor error = ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor SI error <> 0 ENTONCES ejecutar fCancelarModificacionCteProv FIN ENTONCES FIN HACER FIN MIENTRAS FIN ENTONCES } |
| Comentarios | Se puede consultar el nombre de cada campo utilizable para las funciones fLeeDatoCteProv y fSetDatoCteProv en el documento estructura de la BDD comercial (COM_BDD) tabla admClientes. aCampo = Nombre del campo, aValor = Valor del campo Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables. |

fEliminarCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fEliminarCteProv (aCodigoCteProv) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodigoCteProv | Cadena | Por valor | Código del Cliente / Proveedor. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función elimina un Cliente / Proveedor usando su código.

**Ejemplo**

El siguiente código elimina un Cliente / Proveedor, si lo encuentra lo borra, en caso contrario envía el mensaje de error correspondiente:

Elimina Cliente Proveedor

{

VAR Error: ENTERO

Error = fEliminarCteProv recibe PARAMETRO aCodCliente: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEliminarCteProv

FIN SI

}

fSetDatoCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoCteProv (aCampo, aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla de Cliente/Proveedor.

**Ejemplo**

INSERTAR CLIENTE-PROVEEDOR

{

VAR idCteProv CADENA(StringBuilder)

VAR error ENTERO

VAR aCampo CADENA

VAR aValor CADENA

VAR aLong ENTERO

VAR aInserCampos ENTERO

error = ejecutar fInsertaCteProv

SI

error = 0

ENTONCES

ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor,

PARAMETRO aLong

ejecutar fEditaCteProv

MIENTRAS aInserCampos > 0

HACER

aCampo = nuevo campo

aValor = nuevo valor

ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor

FIN HACER

FIN MIENTRAS

ejecutar fGuardaCteProv

FIN ENTONCES

}

**Comentarios**

Se puede consultar el nombre de cada campo utilizable para las funciones **fLeeDatoCteProv**y **fSetDatoCteProv**en el documento estructura de la BDD comercial (COM_BDD) tabla **admClientes**.

**aCampo** = Nombre del campo,**aValor** = Valor del campo

Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables.

fLeeDatoCteProv ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoCteProv (aCampo, aValr, aLen) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por valor | Campo destino |  |
| aValor | Cadena | Por valor | Valor de escritura |  |
| aLen | Entero | Por valor | Longitud del dato de lectura. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aValor: Al finalizar la función este parámetro contiene el valor del campo especificado.

**Descripción**

Esta función lee el valor indicado del campo correspondiente en el registro activo de la tabla de Cliente / Proveedor.

**Ejemplo**

INSERTAR CLIENTE-PROVEEDOR

{

VAR idCteProv CADENA(StringBuilder)

VAR error ENTERO

VAR aCampo CADENA

VAR aValor CADENA

VAR aLong ENTERO

VAR aInserCampos ENTERO

error = ejecutar fInsertaCteProv

SI

error = 0

ENTONCES

ejecutar fLeeDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor,

PARAMETRO aLong

ejecutar fEditaCteProv

MIENTRAS aInserCampos > 0

HACER

aCampo = nuevo campo

aValor = nuevo valor

ejecutar fSetDatoCteProv recibe PARAMETRO aCampo, PARAMETRO aValor

FIN HACER

FIN MIENTRAS

ejecutar fGuardaCteProv

FIN ENTONCES

}

**Comentarios**

Se puede consultar el nombre de cada campo utilizable para las funciones **fLeeDatoCteProv**y **fSetDatoCteProv**en el documento estructura de la BDD comercial (COM_BDD) tabla **admClientes**.

**aCampo** = Nombre del campo,**aValor** = Valor del campo

Se puede asignar un valor a la gran mayoría de campos, algunos tienen restricciones que hay que cumplir y otros tantos como el ID no son editables.