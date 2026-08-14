## Navegación

fPosPrimerEmpresa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosPrimerEmpresa(aIdEmpresa, aNombreEmpresa, aDirectorioEmpresa ) |
| Parámetros | Nombre Tipo Uso Descripción aIdEmpresa Entero Por Referencia Identificador de la empresa. aNombreEmpresa Cadena Por Referencia Nombre de la empresa. aDirectorioEmpresa Cadena Por Referencia Directorio de la empresa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. aIdEmpresa: Al finalizar la función este parámetro contiene el identificador de la primera empresa registrada en la Base de Datos. aNombreEmpresa: Al finalizar la función este parámetro contiene el nombre de la primera empresa registrada en la Base de Datos. aDirectorioEmpresa: Al finalizar la función este parámetro contiene el directorio de la primera empresa registrada en la base de datos. |
| Descripción | Esta función se posiciona en el primer registro de la base de datos de empresas de CONTPAQi Comercial Premium®, modifica los parámetros aNombreEmpresa y aDirectorioEmpresa, en los cuales guarda el nombre de la primera empresa y su ruta, correspondientemente. |
| Ejemplo | El siguiente código indica a la aplicación que se posicione en el primer registro de empresas de la base de datos de CONTPAQi Comercial Premium®. fPosPrimerEmpresa(lIdEmpresa,lNombreEmpresa,lDirectorioEmpresa) |

fPosSiguienteEmpresa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fPosSiguienteEmpresa (aIdEmpresa, aNombreEmpresa, aDirectorioEmpresa ) |
| Parámetros | Nombre Tipo Uso Descripción aIdEmpresa Entero Por Referencia Identificador de la empresa. aNombreEmpresa Cadena Por Referencia Nombre de la empresa. aDirectorioEmpresa Cadena Por Referencia Directorio de la empresa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. aIdEmpresa: Al finalizar la función este parámetro contiene el identificador de la primera empresa registrada en la base de datos. aNombreEmpresa: Al finalizar la función este parámetro contiene el nombre de la primera empresa registrada en la base de datos. aDirectorioEmpresa: Al finalizar la función este parámetro contiene el directorio de la primera empresa registrada en la base de datos. |
| Descripción | Esta función avanza al siguiente registro en la tabla de Empresas de CONTPAQi Comercial Premium®®; en caso de que no exista un siguiente registro, la función retorna un valor distinto de 0 (cero). |
| Ejemplo | El siguiente código termina el SDK de CONTPAQi Comercial Premium®®. fPosSiguienteEmpresa (lIdEmpresa,lNombreEmpresa,lDirectorioEmpresa) |

|  | Nota: Considera que para los parámetros de tipo cadena por referencia, se puede utilizar el tipo de datos Stringbuilder. |
|---|---|