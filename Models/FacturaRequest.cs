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
        /// Razón social del cliente. Opcional: si no existe en CONTPAQi y viene aquí, se crea automáticamente.
        /// </summary>
        public string ClienteRazonSocial { get; set; } = "";

        /// <summary>
        /// RFC del cliente. Opcional: si no existe en CONTPAQi y viene aquí, se usa al crearlo.
        /// </summary>
        public string ClienteRFC { get; set; } = "";

        /// <summary>
        /// Régimen fiscal del cliente (ej: "601", "626"). Opcional: si no existe y viene aquí, se asigna al crearlo.
        /// </summary>
        public string ClienteRegimenFiscal { get; set; } = "";

        /// <summary>
        /// Ruta de la empresa (ej: "C:\Compac\Empresas\adJESUS_LOPEZ_NORIEGA")
        /// </summary>
        public string RutaEmpresa { get; set; } = "";

        /// <summary>
        /// Lista de productos/servicios a facturar
        /// </summary>
        public List<ProductoFactura> Productos { get; set; } = new();

        /// <summary>
        /// Uso CFDI (ej: "G01"). Default: "G01"
        /// </summary>
        public string UsoCFDI { get; set; } = "G01";

        /// <summary>
        /// Forma de pago (01, 03, 99). Default: "99"
        /// </summary>
        public string FormaPago { get; set; } = "99";

        /// <summary>
        /// Método de pago (PUE, PPD). Default: "PUE"
        /// </summary>
        public string MetodoPago { get; set; } = "PUE";
    }

    public class ProductoFactura
    {
        /// <summary>
        /// Código del producto en CONTPAQi
        /// </summary>
        public string Codigo { get; set; } = "";

        /// <summary>
        /// Nombre del producto. Opcional: si no existe en CONTPAQi y viene aquí, se crea automáticamente.
        /// </summary>
        public string Nombre { get; set; } = "";

        /// <summary>
        /// Cantidad a facturar
        /// </summary>
        public double Cantidad { get; set; }

        /// <summary>
        /// Precio unitario (si es 0, usará el precio del catálogo)
        /// </summary>
        public double Precio { get; set; }

        /// <summary>
        /// Unidad de medida (clave SAT). Opcional, usado al crear el producto si no existe.
        /// </summary>
        public string UnidadMedida { get; set; } = "";

        /// <summary>
        /// Clave SAT del producto. Opcional, usado al crear el producto si no existe.
        /// </summary>
        public string ClaveSAT { get; set; } = "";
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
