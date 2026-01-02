using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace ContpaqiBridge.Services
{
    public class ContpaqiSdkService : IContpaqiSdkService
    {
        private readonly ILogger<ContpaqiSdkService> _logger;
        private readonly string _empresasPath;
        private readonly string _defaultUsuario;
        private readonly string _defaultClave;
        private int _lastInitResult = 0;
        private bool _isInitialized = false;
        private string _directorioBase = "";

        // ============ P/Invoke a MGWServicios.dll ============
        // Según el manual oficial, el flujo es:
        // SetCurrentDirectory(DirectorioBase) → fSetNombrePAQ → fAbreEmpresa → (proceso) → fCierraEmpresa → fTerminaSDK

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetNombrePAQ(string aSistema);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAbreEmpresa(string aDirectorioEmpresa);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void fCierraEmpresa();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void fTerminaSDK();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern void fError(int aNumError, StringBuilder aMensaje, int aLen);

        // ============ Funciones para Documentos ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSiguienteFolio(string aCodigoConcepto, StringBuilder aSerie, ref double aFolio);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaDocumento(ref int aIdDocumento, tDocumento aDocumento);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaMovimiento(int aIdDocumento, ref int aIdMovimiento, tMovimiento aMovimiento);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fGuardaDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fAfectaDocto(ref int aIdDocto, bool aAfectar);

        // Funciones bajo nivel para documentos
        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInsertarDocumento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetDatoDocumento(string aCampo, string aValor);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoDocumento(string aCampo, StringBuilder aValor, int aLen);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fEditaDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelarModificacionDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInsertarMovimiento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetDatoMovimiento(string aCampo, string aValor);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fGuardaMovimiento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fEmitirDocumento(string aCodConcepto, string aSerie, double aFolio, string aPassword, string aArchivoXML);

        // ============ Funciones para Clientes/Proveedores ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaCteProv(ref int aIdCteProv, ref tCteProv aCteProv);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaCteProv(string aCodCteProv);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoCteProv(string aCampo, StringBuilder aValor, int aLen);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fEditaCteProv();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fGuardaCteProv();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetDatoCteProv(string aCampo, string aValor);

        // ============ Funciones para Productos ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaProducto(ref int aIdProducto, ref tProducto aProducto);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaProducto(string aCodProducto);

        // Funciones bajo nivel para productos
        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInsertaProducto();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fEditaProducto();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetDatoProducto(string aCampo, string aValor);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fGuardaProducto();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelarModificacionProducto();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosPrimerProducto();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosSiguienteProducto();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoProducto(string aCampo, StringBuilder aValor, int aLen);

        // ============ Funciones para Unidades ============
        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosicionaPrimeraUnidad();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosicionaSiguienteUnidad();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoUnidad(string aCampo, StringBuilder aValor, int aLen);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaDocumento(string aCodConcepto, string aSerie, double aFolio);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaIdDocumento(int aIdDocumento);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosPrimerDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosSiguienteDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosUltimoDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosAnteriorDocumento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetFiltroDocumento(string aFechaInicio, string aFechaFin, string aCodigoConcepto, string aCodigoCteProv);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelaFiltroDocumento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fEntregEnDiscoXML(string aCodConcepto, string aSerie, double aFolio, int aFormato, string aFormatoAmig);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fEntregaxUDD(string aCodConcepto, string aSerie, double aFolio, int aTipoEntrega, string aRutaArchivo);

        // Cancelación CFDI 4.0 - Requiere documento posicionado previamente
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelaDocumentoConMotivo(string aMotivoCancelacion, string aUUIDReemplaza);

        // Establece la contraseña del CSD antes de cancelar
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelaDoctoInfo(string aPassword);

        // Cancelación Administrativa (solo en CONTPAQi, no afecta SAT)
        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelaDocumentoAdministrativamente();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fTimbraComplementoPago(string aRutaINI, StringBuilder aAcuse, int aLongitud);


        // ============ Estructuras del SDK ============
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct tDocumento
        {
            public double aFolio;
            public int aNumMoneda;
            public double aTipoCambio;
            public double aImporte;
            public double aDescuentoDoc1;
            public double aDescuentoDoc2;
            public int aSistemaOrigen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodConcepto;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string aSerie;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string aFecha;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodigoCteProv;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodigoAgente;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
            public string aReferencia;
            public int aAfecta;
            public double aGasto1;
            public double aGasto2;
            public double aGasto3;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct tMovimiento
        {
            public int aConsecutivo;
            public double aUnidades;
            public double aPrecio;
            public double aCosto;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodProdSer;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodAlmacen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string aReferencia;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aCodClasificacion;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct tCteProv
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodigo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aRazonSocial;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string aRFC;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aDenComercial;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aRepLegal;
            public int aTipoCliente; // 1=Cliente, 2=Proveedor, 3=Ambos
            public int aEstatus; // 1=Activo
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aCalle;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aNoExterior;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aNoInterior;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aColonia;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
            public string aCodigoPostal;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aCiudad;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aEstado;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aPais;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
            public string aEmail;
            public int aIdMoneda; // 1=Peso mexicano
            public int aLimiteCreditoFlag; // 0=Sin límite
            public double aLimiteCredito;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct tProducto
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodigo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string aNombre;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string aDescripcion;
            public int aTipoProducto; // 1=Producto, 2=Paquete, 3=Servicio
            public int aEstatus; // 1=Activo
            public double aPrecio1;
            public double aPrecio2;
            public double aPrecio3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aUnidadMedida;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aUnidadMedidaVenta;
            public int aControlExistencia; // 0=Sin control
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string aClaveSAT;
        }

        // Native calls for environment setup
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetCurrentDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        public ContpaqiSdkService(IConfiguration config, ILogger<ContpaqiSdkService> logger)
        {
            _logger = logger;
            _empresasPath = config["Contpaqi:EmpresasPath"] ?? "";
            _defaultUsuario = config["Contpaqi:DefaultUsuario"] ?? "";
            _defaultClave = config["Contpaqi:DefaultClave"] ?? "";

            // Leer DirectorioBase desde el Registro de Windows
            _directorioBase = ObtenerDirectorioBaseDelRegistro();
            
            if (!string.IsNullOrEmpty(_directorioBase))
            {
                _logger.LogInformation($"DirectorioBase obtenido del Registro: {_directorioBase}");
                
                // Configurar el entorno ANTES de cualquier P/Invoke
                SetDllDirectory(_directorioBase);
                SetCurrentDirectory(_directorioBase);
                System.IO.Directory.SetCurrentDirectory(_directorioBase);
                
                // Construir PATH con todos los directorios relevantes de CONTPAQi
                string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
                var pathsToAdd = new List<string> { _directorioBase };

                // Buscar otros directorios en el registro que son vitales para el timbrado
                AgregarPathDesdeRegistro(pathsToAdd, @"SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I Formatos Digitales", "DIRECTORIOBASE");
                AgregarPathDesdeRegistro(pathsToAdd, @"SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I Servidor de Aplicaciones", "DIRECTORIOBASE");
                AgregarPathDesdeRegistro(pathsToAdd, @"SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I SDK", "DIRECTORIOBASE");
                
                // Agregar también subcarpetas conocidas
                string compacBase = Path.GetDirectoryName(_directorioBase) ?? @"C:\Program Files (x86)\Compac";
                string sacPath = Path.Combine(compacBase, "Servidor de Aplicaciones");
                string servidorPath = Path.Combine(compacBase, "Servidor");
                
                if (!pathsToAdd.Contains(sacPath)) pathsToAdd.Add(sacPath);
                if (!pathsToAdd.Contains(servidorPath)) pathsToAdd.Add(servidorPath);

                foreach (var path in pathsToAdd)
                {
                    if (Directory.Exists(path) && !currentPath.Contains(path, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation($"Agregando al PATH: {path}");
                        currentPath = path + ";" + currentPath;
                    }
                }

                Environment.SetEnvironmentVariable("PATH", currentPath);
                _logger.LogInformation("PATH actualizado con múltiples directorios de CONTPAQi para resolver dependencias de CACSql.dll.");
            }
            else
            {
                _logger.LogError("No se pudo obtener DirectorioBase del Registro de Windows.");
            }
        }

        private void AgregarPathDesdeRegistro(List<string> list, string keyPath, string valueName)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        var valor = key.GetValue(valueName);
                        if (valor != null)
                        {
                            string path = valor.ToString() ?? "";
                            if (!string.IsNullOrEmpty(path) && Directory.Exists(path) && !list.Contains(path))
                            {
                                list.Add(path);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"No se pudo leer la clave {keyPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Lee el DirectorioBase desde el Registro de Windows
        /// Clave: HKLM\SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I COMERCIAL
        /// </summary>
        private string ObtenerDirectorioBaseDelRegistro()
        {
            try
            {
                // Para aplicaciones de 32 bits en Windows de 64 bits
                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I COMERCIAL"))
                {
                    if (key != null)
                    {
                        var valor = key.GetValue("DIRECTORIOBASE");
                        if (valor != null)
                        {
                            return valor.ToString() ?? "";
                        }
                    }
                }
                
                // Fallback para sistemas de 32 bits
                using (var key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Computación en Acción, SA CV\CONTPAQ I COMERCIAL"))
                {
                    if (key != null)
                    {
                        var valor = key.GetValue("DIRECTORIOBASE");
                        if (valor != null)
                        {
                            return valor.ToString() ?? "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer el Registro de Windows");
            }
            
            return "";
        }

        /// <summary>
        /// Inicializa el SDK usando el flujo oficial del manual:
        /// SetCurrentDirectory(DirectorioBase) → fSetNombrePAQ("CONTPAQ I Comercial")
        /// </summary>
        public bool InicializarSDK()
        {
            // Si ya está inicializado, retornar true directamente
            if (_isInitialized)
            {
                _logger.LogInformation("SDK ya está inicializado, reutilizando sesión.");
                return true;
            }

            try 
            {
                if (string.IsNullOrEmpty(_directorioBase))
                {
                    _logger.LogError("DirectorioBase no está configurado.");
                    _lastInitResult = -1;
                    return false;
                }

                // Asegurar que estamos en el directorio correcto
                _logger.LogInformation($"SetCurrentDirectory({_directorioBase})");
                SetCurrentDirectory(_directorioBase);

                // Paso 1: fSetNombrePAQ (esto ES la inicialización según el manual)
                _logger.LogInformation("Llamando a fSetNombrePAQ('CONTPAQ I Comercial')...");
                int result = fSetNombrePAQ("CONTPAQ I Comercial");
                _lastInitResult = result;

                if (result != 0)
                {
                    _logger.LogError($"fSetNombrePAQ falló con código: {result}. Mensaje: {GetUltimoError(result)}");
                    return false;
                }

                _logger.LogInformation("SDK inicializado correctamente (fSetNombrePAQ retornó 0)");
                _isInitialized = true;
                return true;
            }
            catch (DllNotFoundException dllEx)
            {
                _lastInitResult = -1;
                _logger.LogCritical(dllEx, $"NO SE ENCONTRÓ MGW_SDK.dll. DirectorioBase: {_directorioBase}");
                return false;
            }
            catch (Exception ex)
            {
                _lastInitResult = -99;
                _logger.LogError(ex, $"Excepción inesperada: {ex.Message}");
                return false;
            }
        }

        public int GetLastInitResult() => _lastInitResult;

        /// <summary>
        /// Abre una empresa según el flujo del manual:
        /// fAbreEmpresa(rutaDirectorioEmpresa)
        /// </summary>
        public bool AbrirEmpresa(string rutaEmpresa)
        {
            try
            {
                _logger.LogInformation($"Llamando a fAbreEmpresa('{rutaEmpresa}')...");
                int result = fAbreEmpresa(rutaEmpresa);
                
                if (result != 0)
                {
                    _logger.LogError($"fAbreEmpresa falló. Código: {result}. Mensaje: {GetUltimoError(result)}");
                    return false;
                }

                _logger.LogInformation("Empresa abierta correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al abrir empresa: {ex.Message}");
                return false;
            }
        }

        public void CerrarEmpresa()
        {
            try
            {
                _logger.LogInformation("Llamando a fCierraEmpresa()...");
                fCierraEmpresa();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al cerrar empresa (ignorado).");
            }
        }

        public int CrearDocumento(string codigoConcepto, string codigoCliente, DateTime fecha, double total)
        {
            // Placeholder básico - usar CrearFactura para implementación completa
            return 0;
        }

        /// <summary>
        /// Crea una factura completa con productos usando funciones de BAJO NIVEL
        /// Flujo: fInsertarDocumento -> fSetDatoDocumento (campos) -> fGuardaDocumento
        /// Esto evita el crash 0xC0000005 causado por marshalling incorrecto de estructuras
        /// </summary>
        public (bool exito, string mensaje, int idDocumento, string serie, double folio) CrearFactura(
            string rutaEmpresa,
            string codigoConcepto,
            string codigoCliente,
            List<(string codigo, double cantidad, double precio)> productos,
            string usoCFDI = "G01",
            string formaPago = "99",
            string metodoPago = "PUE")
        {
            string serieOut = "";
            double folioOut = 0;
            try
            {
                // 1. Inicializar SDK
                if (!InicializarSDK())
                {
                    return (false, "No se pudo inicializar el SDK", 0, "", 0);
                }

                // 2. Abrir empresa
                if (!AbrirEmpresa(rutaEmpresa))
                {
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", 0, "", 0);
                }

                // 3. Obtener siguiente folio
                StringBuilder serieStr = new StringBuilder(12);
                double folioNum = 0;
                int resultFolio = fSiguienteFolio(codigoConcepto, serieStr, ref folioNum);
                if (resultFolio != 0)
                {
                    CerrarEmpresa();
                    return (false, $"Error al obtener folio: {GetUltimoError(resultFolio)}", 0, "", 0);
                }
                string serie = serieStr.ToString().Trim();
                serieOut = serie;
                folioOut = folioNum;

                _logger.LogInformation($"Folio obtenido: Serie={serie}, Folio={folioNum}");

                // 4. Buscar cliente y obtener sus datos
                _logger.LogInformation($"Buscando cliente: {codigoCliente}");
                int resBuscaCte = fBuscaCteProv(codigoCliente);
                if (resBuscaCte != 0)
                {
                    string errCte = GetUltimoError(resBuscaCte);
                    _logger.LogError($"Cliente no encontrado: {codigoCliente} - {errCte}");
                    CerrarEmpresa();
                    return (false, $"Cliente no existe: {codigoCliente} ({errCte})", 0, "", 0);
                }
                
                // Obtener ID, razón social y RFC del cliente
                StringBuilder idClienteStr = new StringBuilder(20);
                fLeeDatoCteProv("CIDCLIENTEPROVEEDOR", idClienteStr, 20);
                string idCliente = idClienteStr.ToString().Trim();
                
                StringBuilder razonSocialStr = new StringBuilder(256);
                fLeeDatoCteProv("CRAZONSOCIAL", razonSocialStr, 256);
                string razonSocial = razonSocialStr.ToString().Trim();
                
                StringBuilder rfcStr = new StringBuilder(20);
                fLeeDatoCteProv("CRFC", rfcStr, 20);
                string rfcCliente = rfcStr.ToString().Trim();
                if (string.IsNullOrEmpty(rfcCliente)) rfcCliente = "XAXX010101000"; // RFC genérico
                
                _logger.LogInformation($"Cliente encontrado. ID: {idCliente}, Razón Social: {razonSocial}, RFC: {rfcCliente}");

                // ============================================================
                // ESTRATEGIA DE PRE-LLENADO: Actualizar Cliente con datos CFDI 4.0
                // ============================================================
                // Dado que fSetDatoDocumento falla para CUSOCFDI/CFORMAPAGO en algunas versiones,
                // actualizamos el CLIENTE antes de crear el documento. El SDK heredará estos datos.
                
                fBuscaCteProv(codigoCliente);
                _logger.LogInformation("Actualizando Cliente con datos CFDI 4.0 para asegurar herencia...");
                int resEditaCte = fEditaCteProv();
                if (resEditaCte == 0)
                {
                    // 1. Uso CFDI
                   string usoFinal = !string.IsNullOrEmpty(usoCFDI) ? usoCFDI : "G01";
                   fSetDatoCteProv("CUSOCFDI", usoFinal);
                   _logger.LogInformation($"Cliente CUSOCFDI actualizado a: {usoFinal}");

                   // 2. Forma de Pago -> Mapeo a ID interno (CMETODOPAG en cliente espera ID)
                   if (!string.IsNullOrEmpty(formaPago))
                   {
                       string idForma = formaPago; // Default
                       // Mapeo basado en SQL admFormasPago: 01->2, 03->1, 99->? (asumimos 0 o null)
                       if (formaPago == "01") idForma = "2";
                       else if (formaPago == "03") idForma = "1";
                       // Agregar más si es necesario o dejar pasar el valor si el sistema lo acepta
                       
                       fSetDatoCteProv("CMETODOPAG", idForma); 
                       _logger.LogInformation($"Cliente CMETODOPAG (FormaPago) actualizado a ID: {idForma} (Orig: {formaPago})");
                   }

                   fGuardaCteProv();
                   _logger.LogInformation("Cliente guardado con datos CFDI.");
                }
                else
                {
                    _logger.LogWarning($"No se pudo editar el cliente para datos CFDI. Código: {resEditaCte}");
                }
                // ============================================================

                // 5. BAJO NIVEL: Insertar documento (cabecera)
                _logger.LogInformation("Llamando a fInsertarDocumento()...");
                int resInsertarDoc = fInsertarDocumento();
                if (resInsertarDoc != 0)
                {
                    string err = GetUltimoError(resInsertarDoc);
                    _logger.LogError($"fInsertarDocumento falló: {resInsertarDoc} - {err}");
                    CerrarEmpresa();
                    return (false, $"Error al insertar documento: {err}", 0, "", 0);
                }

                // ============================================================
                // IMPORTANTE: CONTPAQi pide primero el CÓDIGO DEL CLIENTE, luego el concepto.
                // Una vez ingresado el cliente, el sistema auto-completa los demás datos
                // (razón social, RFC, etc.) desde el catálogo de clientes.
                // EL ORDEN ES CRÍTICO - NO MODIFICAR.
                // ============================================================
                
                // Normalizar Método y Forma de Pago (safeguard para errores comunes)
                // Si metodoPago parece una Forma de Pago (ej: "01", "03"), los intercambiamos
                if (!string.IsNullOrEmpty(metodoPago) && metodoPago.Length == 2 && int.TryParse(metodoPago, out _))
                {
                    _logger.LogWarning($"Detección de Método de Pago incorrecto '{metodoPago}'. Corrigiendo a 'PUE' y moviendo valor a Forma de Pago.");
                    formaPago = metodoPago;
                    metodoPago = "PUE";
                }

                // Asegurar que metodoPago sea PUE o PPD
                if (metodoPago != "PUE" && metodoPago != "PPD") metodoPago = "PUE";
                if (string.IsNullOrEmpty(formaPago)) formaPago = "99";

                // 6. Setear campos en ORDEN ESPECÍFICO (como lo pide CONTPAQi manualmente)
                string fechaHoy = DateTime.Now.ToString("MM/dd/yyyy"); 
                
                var camposDocumento = new List<(string campo, string valor)>
                {
                    ("CIDCONCEPTODOCUMENTO", codigoConcepto),
                    ("CCODIGOCLIENTE", codigoCliente),
                    ("CSERIEDOCUMENTO", serie),
                    ("CFOLIO", folioNum.ToString("F0")),
                    ("CFECHA", fechaHoy),
                    ("CIDMONEDA", "1"),
                    ("CTIPOCAMBIO", "1.00"),
                    ("CREFERENCIA", "API Bridge"),
                    ("COBSERVACIONES", $"Generado via API {DateTime.Now:yyyy-MM-dd HH:mm}"),
                    ("CMETODOPAG", metodoPago),         // PUE o PPD
                    ("CCONDIPAGO", "Contado"),          // Condiciones (Texto)
                    // ("CFORMAPAGO", formaPago),       // SDK marca Error 73 (Campo inválido)
                    ("CTEXTOEXTRA1", formaPago),        // Intento 1: Campo Extra 1
                    ("CTEXTOEXTRA2", formaPago)         // Intento 2: Campo Extra 2
                };

                // Forzar Uso CFDI si viene vacío
                if (string.IsNullOrEmpty(usoCFDI)) usoCFDI = "G01";
                
                // Mapeo manual de Forma de Pago a ID interno (basado en SQL: 01->2, 03->1)
                // Esto es un intento final si CFORMAPAGO falla
                string idFormaPago = "";
                if (formaPago == "01") idFormaPago = "2";
                else if (formaPago == "03") idFormaPago = "1";
                
                if (!string.IsNullOrEmpty(idFormaPago))
                {
                     camposDocumento.Add(("CIDFORMAPAGO", idFormaPago));
                }

                // CFDI 4.0: Exportacion (01 = No aplica)
                camposDocumento.Add(("CEXPORTACION", "01"));

                // El Uso de CFDI envialo siempre
                _logger.LogInformation($"fSetDatoDocumento('CUSOCFDI', '{usoCFDI}')");
                int resUso = fSetDatoDocumento("CUSOCFDI", usoCFDI);
                if (resUso != 0) _logger.LogWarning($"fSetDatoDocumento(CUSOCFDI) falló con {resUso}.");

                foreach (var item in camposDocumento)
                {
                    _logger.LogInformation($"fSetDatoDocumento('{item.campo}', '{item.valor}')");
                    int resSet = fSetDatoDocumento(item.campo, item.valor);
                    if (resSet != 0)
                    {
                        string err = GetUltimoError(resSet);
                        _logger.LogWarning($"fSetDatoDocumento({item.campo}) falló: {resSet} - {err}");
                        // No abortamos, algunos campos pueden ser opcionales
                    }
                }

                // 6. Guardar cabecera del documento
                _logger.LogInformation("Llamando a fGuardaDocumento() para cabecera...");
                int resGuardaCabecera = fGuardaDocumento();
                if (resGuardaCabecera != 0)
                {
                    string err = GetUltimoError(resGuardaCabecera);
                    _logger.LogError($"fGuardaDocumento (cabecera) falló: {resGuardaCabecera} - {err}");
                    fCancelarModificacionDocumento();
                    CerrarEmpresa();
                    return (false, $"Error al guardar cabecera: {err}", 0, "", 0);
                }
                
                StringBuilder idDocSb = new StringBuilder(20);
                fLeeDatoDocumento("CIDDOCUMENTO", idDocSb, 20);
                int.TryParse(idDocSb.ToString().Trim(), out int idDocumento);

                _logger.LogInformation($"Cabecera del documento guardada exitosamente. ID: {idDocumento}");

                int movimientosAgregados = 0;
                int consecutivo = 1;

                // 7. Agregar movimientos (productos) - BAJO NIVEL
                foreach (var producto in productos)
                {
                    _logger.LogInformation($"Validando existencia de producto: {producto.codigo}");
                    int resBusca = fBuscaProducto(producto.codigo);
                    if (resBusca != 0)
                    {
                        string errBusca = GetUltimoError(resBusca);
                        _logger.LogError($"Producto no existe: {producto.codigo} - {errBusca}");
                        CerrarEmpresa();
                        return (false, $"Producto no existe: {producto.codigo} ({errBusca})", 0, serieOut, folioOut);
                    }

                    // Obtener el ID del producto
                    StringBuilder idProductoSb = new StringBuilder(20);
                    fLeeDatoProducto("CIDPRODUCTO", idProductoSb, 20);
                    string idProducto = idProductoSb.ToString().Trim();
                    _logger.LogInformation($"Producto {producto.codigo} tiene ID: {idProducto}");

                    // Insertar movimiento
                    _logger.LogInformation($"fInsertarMovimiento() para producto: {producto.codigo}");
                    int resInsertarMov = fInsertarMovimiento();
                    if (resInsertarMov != 0)
                    {
                        string err = GetUltimoError(resInsertarMov);
                        _logger.LogError($"fInsertarMovimiento falló: {resInsertarMov} - {err}");
                        continue; // Intentar con el siguiente producto
                    }

                    // Setear campos del movimiento - Usar ID del producto
                    // Intentar primero con CIDPRODUCTO (ID), si falla usar código
                    int resSetProd = fSetDatoMovimiento("CIDPRODUCTO", idProducto);
                    if (resSetProd != 0)
                    {
                        _logger.LogWarning($"CIDPRODUCTO falló ({resSetProd}), intentando con CCODIGOPRODUCTO...");
                        resSetProd = fSetDatoMovimiento("CCODIGOPRODUCTO", producto.codigo);
                        if (resSetProd != 0)
                        {
                            _logger.LogWarning($"CCODIGOPRODUCTO falló ({resSetProd}), intentando con CCODPRODSER...");
                            fSetDatoMovimiento("CCODPRODSER", producto.codigo);
                        }
                    }

                    // Obtener la unidad de medida del producto
                    StringBuilder idUnidadSb = new StringBuilder(20);
                    fLeeDatoProducto("CIDUNIDADBASE", idUnidadSb, 20);
                    string idUnidad = idUnidadSb.ToString().Trim();
                    if (string.IsNullOrEmpty(idUnidad) || idUnidad == "0") idUnidad = "1"; // Default ACTIVIDAD o similar
                    _logger.LogInformation($"Producto {producto.codigo} usa Unidad ID: {idUnidad}");

                    // Setear unidades, precio y unidad de medida
                    fSetDatoMovimiento("CUNIDADES", producto.cantidad.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
                    fSetDatoMovimiento("CPRECIO", producto.precio.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
                    fSetDatoMovimiento("CIDUNIDAD", idUnidad);
                    
                    // Almacén (ID 1 es Almacen Uno según SQL)
                    int resAlmacen = fSetDatoMovimiento("CIDALMACEN", "1");
                    if (resAlmacen != 0)
                    {
                        _logger.LogWarning($"CIDALMACEN falló ({resAlmacen}), intentando con 0...");
                        fSetDatoMovimiento("CIDALMACEN", "0");
                    }
                    
                    fSetDatoMovimiento("CREFERENCIA", "API Mov");
                    
                    // CFDI 4.0: Objeto de Impuesto (02 = Sí objeto de impuesto)
                    // Si falla, intentaremos 01 (No objeto) o dejaremos que el SDK decida
                    int resObjImp = fSetDatoMovimiento("COBJETOIMP", "02");
                    if (resObjImp != 0) _logger.LogWarning($"fSetDatoMovimiento(COBJETOIMP) falló: {resObjImp}");
                    else _logger.LogInformation("COBJETOIMP set to 02");

                    // Guardar movimiento
                    _logger.LogInformation($"fGuardaMovimiento() para {producto.codigo}");
                    int resGuardaMov = fGuardaMovimiento();
                    if (resGuardaMov != 0)
                    {
                        string err = GetUltimoError(resGuardaMov);
                        _logger.LogError($"fGuardaMovimiento falló para {producto.codigo}: {resGuardaMov} - {err}");
                        
                        // Intentar sin almacén si falla
                        if (resGuardaMov == 130410)
                        {
                            _logger.LogInformation("Reintentando sin almacén...");
                            fSetDatoMovimiento("CIDALMACEN", "");
                            resGuardaMov = fGuardaMovimiento();
                            if (resGuardaMov == 0)
                            {
                                _logger.LogInformation($"Movimiento guardado (sin almacén) para {producto.codigo}");
                                movimientosAgregados++;
                                consecutivo++;
                                continue;
                            }
                        }
                        continue; // Falló, intentar siguiente producto
                    }
                    
                    _logger.LogInformation($"Movimiento guardado para {producto.codigo}");
                    movimientosAgregados++;
                    consecutivo++;
                }

                // Permitir facturas sin movimientos para testing de cabecera
                if (movimientosAgregados == 0 && productos.Count > 0)
                {
                    _logger.LogError("No se agregaron movimientos válidos.");
                    CerrarEmpresa();
                    return (false, "No se agregaron productos válidos a la factura.", 0, "", 0);
                }
                else if (movimientosAgregados == 0)
                {
                    _logger.LogWarning("Factura creada sin movimientos (solo cabecera para testing).");
                }

                // 8. Cerrar empresa
                CerrarEmpresa();
                _logger.LogInformation($"Factura creada exitosamente. Serie: {serie}, Folio: {folioNum}, Movimientos: {movimientosAgregados}");
                return (true, $"Factura creada exitosamente. Serie: {serie}, Folio: {folioNum}", idDocumento, serie, folioNum);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear factura");
                try { fCancelarModificacionDocumento(); } catch { }
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}", 0, serieOut, folioOut);
            }
        }

        /// <summary>
        /// Timbra una factura existente usando fEmitirDocumento
        /// </summary>
        public (bool exito, string mensaje) TimbrarFactura(string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD)
        {
            try
            {
                if (!InicializarSDK()) return (false, "No se pudo inicializar el SDK");
                if (!AbrirEmpresa(rutaEmpresa)) return (false, "No se pudo abrir la empresa");

                _logger.LogInformation($"Timbrando factura: Concepto={codigoConcepto}, Serie={serie}, Folio={folio}");
                
                // fEmitirDocumento(codConcepto, serie, folio, password, archivoXML)
                // Si archivoXML está vacío, usa el nombre por omisión del concepto
                int result = fEmitirDocumento(codigoConcepto, serie, folio, passCSD, "");
                
                CerrarEmpresa();

                if (result != 0)
                {
                    string err = GetUltimoError(result);
                    _logger.LogError($"Error al timbrar factura {serie}{folio}: {result} - {err}");
                    
                    // Log adicional para diagnosticar error 3
                    if (result == 3)
                    {
                        _logger.LogError("El Error 3 (CACSql.dll) indica un problema con las librerías de base de datos o dependencias del SDK.");
                        _logger.LogInformation($"DirectorioBase: {_directorioBase}");
                        _logger.LogInformation($"PATH actual: {Environment.GetEnvironmentVariable("PATH")}");
                    }
                    
                    return (false, $"Error al timbrar: {err}");
                }

                _logger.LogInformation($"Factura {serie}{folio} timbrada exitosamente.");
                return (true, "Factura timbrada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción durante el timbrado");
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}");
            }
        }

        public (bool exito, string mensaje, string xml) ObtenerXml(string rutaEmpresa, string codigoConcepto, string serie, double folio)
        {
            try
            {
                if (!InicializarSDK()) return (false, "No se pudo inicializar el SDK", "");
                if (!AbrirEmpresa(rutaEmpresa)) return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", "");

                string serieClean = (serie ?? "").Trim().ToUpper();
                string folioStr = folio.ToString();
                
                // ESTRATEGIA DEFINITIVA: fEntregEnDiscoXML + Ruta descubierta
                _logger.LogInformation($"[E1] Intentando fEntregEnDiscoXML: Concepto={codigoConcepto}, Serie={serieClean}, Folio={folio}");
                
                // Intentamos pasarle una ruta en la empresa también
                string suggestedPath = Path.Combine(rutaEmpresa, "XML_SDK", $"{serieClean}{folioStr}.xml");
                int resEntrega = fEntregEnDiscoXML(codigoConcepto, serieClean, folio, 0, suggestedPath);

                if (resEntrega == 0)
                {
                    _logger.LogInformation("¡SDK reportó éxito (0)!");
                    System.Threading.Thread.Sleep(1000);

                    // Lista de búsqueda basada en el hallazgo real
                    var posiblesRutas = new List<string> { 
                        suggestedPath,
                        Path.Combine(rutaEmpresa, "XML_SDK", $"{serieClean}{folioStr}.xml"),
                        Path.Combine(rutaEmpresa, "XML_SDK", $"{folioStr}.xml"),
                        Path.Combine(rutaEmpresa, $"{serieClean}{folioStr}.xml"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Factura_{folioStr}.xml")
                    };

                    foreach (var ruta in posiblesRutas) {
                        _logger.LogInformation($"Verificando: {ruta}");
                        if (File.Exists(ruta)) {
                            _logger.LogInformation($"¡XML ENCONTRADO! Leyendo: {ruta}");
                            string content = File.ReadAllText(ruta);
                            CerrarEmpresa();
                            return (true, $"XML obtenido correctamente", content);
                        }
                    }
                }

                // E2: Fallback por si acaso (sin navegación pesada para evitar crashes)
                _logger.LogWarning("No se encontró el archivo en las rutas estándar. Intentando búsqueda simple...");
                CerrarEmpresa();
                return (false, "El SDK reportó éxito pero el archivo XML no se encontró en la carpeta XML_SDK de la empresa.", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerXml");
                CerrarEmpresa();
                return (false, $"Error: {ex.Message}", "");
            }
        }

        /// <summary>
        /// Lista los últimos documentos de la empresa para diagnóstico
        /// </summary>
        public List<Dictionary<string, object>> ListarUltimosDocumentos(string rutaEmpresa, int cantidad = 10)
        {
            var documentos = new List<Dictionary<string, object>>();
            
            try
            {
                if (!InicializarSDK()) return documentos;
                if (!AbrirEmpresa(rutaEmpresa)) return documentos;

                // Ir al último documento y navegar hacia atrás
                int res = fPosUltimoDocumento();
                int count = 0;
                
                while (res == 0 && count < cantidad)
                {
                    var doc = new Dictionary<string, object>();
                    
                    StringBuilder idSb = new StringBuilder(20);
                    StringBuilder conceptoSb = new StringBuilder(20);
                    StringBuilder serieSb = new StringBuilder(50);
                    StringBuilder folioSb = new StringBuilder(50);
                    StringBuilder fechaSb = new StringBuilder(50);
                    
                    fLeeDatoDocumento("CIDDOCUMENTO", idSb, 20);
                    fLeeDatoDocumento("CIDCONCEPTODOCUMENTO", conceptoSb, 20);
                    fLeeDatoDocumento("CSERIEDOCUMENTO", serieSb, 50);
                    fLeeDatoDocumento("CFOLIO", folioSb, 50);
                    fLeeDatoDocumento("CFECHA", fechaSb, 50);
                    
                    doc["id"] = idSb.ToString().Trim();
                    doc["concepto"] = conceptoSb.ToString().Trim();
                    doc["serie"] = serieSb.ToString().Trim();
                    doc["folio"] = folioSb.ToString().Trim();
                    doc["fecha"] = fechaSb.ToString().Trim();
                    
                    documentos.Add(doc);
                    count++;
                    
                    res = fPosAnteriorDocumento();
                }
                
                CerrarEmpresa();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar documentos");
                CerrarEmpresa();
            }
            
            return documentos;
        }

        /// <summary>
        /// Cancela un documento CFDI 4.0 ante el SAT.
        /// Requiere que el documento esté posicionado previamente.
        /// </summary>
        /// <param name="motivoCancelacion">01=Con relación, 02=Sin relación, 03=No se realizó, 04=Factura global</param>
        /// <param name="uuidSustitucion">Solo requerido si motivoCancelacion es "01"</param>
        public (bool exito, string mensaje, string acuse) CancelarDocumento(
            string rutaEmpresa, 
            string codigoConcepto, 
            string serie, 
            double folio, 
            string motivoCancelacion, 
            string passCSD,
            string uuidSustitucion = "")
        {
            try
            {
                _logger.LogInformation($"Iniciando cancelación: Concepto={codigoConcepto}, Serie={serie}, Folio={folio}, Motivo={motivoCancelacion}");
                
                if (!InicializarSDK()) 
                    return (false, "No se pudo inicializar el SDK", "");
                    
                if (!AbrirEmpresa(rutaEmpresa)) 
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", "");

                string serieClean = (serie ?? "").Trim().ToUpper();

                // 1. Buscar el documento navegando (fBuscaDocumento falla porque espera código de concepto, no ID)
                _logger.LogInformation($"Buscando documento para cancelar: Serie={serieClean}, Folio={folio}");
                
                bool documentoEncontrado = false;
                int idDocumentoEncontrado = 0;
                
                // Navegar desde el último documento hacia atrás buscando serie/folio
                int res = fPosUltimoDocumento();
                int intentos = 0;
                const int maxIntentos = 200;
                
                StringBuilder serieSb = new StringBuilder(50);
                StringBuilder folioSb = new StringBuilder(50);
                StringBuilder idSb = new StringBuilder(20);
                
                while (res == 0 && !documentoEncontrado && intentos < maxIntentos)
                {
                    serieSb.Clear();
                    folioSb.Clear();
                    idSb.Clear();
                    
                    fLeeDatoDocumento("CSERIEDOCUMENTO", serieSb, 50);
                    fLeeDatoDocumento("CFOLIO", folioSb, 50);
                    fLeeDatoDocumento("CIDDOCUMENTO", idSb, 20);
                    
                    string serieDoc = serieSb.ToString().Trim().ToUpper();
                    double.TryParse(folioSb.ToString(), out double folioDoc);
                    int.TryParse(idSb.ToString().Trim(), out int idDoc);
                    
                    if (serieDoc == serieClean && Math.Abs(folioDoc - folio) < 0.1)
                    {
                        documentoEncontrado = true;
                        idDocumentoEncontrado = idDoc;
                        _logger.LogInformation($"Documento encontrado: ID={idDoc}, Serie={serieDoc}, Folio={folioDoc}");
                    }
                    else
                    {
                        res = fPosAnteriorDocumento();
                        intentos++;
                    }
                }
                
                if (!documentoEncontrado)
                {
                    CerrarEmpresa();
                    return (false, $"Documento no encontrado: Serie={serieClean}, Folio={folio}. Revisados {intentos} documentos.", "");
                }

                // 2. Validar motivo de cancelación
                var motivosValidos = new[] { "01", "02", "03", "04" };
                if (!motivosValidos.Contains(motivoCancelacion))
                {
                    CerrarEmpresa();
                    return (false, $"Motivo de cancelación inválido: {motivoCancelacion}. Use 01, 02, 03 o 04.", "");
                }

                // Si motivo es "01", debe tener UUID de sustitución
                if (motivoCancelacion == "01" && string.IsNullOrWhiteSpace(uuidSustitucion))
                {
                    CerrarEmpresa();
                    return (false, "El motivo 01 requiere un UUID de sustitución.", "");
                }

                // Si no es "01", limpiar UUID de sustitución
                if (motivoCancelacion != "01")
                {
                    uuidSustitucion = "";
                }

                // 3. Establecer contraseña del CSD
                if (!string.IsNullOrEmpty(passCSD))
                {
                    _logger.LogInformation($"Llamando a fCancelaDoctoInfo con password del CSD...");
                    int resInfo = fCancelaDoctoInfo(passCSD);
                    if (resInfo != 0)
                    {
                        _logger.LogWarning($"fCancelaDoctoInfo retornó: {resInfo} - {GetUltimoError(resInfo)}");
                        // Continuamos aunque falle, quizás no es crítico
                    }
                }

                // 4. Intentar cancelar
                _logger.LogInformation($"Llamando a fCancelaDocumentoConMotivo('{motivoCancelacion}', '{uuidSustitucion}')...");
                int resCancela = fCancelaDocumentoConMotivo(motivoCancelacion, uuidSustitucion ?? "");

                if (resCancela != 0)
                {
                    string errorMsg = GetUltimoError(resCancela);
                    _logger.LogError($"Error al cancelar: {errorMsg}");
                    CerrarEmpresa();
                    return (false, $"Error al cancelar documento: {errorMsg}", "");
                }

                _logger.LogInformation("¡Documento cancelado exitosamente!");
                
                // 4. Intentar obtener el acuse (si hay un XML de cancelación)
                string acuse = "";
                string acusePath = Path.Combine(rutaEmpresa, "XML_SDK", $"{serieClean}{folio}_Cancelacion.xml");
                if (File.Exists(acusePath))
                {
                    acuse = File.ReadAllText(acusePath);
                    _logger.LogInformation($"Acuse encontrado en: {acusePath}");
                }

                CerrarEmpresa();
                return (true, "Documento cancelado exitosamente ante el SAT.", acuse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al cancelar documento");
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}", "");
            }
        }

        /// <summary>
        /// Cancela un documento administrativamente (solo en CONTPAQi, NO afecta al SAT).
        /// Útil para anular documentos sin enviar cancelación al SAT.
        /// </summary>
        public (bool exito, string mensaje) CancelarDocumentoAdministrativamente(
            string rutaEmpresa, 
            string codigoConcepto, 
            string serie, 
            double folio)
        {
            try
            {
                _logger.LogInformation($"Iniciando cancelación administrativa: Concepto={codigoConcepto}, Serie={serie}, Folio={folio}");
                
                if (!InicializarSDK()) 
                    return (false, "No se pudo inicializar el SDK");
                    
                if (!AbrirEmpresa(rutaEmpresa)) 
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}");

                string serieClean = (serie ?? "").Trim().ToUpper();

                // 1. Posicionar el documento
                int resBusca = fBuscaDocumento(codigoConcepto, serieClean, folio);
                
                if (resBusca != 0)
                {
                    // Intentar con filtros
                    fCancelaFiltroDocumento();
                    fSetFiltroDocumento("01/01/2020", "12/31/2030", codigoConcepto, "");
                    int resNav = fPosPrimerDocumento();
                    bool encontrado = false;
                    int intentos = 0;
                    
                    while (resNav == 0 && !encontrado && intentos < 500)
                    {
                        StringBuilder sSb = new StringBuilder(50);
                        StringBuilder fSb = new StringBuilder(50);
                        fLeeDatoDocumento("CSERIEDOCUMENTO", sSb, 50);
                        fLeeDatoDocumento("CFOLIO", fSb, 50);
                        
                        string s = sSb.ToString().Trim().ToUpper();
                        double.TryParse(fSb.ToString(), out double f);
                        
                        if (s == serieClean && Math.Abs(f - folio) < 0.1)
                        {
                            encontrado = true;
                        }
                        else
                        {
                            resNav = fPosSiguienteDocumento();
                            intentos++;
                        }
                    }
                    fCancelaFiltroDocumento();
                    
                    if (!encontrado)
                    {
                        CerrarEmpresa();
                        return (false, "Documento no encontrado.");
                    }
                }

                // 2. Cancelar administrativamente
                _logger.LogInformation("Llamando a fCancelaDocumentoAdministrativamente()...");
                int resCancela = fCancelaDocumentoAdministrativamente();

                if (resCancela != 0)
                {
                    string errorMsg = GetUltimoError(resCancela);
                    _logger.LogError($"Error al cancelar administrativamente: {errorMsg}");
                    CerrarEmpresa();
                    return (false, $"Error: {errorMsg}");
                }

                _logger.LogInformation("¡Documento cancelado administrativamente!");
                CerrarEmpresa();
                return (true, "Documento cancelado administrativamente (solo en CONTPAQi, no afecta SAT).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al cancelar documento administrativamente");
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un cliente en CONTPAQi
        /// </summary>
        public (bool exito, string mensaje, int idCliente) CrearCliente(
            string rutaEmpresa,
            string codigo,
            string razonSocial,
            string rfc,
            string email = "",
            string calle = "",
            string colonia = "",
            string codigoPostal = "",
            string ciudad = "",
            string estado = "",
            string pais = "México",
            string regimenFiscal = "",
            string usoCFDI = "",
            string formaPago = "")
        {
            try
            {
                // 1. Inicializar SDK
                if (!InicializarSDK())
                {
                    return (false, "No se pudo inicializar el SDK", 0);
                }

                // 2. Abrir empresa
                if (!AbrirEmpresa(rutaEmpresa))
                {
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", 0);
                }

                // 3. Verificar si el cliente ya existe
                // 3. Verificar si el cliente ya existe
                int existe = fBuscaCteProv(codigo);
                if (existe == 0) // Existe
                {
                    _logger.LogInformation($"El cliente {codigo} ya existe. Actualizando datos...");
                    fEditaCteProv();
                    // Continuamos al bloque de actualización abajo...
                }
                else
                {
                    // No existe, procedemos a crear estructura
                }

                // 2.1 Especial para Público en General (CFDI 4.0)
                if (codigo.ToUpper() == "PG")
                {
                    _logger.LogInformation("Detectado cliente PG. Aplicando configuración estándar para Público en General (CFDI 4.0)");
                    razonSocial = "PUBLICO EN GENERAL";
                    rfc = "XAXX010101000";
                    regimenFiscal = "616";
                    usoCFDI = "S01";
                    // No sobreescribimos forma de pago si el usuario mandó una específica, 
                    // pero 01 (Efectivo) es el estándar para PG si viene vacío.
                    if (string.IsNullOrEmpty(formaPago)) formaPago = "01";
                }

                // 4. Crear estructura del cliente
                tCteProv cliente = new tCteProv();
                int result = 0;

                // Solo si NO existe, creamos la estructura y damos de alta
                if (existe != 0)
                {
                    cliente = new tCteProv
                    {
                        aCodigo = codigo,
                        aRazonSocial = razonSocial,
                        aRFC = rfc,
                        aDenComercial = razonSocial,
                        aRepLegal = "",
                        aTipoCliente = 1, // 1 = Cliente
                        aEstatus = 1, // 1 = Activo
                        aCalle = calle,
                        aNoExterior = "",
                        aNoInterior = "",
                        aColonia = colonia,
                        aCodigoPostal = codigoPostal,
                        aCiudad = ciudad,
                        aEstado = estado,
                        aPais = pais,
                        aEmail = email,
                        aIdMoneda = 1, // Peso mexicano
                        aLimiteCreditoFlag = 0,
                        aLimiteCredito = 0
                    };
                     // 5. Crear cliente
                     result = fAltaCteProv(ref idCliente, ref cliente);
                }

                // Si se creó o ya existía, procedemos a actualizar datos complementarios
                if (result == 0)
                {
                    // 6. Setear campos adicionales y asegurar RFC/Nombre
                    fBuscaCteProv(codigo);
                    fEditaCteProv();
                    
                    // Forzar RFC y Razón Social ya que a veces fAltaCteProv no los toma del struct correctamente
                    fSetDatoCteProv("CRFC", rfc);
                    fSetDatoCteProv("CRAZONSOCIAL", razonSocial);
                    
                    if (!string.IsNullOrEmpty(regimenFiscal))
                    {
                        _logger.LogInformation($"Seteando Régimen Fiscal (CREGIMENFISCAL): {regimenFiscal}");
                        fSetDatoCteProv("CREGIMENFISCAL", regimenFiscal);
                    }
                    
                    if (!string.IsNullOrEmpty(usoCFDI))
                    {
                        _logger.LogInformation($"Seteando Uso CFDI por defecto (CUSOCFDI): {usoCFDI}");
                        fSetDatoCteProv("CUSOCFDI", usoCFDI);
                    }

                    if (!string.IsNullOrEmpty(formaPago))
                    {
                        _logger.LogInformation($"Seteando Forma de Pago por defecto (CMETODOPAG): {formaPago}");
                        fSetDatoCteProv("CMETODOPAG", formaPago);
                    }
                    
                    fGuardaCteProv();
                }

                CerrarEmpresa();

                if (result != 0)
                {
                    return (false, $"Error al crear cliente: {GetUltimoError(result)}", 0);
                }

                _logger.LogInformation($"Cliente creado: {codigo} con ID: {idCliente}");
                return (true, $"Cliente {codigo} creado exitosamente", idCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Crea un producto en CONTPAQi
        /// </summary>
        public (bool exito, string mensaje, int idProducto) CrearProducto(
            string rutaEmpresa,
            string codigo,
            string nombre,
            string descripcion = "",
            double precio = 0,
            int tipoProducto = 1, // 1=Producto, 2=Paquete, 3=Servicio
            string unidadMedida = "H87",
            string claveSAT = "")
        {
            try
            {
                // Normalizar clave SAT a 8 dígitos (ej: "1010101" -> "01010101")
                if (!string.IsNullOrEmpty(claveSAT) && claveSAT.Length < 8 && int.TryParse(claveSAT, out _))
                {
                    claveSAT = claveSAT.PadLeft(8, '0');
                }

                // 1. Inicializar SDK
                if (!InicializarSDK())
                {
                    return (false, "No se pudo inicializar el SDK", 0);
                }

                // 2. Abrir empresa
                if (!AbrirEmpresa(rutaEmpresa))
                {
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", 0);
                }

                // 3. Verificar si el producto ya existe
                int existe = fBuscaProducto(codigo);
                _logger.LogInformation($"fBuscaProducto('{codigo}') retornó: {existe}");
                if (existe == 0)
                {
                    _logger.LogInformation($"Producto {codigo} ya existe. Parametros recibidos: Nombre='{nombre}', Precio={precio}, ClaveSAT='{claveSAT}'");
                    
                    // Entrar en modo edición para actualizar datos
                    int resEdita = fEditaProducto();
                    if (resEdita != 0)
                    {
                        _logger.LogError($"Error al poner producto en modo edición: {resEdita} - {GetUltimoError(resEdita)}");
                        CerrarEmpresa();
                        return (false, $"Error al editar producto: {GetUltimoError(resEdita)}", 0);
                    }
                    
                    // Actualizar CLAVE SAT (Crítico para timbrado 4.0)
                    if (!string.IsNullOrEmpty(claveSAT))
                    {
                        // Asegurar 8 dígitos (ej: "1010101" -> "01010101")
                        if (claveSAT.Length < 8 && int.TryParse(claveSAT, out _)) {
                            claveSAT = claveSAT.PadLeft(8, '0');
                        }

                        _logger.LogInformation($"Intentando actualizar Clave SAT a '{claveSAT}'...");
                        
                        // Intentar con todos los campos posibles que usa CONTPAQi para el SAT
                        int r1 = fSetDatoProducto("CCLAVESAT", claveSAT);
                        _logger.LogInformation($"fSetDatoProducto('CCLAVESAT', '{claveSAT}') retornó: {r1}");
                        
                        int r2 = fSetDatoProducto("CCLAVEPRODSERV", claveSAT);
                        _logger.LogInformation($"fSetDatoProducto('CCLAVEPRODSERV', '{claveSAT}') retornó: {r2}");
                        
                        int r3 = fSetDatoProducto("C_SAT_PRODUCTO", claveSAT);
                        _logger.LogInformation($"fSetDatoProducto('C_SAT_PRODUCTO', '{claveSAT}') retornó: {r3}");
                    }
                    
                    // Actualizar Nombre
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        _logger.LogInformation($"Actualizando nombre a '{nombre}'...");
                        fSetDatoProducto("CNOMBREPRODUCTO", nombre);
                    }

                    // Actualizar Precios
                    if (precio > 0)
                    {
                        string precioStr = precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                        fSetDatoProducto("CPRECIO1", precioStr);
                        fSetDatoProducto("CPRECIO2", precioStr);
                        fSetDatoProducto("CPRECIO3", precioStr);
                        fSetDatoProducto("CPRECIO4", precioStr);
                        fSetDatoProducto("CPRECIO5", precioStr);
                        fSetDatoProducto("CPRECIO6", precioStr);
                        fSetDatoProducto("CPRECIO7", precioStr);
                        fSetDatoProducto("CPRECIO8", precioStr);
                        fSetDatoProducto("CPRECIO9", precioStr);
                        fSetDatoProducto("CPRECIO10", precioStr);
                    }
                    
                    // Actualizar descripción
                    if (!string.IsNullOrEmpty(descripcion))
                    {
                        fSetDatoProducto("CDESCRIPCIONPRODUCTO", descripcion);
                    }

                    // Actualizar Unidad de Medida (Clave SAT de la unidad)
                    if (!string.IsNullOrEmpty(unidadMedida))
                    {
                        _logger.LogInformation($"Actualizando Unidad SAT a '{unidadMedida}'...");
                        fSetDatoProducto("CCOMNOMBREUNIDAD", unidadMedida);
                        fSetDatoProducto("CCODIGOUNIDADNOCONVERTIBLE", unidadMedida);
                    }

                    _logger.LogInformation("Llamando a fGuardaProducto()...");
                    int resGuardaUpdate = fGuardaProducto();
                    if (resGuardaUpdate != 0)
                    {
                        _logger.LogError($"Error al guardar producto: {resGuardaUpdate} - {GetUltimoError(resGuardaUpdate)}");
                    }
                    else
                    {
                        // VERIFICACIÓN: Leer de nuevo para asegurar que se guardó
                        StringBuilder valSAT = new StringBuilder(20);
                        fLeeDatoProducto("CCLAVESAT", valSAT, 20);
                        if (valSAT.Length == 0) fLeeDatoProducto("CCLAVEPRODSERV", valSAT, 20);
                        
                        _logger.LogInformation($"Producto {codigo} actualizado. Valor SAT actual en CONTPAQi: '{valSAT.ToString().Trim()}'");
                    }
                    
                    CerrarEmpresa();
                    return (true, $"Producto {codigo} actualizado (SAT: '{claveSAT}')", 0);
                }

                // 4. Usar flujo bajo nivel: fInsertaProducto -> fSetDatoProducto -> fGuardaProducto
                _logger.LogInformation("Llamando a fInsertaProducto()...");
                int resultInserta = fInsertaProducto();
                if (resultInserta != 0)
                {
                    string error = GetUltimoError(resultInserta);
                    _logger.LogError($"fInsertaProducto falló: {resultInserta} - {error}");
                    CerrarEmpresa();
                    return (false, $"Error en fInsertaProducto: {error}", 0);
                }

                // 5. Setear campos uno por uno - primero los campos obligatorios
                var camposObligatorios = new Dictionary<string, string>
                {
                    { "CCODIGOPRODUCTO", codigo },
                    { "CNOMBREPRODUCTO", nombre },
                    { "CTIPOPRODUCTO", tipoProducto.ToString() },      // 1=Producto, 2=Paquete, 3=Servicio
                    { "CSTATUSPRODUCTO", "1" },                         // 1=Alta/Activo
                };

                foreach (var campo in camposObligatorios)
                {
                    _logger.LogInformation($"fSetDatoProducto('{campo.Key}', '{campo.Value}')");
                    int resultSet = fSetDatoProducto(campo.Key, campo.Value);
                    if (resultSet != 0)
                    {
                        string error = GetUltimoError(resultSet);
                        _logger.LogError($"fSetDatoProducto({campo.Key}) falló: {resultSet} - {error}");
                    }
                }

                // Campos opcionales que pueden fallar sin bloquear la creación
                var camposOpcionales = new Dictionary<string, string>
                {
                    { "CCONTROLEXISTENCIA", "0" },                      // 0=Sin control
                    { "CMETODOCOSTEO", "1" },                           // 1=UEPS
                    { "CPRECIO1", precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) },
                    { "CPRECIO2", precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) },
                    { "CPRECIO3", precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) },
                };

                // Agregar clave SAT si se proporciona
                if (!string.IsNullOrEmpty(claveSAT))
                {
                    camposOpcionales.Add("CCLAVESAT", claveSAT);
                    camposOpcionales.Add("CCLAVEPRODSERV", claveSAT);
                    camposOpcionales.Add("C_SAT_PRODUCTO", claveSAT);
                }

                // Agregar descripción si se proporciona
                if (!string.IsNullOrEmpty(descripcion))
                {
                    camposOpcionales.Add("CDESCRIPCIONPRODUCTO", descripcion);
                }

                foreach (var campo in camposOpcionales)
                {
                    _logger.LogInformation($"fSetDatoProducto('{campo.Key}', '{campo.Value}')");
                    int resultSet = fSetDatoProducto(campo.Key, campo.Value);
                    if (resultSet != 0)
                    {
                        string error = GetUltimoError(resultSet);
                        _logger.LogWarning($"fSetDatoProducto({campo.Key}) falló: {resultSet} - {error}");
                    }
                }

                // Intentar setear unidad de medida
                if (!string.IsNullOrEmpty(unidadMedida))
                {
                    _logger.LogInformation($"fSetDatoProducto('CCOMNOMBREUNIDAD', '{unidadMedida}')");
                    int resUnidad = fSetDatoProducto("CCOMNOMBREUNIDAD", unidadMedida);
                    if (resUnidad != 0)
                    {
                        _logger.LogWarning($"Unidad '{unidadMedida}' rechazada por nombre. Intentando con CIDUNIDADBASE...");
                        fSetDatoProducto("CIDUNIDADBASE", "1"); 
                        resUnidad = 0; // Continuamos
                    }
                    
                    // En muchas versiones de Comercial, este campo guarda la Clave SAT de la unidad
                    fSetDatoProducto("CCODIGOUNIDADNOCONVERTIBLE", unidadMedida);
                }

                // 6. Guardar producto
                _logger.LogInformation("Llamando a fGuardaProducto()...");
                int resultGuarda = fGuardaProducto();
                
                if (resultGuarda != 0)
                {
                    string error = GetUltimoError(resultGuarda);
                    _logger.LogError($"fGuardaProducto falló: {resultGuarda} - {error}");
                    fCancelarModificacionProducto();
                    CerrarEmpresa();
                    return (false, $"Error al guardar producto: {error}", 0);
                }

                // Producto creado exitosamente
                _logger.LogInformation($"Producto creado: {codigo}");
                
                CerrarEmpresa();
                return (true, $"Producto {codigo} creado exitosamente", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                fCancelarModificacionProducto();
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}", 0);
            }
        }

        private void SetDatoDocumentoLog(string campo, string valor)
        {
            int res = fSetDatoDocumento(campo, valor);
            if (res != 0)
            {
                _logger.LogWarning($"fSetDatoDocumento('{campo}', '{valor}') falló: {res} - {GetUltimoError(res)}");
            }
            else
            {
                _logger.LogInformation($"fSetDatoDocumento('{campo}', '{valor}') OK");
            }
        }

        private void SetDatoMovimientoLog(string campo, string valor)
        {
            int res = fSetDatoMovimiento(campo, valor);
            if (res != 0)
            {
                _logger.LogWarning($"fSetDatoMovimiento('{campo}', '{valor}') falló: {res} - {GetUltimoError(res)}");
            }
            else
            {
                _logger.LogInformation($"fSetDatoMovimiento('{campo}', '{valor}') OK");
            }
        }

        public void SetUsuario(string usuario, string clave)
        {
            // Placeholder for user session
        }

        /// <summary>
        /// Obtiene el mensaje de error del SDK usando fError.
        /// </summary>
        public string GetUltimoError(int errorCode = 0)
        {
            try
            {
                StringBuilder mensaje = new StringBuilder(512);
                fError(errorCode, mensaje, 512);
                return mensaje.ToString();
            }
            catch
            {
                return "No se pudo obtener mensaje con fError()";
            }
        }

        public string ListarUnidades()
        {
            if (!_isInitialized) return "SDK no inicializado";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Catalogo de Unidades del SDK:");

            int res = fPosicionaPrimeraUnidad();
            while (res == 0)
            {
                StringBuilder nombre = new StringBuilder(60);
                StringBuilder abrev = new StringBuilder(20);
                StringBuilder desp = new StringBuilder(20);

                fLeeDatoUnidad("CNOMBREUNIDAD", nombre, 60);
                fLeeDatoUnidad("CABREVIATURA", abrev, 20);
                fLeeDatoUnidad("CDESPLIEGUE", desp, 20);

                sb.AppendLine($"- Nombre: [{nombre}], Abrev: [{abrev}], Desp: [{desp}]");
                res = fPosicionaSiguienteUnidad();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Lista los conceptos de la empresa
        /// </summary>
        public List<(string codigo, string nombre)> ListarConceptos(string rutaEmpresa)
        {
            _logger.LogWarning("ListarConceptos no está disponible en esta versión del SDK.");
            return new List<(string codigo, string nombre)>();
        }

        /// <summary>
        /// Lista los primeros N productos de la empresa
        /// </summary>
        public List<(string codigo, string nombre, double precio)> ListarProductos(string rutaEmpresa, int limite = 20)
        {
            var productos = new List<(string codigo, string nombre, double precio)>();
            
            try
            {
                if (!InicializarSDK())
                {
                    _logger.LogError("No se pudo inicializar SDK para listar productos");
                    return productos;
                }

                if (!AbrirEmpresa(rutaEmpresa))
                {
                    _logger.LogError("No se pudo abrir empresa para listar productos");
                    return productos;
                }

                int res = fPosPrimerProducto();
                int count = 0;
                
                while (res == 0 && count < limite)
                {
                    StringBuilder codigoSb = new StringBuilder(50);
                    StringBuilder nombreSb = new StringBuilder(256);
                    StringBuilder precioSb = new StringBuilder(50);

                    fLeeDatoProducto("CCODIGOPRODUCTO", codigoSb, 50);
                    fLeeDatoProducto("CNOMBREPRODUCTO", nombreSb, 256);
                    fLeeDatoProducto("CPRECIO1", precioSb, 50);

                    string codigo = codigoSb.ToString().Trim();
                    string nombre = nombreSb.ToString().Trim();
                    double.TryParse(precioSb.ToString().Trim(), out double precio);

                    if (!string.IsNullOrEmpty(codigo))
                    {
                        productos.Add((codigo, nombre, precio));
                        _logger.LogInformation($"Producto encontrado: {codigo} - {nombre}");
                    }

                    res = fPosSiguienteProducto();
                    count++;
                }

                CerrarEmpresa();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                CerrarEmpresa();
            }

            return productos;
        }

        public void Dispose()
        {
            try
            {
                _logger.LogInformation("Llamando a fTerminaSDK()...");
                fTerminaSDK();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al terminar SDK (ignorado).");
            }
        }
    }
}
