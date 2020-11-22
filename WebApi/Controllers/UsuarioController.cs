using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{   
    [AllowAnonymous]
    public class UsuarioController : MiControllerBase
    {
        //EndPoint => http://localhost:5000/api/Usuario/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioData>> Login(Login.Ejecuta data){
            return await Mediator.Send(data);
        }

        //EndPoint => http://localhost:5000/api/Usuario/registrar
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioData>> RegistrarUsuario(Registrar.Ejecuta data){
            return await Mediator.Send(data);
        }

        //EndPoint => http://localhost:5000/api/Usuario
        [HttpGet]
        public async Task<ActionResult<UsuarioData>> DevolverUsuarioLogueado(){
            return await Mediator.Send(new UsuarioActual.Ejecuta());
        }

        [HttpPut]
        public async Task<ActionResult<UsuarioData>> ActualizarUsuario(UsuarioActualiza.Ejecuta data){
            return await Mediator.Send(data);
        }
    }
}