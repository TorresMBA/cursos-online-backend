using System.Collections.Generic;
using Dominio;

namespace Aplicacion.Contratos
{   

    //El nombre de la carpeta Contrato tambien es conocido como Interfaces
    public interface IJwtGenerador
    {
         string CrearToken(Usuario usuario, List<string> roles);
    }
}