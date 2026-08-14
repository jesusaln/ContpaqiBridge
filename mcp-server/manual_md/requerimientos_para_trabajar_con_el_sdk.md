# Requerimientos para trabajar con el SDK

****

## Ambiente

- CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica® instalado (monousuario o como estación).
- Entorno de programación. Editor/Compilador del lenguaje elegido (VB / Delphi / C / Plataforma .net, etc).
- Si estás programando en VBA (Excel) el SDK solo funciona en Microsoft® Office de 32 bits.
- Verifica contar con la licencia requerida por las funciones. Algunas funciones, como las de timbrado requieren licencias de un número de usuarios específico:

| Sistema | Versión | Función | Usuarios | Licenciamiento | Número de empresas |
|---|---|---|---|---|---|
| CONTPAQi Factura Electrónica® | 14.1.0 | fEmiteDocumento fTimbraXML fTimbraNominaXML | 2 usuarios o superior 5 usuarios o superior 5 usuarios o superior | Anual | Multiempresa (MultiRFC) |
| CONTPAQi Comercial Premium® | 12.1.0 | fEmiteDocumento | 5 usuarios o superior |  |  |

Archivos usados por el SDK

Todos estos archivos son utilizados por el SDK:

| Archivo | Descripción | Ubicación |
|---|---|---|
| MGW_SDK.dll | Es la interfase del SDK con CONTPAQi Factura Electrónica®. Libreria de encadenado, aquí se encuentran las funciones del SDK. | C:\Archivos de programa\Compacw\Facturacion |
| MGWServicios | Es la interfase del SDK con Comercial Premium. Libreria de encadenado, aquí se encuentran las funciones del SDK. | C:\Program Files (x86)\Compac\COMERCIAL |

|  | Recuerda: Para el caso del sistema de CONTPAQi Factura Electrónica®, los archivos se encuentran en la carpeta Facturacion. |
|---|---|

|  | Importante: Se debe tener especial cuidado con el control de versiones con el SDK en la que se desarrolla una aplicación y la versión del sistema con la que se va a interactuar. Es decir, no se recomienda desarrollar una aplicación con el SDK de CONTPAQi Factura Electrónica®10.0.0 para interactuar con un CONTPAQi Factura Electrónica® 12.0.0. |
|---|---|