using System;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class RolNuevo
    {
        public class Ejecuta : IRequest{
            public string Nombre { get; set; }
        }

        public class ValidaEjecuta : AbstractValidator<Ejecuta>{
            
            public ValidaEjecuta(){
                RuleFor(x => x.Nombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            public readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(RoleManager<IdentityRole> roleManeger){
                _roleManager = roleManeger;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                if (role != null){
                    throw new ManejadorException(System.Net.HttpStatusCode.BadRequest, new { mensaje = "Ya existe el rol"});
                }
                
                var resul = await _roleManager.CreateAsync(new IdentityRole { Name = request.Nombre });
                if(resul.Succeeded){
                     return Unit.Value;
                }
                throw new Exception("No se pudo guardar el rol");
            }
        }
    }
}