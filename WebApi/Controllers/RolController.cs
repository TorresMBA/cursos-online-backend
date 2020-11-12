using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class RolController : MiControllerBase
    {
        [HttpPost("Crear")]
        public async Task<ActionResult<Unit>> Crear(RolNuevo.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Unit>> Eliminar(RolEliminar.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpGet("Lista")]
        public async Task<ActionResult<List<IdentityRole>>> Lista(){
            return await Mediator.Send(new RolLista.Ejecuta());
        }

        [HttpPost("AgregarRolUsuario")]
        public async Task<ActionResult<Unit>> AgregarRol(UsuarioRolAgregar.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpDelete("EliminarRolUsuario")]
        public async Task<ActionResult<Unit>> EliminarRol(UsuarioRolEliminar.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<List<string>>> ListaRoles(string username){
            return await Mediator.Send(new RolPorUsuario.Ejecuta { UserName = username });
        }
    }
}