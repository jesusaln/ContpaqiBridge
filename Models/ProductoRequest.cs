namespace ContpaqiBridge.Models
{
    public class ProductoRequest
    {
        /// <summary>
        /// Ruta de la empresa
        /// </summary>
        public string RutaEmpresa { get; set; } = "";

        /// <summary>
        /// Código único del producto (ej: "PROD001")
        /// </summary>
        public string Codigo { get; set; } = "";

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; } = "";

        /// <summary>
        /// Descripción del producto
        /// </summary>
        public string Descripcion { get; set; } = "";

        /// <summary>
        /// Precio del producto
        /// </summary>
        public double Precio { get; set; }

        /// <summary>
        /// Tipo: 1=Producto, 2=Paquete, 3=Servicio
        /// </summary>
        public int TipoProducto { get; set; } = 1;

        /// <summary>
        /// Código de la unidad de medida (Ej: P para Pieza en esta empresa)
        /// </summary>
        public string UnidadMedida { get; set; } = "P";

        /// <summary>
        /// Clave SAT para facturación electrónica
        /// </summary>
        public string ClaveSAT { get; set; } = "";
    }
}
