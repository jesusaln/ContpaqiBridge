Escenario 2: Documento sin complemento previamente creado en el sistema

En este segundo escenario, el proceso de emisión del documento se realizará mediante la función **fEmitirDocumento**. Sin embargo, debido a que el documento no cuenta con el complemento Carta Porte incluido de origen, será necesario adjuntar dicha información de manera externa durante el proceso de emisión.

Para lograrlo, se hará uso del parámetro **aArchivoAdicional**, el cual permite incorporar información adicional al documento.

¿Qué recibirá el parámetro aArchivoAdicional?

El documento requiere la información correspondiente al complemento Carta Porte, por lo que dichos datos deberán enviarse como un archivo de texto.

Dentro del aplicativo existe esta funcionalidad que permite adjuntar información de complementos a un documento mediante un archivo con extensión .ini o .xml, y es la que se utilizará para completar el documento desde el SDK.

El valor que se le de a la variable **aArchivoAdicional**deberá llevar la siguiente estructura:

Tipo de dato: **String**

Valor: **“Complemento:[ruta del archivo xml y/o ini]”**

La palabra **Complemento**indica el tipo de archivo que se intentará adjuntar al documento. Esta debe ir seguida del caracter **":"** (dos puntos), el cual indica el inicio de la ruta donde el SDK buscará el archivo .ini o .xml del que tomará la información del complemento.

|  | Nota Puedes consultar más información relacionada con este tema en este apartado Agregar archivo XML con complemento. |
|---|---|

|  | Importante La estructura del archivo .ini puede consultarse en el documento CartaPorte.ini, el cual se encuentra ubicado en la siguiente ruta: C:\Program Files (x86)\Compac\COMERCIAL |
|---|---|

A continuación, te mostramos un ejemplo de cómo implementar esta funcionalidad:

| Paso | Acción |  |
|---|---|---|
|  | Implementación C#: public static void EmitirDocumento(string aCodigoConcepto, double aFolio, string aSerie, string aContraseña) { //variables // Palabra Complemento posteriormente el carácter : y a continuación la ruta donde se encuentra la información del complemento seguida del nombre y extensión del archivo string aArchivoAdicional = @"Complemento:C:\Compac\Empresas\Esquemas\COMERCIAL\CartaPorte.ini"; MGWServicios.fEmitirDocumento(aCodigoConcepto, aSerie, aFolio, aContraseña, aArchivoAdicional); //hay que recordar que en caso de que la función retorne un código diferente de 0 indicara que no se ejecutó con éxito por lo que se puede utilizar una variable para consultar el valor que retorne la función. }  | Nota Aunque el ejemplo muestra un archivo con extensión .ini, también es posible utilizar un archivo con extensión .xml. |

|  | Consideraciones importantes Para realizar la emisión de la factura mediante fEmitirDocumento, el archivo .ini deberá encontrarse dentro de una carpeta llamada Adicionales, ubicada dentro del directorio correspondiente a la empresa. Es indispensable que el sistema pueda identificar que el archivo .ini contiene información del complemento Carta Porte. |
|---|---|

****

**Identificación del complemento Carta Porte:**

Para que el sistema reconozca correctamente que el archivo .ini contiene información correspondiente al complemento de Carta Porte, es obligatorio que la primera línea del archivo contenga la siguiente etiqueta:

**[CartaPorte3.1]**

Posteriormente, las secciones correspondientes a transportes, ubicaciones, figuras, entre otras, pueden organizarse de forma flexible, siempre y cuando los valores cumplan con lo establecido en la guía de llenado del complemento Carta Porte.

**Ejemplo:**

[CartaPorte3.1]

IdCCP = CCC945d7 - 600e-43f5 - 9b7b - ebbee802f670

TranspInternac = No

TotalDistRec = 254.

RegistroISTMO = No

[Transporte1]

CodigoMedio = MT01

Clave = 01

PermSCT = TPAF02

NumPermisoSCT = 554486

ConfigVehicular = VL

AseguraRespCivil = AXA

PolizaRespCivil = 56775ht56

PlacaVM = L785JH

AnioModeloVM=2023

AseguraCarga=222

PolizaCarga=777

[Figura1]

TipoFigura = 01

NombreFigura = Figura Transporte 01

RFCFigura = AAA010101000

NumLicencia = UIFUY847584

[Ubicacion1]

TipoUbicacion = Origen

IDUbicacion = OR000001

RFCRemitenteDestinatario = EKU9003173C9

NombreRemitenteDestinatario = DEMO

FechaHoraSalidaLlegada = 2024 - 01 - 30T16: 26:49

[Ubicacion1.Domicilio]

Pais = MEX

CodigoPostal = 26015

Estado = COA

Municipio = 025

Localidad = 06

Colonia = 2613

Calle = pablo N

NumeroExterior = 435

[Ubicacion2]

TipoUbicacion=Destino

IDUbicacion=DE000001

RFCRemitenteDestinatario=IIA040805DZ4

NombreRemitenteDestinatario=INDISTRIA ILUMINADORA DE ALMACENES SA DE CV

FechaHoraSalidaLlegada=2024-01-30T16:26:49

DistanciaRecorrida = 254.00

[Ubicacion2.Domicilio]

Pais = MEX

CodigoPostal = 29960

Estado = CHP

Municipio = 065

Localidad = 10

Colonia = 2034

Calle = PABLO N

NumeroExterior = 444

[Mercancias]

NumTotalMercancias=1

UnidadPeso=KGM

PesoBrutoTotal=1.000

PesoNetoTotal=1000.000

CargoPorTasacion=1000.00

[Mercancia1]

BienesTransp = 43211500

Descripcion = PRODUCTO 01

Cantidad = 20.00

ClaveUnidad = H87

Unidad = Pieza

PesoEnKg = 1

En estos archivos tanto archivo INI como XML, podrás incluir cualquier información del complemento sin ninguna restricción, sólo deberás verificar el XSD del Complemento de Carta Porte y la Guía de llenado correspondiente al "Medio de transporte".

****

Ejemplo del complemento en formato XML:

|  | Importante Es fundamental cumplir con todos los requerimientos establecidos por la autoridad fiscal para el uso correcto del complemento Carta Porte 3.1. El cumplimiento de estas disposiciones garantiza la validez fiscal del documento y evita posibles inconsistencias durante procesos de validación o auditoría. En caso de tener dudas sobre el diseño, estructura o generación adecuada de los complementos de Carta Porte 3.1, se recomienda consultar la información oficial publicada por el Servicio de Administración Tributaria (SAT), donde se detallan las guías de llenado, catálogos, ejemplos y normatividad vigente relacionada con este complemento. La información oficial puede consultarse directamente en los canales y documentos publicados por el SAT. Página del complemento: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/complemento_carta_porte.htm. La ruta del archivo de excel con los nuevos catálogos: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/CatalogosCartaPorte31.xls. Ruta del XSD del CCP 3.1: http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte31.xsd. Ruta del XSLT para la secuencia de cadena original CCP 3.1: http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte31.xslt. Ruta matriz de errores CCP 3.1: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/Matriz_Errores_CCP_V31.xls. Ruta XSD catálogos CCP: http://www.sat.gob.mx/sitio_internet/cfd/catalogos/CartaPorte/catCartaPorte.xsd. Ruta del estándar: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/Carta_Porte_31.pdf. |
|---|---|