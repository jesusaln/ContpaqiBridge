## Alto nivel - Búsqueda/Navegación

fBuscaDocumento ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fBuscaDocumento (aLlaveDocto) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aLlaveDocto | tLlaveDocto | Por valor | Tipo de dato abstracto. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función busca un documento por su llave, si lo encuentra se posiciona en el registro correspondiente.

**Ejemplo**

BuscaDocumento

{

REFERENCIA RegLlaveDoc: SDK

VAR Error: ENTERO

VAR RegLlaveDoc.aCodConcepto: CADENA

VAR RegLlaveDoc.aSerie: CADENA

VAR RegLlaveDoc.aFolio: DOUBLE

Error = fBuscaDocumento recibe REFERENCIA RegLlaveDoc

SI

Error <> 0

ENTONCES

Error

SI NO

fBuscaDocumento

FIN SI

}