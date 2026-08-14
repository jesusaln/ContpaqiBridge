## Inicialización / Terminación

fInicializaSDK ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |
|---|---|
| Sintaxis | fInicializaSDK() |
| Parámetros | No usa. |
| Retorna | Valores enteros: kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito. !kSIN_ERRORES = Diferente de 0 (cero) – Código del error. |
| Descripción | Inicializa el SDK de CONTPAQi Comercial Premium®. Se requiere llamar esta función al inicio de cualquier aplicación que utilice el SDK. Establece la conexión entre la aplicación desarrollada y la Base de datos de CONTPAQi Comercial Premium®. Su uso es obligatorio. |
| Ejemplo | El siguiente código inicializa el SDK de CONTPAQi Comercial Premium® y asigna el resultado a una variable entera que se evalúa posteriormente; si su valor es distinto de 0 (cero) la aplicación se detiene. lError = fInicializaSDK() If lError <> 0 Then MensajeError lError End End If |

****

fTerminaSDK ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |
|---|---|---|
| Sintaxis | fTerminaSDK () |  |
| Parámetros | No usa. |  |
| Retorna | No tiene valor de retorno. |  |
| Descripción | Libera todos los recursos solicitados por el SDK, se requiere llamar al terminar de utilizar el SDK. |  |
| Ejemplo | Termina SDK{ VAR Error: ENTERO fInicializaSDK Error = fTerminaSDK SI Error <> 0 ENTONCES Error SI NO fTerminaSDK FIN SI } |  |
|  | Para utilizar esta función es necesario principalmente inicializar SDK. Puede ser utilizada fTerminaSDK desde un botón o por medio de una ejecución de form closing.  | Nota: Es muy importante que siempre se termine la sesión de SDK ya que el servicio puede quedarse colgado y ocasionar problemas en los siguientes inicios de sesión. |

fSetNombrePAQ ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |
|---|---|---|
| Sintaxis | fSetNombrePAQ(aSistema) |  |
| Parámetros | Nombre Tipo Uso Descripción aSistema Cadena Por referencia Nombre del sistema al que se conectará el SDK.  | Importante: Para establecer una conexión a CONTPAQi Factura Electrónica® este parámetro deberá ser igual a “CONTPAQ I Facturacion”. |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error.

**Descripción**

Esta función define el sistema al que se conectará el SDK. Sino se usa esta función la conexión por omisión será al sistema**CONTPAQi Comercial Premium®**.

Si se desea establecer una conexión a **CONTPAQi Factura Electrónica®** el parámetro **aSistema** deberá ser CONTPAQ I Facturacion y se deberá utilizar en vez de la función fInicializaSDK().

**Ejemplo**

Set Nombre PAQ{

VAR Error: ENTERO

VAR aNombrePAQ: CADENA

Error = fSetNombrePAQ recibe aNombrePAQ

SI

Error <> 0

ENTONCES

Error

SI NO

fSetNombrePAQ

FIN SI

}