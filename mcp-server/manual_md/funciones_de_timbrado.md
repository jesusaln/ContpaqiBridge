# Funciones de timbrado

fTimbraXML () | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fTimbraXML |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Se envía la información del XML para ser procesada por el PAC. |
| Ejemplo | Timbra XML { fInicializaLicenseInfo recibe VAR aSistema byte VAR Error: ENTERO VAR rutaXML: CADENA VAR codConcepto: CADENA VAR UUID: StringBuilder VAR rutaDDA: CADENA VAR rutaResultado: CADENA VAR aPass: CADENA VAR rutaFormato: CADENA Error = fTimbraXML recibe rutaXML, codConcepto, UUID, rutaDDA, rutaResultado, aPass, rutaFormato SI Error <> 0 ENTONCES Error SI NO fTimbraXML FIN SI } |

fTimbraXML () | Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fTimbraNominaXML |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error |
| Descripción | Se envía la información del XML de nómina para ser procesada por el PAC. |
| Ejemplo | Timbra Nomina XML { fInicializaLicenseInfo recibe VAR aSistema byte VAR Error: ENTERO VAR rutaXML: CADENA VAR codConcepto: CADENA VAR UUID: StringBuilder VAR rutaDDA: CADENA VAR rutaResultado: CADENA VAR aPass: CADENA VAR rutaFormato: CADENA Error = fTimbraNominaXML recibe rutaXML, codConcepto, UUID, rutaDDA, rutaResultado, aPass, rutaFormato SI Error <> 0 ENTONCES Error SI NO fTimbraNominaXML FIN SI } |