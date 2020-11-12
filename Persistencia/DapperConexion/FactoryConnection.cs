using System.Globalization;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {
        private IDbConnection _connnection;
        private readonly IOptions<ConexionConfiguracion> _config; // Como que extrae la cadena conexion del appsettings mediante la clase Startup con la creacopm deñ servicio

        public FactoryConnection(IOptions<ConexionConfiguracion> config){
            _config = config;
        }
        
        public void CloseConnection()
        {
            if(_connnection != null && _connnection.State == ConnectionState.Open){
                _connnection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            //Estoy evaluando que la cadena de conexion exista
            if(_connnection == null){
                _connnection = new SqlConnection(_config.Value.DefaultConnection);
            }

            //Luego de crear devo evaluar este abierta sino lo esta se va a abrir
            if(_connnection.State != ConnectionState.Open){
                _connnection.Open();
            }

            //y retornamos el objeto para poder realizar las transcciones
            return _connnection;
        }
    }
}