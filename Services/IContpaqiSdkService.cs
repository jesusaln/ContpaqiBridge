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
    }
}
