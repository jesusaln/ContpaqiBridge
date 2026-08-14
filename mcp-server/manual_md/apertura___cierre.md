## Apertura / Cierre

fAbreEmpresa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fAbreEmpresa (aDirectorioEmpresa ) |
| Parámetros | Nombre Tipo Uso Descripción aDirectorioEmpresa Cadena Por Referencia Directorio de la empresa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Esta función abre la empresa que corresponde a la ruta especificada en el parámetro aDirectorioEmpresa. |
| Ejemplo | El siguiente código indica a la aplicación que abra la empresa ubicada el la ruta C:\Compacw\Empresas\EmpresaEjemplo. lDirectorioEmpresa = “C:\Compacw\Empresas\EmpresaEjemplo” fAbreEmpresa (lDirectorioEmpresa) |

fCierraEmpresa ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fCierraEmpresa () |
| Parámetros | No usa. |
| Retorna | No tiene valor de retorno. |
| Descripción | Cierra la conexión con la empresa activa en la aplicación que usa el SDK. |
| Ejemplo | El siguiente código cierra la empresa activa. fCierraEmpresa() |