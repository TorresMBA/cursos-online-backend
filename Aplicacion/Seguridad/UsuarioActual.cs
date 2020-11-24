using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta : IRequest<UsuarioData>{ }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            public readonly UserManager<Usuario> _userManager;

            public readonly IJwtGenerador _jwtGenerador;

            public readonly IUsuarioSesion _userSesion;

            private readonly CursosOnlineContext _context;

            public Manejador(UserManager<Usuario> usuarioManager, IJwtGenerador jwtGenerador, IUsuarioSesion userSesion, CursosOnlineContext context){
                _userManager = usuarioManager;
                _jwtGenerador = jwtGenerador;
                _userSesion = userSesion;
                _context = context;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userSesion.ObtenerUsuarioSesion());
                
                //Sacar lista de roles
                var resulRoles = await _userManager.GetRolesAsync(user);

                //para transformar de IList(resulRoles) a List
                var listaRoles = new List<string>(resulRoles);

                //Validaa si el usuario tiene una imagen de perfil
                var imagenPerfil = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(user.Id)).FirstOrDefaultAsync();     
                if(imagenPerfil != null){
                    var imagenCliente = new ImagenGeneral{
                        Data = Convert.ToBase64String(imagenPerfil.Contenido),
                        Extension = imagenPerfil.Extension,
                        Nombre = imagenPerfil.Nombre
                    };

                    return new UsuarioData{
                        Nombre = user.Nombre,
                        Apellidos = user.Apellidos,
                        UserName = user.UserName,
                        Token = _jwtGenerador.CrearToken(user, listaRoles),
                        Email = user.Email,
                        ImagenPerfil = imagenCliente
                    };
                }else{
                    return new UsuarioData{
                        //NombreCompleto = user.NombreCompleto,
                        Nombre = user.Nombre,
                        Apellidos = user.Apellidos,
                        UserName = user.UserName,
                        Token = _jwtGenerador.CrearToken(user, listaRoles),
                        Email = user.Email, 
                    };    
                }
                
            }
        }
    }
}