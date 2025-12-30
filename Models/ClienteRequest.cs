namespace ContpaqiBridge.Models
{
    public class ClienteRequest
    {
        /// <summary>
        /// Ruta de la empresa
        /// </summary>
        public string RutaEmpresa { get; set; } = "";

        /// <summary>
        /// Código único del cliente (ej: "CLI001")
        /// </summary>
        public string Codigo { get; set; } = "";

        /// <summary>
        /// Razón social del cliente
        /// </summary>
        public string RazonSocial { get; set; } = "";

        /// <summary>
        /// RFC del cliente
        /// </summary>
        public string RFC { get; set; } = "";

        /// <summary>
        /// Correo electrónico
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Calle
        /// </summary>
        public string Calle { get; set; } = "";

        /// <summary>
        /// Colonia
        /// </summary>
        public string Colonia { get; set; } = "";

        /// <summary>
        /// Código postal
        /// </summary>
        public string CodigoPostal { get; set; } = "";

        /// <summary>
        /// Ciudad
        /// </summary>
        public string Ciudad { get; set; } = "";

        /// <summary>
        /// Estado
        /// </summary>
        public string Estado { get; set; } = "";

        /// <summary>
        /// País (default: México)
        /// </summary>
        public string Pais { get; set; } = "México";
    }
}
