## Alto nivel – Lectura/Escritura

fAltaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fAltaDireccion (aIdDireccion, astDireccion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdDireccion | Entero | Por referencia | Identificador de la dirección. |  |
| astDireccion | tDireccion | Por valor | Tipo de dato abstracto. |  |

|  | Importante Al usar esta función de alto nivel, es necesario asignar al campo cTipoDireccion alguno de los siguientes valores: 1 = Domicilio Fiscal 2 = Domicilio Envío |
|---|---|

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

aIdDireccion: Al finalizar la función este parámetro contiene el identificador del nuevo producto.

**Descripción**

Esta función da de alta una nueva dirección.

**Ejemplo**

Alta Dirección{

VAR Error, idDireccion: ENTERO

REFERENCIA tDireccion: SDK

Ejecuta fBuscaCteProv recibe PARAMETRO aCodCteProv: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

VAR cTipoDireccion: tDireccion

VAR cPais: tDireccion

VAR cCodigoPostal: tDireccion

VAR cEstado: tDireccion

VAR cCiudad: tDireccion

VAR cColonia: tDireccion

VAR cNombreCalle: tDireccion

VAR cNumeroExterior: tDireccion

Ejecuta fAltaDireccion recibe REFERENCIA idDireccion, REFERENCIA tDireccion

SI

Error <> 0

ENTONCES

Error

SI NO

fAltaDireccion

FIN SI

FIN SI

}

**Comentarios**

Conforme al objeto **tDireccion**, se agregarán los datos que sean requeridos y mínimos según la solicitud para dar de alta una dirección. En el ejemplo se muestran algunos, esto es dependiendo de la funcionalidad que se desarrolle en el proyecto

Para mas detalles de los campos para tenerse en cuenta en el uso de la estructura tDireccion, se puede consultar el documento COM_BDD en la tabla de domicilios, este documento contiene la estructura de la Base de Datos y se encuentra en C:\Program Files (x86)\Compac\COMERCIAL\Ayuda. Donde podemos revisar los datos importantes a considerar como lo son CIDCATALOGO, CTIPOCATALOGO, CTIPODIRECCION, entre otros.

fActualizaDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fActualizaProducto (astDireccion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| astDireccion | tDireccion | Por valor | Tipo de dato abstracto. |  |

|  | Importante Al usar esta función de alto nivel, es necesario asignar al campo cTipoDireccion alguno de los siguientes valores: 1 = Domicilio Fiscal 2 = Domicilio Envío |
|---|---|

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función actualiza la dirección del registro de Cliente/Proveedor activo.

**Ejemplo**

Actualiza Dirección{

VAR Error: ENTERO

REFERENCIA tDireccion: SDK

Ejecuta fBuscaCteProv recibe PARAMETRO aCodCteProv: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

VAR cCodCteProv: tDireccion

VAR cTipoDireccion: tDireccion

VAR cTipoCatalogo: tDireccion

VAR cCodigoPostal: tDireccion

VAR cEstado: tDireccion

VAR cCiudad: tDireccion

VAR cColonia: tDireccion

VAR cNombreCalle: tDireccion

VAR cNumeroExterior: tDireccion

Ejecuta fActualizaDireccion recibe REFERENCIA tDireccion

SI

Error <> 0

ENTONCES

Error

SI NO

fActualizaDireccion

FIN SI

FIN SI

}

**Comentario**

Para más detalles de los campos para tenerse en cuenta en el uso de la estructura tDireccion, se puede consultar el documento COM_BDD en la tabla de domicilios, este documento contiene la estructura de la Base de Datos y se encuentra en C:\Program Files (x86)\Compac\COMERCIAL\Ayuda. Donde podemos revisar los datos importantes a considerar como lo son CIDCATALOGO, CTIPOCATALOGO, CTIPODIRECCION, entre otros.

fLlenaRegistroDireccion ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLlenaRegistroDireccion (astDireccion, aEsAlta ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| astDireccion | tDireccion | Por valor | Tipo de dato abstracto. |  |
| aEsAlta | Entero | Por valor | 1 = Nueva dirección. 2 = Actualización. |  |

|  | Importante Al usar esta función de alto nivel, es necesario asignar al campo cTipoDireccion alguno de los siguientes valores: 1 = Domicilio Fiscal 2 = Domicilio Envío |
|---|---|

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función asigna al registro de la base de datos los valores de la estructura de datos de la Dirección.

**Ejemplo**

Llena Registro Dirección{

VAR Error: ENTERO

REFERENCIA tDireccion: SDK

Ejecuta fInsertaDireccion

SI

Error <> 0

ENTONCES

Error

SI NO

VAR cNombreCalle: tDireccion

VAR cNumeroExterior: tDireccion

VAR cColonia: tDireccion

VAR cCodigoPostal: tDireccion

VAR cCiudad: tDireccion

VAR cEstado: tDireccion

VAR cPais: tDireccion

Ejecuta fLlenaRegistroDireccion recibe REFERENCIA tDireccion, PARAMETRO

aEsAlta: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

Ejecuta fGuardaDireccion

FIN SI

FIN SI

}