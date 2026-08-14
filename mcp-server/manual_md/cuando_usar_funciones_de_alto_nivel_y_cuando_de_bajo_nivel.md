# Cuándo usar funciones de alto nivel y cuando de bajo nivel

****

En términos generales se recomienda usar las funciones de alto nivel debido a que estas realizan todo los procesos necesarios para mantener las reglas de negocio y la base de datos estable.

Cualquier lenguaje de programación que soporte estructuras de datos podrá hacer uso de las funciones de alto nivel, la razón es que como generalidad las funciones de alto nivel efectúan operaciones con registros completos.

Las funciones de bajo nivel permiten más flexibilidad en cuanto que datos se graban el la base de datos, pero implican más trabajo, por realizar escritura campo por campo, y complejidad pues se tienen que validar diversos puentos para no romper las reglas de negocio, por lo que para su uso se requiere mas precisión al desarrollar el proceso.

Estas funciones se pueden usar en cualquier lenguaje de programación, más son de carácter obligatorio en aquellos que no manejen estructuras de datos. Por ejemplo Visual FoxPro.

Ejemplo: Dar de alta de datos extras del catálogo sólo se puede efectuar con las funciones de “bajo nivel”

Algunos lenguajes como Visual FoxPro no soportan el uso de estructuras de datos, por lo que forzosamente se deben usar las funciones de bajo nivel.

Restricciones al usar funciones de bajo nivel

****

Las funciones de bajo nivel permiten la escritura campo a campo en la BD de **CONTPAQi Comercial Premium®**, sin embargo existen campos que no pueden ser modificadas por dichas funciones pues son valores que calcula o modifica **CONTPAQi Comercial Premium®** o **CONTPAQi Factura Electrónica®**.

| Campo | Razón |
|---|---|
| cIdDocumento | Es un dato autogenerado. |
| cIdDocumentoDe | Depende de la plantilla del documento. |
| cIdConcepto | Es un dato autogenerado. |
| cIdCteProv | Es un dato autogenerado. |
| cIdAgente | Es un dato autogenerado. |
| cIdConcepto | Es un dato autogenerado. |
| cNeto | Es un campo calculado. |
| cTotal | Es un campo calculado. |
| cAfectado | Es un campo protegido. |
| cNaturaleza | Es un dato autogenerado. |
| cDocumentoOrigen | Es un dato autogenerado. |
| cPlantillacUsaProveedor | Es un campo calculado. |
| cUsaCliente | Es un dato autogenerado. |
| cNetocTotalUnidades | Es un campo calculado. |
| cBanObsevaciones | Es un dato autogenerado. |
| cBanDatosEnvio | Es un dato autogenerado. |
| cBanCondCredito | Es un dato autogenerado. |
| CUnidadesPendientes | Es un campo calculado. |
| cTimeStamp | Es un dato autogenerado. |