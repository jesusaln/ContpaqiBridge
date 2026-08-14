## Emisión del complemento de Carta Porte 3.1 mediante SDK

La autoridad fiscal modificó la forma en que se emiten los documentos con el complemento Carta Porte, estableciendo una estructura fija en el XML.

El sistema **CONTPAQi Comercial Premium®**, junto con el SDK para desarrolladores, permite la emisión de CFDI con el complemento Carta Porte 3.1.

El SDK permite emitir documentos con dicho complemento, pero no cubrir su llenado o modificación dentro del sistema.

Requerimientos técnicos:

Para poder implementar este caso de uso, es necesario contar con:

- Versión mínima de CONTPAQi Comercial Premium® 9.1.1.
- Versión mínima de CONTPAQi SDK 16.3.0.
- Instalación estable y funcional de ambos sistemas, con un desarrollo previo que permita acceder a una empresa mediante SDK.
- Conocimiento de las reglas de llenado del complemento Carta Porte 3.1 estipuladas por el SAT (consultables en: Carta Porte 3.1 SAT).
- Un documento dentro de CONTPAQi Comercial Premium® al que se quiera adjuntar el complemento.
- CSD vigente con su contraseña (pueden ser DEMO), previamente agregado en la configuración del concepto que se usará para la emisión.

Recomendaciones de uso:

Contar con una licencia Comercial con al menos 5 usuarios, lo cual es recomendable para un correcto funcionamiento al momento de emitir documentos.

Implementación en el sistema de CONTPAQi Comercial Premium®

La emisión de documentos con el complemento Carta Porte 3.1 mediante el SDK de **CONTPAQi Comercial Premium®** puede realizarse en dos escenarios:

Escenario 1: El documento ya existe en el sistema y cuenta con el complemento Carta Porte 3.1 previamente generado y correctamente llenado.

Escenario 2: El documento ya está creado en el sistema, pero aún no tiene asociado el complemento Carta Porte 3.1.

En ambos casos, la emisión se realiza utilizando la función fEmitirDocumento, que es la encargada de ejecutar el proceso desde el SDK. Su declaración es la siguiente:

[DllImport("MGWServicios.DLL")]

public static extern Int32 fEmitirDocumento(

[MarshalAs(UnmanagedType.LPStr)] string aCodConcepto,

[MarshalAs(UnmanagedType.LPStr)] string aSerie,double aFolio,

[MarshalAs(UnmanagedType.LPStr)] string aPassword,

[MarshalAs(UnmanagedType.LPStr)] string aArchivoAdicional

);

|  | Nota La función fEmitirDocumento se utiliza de la misma manera que en versiones anteriores; su declaración no ha cambiado. |
|---|---|