using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class Login
    {
        //Este irequest definir de que tipo va ser, en este caso tipo usuario
        //la clase usuario viene de dominio y proviene, esta hereando del IdentityCore
        public class Ejecuta : IRequest<UsuarioData>{
            public string Email { get; set; }
            
            public string Password { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            public EjecutaValidacion(){
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            public readonly UserManager<Usuario> _userManager;

            public readonly SignInManager<Usuario> _signInManager;

            public readonly IJwtGenerador _jwtGenerador;

            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signManager, IJwtGenerador jwtGenerador){
                _userManager = userManager;
                _signInManager = signManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByEmailAsync(request.Email);
                if(usuario == null){
                    throw new ManejadorException(HttpStatusCode.Unauthorized);
                }

                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);

                //Sacar lista de roles
                var resulRoles = await _userManager.GetRolesAsync(usuario);
                //para transformar de IList(resulRoles) a List
                var listaRoles = new List<string>(resulRoles);

                if(resultado.Succeeded){
                    return new UsuarioData() {
                        //NombreCompleto = usuario.NombreCompleto,
                        Nombre = usuario.Nombre,
                        Apellidos = usuario.Apellidos,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                        UserName = usuario.UserName,
                        Email = usuario.Email,
                        Imagen = null
                    };
                }

                throw new ManejadorException(HttpStatusCode.Unauthorized);
            }
        }
    }
}