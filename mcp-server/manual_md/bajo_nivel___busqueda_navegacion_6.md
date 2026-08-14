## Bajo nivel - Búsqueda/Navegación

fBuscaConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaConceptoDocto (aCodConcepto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodConcepto | Cadena | Por valor | Código del concepto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca un concepto por su código.

**Ejemplo**

Busca Concepto Documento

{

VAR Error: ENTERO

Error = fBuscaConceptoDocto recibe PARAMETRO lCodConcepto: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaConceptoDocto

FIN SI

}

fBuscaIdConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaIdConceptoDocto (aIdConcepto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdConcepto | Entero | Por valor | Identificador del concepto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función busca un concepto por su Identificador.

**Ejemplo**

Busca Id Concepto Documento

{

VAR Error: ENTERO

Error = fBuscaIdConceptoDocto recibe PARAMETRO aIdConcepto: ENTERO

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaIdConceptoDocto

FIN SI

}

fPosPrimerConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el primer registro de la tabla de Conceptos. |
| Ejemplo | Posición Primer Concepto Documento { VAR Error: ENTERO Error = fPosPrimerConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosPrimerConceptoDocto FIN SI } |

fPosUltimaConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosUltimaConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el último registro de la tabla de Conceptos. |
| Ejemplo | Posición Ultima Concepto Documento { VAR Error: ENTERO Error = fPosUltimaConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosUltimaConceptoDocto FIN SI } |

fPosSiguienteConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el siguiente registro de la posición actual de la tabla de Conceptos. |
| Ejemplo | Posición Siguiente Concepto Documento { VAR Error: ENTERO Error = fPosSiguienteConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosSiguienteConceptoDocto FIN SI } |

fPosAnteriorConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Esta función se ubica en el registro anterior de la posición actual de la tabla de Conceptos. |
| Ejemplo | Posición Anterior Concepto Documento { VAR Error: ENTERO Error = fPosAnteriorConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosAnteriorConceptoDocto FIN SI } |

fPosBOFConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosAnteriorConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el inicio de la tabla de Conceptos. |
| Ejemplo | Posición BOF Concepto Documento { VAR Error: ENTERO Error = fPosBOFConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosBOFConceptoDocto FIN SI } |

fPosEOFConceptoDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosEOFConceptoDocto () |
| Parámetros | No usa. |
| Retorna | Valores enteros: 1 (uno) – Verdadero. 0 (cero) – Falso. |
| Descripción | Informa si el registro activo se encuentra en el fin de la tabla de Conceptos. |
| Ejemplo | Posición EOF Concepto Documento { VAR Error: ENTERO Error = fPosEOFConceptoDocto SI Error <> 0 ENTONCES Error SI NO fPosEOFConceptoDocto FIN SI } |