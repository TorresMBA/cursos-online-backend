using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta : IRequest<UsuarioData>{}

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            public readonly UserManager<Usuario> _userManager;

            public readonly IJwtGenerador _jwtGenerador;

            public readonly IUsuarioSesion _userSesion;

            public Manejador(UserManager<Usuario> usuarioManager, IJwtGenerador jwtGenerador, IUsuarioSesion userSesion){
                _userManager = usuarioManager;
                _jwtGenerador = jwtGenerador;
                _userSesion = userSesion;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userSesion.ObtenerUsuarioSesion());
                
                 //Sacar lista de roles
                var resulRoles = await _userManager.GetRolesAsync(user);
                //para transformar de IList(resulRoles) a List
                var listaRoles = new List<string>(resulRoles);

                return new UsuarioData{
                    //NombreCompleto = user.NombreCompleto,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    UserName = user.UserName,
                    Token = _jwtGenerador.CrearToken(user, listaRoles),
                    Imagen = null,
                    Email = user.Email
                };
            }
        }
    }
}