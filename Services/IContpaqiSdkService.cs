using System;

namespace ContpaqiBridge.Services
{
    public interface IContpaqiSdkService : IDisposable
    {
        bool InicializarSDK();
        bool AbrirEmpresa(string rutaEmpresa);
        int CrearDocumento(string codigoConcepto, string codigoCliente, DateTime fecha, double total);
        void CerrarEmpresa();
        
        // Helper to check last error
        string GetUltimoError(int errorCode = 0);
        
        // Set user credentials for the session
        void SetUsuario(string usuario, string clave);
        int GetLastInitResult();
        string ListarUnidades();
                (bool exito, string mensaje, int idCliente) CrearCliente(string rutaEmpresa, string codigo, string razonSocial, string rfc, string email = "", string calle = "", string colonia = "", string codigoPostal = "", string ciudad = "", string estado = "", string pais = "México", string regimenFiscal = "", string usoCFDI = "", string formaPago = "");
        (bool exito, string mensaje, int idProducto) CrearProducto(string rutaEmpresa, string codigo, string nombre, string descripcion = "", double precio = 0, int tipoProducto = 1, string unidadMedida = "H87", string claveSAT = "");
        (bool exito, string mensaje, int idDocumento, string serie, double folio) CrearFactura(string rutaEmpresa, string codigoConcepto, string codigoCliente, List<(string codigo, double cantidad, double precio)> productos, string usoCFDI = "G01", string formaPago = "99", string metodoPago = "PUE");
        (bool exito, string mensaje) TimbrarFactura(string rutaEmpresa, string codigoConcepto, string serie, double folio, string passCSD);
        (bool exito, string mensaje, string xml) ObtenerXml(string rutaEmpresa, string codigoConcepto, string serie, double folio);
        List<(string codigo, string nombre)> ListarConceptos(string rutaEmpresa);
        List<(string concepto, string serie, double folio)> ListarUltimosDocumentos(string rutaEmpresa, int limite = 20);
    }
}
