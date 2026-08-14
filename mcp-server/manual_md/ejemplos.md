## Ejemplos

Pseudocódigo “Funciones de empresas”

Buscar empresa {

VAR aIdEmpresa, error: ENTERO

VAR aNombreEmpresa, aDirectorioEmpresa: CADENA (StringBuilder)

VAR alDirectorioEmpresa: String

Error = fPosPrimerEmpresa recibe REFERENCIA aIdEmpresa, PARÁMETRO aNombreEmpresa, PARÁMETRO aDirectorioEmpresa

HACER

SI

aNombreEmpresa = EMPRESA QUE BUSCAMOS

ENTONCES

alDirectorioEmpresa convierte : CADENA (String) aDirectorioEmpresa

fAbrirEmpresa recibe PARÁMETRO alDirectorioEmpresa

CORTAR

SI NO

fPosSiguienteEmpresa recibe REFERENCIA aIdEmpresa, PARÁMETRO aNombreEmpresa, PARÁMETRO aDirectorioEmpresa

FIN SI

MIENTRAS verdadero

}

Comentario de retroalimentación

Las funciones **fPosPrimerEmpresa ()**y **fPosSiguienteEmpresa ()** están marcadas como que reciben tres referencias a parámetros:

| Nombre Tipo Uso Descripción aIdEmpresa Entero Por Referencia Identificador de la empresa. aNombreEmpresa Cadena Por Referencia Nombre de la empresa. aDirectorioEmpresa Cadena Por Referencia Directorio de la empresa. |
|---|

La situación real es que solo **aIdEmpresa**es enviado como referencia para el caso de **aNombreEmpresa** y **aDirectorioEmpresa**es necesario enviar un parámetro pero que a su vez se pueda consumir como una referencia, para este caso un StringBuilder cumple esos requisitos.

|  | Considera que: Si se intenta mandar una referencia en lugar de un parámetro marcara un error de incompatibilidad de código. Si se utiliza directamente una cadena no se podrá consumir el valor de esta. |
|---|---|

La función **aDirectorioEmpresa**se tiene documentada indicando que recibe una referencia al parámetro, pero en realidad recibe el parámetro simple de tipo cadena.

**Nombre Tipo Uso Descripción**

aDirectorioEmpresa Cadena Por Referencia Directorio de la empresa.

Si se envía como referencia marcara error de sintaxis.