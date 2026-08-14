## Bajo nivel – Lectura/Escritura

fLeeDatoConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fLeeDatoConceptoDocto (aCampo, aValor, aLen) |  |  |  |
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

Esta función lee un campo del registro actual de conceptos documentos.

**Ejemplo**

Lee Dato Concepto Documento

{

VAR Error: ENTERO

VAR aValor: STRINGBUILDER

Error = fBuscaConceptoDocto recibe PARAMETRO aCodConcepto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fLeeDatoConceptoDocto recibe PARAMETRO aCampo: CADENA, aValor, PARAMETRO aLen: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

FLeeDatoConceptoDocto

FIN SI

FIN SI

}

fRegresPorcentajeImpuesto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fRegresPorcentajeImpuesto (aIdConceptoDocumento, aIdClienteProveedor, aIdProducto, aPorcentajeImpuesto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdConceptoDocumento | Entero | Por valor | Identificador del concepto del documento. |  |
| aIdClienteProveedor | Entero | Por valor | Identificador del cliente o proveedor. |  |
| aIdProducto | Entero | Por valor | Identificador del producto. |  |
| aPorcentajeImpuesto | Doble | Por referencia | Porcentaje de impuesto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

aPorcentajeImpuesto: Al finalizar la función este parámetro contiene el porcentaje del impuesto requerido.

**Descripción**

Esta función regresa el porcentaje de impuesto de un concepto documento, del cual se obtiene su configuración y se busca el porcentaje de la tabla de Clientes/Proveedores, Productos o de Parámetros generales.

**Ejemplo**

Regresa Porcentaje Impuesto

{

VAR Error: ENTERO

VAR aPorcentajeImpuesto: DOBLE

Error = fRegresPorcentajeImpuesto PARAMETRO aIdConceptoDocumento: ENTERO, PARAMETRO aIdClienteProveedor: ENTERO, PARAMETRO aIdProducto: ENTERO, REFERENCIA aPorcentajeImpuesto

SI

Error <> 0

ENTONCES

Error

SI NO

fRegresPorcentajeImpuesto

FIN SI

}

fEditaConceptoDocto()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fEditaConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función activa el modo de edición de un registro del catálogo Conceptos. |
| Ejemplo | Edita Concepto Documento { VAR Error: ENTERO VAR aCampo, aValor: STRINGBUILDER Error = fBuscaConceptoDocto recibe PARAMETRO: aCodConcepto: CADENA SI Error <> 0 ENTONCES Error SI NO fEditaConceptoDocto Error = fSetDatoConceptoDocto recibe aCampo, aValor SI Error <> 0 ENTONCES Error SI NO fGuardaConceptoDocto FIN SI FIN SI } |

fSetDatoConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fSetDatoConceptoDocto (const char *aCampo, char *aValor) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCampo | Cadena | Por referencia | Nombre del campo |  |
| aValor | Cadena | Por referencia | Valor del campo |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función escribe el valor indicado en el campo correspondiente en el registro activo de la tabla Conceptos.

**Ejemplo**

Set Dato Concepto Documento

{

VAR Error: ENTERO

VAR aCampo, aValor: STRINGBUILDER

Error = fBuscaConceptoDocto recibe PARAMETRO: aCodConcepto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fEditaConceptoDocto

Error = fSetDatoConceptoDocto recibe aCampo, aValor

SI

Error <> 0

ENTONCES

Error

SI NO

fGuardaConceptoDocto

FIN SI

FIN SI

}

fGuardaConceptoDocto()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fGuardaConceptoDocto() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función guarda los cambios efectuados al registro de la tabla Conceptos. |