using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

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

            private readonly CursosOnlineContext _context;

            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signManager, IJwtGenerador jwtGenerador, CursosOnlineContext context){
                _userManager = userManager;
                _signInManager = signManager;
                _jwtGenerador = jwtGenerador;
                _context = context;
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


                //
                var imagenPerfil = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();
                if(resultado.Succeeded){
                    if(imagenPerfil != null){
                        var imagenCliente = new ImagenGeneral{
                            Data = Convert.ToBase64String(imagenPerfil.Contenido),
                            Extension = imagenPerfil.Extension,
                            Nombre = imagenPerfil.Nombre
                        };

                        return new UsuarioData() {
                            //NombreCompleto = usuario.NombreCompleto,
                            Nombre = usuario.Nombre,
                            Apellidos = usuario.Apellidos,
                            Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                            UserName = usuario.UserName,
                            Email = usuario.Email,
                            ImagenPerfil = imagenCliente
                        };
                    }else{
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
                }
                throw new ManejadorException(HttpStatusCode.Unauthorized);
            }
        }
    }
}