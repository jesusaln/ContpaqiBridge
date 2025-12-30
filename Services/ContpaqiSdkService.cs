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

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fEditarDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fCancelarModificacionDocumento();

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInsertarMovimiento();

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fSetDatoMovimiento(string aCampo, string aValor);

        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fGuardaMovimiento();

        // ============ Funciones para Clientes/Proveedores ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaCteProv(ref int aIdCteProv, ref tCteProv aCteProv);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaCteProv(string aCodCteProv);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fLeeDatoCteProv(string aCampo, StringBuilder aValor, int aLen);

        // ============ Funciones para Productos ============
        
        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fAltaProducto(ref int aIdProducto, ref tProducto aProducto);

        [DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int fBuscaProducto(string aCodProducto);

        // Funciones bajo nivel para productos
        [DllImport("MGWServicios.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int fInsertaProducto();

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

            // Leer DirectorioBase desde el Registro de Windows (como indica el ejemplo oficial)
            _directorioBase = ObtenerDirectorioBaseDelRegistro();
            
            if (!string.IsNullOrEmpty(_directorioBase))
            {
                _logger.LogInformation($"DirectorioBase obtenido del Registro: {_directorioBase}");
                
                // Configurar el entorno ANTES de cualquier P/Invoke
                SetDllDirectory(_directorioBase);
                SetCurrentDirectory(_directorioBase);
                System.IO.Directory.SetCurrentDirectory(_directorioBase);
                
                // Agregar al PATH
                string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (!currentPath.Contains(_directorioBase))
                {
                    Environment.SetEnvironmentVariable("PATH", _directorioBase + ";" + currentPath);
                }
            }
            else
            {
                _logger.LogError("No se pudo obtener DirectorioBase del Registro de Windows.");
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
        public (bool exito, string mensaje, int idDocumento) CrearFactura(
            string rutaEmpresa,
            string codigoConcepto,
            string codigoCliente,
            List<(string codigo, double cantidad, double precio)> productos)
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

                // 3. Obtener siguiente folio
                StringBuilder serieStr = new StringBuilder(12);
                double folioNum = 0;
                int resultFolio = fSiguienteFolio(codigoConcepto, serieStr, ref folioNum);
                if (resultFolio != 0)
                {
                    CerrarEmpresa();
                    return (false, $"Error al obtener folio: {GetUltimoError(resultFolio)}", 0);
                }
                string serie = serieStr.ToString().Trim();
                _logger.LogInformation($"Folio obtenido: Serie={serie}, Folio={folioNum}");

                // 4. Buscar cliente y obtener sus datos
                _logger.LogInformation($"Buscando cliente: {codigoCliente}");
                int resBuscaCte = fBuscaCteProv(codigoCliente);
                if (resBuscaCte != 0)
                {
                    string errCte = GetUltimoError(resBuscaCte);
                    _logger.LogError($"Cliente no encontrado: {codigoCliente} - {errCte}");
                    CerrarEmpresa();
                    return (false, $"Cliente no existe: {codigoCliente} ({errCte})", 0);
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

                // 5. BAJO NIVEL: Insertar documento (cabecera)
                _logger.LogInformation("Llamando a fInsertarDocumento()...");
                int resInsertarDoc = fInsertarDocumento();
                if (resInsertarDoc != 0)
                {
                    string err = GetUltimoError(resInsertarDoc);
                    _logger.LogError($"fInsertarDocumento falló: {resInsertarDoc} - {err}");
                    CerrarEmpresa();
                    return (false, $"Error al insertar documento: {err}", 0);
                }

                // ============================================================
                // IMPORTANTE: CONTPAQi pide primero el CÓDIGO DEL CLIENTE, luego el concepto.
                // Una vez ingresado el cliente, el sistema auto-completa los demás datos
                // (razón social, RFC, etc.) desde el catálogo de clientes.
                // EL ORDEN ES CRÍTICO - NO MODIFICAR.
                // ============================================================
                
                // 6. Setear campos en ORDEN ESPECÍFICO (como lo pide CONTPAQi manualmente)
                string fechaHoy = DateTime.Now.ToString("MM/dd/yyyy"); // Formato americano para SDK
                
                // Lista ordenada: PRIMERO concepto, LUEGO cliente (así lo pide CONTPAQi)
                var camposDocumento = new List<(string campo, string valor)>
                {
                    ("CIDCONCEPTODOCUMENTO", codigoConcepto),    // 1. PRIMERO el concepto del documento
                    ("CCODIGOCLIENTE", codigoCliente),           // 2. LUEGO el cliente (ej: "AANL") - CONTPAQi autocompleta lo demás
                    ("CSERIEDOCUMENTO", serie),
                    ("CFOLIO", folioNum.ToString("F0")),
                    ("CFECHA", fechaHoy),
                    ("CIDMONEDA", "1"),                          // 1 = MXN
                    ("CTIPOCAMBIO", "1.00"),
                    ("CREFERENCIA", "API Bridge"),
                    ("COBSERVACIONES", $"Generado via API {DateTime.Now:yyyy-MM-dd HH:mm}")
                };

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
                    return (false, $"Error al guardar cabecera: {err}", 0);
                }
                _logger.LogInformation("Cabecera del documento guardada exitosamente.");

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
                        return (false, $"Producto no existe: {producto.codigo} ({errBusca})", 0);
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
                    return (false, "No se agregaron productos válidos a la factura.", 0);
                }
                else if (movimientosAgregados == 0)
                {
                    _logger.LogWarning("Factura creada sin movimientos (solo cabecera para testing).");
                }

                // 8. Cerrar empresa
                CerrarEmpresa();
                _logger.LogInformation($"Factura creada exitosamente. Serie: {serie}, Folio: {folioNum}, Movimientos: {movimientosAgregados}");
                return (true, $"Factura creada exitosamente. Serie: {serie}, Folio: {folioNum}", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear factura");
                try { fCancelarModificacionDocumento(); } catch { }
                CerrarEmpresa();
                return (false, $"Excepción: {ex.Message}", 0);
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
            string pais = "México")
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
                int existe = fBuscaCteProv(codigo);
                if (existe == 0)
                {
                    CerrarEmpresa();
                    return (true, "El cliente ya existe en el catálogo", 0);
                }

                // 4. Crear estructura del cliente
                tCteProv cliente = new tCteProv
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
                int idCliente = 0;
                int result = fAltaCteProv(ref idCliente, ref cliente);
                
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
                    CerrarEmpresa();
                    return (true, "El producto ya existe en el catálogo", 0);
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

                // Intentar setear unidad de medida usando H87 (Código SAT)
                if (tipoProducto != 3) // Si es un producto físico
                {
                    _logger.LogInformation($"fSetDatoProducto('CCODIGOUNIDADBASE', '{unidadMedida}')");
                    int resultUnidad = fSetDatoProducto("CCODIGOUNIDADBASE", unidadMedida);
                    
                    if (resultUnidad != 0)
                    {
                        string errorStr = GetUltimoError(resultUnidad);
                        _logger.LogWarning($"Unidad '{unidadMedida}' rechazada ({resultUnidad} - {errorStr}). Intentando con '6' (ID Interno)...");
                        
                        // Fallback: Intentar con el ID interno como string ("6" = Pieza)
                        resultUnidad = fSetDatoProducto("CCODIGOUNIDADBASE", "6");
                        
                        if (resultUnidad != 0)
                        {
                            _logger.LogError("No se pudo asignar la unidad de medida. El producto podría fallar al facturar.");
                        }
                    }
                    
                    if (resultUnidad == 0)
                    {
                        fSetDatoProducto("CCODIGOUNIDADNOCONVERTIBLE", unidadMedida == "H87" ? "H87" : "6");
                    }
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
