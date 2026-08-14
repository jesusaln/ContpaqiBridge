## Bajo nivel – Lectura/Escritura

fInsertaDatoCompEducativo ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fInsertaDatoCompEducativo(int aIdServicio, int aNumCampo, char *aDato ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdServicio | Entero | Por valor | Identificador del servicio |  |
| aNumCampo | Entero | Por valor | Número de campo |  |
| aDato | Cadena | Por referencia | Valor a insertar |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función inserta un registro correspondiente a los datos adicionales para el complemento educativo del catálogo servicios.

**Ejemplo**

Inserta Complemento Educativo{

VAR Error: ENTERO

Ejecuta fInsertaDatoCompEducativo recibe VAR aIdServicio: ENTERO, VAR aNumCampo: ENTERO, VAR aDato: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fInsertaDatoCompEducativo

FIN SI

}

**Comentarios**

Para insertar el complemento educativo es necesario utiliza la función fInsertaDatoCompEducativo como lo muestra el ejemplo; esta función requiere de tres parámetros, los cuales son:

- Id del servicio
- Número de campo (se muestra el listado de los campos más adelante)
- Valor que se le asignará al campo

Los campos requeridos por el complemento educativo son los siguientes:

- Nombre del alumno
- CURP del alumno
- Nivel educativo
- Autorización o reconocimiento
- RFC de quien realiza el pago

fInsertaDatoAddendaDocto ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fInsertaDatoAddendaDocto(aIdAddenda, aIdCatalogo, aNumCampo, aDato) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aIdAddenda | Entero | Por valor | Identificador de la Addenda |  |
| aIdCatalogo | Entero | Por valor | Identificador del documento |  |
| aNumCampo | Entero | Por valor | Número de campo |  |
| aDato | Cadena | Por referencia | Valor a insertar |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Agrega los datos de la addenda para los documentos.

**Ejemplo**

Inserta Dato Addenda Documento{

VAR Error: ENTERO

Ejecuta fAltaDocumento

Ejecuta fInsertaDatoAddendaDocto recibe VAR aIdAddenda: ENTERO, VAR aIdCatalogo: ENTERO, VAR aNumCampo: ENTERO, VAR aDato: CADENA

SI

Error <> 0

ENTONCES

Error

SI NO

fInsertaDatoAddendaDocto

FIN SI

}

fObtieneLicencia ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fObtieneLicencia (aCodActiva, aCodSitio, aSerie, aTagVersion) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aCodActiva | Cadena | Por referencia | Variable en la que regresa el código de activación del sistema. |  |
| aCodSitio | Cadena | Por referencia | Variable en la que regresa el código de sitio del sistema. |  |
| aSerie | Cadena | Por referencia | Variable en la que regresa el número de serie del sistema. |  |
| aTagVersion | Cadena | Por referencia | Variable en la que regresa el versión del sistema. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función regresa la licencia del producto.

|  | Nota: Antes de llamar la función fObtieneLicencia se deberá llamar la función fInicializaLicenseInfo. |
|---|---|

**Ejemplo**

Obtiene Licencia

{

fInicializaLicenseInfo recibe VAR aSistema BYTE

VAR Error: ENTERO

VAR aCodActiva: StringBuilder

VAR aCodSitio: StringBuilder

VAR aSerie: StringBuilder

VAR aTagVersion: StringBuilder

Error = fObtieneLicencia recibe aCodActiva, aCodSitio, aSerie, aTagVersion

SI

Error <> 0

ENTONCES

Error

SI NO

fObtieneLicencia

FIN SI

}

**Comentarios**

|  | Nota: Este proceso funciona únicamente con CONTPAQi Factura Electrónica®. |
|---|---|

fObtienePassProxy ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fObtienePassProxy(aPassProxy ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aPassProxy | Cadena | Por referencia | Variable en la que regresa la contraseña del proxy. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Regresa la contraseña del proxy.

**Ejemplo**

El siguiente código regresa la contraseña del proxy.

Obtiene Pass Proxy

{

fInicializaLicenseInfo recibe VAR aSistema BYTE

VAR Error: ENTERO

VAR aPassProxy: STRINGBUILDER

Error = fObtienePassProxy recibe aPassProxy

SI

Error <> 0

ENTONCES

Error

SI NO

fObtienePassProxy

FIN SI

}

fTimbraXML ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fTimbraXML( char *aRutaXML, char *aCodConcepto, char *aUUID, char *aRutaDDA, char *aRutaResultado, char *aPass, char *aRutaFormato ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aRutaXML | Cadena | Por referencia | Variable donde se especifica la ruta y archivo donde está ubicado el XML a timbrar. |  |
| aCodConcepto | Cadena | Por referencia | Variable donde se pasa el código del concepto a utilizar para timbrar el XML. Este concepto deberá estar configurado como CFDI. |  |
| aUUID | Cadena | Por referencia | Variable donde se regresa el UUID del XML timbrado. |  |
| aRutaDDA | Cadena | Por referencia | Variable donde se especifica la ruta y archivo DDA que contiene información adicional del XML. |  |
| aRutaResultado | Cadena | Por referencia | Variable donde se especifica la ruta donde se generará el XML, HTML y las imágenes para la entrega en formato amigable. |  |
| aPass | Cadena | Por referencia | Variable donde se especifica la contraseña del certificado para timbrar el XML. |  |
| aRutaFormato | Cadena | Por referencia | Variable con la ruta y archivo del formato de impresión. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función timbra un XML creado con una aplicación de un tercero.

El XML deberá estar sin emitir, sin sello y sin certificado.

Esta función requeire una liciencia de 2 o más usuarios. Si cuentas con un licenciamiento anual además se requeire que la licencia sea multiempresa.

**Ejemplo**

El siguiente código timbra un XML.

Timbra XML

{

fInicializaLicenseInfo recibe VAR aSistema BYTE

VAR Error: ENTERO

VAR rutaXML: CADENA

VAR codConcepto: CADENA

VAR UUID: STRINGBUILDER

VAR rutaDDA: CADENA

VAR rutaResultado: CADENA

VAR aPass: CADENA

VAR rutaFormato: CADENA

Error = fTimbraXML recibe rutaXML, codConcepto, UUID, rutaDDA, rutaResultado, aPass, rutaFormato

SI

Error <> 0

ENTONCES

Error

SI NO

fTimbraXML

FIN SI

}

fTimbraNominaXML ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fTimbraNominaXML( char *aRutaXML, char *aCodConcepto, char *aUUID, char *aRutaDDA, char *aRutaResultado, char *aPass, char *aRutaFormato ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aRutaXML | Cadena | Por referencia | Variable donde se especifica la ruta y archivo donde está ubicado el XML a timbrar. |  |
| aCodConcepto | Cadena | Por referencia | Variable donde se pasa el código del concepto a utilizar para timbrar el XML. Este concepto deberá estar configurado como CFDI. |  |
| aUUID | Cadena | Por referencia | Variable donde se regresa el UUID del XML timbrado. |  |
| aRutaDDA | Cadena | Por referencia | Variable donde se especifica la ruta y archivo DDA que contiene información adicional del XML. |  |
| aRutaResultado | Cadena | Por referencia | Variable donde se especifica la ruta donde se generará el XML, HTML y las imágenes para la entrega en formato amigable. |  |
| aPass | Cadena | Por referencia | Variable donde se especifica la contraseña del certificado para timbrar el XML. |  |
| aRutaFormato | Cadena | Por referencia | Variable con la ruta y archivo del formato de impresión. |  |

**Retorna**

Valores enteros:

kSIN_ERRORES = 0 (cero) – La operación fue realizada con éxito.

!kSIN_ERRORES = Diferente de 0 (cero) – Código del error

**Descripción**

Esta función timbra un XML de una nómina creado con una aplicación de un tercero.

El XML deberá estar sin emitir, sin sello y sin certificado. Es obligatorio que el XML lleve el domicilio del emisor.

Si deseas ver en la impresión del formato amigable algún dato del complemento de nómina se deberá insertar en el DDA.

Esta función requiere una liciencia de 5 o más usuarios. Si cuentas con un licenciamiento anual además se requiere que la licencia sea multiempresa.

**Ejemplo**

Timbra Nomina XML

{

fInicializaLicenseInfo recibe VAR aSistema BYTE

VAR Error: ENTERO

VAR rutaXML: CADENA

VAR codConcepto: CADENA

VAR UUID: STRINGBUILDER

VAR rutaDDA: CADENA

VAR rutaResultado: CADENA

VAR aPass: CADENA

VAR rutaFormato: CADENA

Error = fTimbraNominaXML recibe rutaXML, codConcepto, UUID, rutaDDA, rutaResultado, aPass, rutaFormato

SI

Error <> 0

ENTONCES

Error

SI NO

fTimbraNominaXML

FIN SI

}