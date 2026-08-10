using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _instanceSql;
        private readonly string _sqlUser;
        private readonly string _sqlPassword;
        private int _lastInitResult = 0;
        private bool _isInitialized = false;
        private string _directorioBase = "";
        private readonly object _lock = new object();

        // ============ P/Invoke a MGWServicios.dll ============
        // Según el manual oficial, el flujo es:
        // SetCurrentDirectory(DirectorioBase) → fSetNombrePAQ → fAbreEmpresa → (proceso) → fCierraEmpresa → fTerminaSDK

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetNombrePAQ(string aSistema);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fInicioSesionSDK(string aUsuario, string aContrasena);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAbreEmpresa(StringBuilder aDirectorioEmpresa);

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
        private static extern int fLeeDatoMovimiento(string aCampo, StringBuilder aValor, int aLen);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosPrimerMovimiento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosSiguienteMovimiento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fEmitirDocumento(string aCodConcepto, string aSerie, double aFolio, string aPassword, string aArchivoXML);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInicializaLicenseInfo(byte aSistema);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fAfectaDocto(ref tLlaveDocto aLlaveDocto, bool aAfecta);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fSaldarDocumento(ref tLlaveDocto aDoctoPagar, ref tLlaveDocto aDoctoPago, double aImporte, int aIdMoneda, string aFecha);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fDocumentoUUID(string aCodigoConcepto, string aSerie, double aFolio, StringBuilder aCFDIUUID);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fObtieneDatosCFDI(string aPassword);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoCFDI(StringBuilder aValor, int aDato);

        // ============ Funciones para Clientes/Proveedores ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaCteProv(ref int aIdCteProv, ref tCteProv aCteProv);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaCteProv(string aCodCteProv);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosPrimerCteProv();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fPosSiguienteCteProv();

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
        public struct tLlaveDocto
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string aCodConcepto;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string aSerie;
            public double aFolio;
        }

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

            // ============ Campos CFDI 4.0 (MEJORA #A - refactor alto nivel) ============
            // Solo se usan si decides llamar fAltaDocumento con esta struct.
            // El SDK puede ignorar estos campos si están vacíos.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string aUsoCFDI;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string aIdFormaPago;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string aExportacion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string aMetodoPago;
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

            // ============ Campos CFDI 4.0 (MEJORA #A - refactor alto nivel) ============
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string aObjetoImp;
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
            _instanceSql = config["Contpaqi:InstanceSql"] ?? "localhost\\COMPAC22";
            _sqlUser = config["Contpaqi:SqlUser"] ?? "sa";
            _sqlPassword = config["Contpaqi:SqlPassword"] ?? "";

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
            lock (_lock)
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
        }

        public int GetLastInitResult() => _lastInitResult;

        /// <summary>
        /// Abre una empresa según el flujo del manual:
        /// fAbreEmpresa(rutaDirectorioEmpresa)
        /// </summary>
        public bool AbrirEmpresa(string rutaEmpresa)
        {
            lock (_lock)
            {
            try
            {
                _logger.LogInformation($"Llamando a fAbreEmpresa('{rutaEmpresa}')...");
                StringBuilder rutaSb = new StringBuilder(rutaEmpresa);
                int result = fAbreEmpresa(rutaSb);
                
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
        }

        public void CerrarEmpresa()
        {
            lock (_lock)
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
        }

        public int CrearDocumento(string codigoConcepto, string codigoCliente, DateTime fecha, double total)
        {
            lock (_lock)
            {
                try
                {
                    if (!_isInitialized)
                    {
                        _logger.LogError("SDK no inicializado para CrearDocumento");
                        return -1;
                    }

                    // Crear un documento básico sin productos usando flujo bajo nivel
                    int resInserta = fInsertarDocumento();
                    if (resInserta != 0)
                    {
                        _logger.LogError($"fInsertarDocumento falló en CrearDocumento: {resInserta}");
                        return -1;
                    }

                    SetDatoDocumentoLog("CIDCONCEPTODOCUMENTO", codigoConcepto);
                    SetDatoDocumentoLog("CCODIGOCLIENTE", codigoCliente);
                    SetDatoDocumentoLog("CFECHA", fecha.ToString("MM/dd/yyyy"));
                    SetDatoDocumentoLog("CIDMONEDA", "1");
                    SetDatoDocumentoLog("CTIPOCAMBIO", "1.00");

                    int resGuarda = fGuardaDocumento();
                    if (resGuarda != 0)
                    {
                        _logger.LogError($"fGuardaDocumento falló en CrearDocumento: {resGuarda}");
                        fCancelarModificacionDocumento();
                        return -1;
                    }

                    StringBuilder idSb = new StringBuilder(20);
                    fLeeDatoDocumento("CIDDOCUMENTO", idSb, 20);
                    int.TryParse(idSb.ToString().Trim(), out int idDoc);
                    _logger.LogInformation($"Documento creado vía CrearDocumento. ID={idDoc}");
                    return idDoc;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Excepción en CrearDocumento");
                    try { fCancelarModificacionDocumento(); } catch { }
                    return -1;
                }
            }
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
            List<(string codigo, double cantidad, double precio, string nombre, string unidadMedida, string claveSAT)> productos,
            string usoCFDI = "G01",
            string formaPago = "99",
            string metodoPago = "PUE",
            string clienteRazonSocial = "",
            string clienteRFC = "",
            string clienteRegimenFiscal = "")
        {
            string serieOut = "";
            double folioOut = 0;
            lock (_lock)
            {
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

                // 2.5 Auto-crear cliente si no existe (antes de quemar folio)
                if (!string.IsNullOrEmpty(codigoCliente))
                {
                    int existeCte = fBuscaCteProv(codigoCliente);
                    if (existeCte != 0)
                    {
                        _logger.LogInformation($"Cliente {codigoCliente} no existe. Auto-creando...");
                        if (!AsegurarClienteInterno(codigoCliente, clienteRazonSocial, clienteRFC, clienteRegimenFiscal))
                        {
                            CerrarEmpresa();
                            return (false, $"No se pudo crear el cliente {codigoCliente}: {GetUltimoError()}", 0, "", 0);
                        }
                    }
                }

                // 2.6 Auto-crear productos faltantes (antes de quemar folio)
                if (productos != null && productos.Count > 0)
                {
                    foreach (var prod in productos)
                    {
                        if (string.IsNullOrEmpty(prod.codigo)) continue;
                        int existeProd = fBuscaProducto(prod.codigo);
                        if (existeProd != 0)
                        {
                            _logger.LogInformation($"Producto {prod.codigo} no existe. Auto-creando...");
                            if (!AsegurarProductoInterno(prod.codigo, prod.nombre, prod.unidadMedida, prod.claveSAT, prod.precio))
                            {
                                CerrarEmpresa();
                                return (false, $"No se pudo crear el producto {prod.codigo}: {GetUltimoError()}", 0, "", 0);
                            }
                        }
                    }
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
                    ("CMETODOPAG", formaPago),         // FORMA DE PAGO: 01, 03, 99 (NO PUE/PPD)
                    ("CCONDIPAGO", metodoPago)         // Condiciones: PUE, PPD u otro texto
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

                // MEJORA #7: Nota sobre refactor a funciones de alto nivel.
                // El bridge actualmente usa funciones de bajo nivel (fInsertaDocumento/fSetDatoDocumento/etc)
                // porque la struct tDocumento disponible no incluye todos los campos CFDI 4.0
                // (CUSOCFDI, CIDFORMAPAGO, CEXPORTACION, etc.). Migrar a fAltaDocumento requeriría
                // extender la struct tDocumento y validar todos los campos. Pendiente para v2.

                // 8. Validar estado del documento (#6) y Afectar (#1)
                // Para afectar, el documento no debe estar cancelado ni timbrado.
                StringBuilder timbradoSb = new StringBuilder(20);
                fLeeDatoDocumento("CTIMBRADO", timbradoSb, 20);
                string timbradoDoc = timbradoSb.ToString().Trim();
                StringBuilder canceladoSb = new StringBuilder(20);
                fLeeDatoDocumento("CCANCELADO", canceladoSb, 20);
                string canceladoDoc = canceladoSb.ToString().Trim();

                if (timbradoDoc == "1" || canceladoDoc == "1")
                {
                    _logger.LogWarning($"Documento en estado no afectable: CTIMBRADO={timbradoDoc}, CCANCELADO={canceladoDoc}. Saltando fAfectaDocto.");
                }
                else
                {
                    // MEJORA #1: Afectar documento para actualizar acumulados/inventario/saldos
                    tLlaveDocto llave = new tLlaveDocto
                    {
                        aCodConcepto = codigoConcepto,
                        aSerie = serie,
                        aFolio = folioNum
                    };
                    _logger.LogInformation($"Llamando a fAfectaDocto({codigoConcepto}, {serie}, {folioNum}, true)...");
                    int resAfecta = fAfectaDocto(ref llave, true);
                    if (resAfecta != 0)
                    {
                        string errAfecta = GetUltimoError(resAfecta);
                        _logger.LogWarning($"fAfectaDocto retornó {resAfecta} - {errAfecta}. La factura se creó pero los acumulados no se actualizaron.");
                    }
                    else
                    {
                        _logger.LogInformation("Documento afectado correctamente (acumulados actualizados).");
                    }
                }

                // 9. Cerrar empresa
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
        }

        /// <summary>
        /// Valida un documento antes de timbrarlo para detectar errores críticos sin hacer la llamada al SAT/PAC.
        /// Devuelve una lista de issues con severidad (Error/Warning) y descripción.
        /// NO envía la factura al PAC.
        /// </summary>
        public List<(string severidad, string categoria, string mensaje)> ValidarParaTimbrado(
            string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD)
        {
            var issues = new List<(string, string, string)>();
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK())
                {
                    issues.Add(("Error", "SDK", "No se pudo inicializar el SDK"));
                    return issues;
                }
                if (!AbrirEmpresa(rutaEmpresa))
                {
                    issues.Add(("Error", "Empresa", $"No se pudo abrir la empresa: {GetUltimoError()}"));
                    return issues;
                }

                string serieClean = (serie ?? "").Trim().ToUpper();

                // 1. Buscar el documento navegando (fBuscaDocumento a veces falla por codConcepto)
                bool documentoEncontrado = false;
                int idDocumentoEncontrado = 0;

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
                    }
                    else
                    {
                        res = fPosAnteriorDocumento();
                        intentos++;
                    }
                }

                if (!documentoEncontrado)
                {
                    issues.Add(("Error", "Documento", $"No se encontró documento Serie={serieClean} Folio={folio}. Revisados {intentos}."));
                    CerrarEmpresa();
                    return issues;
                }

                // Reposicionar el documento con fBuscaDocumento para que fLeeDatoDocumento/fPosPrimerMovimiento
                // operen sobre este documento y no sobre el último que se iteró.
                int resReposition = fBuscaDocumento(codigoConcepto, serieClean, folio);
                bool documentoPosicionado = (resReposition == 0);
                if (!documentoPosicionado)
                {
                    issues.Add(("Warning", "Documento", $"No se pudo reposicionar el documento por concepto ({GetUltimoError(resReposition)}). Validaciones se harán por SQL."));
                }

                // 2. Estado del documento (timbrado / cancelado) - se valida por SQL más abajo para no depender del documento posicionado

                // 3. Verificar movimientos del documento vía SQL (vía sqlcmd.exe)
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                try
                {
                    // Primero obtener el ID del documento
                    string sqlId = $"IF OBJECT_ID('dbo.admDocumentos') IS NOT NULL SELECT CAST(CIDDOCUMENTO AS VARCHAR(20)) FROM dbo.admDocumentos WHERE CSERIEDOCUMENTO = '{serieClean}' AND CFOLIO = {folio}";
                    string idDocResult = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sqlId).Trim();

                    if (string.IsNullOrEmpty(idDocResult))
                    {
                        issues.Add(("Error", "Documento", "No se encontró el documento en la base de datos SQL."));
                    }
                    else
                    {
                        string sqlMovs = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT CAST(p.CCODIGOPRODUCTO AS VARCHAR(50)) + @sep +
       CAST(ISNULL(NULLIF(LTRIM(RTRIM(p.CCLAVESAT)), ''), NULLIF(LTRIM(RTRIM(p.CCLAVEPRODSERV)), '')) AS VARCHAR(20)) + @sep +
       CAST(ISNULL(NULLIF(LTRIM(RTRIM(p.CIDUNIDADNOCONVERTIBLE)), ''), '0') AS VARCHAR(20)) + @sep +
       CAST(p.CNOMBREPRODUCTO AS VARCHAR(120))
FROM admMovimientos m JOIN admProductos p ON m.CIDPRODUCTO = p.CIDPRODUCTO
WHERE m.CIDDOCUMENTO = {idDocResult};
PRINT @end;";
                        string movResult = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sqlMovs);
                        var lineas = movResult.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l) && !l.Contains("~END~") && !l.StartsWith("Changed") && !l.StartsWith("(1 rows")).ToList();

                        if (lineas.Count == 0)
                        {
                            issues.Add(("Error", "Documento", "El documento no tiene movimientos (productos)."));
                        }
                        else
                        {
                            foreach (var linea in lineas)
                            {
                                var parts = linea.Split('|');
                                if (parts.Length < 3) continue;
                                string codProd = parts[0];
                                string cveSat = parts[1];
                                string unidad = parts[2];
                                string nombreProd = parts.Length > 3 ? parts[3] : "";

                                if (string.IsNullOrEmpty(cveSat))
                                {
                                    issues.Add(("Error", "Producto", $"Producto '{codProd}' ({nombreProd}) no tiene Clave SAT. El PAC rechazará la factura."));
                                }
                                if (string.IsNullOrEmpty(unidad) || unidad == "0")
                                {
                                    issues.Add(("Warning", "Producto", $"Producto '{codProd}' no tiene Clave Unidad SAT explícita (se usará default)."));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    issues.Add(("Warning", "Documento", $"No se pudo validar movimientos por SQL: {ex.Message}."));
                }

                // 4. Validar cliente del documento y campos CFDI 4.0
                if (documentoPosicionado)
                {
                    StringBuilder codClienteSb = new StringBuilder(50);
                    fLeeDatoDocumento("CCODIGOCLIENTE", codClienteSb, 50);
                    string codCliente = codClienteSb.ToString().Trim();

                    if (string.IsNullOrEmpty(codCliente))
                    {
                        issues.Add(("Error", "Cliente", "El documento no tiene cliente asignado."));
                    }
                    else
                    {
                        int resCte = fBuscaCteProv(codCliente);
                        if (resCte != 0)
                        {
                            int resNavCte = fPosPrimerCteProv();
                            while (resNavCte == 0)
                            {
                                StringBuilder sCte = new StringBuilder(50);
                                fLeeDatoCteProv("CCODIGOCLIENTE", sCte, 50);
                                if (sCte.ToString().Trim() == codCliente)
                                {
                                    resCte = 0;
                                    break;
                                }
                                resNavCte = fPosSiguienteCteProv();
                            }
                        }
                        if (resCte != 0)
                        {
                            issues.Add(("Error", "Cliente", $"Cliente '{codCliente}' no existe en el catálogo."));
                        }
                        else
                        {
                            StringBuilder rfcSb = new StringBuilder(20);
                            fLeeDatoCteProv("CRFC", rfcSb, 20);
                            string rfc = rfcSb.ToString().Trim();

                            StringBuilder regimenSb = new StringBuilder(10);
                            fLeeDatoCteProv("CREGIMENFISCAL", regimenSb, 10);
                            string regimen = regimenSb.ToString().Trim();

                            StringBuilder cpSb = new StringBuilder(10);
                            fLeeDatoCteProv("CCODIGOPOSTAL", cpSb, 10);
                            string cp = cpSb.ToString().Trim();

                            if (string.IsNullOrEmpty(rfc) || rfc == "XAXX010101000")
                            {
                                issues.Add(("Warning", "Cliente", $"Cliente '{codCliente}' usa RFC genérico ({rfc}). Solo válido para Público en General (UsoCFDI=S01)."));
                            }
                            if (string.IsNullOrEmpty(regimen))
                            {
                                issues.Add(("Error", "Cliente", $"Cliente '{codCliente}' no tiene Régimen Fiscal. CFDI 4.0 lo rechaza."));
                            }
                            if (string.IsNullOrEmpty(cp))
                            {
                                issues.Add(("Error", "Cliente", $"Cliente '{codCliente}' no tiene Código Postal del domicilio fiscal."));
                            }
                        }
                    }

                    // 5. Validar campos del documento (CFDI 4.0)
                    StringBuilder usoCfdiSb = new StringBuilder(10);
                    fLeeDatoDocumento("CUSOCFDI", usoCfdiSb, 10);
                    if (string.IsNullOrEmpty(usoCfdiSb.ToString().Trim()))
                    {
                        issues.Add(("Error", "Documento", "Falta UsoCFDI (CUSOCFDI) en el documento."));
                    }

                    StringBuilder formaPagoSb = new StringBuilder(10);
                    fLeeDatoDocumento("CIDFORMAPAGO", formaPagoSb, 10);
                    if (formaPagoSb.Length == 0) fLeeDatoDocumento("CMETODOPAG", formaPagoSb, 10);
                    if (string.IsNullOrEmpty(formaPagoSb.ToString().Trim()))
                    {
                        issues.Add(("Warning", "Documento", "Falta Forma de Pago (CFDI 4.0 la requiere)."));
                    }
                }
                else
                {
                    // Si no se pudo reposicionar el documento, leer cliente y campos CFDI directamente de SQL
                    try
                    {
                        string sqlDoc = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT CAST(ISNULL(d.CIDCLIENTEPROVEEDOR, '') AS VARCHAR(20)) + @sep +
       CAST(ISNULL(c.CRFC, '') AS VARCHAR(20)) + @sep +
       CAST(ISNULL(c.CREGIMFISC, '') AS VARCHAR(10)) + @sep +
       CAST(ISNULL(d.CMETODOPAG, '') AS VARCHAR(10)) + @sep +
       CAST(ISNULL(d.CCANCELADO, '0') AS VARCHAR(5))
FROM admDocumentos d
LEFT JOIN admClientes c ON d.CIDCLIENTEPROVEEDOR = c.CIDCLIENTEPROVEEDOR
WHERE d.CSERIEDOCUMENTO = '{serieClean}' AND d.CFOLIO = {folio};
PRINT @end;";
                        string docResult = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sqlDoc);
                        var lineaDoc = docResult.Split('\n').FirstOrDefault(l => l.Contains("|") && !l.Contains("~END~") && !l.StartsWith("Changed"));
                        if (lineaDoc == null)
                        {
                            issues.Add(("Warning", "Documento", "No se pudo leer el documento desde SQL (sin resultados)."));
                        }
                        else
                        {
                            var parts = lineaDoc.Trim().Split('|');
                            string idCliente = parts.Length > 0 ? parts[0].Trim() : "";
                            string rfc = parts.Length > 1 ? parts[1].Trim() : "";
                            string regimen = parts.Length > 2 ? parts[2].Trim() : "";
                            string metodoPag = parts.Length > 3 ? parts[3].Trim() : "";
                            string cancelado = parts.Length > 4 ? parts[4].Trim() : "0";

                            if (cancelado == "1") issues.Add(("Error", "Documento", "El documento está cancelado (CCANCELADO=1)."));

                            if (string.IsNullOrEmpty(idCliente) || idCliente == "0")
                            {
                                issues.Add(("Error", "Cliente", "El documento no tiene cliente asignado."));
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(rfc) || rfc == "XAXX010101000")
                                {
                                    issues.Add(("Warning", "Cliente", $"Cliente ID={idCliente} usa RFC genérico ({rfc}). Solo válido para Público en General (UsoCFDI=S01)."));
                                }
                                if (string.IsNullOrEmpty(regimen))
                                {
                                    issues.Add(("Error", "Cliente", $"Cliente ID={idCliente} no tiene Régimen Fiscal (CREGIMFISC). CFDI 4.0 lo rechaza."));
                                }
                            }
                            if (string.IsNullOrEmpty(metodoPag))
                            {
                                issues.Add(("Warning", "Documento", "Falta Forma/Método de Pago (CMETODOPAG)."));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        issues.Add(("Warning", "Documento", $"No se pudo leer campos del documento por SQL: {ex.Message}."));
                    }
                }

                // 6. CSD
                if (string.IsNullOrEmpty(passCSD))
                {
                    issues.Add(("Error", "CSD", "No se proporcionó la contraseña del CSD (passCSD)."));
                }

                string csdCer = Path.Combine(rutaEmpresa, "CSD", "CSD.cer");
                string csdKey = Path.Combine(rutaEmpresa, "CSD", "CSD.key");
                if (!File.Exists(csdCer))
                {
                    csdCer = Path.Combine(rutaEmpresa, "CSD.cer");
                }
                if (!File.Exists(csdKey))
                {
                    csdKey = Path.Combine(rutaEmpresa, "CSD.key");
                }
                if (!File.Exists(csdCer))
                {
                    issues.Add(("Warning", "CSD", $"No se encontró archivo .cer en ruta esperada (CSD\\CSD.cer ni {rutaEmpresa}\\CSD.cer). El SDK puede usar uno por default del sistema."));
                }
                if (!File.Exists(csdKey))
                {
                    issues.Add(("Warning", "CSD", $"No se encontró archivo .key en ruta esperada. Verifica la configuración del CSD en CONTPAQi."));
                }

                CerrarEmpresa();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al validar documento");
                issues.Add(("Error", "Sistema", $"Excepción: {ex.Message}"));
                try { CerrarEmpresa(); } catch { }
            }
            return issues;
            }
        }

        /// <summary>
        /// Timbra una factura existente usando fEmitirDocumento
        /// </summary>
        public (bool exito, string mensaje) TimbrarFactura(string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD)
        {
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK()) return (false, "No se pudo inicializar el SDK");
                if (!AbrirEmpresa(rutaEmpresa)) return (false, "No se pudo abrir la empresa");

                _logger.LogInformation($"Timbrando factura: Concepto={codigoConcepto}, Serie={serie}, Folio={folio}");
                
                // MEJORA #2: Inicializar info de licencia antes de emitir (requerido por fEmitirDocumento)
                // aSistema: 1 = CONTPAQi Comercial Premium
                _logger.LogInformation("Llamando a fInicializaLicenseInfo(1)...");
                int resLic = fInicializaLicenseInfo(1);
                if (resLic != 0)
                {
                    CerrarEmpresa();
                    string errLic = resLic == -1
                        ? "No se pudo conectar con el Servidor de Licencias. Verifica que CONTPAQi esté activado, tengas licencia multiusuario (5+ usuarios) y conexión al servidor de licencias."
                        : $"Error {resLic} al inicializar licencia: {GetUltimoError(resLic)}";
                    _logger.LogError($"fInicializaLicenseInfo falló: {resLic} - {errLic}");
                    return (false, errLic);
                }
                _logger.LogInformation("Licencia inicializada correctamente.");

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

                    EmitirWebhook("timbrado.fallido", new {
                        rutaEmpresa, codigoConcepto, serie, folio, error = err
                    });

                    return (false, $"Error al timbrar: {err}");
                }

                _logger.LogInformation($"Factura {serie}{folio} timbrada exitosamente.");

                // Notificar a los webhooks suscritos
                string uuid = "";
                try { uuid = ObtenerUuid(rutaEmpresa, codigoConcepto, serie, folio).uuid; } catch { }
                EmitirWebhook("timbrado.exitoso", new {
                    rutaEmpresa, codigoConcepto, serie, folio, uuid,
                    timestamp = DateTime.UtcNow
                });

                return (true, "Factura timbrada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción durante el timbrado");
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}");
            }
            }
        }

        public (bool exito, string mensaje, string xml) ObtenerXml(string rutaEmpresa, string codigoConcepto, string serie, double folio, int formato = 0)
        {
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK()) return (false, "No se pudo inicializar el SDK", "");
                if (!AbrirEmpresa(rutaEmpresa)) return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", "");

                string serieClean = (serie ?? "").Trim().ToUpper();
                string folioStr = folio.ToString();
                string extension = formato == 1 ? ".pdf" : ".xml";
                
                // MEJORA #10: formato 0=XML, 1=PDF (manual SDK)
                _logger.LogInformation($"[E1] fEntregEnDiscoXML: Concepto={codigoConcepto}, Serie={serieClean}, Folio={folio}, Formato={formato}({extension})");
                
                string suggestedPath = Path.Combine(rutaEmpresa, "XML_SDK", $"{serieClean}{folioStr}{extension}");
                int resEntrega = fEntregEnDiscoXML(codigoConcepto, serieClean, folio, formato, suggestedPath);

                if (resEntrega == 0)
                {
                    _logger.LogInformation("¡SDK reportó éxito (0)!");
                    System.Threading.Thread.Sleep(1000);

                    // Lista de búsqueda basada en el hallazgo real
                    var posiblesRutas = new List<string> { 
                        suggestedPath,
                        Path.Combine(rutaEmpresa, "XML_SDK", $"{serieClean}{folioStr}{extension}"),
                        Path.Combine(rutaEmpresa, "XML_SDK", $"{folioStr}{extension}"),
                        Path.Combine(rutaEmpresa, $"{serieClean}{folioStr}{extension}"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Factura_{folioStr}{extension}")
                    };

                    foreach (var ruta in posiblesRutas) {
                        _logger.LogInformation($"Verificando: {ruta}");
                        if (File.Exists(ruta)) {
                            _logger.LogInformation($"¡ARCHIVO ENCONTRADO! Leyendo: {ruta}");
                            byte[] bytes = File.ReadAllBytes(ruta);
                            string b64 = Convert.ToBase64String(bytes);
                            CerrarEmpresa();
                            return (true, $"Archivo {extension.ToUpper()} obtenido correctamente", b64);
                        }
                    }
                }

                _logger.LogWarning($"No se encontró el archivo en las rutas estándar. Formato={formato}.");
                CerrarEmpresa();
                return (false, $"El SDK reportó éxito pero el archivo {extension} no se encontró en la carpeta XML_SDK de la empresa.", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerXml");
                CerrarEmpresa();
                return (false, $"Error: {ex.Message}", "");
            }
            }
        }

        /// <summary>
        /// Lista los últimos documentos de la empresa para diagnóstico
        /// </summary>
        public List<Dictionary<string, object>> ListarUltimosDocumentos(string rutaEmpresa, int cantidad = 10)
        {
            lock (_lock)
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
            lock (_lock)
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
                    EmitirWebhook("cancelacion.fallida", new {
                        rutaEmpresa, codigoConcepto, serie, folio, motivoCancelacion, error = errorMsg
                    });
                    return (false, $"Error al cancelar documento: {errorMsg}", "");
                }

                _logger.LogInformation("¡Documento cancelado exitosamente!");

                EmitirWebhook("cancelacion.exitosa", new {
                    rutaEmpresa, codigoConcepto, serie, folio, motivoCancelacion,
                    uuidSustitucion, timestamp = DateTime.UtcNow
                });
                
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
            lock (_lock)
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
            lock (_lock)
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
                int idCliente = 0; // Declaración en ámbito principal para evitar CS0103

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
            lock (_lock)
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

                    // ===== CFDI 4.0: FORZAR IVA 16% =====
                    // Sin esto, el XML sale sin impuestos y falla con ObjetoImp=02
                    _logger.LogInformation("Forzando configuración de IVA 16% en producto...");
                    fSetDatoProducto("CBANIMPUESTO", "1");    // 1 = Aplica impuestos
                    fSetDatoProducto("CIMPUESTO1", "16.0");   // IVA 16%
                    // ====================================

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
        }

        /// <summary>
        /// Ejecuta SQLCMD y devuelve la salida estándar. Lanza excepción si hay error.
        /// </summary>
        private string EjecutarSqlCmd(string instance, string user, string pass, string bd, string sqlQuery)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
                Arguments = $"-S \"{instance}\" -U \"{user}\" -P \"{pass}\" -d \"{bd}\" -Q \"{sqlQuery.Replace("\"", "\\\"")}\" -W -h -1",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = System.Diagnostics.Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            if (!p.WaitForExit(15000))
            {
                try { p.Kill(); } catch { }
                throw new Exception("SQLCMD timeout");
            }
            if (p.ExitCode != 0)
            {
                throw new Exception($"SQLCMD exit {p.ExitCode}: {error.Trim()}");
            }
            return output;
        }

        /// <summary>
        /// MEJORA #B: Helper para ejecutar SQL y devolver resultados como List de diccionarios.
        /// Usa sqlcmd con separador | y cabecera, espera una columna "COL_NAMES" con headers.
        /// </summary>
        private List<Dictionary<string, object>> EjecutarSqlCmdLista(string bd, string sqlQuery, string colNamesHeader = "COL_NAMES")
        {
            // Construir wrapper SQL: emite headers como primera línea, luego datos con NULL para vacíos
            string wrapped = $@"SET NOCOUNT ON;
DECLARE @cols TABLE(i INT IDENTITY, n NVARCHAR(200));
DECLARE @sql NVARCHAR(MAX) = N'{sqlQuery.Replace("'", "''")}';
DECLARE @dynsql NVARCHAR(MAX) = N'SELECT * INTO #tmp FROM (' + @sql + N') x';
EXEC sp_executesql @dynsql;
DECLARE @cnt INT = (SELECT COUNT(*) FROM tempdb.sys.columns WHERE object_id = OBJECT_ID('tempdb..#tmp'));
DECLARE @i INT = 1;
WHILE @i <= @cnt
BEGIN
    DECLARE @cname NVARCHAR(200) = (SELECT name FROM tempdb.sys.columns WHERE object_id = OBJECT_ID('tempdb..#tmp') AND column_id = @i);
    INSERT INTO @cols(n) VALUES (@cname);
    SET @i = @i + 1;
END
DECLARE @sep NVARCHAR(5) = N'|';
DECLARE @hdr NVARCHAR(MAX) = N'';
SET @i = 1;
WHILE @i <= @cnt
BEGIN
    SET @hdr = @hdr + (SELECT n FROM @cols WHERE i=@i) + @sep;
    SET @i = @i + 1;
END
SET @hdr = LEFT(@hdr, LEN(@hdr) - LEN(@sep));
PRINT '{colNamesHeader}=' + @hdr;
SELECT
    (SELECT STUFF((SELECT @sep + ISNULL(CAST([col] AS NVARCHAR(MAX)), '') FROM #tmp FOR XML PATH('')), 1, LEN(@sep), '')
FROM #tmp
UNPIVOT ([col] FOR col_name IN ([col1])) p;
DROP TABLE #tmp;
";
            // Para evitar UNPIVOT complejo, uso otro enfoque: query directo + headers vía PRINT
            // Más simple: ejecutar la query y separar headers vs datos con -h -1 y otro truco
            // Hago un método más simple: leer primera línea como headers, resto como datos
            var result = new List<Dictionary<string, object>>();

            // Por simplicidad y robustez, ejecuto la query con -W -s "|" y separo la primera línea (headers) del resto (datos)
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
                Arguments = $"-S \"{_instanceSql}\" -U \"{_sqlUser}\" -P \"{_sqlPassword}\" -d \"{bd}\" -Q \"{sqlQuery.Replace("\"", "\\\"").Replace("|", "^|")}\" -W -s \"|\" -h -1",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = System.Diagnostics.Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            if (!p.WaitForExit(30000))
            {
                try { p.Kill(); } catch { }
                throw new Exception("SQLCMD timeout");
            }
            if (p.ExitCode != 0)
            {
                throw new Exception($"SQLCMD exit {p.ExitCode}: {error.Trim()}");
            }

            var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l) && !l.Contains("rows affected") && !l.StartsWith("WARNING") && !l.StartsWith("Changed database context"))
                .ToList();
            if (lines.Count == 0) return result;

            // Primera línea = headers
            string[] headers = lines[0].Split('|').Select(h => h.Trim()).ToArray();
            for (int i = 1; i < lines.Count; i++)
            {
                var fields = lines[i].Split('|');
                if (fields.Length < headers.Length) continue;
                var dict = new Dictionary<string, object>();
                for (int j = 0; j < headers.Length; j++)
                {
                    string val = fields[j].Trim();
                    dict[headers[j]] = string.IsNullOrEmpty(val) ? null : (object)val;
                }
                result.Add(dict);
            }
            return result;
        }

        /// <summary>
        /// Helper que ejecuta SQL en una BD específica (para sync catálogos).
        /// </summary>
        private List<Dictionary<string, object>> EjecutarSqlCmdListaEnBd(string bd, string sqlQuery)
        {
            return EjecutarSqlCmdLista(bd, sqlQuery);
        }

        /// <summary>
        /// Asegura que el cliente exista en la empresa actualmente abierta. Si no existe, lo crea.
        /// Asume SDK ya inicializado y empresa abierta. No cierra la empresa.
        /// </summary>
        private bool AsegurarClienteInterno(string codigo, string razonSocial, string rfc, string regimenFiscal)
        {
            // Si ya existe, salir
            int existe = fBuscaCteProv(codigo);
            if (existe == 0)
            {
                _logger.LogInformation($"AsegurarClienteInterno: cliente {codigo} ya existe.");
                return true;
            }

            // Valores por defecto si vienen vacíos
            string rznFinal = !string.IsNullOrEmpty(razonSocial) ? razonSocial : $"Cliente generado por API ({codigo})";
            string rfcFinal = !string.IsNullOrEmpty(rfc) ? rfc : "XAXX010101000";
            string regimenFinal = regimenFiscal ?? "";

            int idCliente = 0;
            tCteProv cliente = new tCteProv
            {
                aCodigo = codigo,
                aRazonSocial = rznFinal,
                aRFC = rfcFinal,
                aDenComercial = rznFinal,
                aTipoCliente = 1,
                aEstatus = 1,
                aPais = "México",
                aIdMoneda = 1,
                aLimiteCreditoFlag = 0,
                aLimiteCredito = 0
            };

            int result = fAltaCteProv(ref idCliente, ref cliente);
            if (result != 0)
            {
                _logger.LogError($"fAltaCteProv falló para {codigo}: {result} - {GetUltimoError(result)}");
                return false;
            }

            // Reforzar RFC, Razón Social y Régimen Fiscal
            fBuscaCteProv(codigo);
            fEditaCteProv();
            fSetDatoCteProv("CRFC", rfcFinal);
            fSetDatoCteProv("CRAZONSOCIAL", rznFinal);
            if (!string.IsNullOrEmpty(regimenFinal))
            {
                fSetDatoCteProv("CREGIMFISC", regimenFinal);
                _logger.LogInformation($"Cliente CREGIMFISC seteado a: {regimenFinal}");
            }
            int resGuarda = fGuardaCteProv();
            if (resGuarda != 0)
            {
                _logger.LogWarning($"fGuardaCteProv después de alta tuvo: {resGuarda}");
            }

            _logger.LogInformation($"Cliente {codigo} auto-creado con ID {idCliente}.");
            return true;
        }

        /// <summary>
        /// Asegura que el producto exista en la empresa actualmente abierta. Si no existe, lo crea.
        /// Asume SDK ya inicializado y empresa abierta. No cierra la empresa.
        /// </summary>
        private bool AsegurarProductoInterno(string codigo, string nombre, string unidadMedida, string claveSAT, double precio)
        {
            // Si ya existe, salir
            int existe = fBuscaProducto(codigo);
            if (existe == 0)
            {
                _logger.LogInformation($"AsegurarProductoInterno: producto {codigo} ya existe.");
                return true;
            }

            string nombreFinal = !string.IsNullOrEmpty(nombre) ? nombre : codigo;
            string unidadFinal = !string.IsNullOrEmpty(unidadMedida) ? unidadMedida : "H87";

            // Normalizar clave SAT a 8 dígitos
            if (!string.IsNullOrEmpty(claveSAT) && claveSAT.Length < 8 && int.TryParse(claveSAT, out _))
            {
                claveSAT = claveSAT.PadLeft(8, '0');
            }

            int resInserta = fInsertaProducto();
            if (resInserta != 0)
            {
                _logger.LogError($"fInsertaProducto falló para {codigo}: {resInserta} - {GetUltimoError(resInserta)}");
                return false;
            }

            fSetDatoProducto("CCODIGOPRODUCTO", codigo);
            fSetDatoProducto("CNOMBREPRODUCTO", nombreFinal);
            fSetDatoProducto("CTIPOPRODUCTO", "1");
            fSetDatoProducto("CSTATUSPRODUCTO", "1");
            fSetDatoProducto("CMETODOCOSTEO", "1");
            fSetDatoProducto("CCONTROLEXISTENCIA", "0");

            if (precio > 0)
            {
                string precioStr = precio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                fSetDatoProducto("CPRECIO1", precioStr);
                fSetDatoProducto("CPRECIO2", precioStr);
                fSetDatoProducto("CPRECIO3", precioStr);
            }

            if (!string.IsNullOrEmpty(claveSAT))
            {
                fSetDatoProducto("CCLAVESAT", claveSAT);
                fSetDatoProducto("CCLAVEPRODSERV", claveSAT);
            }

            // Unidad SAT
            fSetDatoProducto("CCODIGOUNIDADNOCONVERTIBLE", unidadFinal);
            fSetDatoProducto("CCOMNOMBREUNIDAD", unidadFinal);

            int resGuarda = fGuardaProducto();
            if (resGuarda != 0)
            {
                _logger.LogError($"fGuardaProducto falló para {codigo}: {resGuarda} - {GetUltimoError(resGuarda)}");
                fCancelarModificacionProducto();
                return false;
            }

            _logger.LogInformation($"Producto {codigo} auto-creado.");
            return true;
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
        /// MEJORA #8: Asocia un documento de pago con un documento a pagar (CxC/CxP).
        /// Implementa fSaldarDocumento del SDK.
        /// </summary>
        public (bool exito, string mensaje) SaldarDocumento(
            string rutaEmpresa,
            string codConceptoPagar, string seriePagar, double folioPagar,
            string codConceptoPago, string seriePago, double folioPago,
            double importe, int idMoneda, string fecha)
        {
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK())
                    return (false, "No se pudo inicializar el SDK");
                if (!AbrirEmpresa(rutaEmpresa))
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}");

                tLlaveDocto llavePagar = new tLlaveDocto
                {
                    aCodConcepto = codConceptoPagar,
                    aSerie = seriePagar,
                    aFolio = folioPagar
                };
                tLlaveDocto llavePago = new tLlaveDocto
                {
                    aCodConcepto = codConceptoPago,
                    aSerie = seriePago,
                    aFolio = folioPago
                };

                _logger.LogInformation($"fSaldarDocumento({codConceptoPagar}/{seriePagar}/{folioPagar} <- {codConceptoPago}/{seriePago}/{folioPago}, {importe})");
                int result = fSaldarDocumento(ref llavePagar, ref llavePago, importe, idMoneda, fecha);
                CerrarEmpresa();

                if (result != 0)
                {
                    return (false, $"Error al saldar documento: {GetUltimoError(result)}");
                }
                return (true, "Documento saldado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al saldar documento");
                try { CerrarEmpresa(); } catch { }
                return (false, $"Excepción: {ex.Message}");
            }
            }
        }

        /// <summary>
        /// MEJORA #9: Obtiene los datos del CFDI (UUID, fecha, sello, etc.) de un documento timbrado.
        /// Implementa fDocumentoUUID / fObtieneDatosCFDI + fLeeDatoCFDI del SDK.
        /// </summary>
        public (bool exito, string mensaje, string uuid) ObtenerUuid(string rutaEmpresa, string codigoConcepto, string serie, double folio)
        {
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK())
                    return (false, "No se pudo inicializar el SDK", "");
                if (!AbrirEmpresa(rutaEmpresa))
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", "");

                StringBuilder uuidSb = new StringBuilder(50);
                int result = fDocumentoUUID(codigoConcepto, (serie ?? "").Trim().ToUpper(), folio, uuidSb);
                CerrarEmpresa();

                if (result != 0)
                {
                    return (false, $"Error al obtener UUID: {GetUltimoError(result)}", "");
                }
                return (true, "UUID obtenido", uuidSb.ToString().Trim());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener UUID");
                try { CerrarEmpresa(); } catch { }
                return (false, $"Excepción: {ex.Message}", "");
            }
            }
        }

        /// <summary>
        /// MEJORA #9: Lee un dato específico del CFDI timbrado de un documento.
        /// aDato: 1=SerieCertEmisor, 2=UUID, 3=SerieCertSAT, 4=FechaHoraCert,
        /// 5=SelloDigitalCFDI, 6=SelloSAT, 7=CadenaOriginalSAT, 8=MetodoPago,
        /// 9=LugarExpedicion, 10=RegimenFiscal.
        /// Requiere fBuscarDocumento previo (no implementado aquí; usa fDocumentoUUID para UUID directo).
        /// </summary>
        public (bool exito, string mensaje, string valor) ObtenerDatoCfdi(string rutaEmpresa, string password, string codigoConcepto, string serie, double folio, int dato)
        {
            lock (_lock)
            {
            try
            {
                if (!InicializarSDK())
                    return (false, "No se pudo inicializar el SDK", "");
                if (!AbrirEmpresa(rutaEmpresa))
                    return (false, $"No se pudo abrir la empresa: {GetUltimoError()}", "");

                // fObtieneDatosCFDI requiere que el documento esté previamente posicionado.
                // Usamos fBuscaDocumento primero.
                string serieClean = (serie ?? "").Trim().ToUpper();
                int resBusca = fBuscaDocumento(codigoConcepto, serieClean, folio);
                if (resBusca != 0)
                {
                    CerrarEmpresa();
                    return (false, $"No se encontró el documento: {GetUltimoError(resBusca)}", "");
                }

                int resObten = fObtieneDatosCFDI(password ?? "");
                if (resObten != 0)
                {
                    CerrarEmpresa();
                    return (false, $"Error al obtener datos CFDI: {GetUltimoError(resObten)}", "");
                }

                StringBuilder valorSb = new StringBuilder(512);
                int resLee = fLeeDatoCFDI(valorSb, dato);
                CerrarEmpresa();

                if (resLee != 0)
                {
                    return (false, $"Error al leer dato CFDI: {GetUltimoError(resLee)}", "");
                }
                return (true, "Dato CFDI obtenido", valorSb.ToString().Trim());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener dato CFDI");
                try { CerrarEmpresa(); } catch { }
                return (false, $"Excepción: {ex.Message}", "");
            }
            }
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
        /// Lista los conceptos de la empresa leyendo directamente desde SQL Server
        /// (el SDK de CONTPAQi no expone una función directa para conceptos, hay que navegar
        /// documento por documento para inferirlos; SQL es más confiable).
        /// </summary>
        public List<(string codigo, string nombre)> ListarConceptos(string rutaEmpresa)
        {
            var conceptos = new List<(string codigo, string nombre)>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string sql = @"SET NOCOUNT ON;
SELECT DISTINCT CAST(CCODIGOCONCEPTO AS VARCHAR(30)) + '|' + CAST(CNOMBRECONCEPTO AS VARCHAR(60)) AS linea
FROM admConceptos
WHERE CESTATUSCONCEPTO = 1
ORDER BY CCODIGOCONCEPTO;
PRINT '~END~';";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var parts = l.Split('|');
                    if (parts.Length >= 2)
                    {
                        conceptos.Add((parts[0].Trim(), parts[1].Trim()));
                    }
                }
                _logger.LogInformation($"ListarConceptos: {conceptos.Count} conceptos encontrados en {bdEmpresa}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar conceptos vía SQL.");
            }
            return conceptos;
        }

        /// <summary>
        /// Lista los primeros N productos de la empresa
        /// </summary>
        public List<(string codigo, string nombre, double precio)> ListarProductos(string rutaEmpresa, int limite = 20)
        {
            lock (_lock)
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

        // =====================================================================
        // ============ MÉTODOS DE SINCRONIZACIÓN BIDIRECCIONAL =================
        // =====================================================================

        // Lista de webhooks en memoria (también se persisten a disco en webhookService)
        private readonly List<(string evento, string url)> _webhooks = new();
        private readonly object _webhooksLock = new();

        /// <summary>
        /// Lista TODOS los clientes de la empresa leyendo desde SQL Server.
        /// Devuelve un diccionario con los campos clave para sincronizar con Laravel/MySQL.
        /// </summary>
        public List<Dictionary<string, object>> ListarClientesTodos(string rutaEmpresa, int limite = 500)
            => EjecutarListadoClientes(rutaEmpresa, desde: null, limite);

        /// <summary>
        /// Lista clientes modificados desde una fecha (sincronización incremental).
        /// </summary>
        public List<Dictionary<string, object>> ListarClientesModificados(string rutaEmpresa, DateTime desde, int limite = 500)
            => EjecutarListadoClientes(rutaEmpresa, desde, limite);

        private List<Dictionary<string, object>> EjecutarListadoClientes(string rutaEmpresa, DateTime? desde, int limite)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string filtroFecha = desde.HasValue
                    ? $"AND (c.CTIMESTAMP >= '{desde.Value:yyyy-MM-dd HH:mm:ss}' OR c.CFECHAALTA >= '{desde.Value:yyyy-MM-dd}')"
                    : "";

                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT TOP {limite}
  CAST(c.CIDCLIENTEPROVEEDOR AS VARCHAR(20)) + @sep +
  CAST(ISNULL(c.CCODIGOCLIENTE,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(c.CRAZONSOCIAL,'') AS VARCHAR(120)) + @sep +
  CAST(ISNULL(c.CRFC,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(c.CEMAIL,'') AS VARCHAR(120)) + @sep +
  CAST(ISNULL(c.CCALLE,'') AS VARCHAR(80)) + @sep +
  CAST(ISNULL(c.CCOLONIA,'') AS VARCHAR(80)) + @sep +
  CAST(ISNULL(c.CCODIGOPOSTAL,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(c.CCIUDAD,'') AS VARCHAR(80)) + @sep +
  CAST(ISNULL(c.CESTADO,'') AS VARCHAR(80)) + @sep +
  CAST(ISNULL(c.CPAIS,'') AS VARCHAR(80)) + @sep +
  CAST(ISNULL(c.CREGIMFISC,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(c.CUSOCFDI,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(c.CMETODOPAG,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(c.CTELEFONO1,'') AS VARCHAR(40)) + @sep +
  CAST(ISNULL(c.CTIMESTAMP,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(c.CESTATUS,'') AS VARCHAR(5)) + @sep +
  CAST(ISNULL(c.CFECHAALTA,'') AS VARCHAR(20))
FROM admClientes c
WHERE c.CTIPOCLIENTE IN (1,3) {filtroFecha}
ORDER BY c.CIDCLIENTEPROVEEDOR DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 7) continue;
                    lista.Add(new Dictionary<string, object>
                    {
                        ["contpaqi_id"] = p[0].Trim(),
                        ["codigo"] = p[1].Trim(),
                        ["razon_social"] = p[2].Trim(),
                        ["rfc"] = p[3].Trim(),
                        ["email"] = p[4].Trim(),
                        ["calle"] = p[5].Trim(),
                        ["colonia"] = p[6].Trim(),
                        ["codigo_postal"] = p[7].Trim(),
                        ["ciudad"] = p[8].Trim(),
                        ["estado"] = p[9].Trim(),
                        ["pais"] = p[10].Trim(),
                        ["regimen_fiscal"] = p[11].Trim(),
                        ["uso_cfdi"] = p[12].Trim(),
                        ["forma_pago"] = p[13].Trim(),
                        ["telefono"] = p[14].Trim(),
                        ["timestamp"] = p[15].Trim(),
                        ["estatus"] = p[16].Trim(),
                        ["fecha_alta"] = p[17].Trim()
                    });
                }
                _logger.LogInformation($"ListarClientes: {lista.Count} registros");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes vía SQL");
            }
            return lista;
        }

        /// <summary>
        /// Lista TODOS los productos del catálogo de la empresa.
        /// </summary>
        public List<Dictionary<string, object>> ListarProductosTodos(string rutaEmpresa, int limite = 500)
            => EjecutarListadoProductos(rutaEmpresa, desde: null, limite);

        /// <summary>
        /// Lista productos modificados desde una fecha (sincronización incremental).
        /// </summary>
        public List<Dictionary<string, object>> ListarProductosModificados(string rutaEmpresa, DateTime desde, int limite = 500)
            => EjecutarListadoProductos(rutaEmpresa, desde, limite);

        private List<Dictionary<string, object>> EjecutarListadoProductos(string rutaEmpresa, DateTime? desde, int limite)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string filtroFecha = desde.HasValue
                    ? $"AND (p.CTIMESTAMP >= '{desde.Value:yyyy-MM-dd HH:mm:ss}' OR p.CFECHAALTA >= '{desde.Value:yyyy-MM-dd}')"
                    : "";

                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT TOP {limite}
  CAST(p.CIDPRODUCTO AS VARCHAR(20)) + @sep +
  CAST(ISNULL(p.CCODIGOPRODUCTO,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(p.CNOMBREPRODUCTO,'') AS VARCHAR(120)) + @sep +
  CAST(ISNULL(p.CDESCRIPCIONPRODUCTO,'') AS VARCHAR(255)) + @sep +
  CAST(ISNULL(p.CCLAVESAT, p.CCLAVEPRODSERV,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(p.CCODIGOUNIDADNOCONVERTIBLE, p.CCOMNOMBREUNIDAD,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(p.CPRECIO1,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(p.CPRECIO2,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(p.CTIPOPRODUCTO,0) AS VARCHAR(5)) + @sep +
  CAST(ISNULL(p.CSTATUSPRODUCTO,0) AS VARCHAR(5)) + @sep +
  CAST(ISNULL(p.CCONTROLEXISTENCIA,0) AS VARCHAR(5)) + @sep +
  CAST(ISNULL(p.CEXISTENCIA,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(p.CCLAVEPRODSERV,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(p.CTIMESTAMP,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(p.CFECHAALTA,'') AS VARCHAR(20))
FROM admProductos p
WHERE 1=1 {filtroFecha}
ORDER BY p.CIDPRODUCTO DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 7) continue;
                    double.TryParse(p[6], out double precio1);
                    double.TryParse(p[7], out double precio2);
                    double.TryParse(p[11], out double existencia);
                    lista.Add(new Dictionary<string, object>
                    {
                        ["contpaqi_id"] = p[0].Trim(),
                        ["codigo"] = p[1].Trim(),
                        ["nombre"] = p[2].Trim(),
                        ["descripcion"] = p[3].Trim(),
                        ["clave_sat"] = p[4].Trim(),
                        ["unidad_sat"] = p[5].Trim(),
                        ["precio1"] = precio1,
                        ["precio2"] = precio2,
                        ["tipo_producto"] = p[8].Trim(),
                        ["estatus"] = p[9].Trim(),
                        ["control_existencia"] = p[10].Trim(),
                        ["existencia"] = existencia,
                        ["clave_prod_serv"] = p[12].Trim(),
                        ["timestamp"] = p[13].Trim(),
                        ["fecha_alta"] = p[14].Trim()
                    });
                }
                _logger.LogInformation($"ListarProductos: {lista.Count} registros");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos vía SQL");
            }
            return lista;
        }

        /// <summary>
        /// Lista documentos (facturas, notas de crédito, etc.) modificados desde una fecha.
        /// Útil para sincronizar ventas desde CONTPAQi hacia Laravel.
        /// </summary>
        public List<Dictionary<string, object>> ListarDocumentosModificados(string rutaEmpresa, DateTime desde, int limite = 500)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT TOP {limite}
  CAST(d.CIDDOCUMENTO AS VARCHAR(20)) + @sep +
  CAST(ISNULL(d.CIDCONCEPTODOCUMENTO,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(d.CCODIGOCONCEPTO,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(d.CSERIEDOCUMENTO,'') AS VARCHAR(12)) + @sep +
  CAST(ISNULL(d.CFOLIO,0) AS VARCHAR(20)) + @sep +
  CAST(ISNULL(d.CFECHA,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(d.CIDCLIENTEPROVEEDOR,'') AS VARCHAR(20)) + @sep +
  CAST(ISNULL(c.CCODIGOCLIENTE,'') AS VARCHAR(30)) + @sep +
  CAST(ISNULL(c.CRAZONSOCIAL,'') AS VARCHAR(120)) + @sep +
  CAST(ISNULL(d.CIMPORTE,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(d.CIVA,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(d.CTOTAL,0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(d.CMETODOPAG,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(d.CUSOCFDI,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(d.CCANCELADO,0) AS VARCHAR(5)) + @sep +
  CAST(ISNULL(d.CUUID,'') AS VARCHAR(50)) + @sep +
  CAST(ISNULL(d.CIDFORMAPAGO,'') AS VARCHAR(10)) + @sep +
  CAST(ISNULL(d.CFOLIOSAT,'') AS VARCHAR(50)) + @sep +
  CAST(ISNULL(d.CTIMESTAMP,'') AS VARCHAR(30))
FROM admDocumentos d
LEFT JOIN admClientes c ON d.CIDCLIENTEPROVEEDOR = c.CIDCLIENTEPROVEEDOR
WHERE d.CTIMESTAMP >= '{desde:yyyy-MM-dd HH:mm:ss}'
ORDER BY d.CIDDOCUMENTO DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 12) continue;
                    double.TryParse(p[4], out double folio);
                    double.TryParse(p[9], out double importe);
                    double.TryParse(p[10], out double iva);
                    double.TryParse(p[11], out double total);
                    lista.Add(new Dictionary<string, object>
                    {
                        ["contpaqi_id"] = p[0].Trim(),
                        ["id_concepto"] = p[1].Trim(),
                        ["codigo_concepto"] = p[2].Trim(),
                        ["serie"] = p[3].Trim(),
                        ["folio"] = folio,
                        ["fecha"] = p[5].Trim(),
                        ["cliente_id"] = p[6].Trim(),
                        ["cliente_codigo"] = p[7].Trim(),
                        ["cliente_razon_social"] = p[8].Trim(),
                        ["importe"] = importe,
                        ["iva"] = iva,
                        ["total"] = total,
                        ["metodo_pago"] = p[12].Trim(),
                        ["uso_cfdi"] = p[13].Trim(),
                        ["cancelado"] = p[14].Trim(),
                        ["uuid"] = p[15].Trim(),
                        ["forma_pago"] = p[16].Trim(),
                        ["folio_sat"] = p[17].Trim(),
                        ["timestamp"] = p[18].Trim()
                    });
                }
                _logger.LogInformation($"ListarDocumentosModificados: {lista.Count} documentos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar documentos vía SQL");
            }
            return lista;
        }

        /// <summary>
        /// Obtiene un cliente específico por su código de CONTPAQi.
        /// </summary>
        public Dictionary<string, object>? ObtenerClientePorCodigo(string rutaEmpresa, string codigo)
        {
            var todos = ListarClientesTodos(rutaEmpresa, 5000);
            return todos.FirstOrDefault(c => string.Equals(c.GetValueOrDefault("codigo")?.ToString(), codigo, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtiene un producto específico por su código.
        /// </summary>
        public Dictionary<string, object>? ObtenerProductoPorCodigo(string rutaEmpresa, string codigo)
        {
            var todos = ListarProductosTodos(rutaEmpresa, 5000);
            return todos.FirstOrDefault(p => string.Equals(p.GetValueOrDefault("codigo")?.ToString(), codigo, StringComparison.OrdinalIgnoreCase));
        }

        // =====================================================================
        // ============ REPORTES ===============================================
        // =====================================================================

        /// <summary>
        /// Reporte de ventas por día en un periodo.
        /// </summary>
        public List<Dictionary<string, object>> ReporteVentasPorPeriodo(string rutaEmpresa, DateTime desde, DateTime hasta)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT
  CAST(d.CFECHA AS VARCHAR(20)) + @sep +
  CAST(COUNT(*) AS VARCHAR(10)) + @sep +
  CAST(ISNULL(SUM(d.CIMPORTE),0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(SUM(d.CIVA),0) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(SUM(d.CTOTAL),0) AS VARCHAR(30))
FROM admDocumentos d
WHERE d.CIDCONCEPTODOCUMENTO IN (
  SELECT CIDCONCEPTODOCUMENTO FROM admConceptos WHERE CTIPODOCUMENTO = 4
)
AND d.CCANCELADO = 0
AND d.CFECHA BETWEEN '{desde:yyyy-MM-dd}' AND '{hasta:yyyy-MM-dd}'
GROUP BY d.CFECHA
ORDER BY d.CFECHA DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 5) continue;
                    int.TryParse(p[1], out int numDocs);
                    double.TryParse(p[2], out double importe);
                    double.TryParse(p[3], out double iva);
                    double.TryParse(p[4], out double total);
                    lista.Add(new Dictionary<string, object>
                    {
                        ["fecha"] = p[0].Trim(),
                        ["documentos"] = numDocs,
                        ["importe"] = importe,
                        ["iva"] = iva,
                        ["total"] = total
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en reporte de ventas");
            }
            return lista;
        }

        /// <summary>
        /// Top N clientes por ventas en un periodo.
        /// </summary>
        public List<Dictionary<string, object>> ReporteTopClientes(string rutaEmpresa, DateTime desde, DateTime hasta, int top = 10)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT TOP {top}
  CAST(c.CCODIGOCLIENTE AS VARCHAR(30)) + @sep +
  CAST(c.CRAZONSOCIAL AS VARCHAR(120)) + @sep +
  CAST(COUNT(d.CIDDOCUMENTO) AS VARCHAR(10)) + @sep +
  CAST(ISNULL(SUM(d.CTOTAL),0) AS VARCHAR(30))
FROM admDocumentos d
INNER JOIN admClientes c ON d.CIDCLIENTEPROVEEDOR = c.CIDCLIENTEPROVEEDOR
WHERE d.CIDCONCEPTODOCUMENTO IN (
  SELECT CIDCONCEPTODOCUMENTO FROM admConceptos WHERE CTIPODOCUMENTO = 4
)
AND d.CCANCELADO = 0
AND d.CFECHA BETWEEN '{desde:yyyy-MM-dd}' AND '{hasta:yyyy-MM-dd}'
GROUP BY c.CCODIGOCLIENTE, c.CRAZONSOCIAL
ORDER BY SUM(d.CTOTAL) DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 4) continue;
                    int.TryParse(p[2], out int numDocs);
                    double.TryParse(p[3], out double total);
                    lista.Add(new Dictionary<string, object>
                    {
                        ["codigo"] = p[0].Trim(),
                        ["razon_social"] = p[1].Trim(),
                        ["documentos"] = numDocs,
                        ["total"] = total
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en reporte top clientes");
            }
            return lista;
        }

        /// <summary>
        /// Top N productos más vendidos en un periodo.
        /// </summary>
        public List<Dictionary<string, object>> ReporteTopProductos(string rutaEmpresa, DateTime desde, DateTime hasta, int top = 10)
        {
            var lista = new List<Dictionary<string, object>>();
            try
            {
                string bdEmpresa = Path.GetFileName(rutaEmpresa.TrimEnd('\\'));
                string sql = $@"SET NOCOUNT ON; DECLARE @sep VARCHAR(5) = '|'; DECLARE @end VARCHAR(5) = '~END~';
SELECT TOP {top}
  CAST(p.CCODIGOPRODUCTO AS VARCHAR(30)) + @sep +
  CAST(p.CNOMBREPRODUCTO AS VARCHAR(120)) + @sep +
  CAST(SUM(m.CUNIDADES) AS VARCHAR(30)) + @sep +
  CAST(ISNULL(SUM(m.CTOTAL),0) AS VARCHAR(30))
FROM admMovimientos m
INNER JOIN admDocumentos d ON m.CIDDOCUMENTO = d.CIDDOCUMENTO
INNER JOIN admProductos p ON m.CIDPRODUCTO = p.CIDPRODUCTO
WHERE d.CIDCONCEPTODOCUMENTO IN (
  SELECT CIDCONCEPTODOCUMENTO FROM admConceptos WHERE CTIPODOCUMENTO = 4
)
AND d.CCANCELADO = 0
AND d.CFECHA BETWEEN '{desde:yyyy-MM-dd}' AND '{hasta:yyyy-MM-dd}'
GROUP BY p.CCODIGOPRODUCTO, p.CNOMBREPRODUCTO
ORDER BY SUM(m.CTOTAL) DESC;
PRINT @end;";

                string resultado = EjecutarSqlCmd(_instanceSql, _sqlUser, _sqlPassword, bdEmpresa, sql);
                foreach (var linea in resultado.Split('\n'))
                {
                    var l = linea.Trim();
                    if (string.IsNullOrEmpty(l) || l.Contains("~END~") || l.StartsWith("Changed") || l.StartsWith("(0 rows")) continue;
                    var p = l.Split('|');
                    if (p.Length < 4) continue;
                    double.TryParse(p[2], out double unidades);
                    double.TryParse(p[3], out double total);
                    lista.Add(new Dictionary<string, object>
                    {
                        ["codigo"] = p[0].Trim(),
                        ["nombre"] = p[1].Trim(),
                        ["unidades"] = unidades,
                        ["total"] = total
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en reporte top productos");
            }
            return lista;
        }

        // =====================================================================
        // ============ WEBHOOKS ================================================
        // =====================================================================

        public void RegistrarWebhook(string evento, string url)
        {
            lock (_webhooksLock)
            {
                _webhooks.RemoveAll(w => w.evento == evento && w.url == url);
                _webhooks.Add((evento, url));
                _logger.LogInformation($"Webhook registrado: evento={evento} url={url}");
            }
        }

        public List<(string evento, string url)> ListarWebhooks()
        {
            lock (_webhooksLock)
            {
                return new List<(string, string)>(_webhooks);
            }
        }

        /// <summary>
        /// Emite un webhook a todas las URLs registradas para ese evento.
        /// Se ejecuta de forma asíncrona para no bloquear la operación principal.
        /// </summary>
        public void EmitirWebhook(string evento, object payload)
        {
            List<(string evento, string url)> destinos;
            lock (_webhooksLock)
            {
                destinos = _webhooks.Where(w => w.evento == evento || w.evento == "*").ToList();
            }
            if (destinos.Count == 0) return;

            var json = System.Text.Json.JsonSerializer.Serialize(new {
                evento,
                timestamp = DateTime.UtcNow,
                payload
            });

            foreach (var d in destinos)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var resp = await http.PostAsync(d.url, content);
                        _logger.LogInformation($"Webhook enviado: evento={evento} url={d.url} status={resp.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Error enviando a {d.url}: {ex.Message}");
                    }
                });
            }
        }
    }
}
