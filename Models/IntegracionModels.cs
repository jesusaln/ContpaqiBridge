using System.Collections.Generic;

namespace ContpaqiBridge.Models
{
    public class IntegracionRequest
    {
        public string RutaEmpresa { get; set; } = "";
        public ClienteRequest Cliente { get; set; } = new();
        public IntegratedProductoRequest Producto { get; set; } = new();
        public IntegratedFacturaRequest Factura { get; set; } = new();
    }

    public class IntegratedProductoRequest
    {
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public double Precio { get; set; }
        public string? ClaveSAT { get; set; }
        public string UnidadMedida { get; set; } = "H87";
    }

    public class IntegratedFacturaRequest
    {
        public string CodigoConcepto { get; set; } = "";
        public string? PassCSD { get; set; }
        public double Cantidad { get; set; } = 1;
        public string? UsoCFDI { get; set; }
        public string? FormaPago { get; set; }
        public string? MetodoPago { get; set; }
    }

    public class IntegracionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public int IdDocumento { get; set; }
    }
}
