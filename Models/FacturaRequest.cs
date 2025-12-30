namespace ContpaqiBridge.Models
{
    public class FacturaRequest
    {
        /// <summary>
        /// Código del concepto (ej: "4CLIMAS")
        /// </summary>
        public string CodigoConcepto { get; set; } = "";

        /// <summary>
        /// Código del cliente
        /// </summary>
        public string CodigoCliente { get; set; } = "";

        /// <summary>
        /// Ruta de la empresa (ej: "C:\Compac\Empresas\adJESUS_LOPEZ_NORIEGA")
        /// </summary>
        public string RutaEmpresa { get; set; } = "";

        /// <summary>
        /// Lista de productos/servicios a facturar
        /// </summary>
        public List<ProductoFactura> Productos { get; set; } = new();
    }

    public class ProductoFactura
    {
        /// <summary>
        /// Código del producto en CONTPAQi
        /// </summary>
        public string Codigo { get; set; } = "";

        /// <summary>
        /// Cantidad a facturar
        /// </summary>
        public double Cantidad { get; set; }

        /// <summary>
        /// Precio unitario (si es 0, usará el precio del catálogo)
        /// </summary>
        public double Precio { get; set; }
    }

    public class TimbrarRequest
    {
        public string RutaEmpresa { get; set; } = "";
        public string CodigoConcepto { get; set; } = "";
        public string Serie { get; set; } = "";
        public double Folio { get; set; }
        public string PassCSD { get; set; } = "";
    }
}
