using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioRolEliminar
    {
        public class Ejecuta : IRequest{
            public string UserName { get; set; }

            public string RolNombre { get; set; }
        }

        public class EjecutaValida : AbstractValidator<Ejecuta>{
            
            public EjecutaValida(){
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.RolNombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager){
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByNameAsync(request.RolNombre);
                if(role == null){
                   throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje="No se encontro el rol"}); 
                }

                var user = await _userManager.FindByNameAsync(request.UserName);
                if(user == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje="No se encontro el Usuario"}); 
                }

                var resultador = await _userManager.RemoveFromRoleAsync(user, request.RolNombre);
                if(resultador.Succeeded){
                    return Unit.Value;
                }
                throw new Exception("No se pudo eliminar el rol del usuario"); 
            }
        }
    }
}