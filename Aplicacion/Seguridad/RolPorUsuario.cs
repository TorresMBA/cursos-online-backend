using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolPorUsuario
    {
        public class Ejecuta : IRequest<List<string>>{
            public string UserName { get; set; }    
        }

        public class EjecutaValida : AbstractValidator<Ejecuta>{

        }

        public class Manejador : IRequestHandler<Ejecuta, List<string>>
        {
            private  readonly UserManager<Usuario> _userManager;

            private readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager){
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<List<string>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if(user == null){
                    throw new ManejadorException(System.Net.HttpStatusCode.NotFound, new {mensaje="No existe el usuario"});
                }
                                    //Este metodo me devuelve uyna lista de string
                var resultad = await _userManager.GetRolesAsync(user);
                return new List<string>(resultad);//Se usa para tranformas de un IList a un List
            }
        }
    }
}