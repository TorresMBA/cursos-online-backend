using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Identity;

namespace Persistencia
{
    public class DataPrueba
    {
        public static async Task InsertarData(CursosOnlineContext context, UserManager<Usuario> usuarioManager){
            //Lo que hace esto es validar que exista algun usuario dentro del CoreIdentity, 
            //dentro de la base de datos
            if(!usuarioManager.Users.Any()){
                var usuario = new Usuario(){ 
                    //NombreCompleto = "Brian Torres",
                    Nombre = "Vaxi",
                    Apellidos = "Drezz",
                    UserName = "vaxi.drezz",
                    Email = "vaxi.drezz@gmail.com"
                };
                await usuarioManager.CreateAsync(usuario, "brayantm123xD$");
            }
        }
    }
}