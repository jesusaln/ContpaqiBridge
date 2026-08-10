using System;

namespace ContpaqiBridge.Services
{
    public interface IContpaqiSdkService : IDisposable
    {
        bool InicializarSDK();
        bool AbrirEmpresa(string rutaEmpresa);
        int CrearDocumento(string codigoConcepto, string codigoCliente, DateTime fecha, double total);
        void CerrarEmpresa();

        string GetUltimoError(int errorCode = 0);
        void SetUsuario(string usuario, string clave);
        int GetLastInitResult();
        string ListarUnidades();

        (bool exito, string mensaje, int idCliente) CrearCliente(string rutaEmpresa, string codigo, string razonSocial, string rfc, string email = "", string calle = "", string colonia = "", string codigoPostal = "", string ciudad = "", string estado = "", string pais = "México", string regimenFiscal = "", string usoCFDI = "", string formaPago = "");
        (bool exito, string mensaje, int idProducto) CrearProducto(string rutaEmpresa, string codigo, string nombre, string descripcion = "", double precio = 0, int tipoProducto = 1, string unidadMedida = "H87", string claveSAT = "");
        (bool exito, string mensaje, int idDocumento, string serie, double folio) CrearFactura(string rutaEmpresa, string codigoConcepto, string codigoCliente, List<(string codigo, double cantidad, double precio, string nombre, string unidadMedida, string claveSAT)> productos, string usoCFDI = "G01", string formaPago = "99", string metodoPago = "PUE", string clienteRazonSocial = "", string clienteRFC = "", string clienteRegimenFiscal = "");
        (bool exito, string mensaje) TimbrarFactura(string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD);
        (bool exito, string mensaje, string xml) ObtenerXml(string rutaEmpresa, string codigoConcepto, string serie, double folio, int formato = 0);
        List<(string severidad, string categoria, string mensaje)> ValidarParaTimbrado(string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD);
        List<(string codigo, string nombre)> ListarConceptos(string rutaEmpresa);
        List<Dictionary<string, object>> ListarUltimosDocumentos(string rutaEmpresa, int cantidad = 10);
        (bool exito, string mensaje, string acuse) CancelarDocumento(string rutaEmpresa, string codigoConcepto, string serie, double folio, string motivoCancelacion, string passCSD, string uuidSustitucion = "");
        (bool exito, string mensaje) CancelarDocumentoAdministrativamente(string rutaEmpresa, string codigoConcepto, string serie, double folio);
        (bool exito, string mensaje) SaldarDocumento(string rutaEmpresa, string codConceptoPagar, string seriePagar, double folioPagar, string codConceptoPago, string seriePago, double folioPago, double importe, int idMoneda, string fecha);
        (bool exito, string mensaje, string uuid) ObtenerUuid(string rutaEmpresa, string codigoConcepto, string serie, double folio);
        (bool exito, string mensaje, string valor) ObtenerDatoCfdi(string rutaEmpresa, string password, string codigoConcepto, string serie, double folio, int dato);

        // ============ SYNC: Catálogos hacia Laravel (lectura desde SQL de CONTPAQi) ============
        List<Dictionary<string, object>> ListarClientesTodos(string rutaEmpresa, int limite = 500);
        List<Dictionary<string, object>> ListarProductosTodos(string rutaEmpresa, int limite = 500);
        List<Dictionary<string, object>> ListarClientesModificados(string rutaEmpresa, DateTime desde, int limite = 500);
        List<Dictionary<string, object>> ListarProductosModificados(string rutaEmpresa, DateTime desde, int limite = 500);
        List<Dictionary<string, object>> ListarDocumentosModificados(string rutaEmpresa, DateTime desde, int limite = 500);
        Dictionary<string, object>? ObtenerClientePorCodigo(string rutaEmpresa, string codigo);
        Dictionary<string, object>? ObtenerProductoPorCodigo(string rutaEmpresa, string codigo);

        // ============ Reportes ============
        List<Dictionary<string, object>> ReporteVentasPorPeriodo(string rutaEmpresa, DateTime desde, DateTime hasta);
        List<Dictionary<string, object>> ReporteTopClientes(string rutaEmpresa, DateTime desde, DateTime hasta, int top = 10);
        List<Dictionary<string, object>> ReporteTopProductos(string rutaEmpresa, DateTime desde, DateTime hasta, int top = 10);

        // ============ Webhooks ============
        void RegistrarWebhook(string evento, string url);
        void EmitirWebhook(string evento, object payload);
        List<(string evento, string url)> ListarWebhooks();
    }
}