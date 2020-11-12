using System;
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
    public class Registrar
    {
        public class Ejecuta : IRequest<UsuarioData>{
            public string Nombre { get; set; }

            public string Apellidos { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string Username { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            public EjecutaValidacion(){
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Username).Must(x => !x.Contains(" ")).WithMessage("El User Name no debe Contener Espacios");
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {   
            private readonly CursosOnlineContext _context;

            private readonly UserManager<Usuario> _userManager;

            private readonly IJwtGenerador _jwtGenerador;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador){
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var existe = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();
                if(existe){
                    throw new ManejadorException(HttpStatusCode.BadRequest, new { mensaje = "Existe un usuario registrado con ese Email" });
                }

                var existeUserName = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();
                if(existe){
                    throw new ManejadorException(HttpStatusCode.BadRequest, new { mensaje = "Existe un usuario con este username" });
                }

                var usuario = new Usuario{
                    //NombreCompleto =  request.Nombre + " " + request.Apellidos,
                    Nombre = request.Nombre,
                    Apellidos = request.Apellidos,
                    Email = request.Email,   
                    UserName = request.Username                 
                };

                var resultado = await _userManager.CreateAsync(usuario, request.Password);
                if(resultado.Succeeded){//Si el resultado es exitoso
                    return new UsuarioData{
                        //NombreCompleto = usuario.NombreCompleto,
                        Nombre = usuario.Nombre,
                        Apellidos = usuario.Apellidos,
                        Token = _jwtGenerador.CrearToken(usuario, null),//Se pasa un null porque recien se esta creando el usuario y no tiene roles
                        Email = usuario.Email,
                        UserName = usuario.UserName
                    };
                }
                throw new Exception("No se pudo agregar el nuevo usuario");
            }
        }
    }
}