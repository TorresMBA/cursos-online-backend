using System.ComponentModel.DataAnnotations;
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
using System.Collections.Generic;

namespace Aplicacion.Seguridad
{
    public class UsuarioActualiza
    {
        public class Ejecuta : IRequest<UsuarioData>{
            public string Nombre { get; set; }

            public string Apellidos { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string UserName { get; set; }
        }

        public class EjecutaValidador : AbstractValidator<Ejecuta>{
            public EjecutaValidador(){
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>{

            private readonly CursosOnlineContext _context;

            private readonly UserManager<Usuario> _userManager;

            private readonly IJwtGenerador _jwtGnerador;

            //Objeto para poder encriptar la password
            private readonly IPasswordHasher<Usuario> _passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher){
                _context = context ;
                _userManager = userManager;
                _jwtGnerador = jwtGenerador;
                _passwordHasher = passwordHasher;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Logica para actualizar un usuario

                //Evaluar la existencia de un usuario
                var usuarioIden = await _userManager.FindByNameAsync(request.UserName);
                if(usuarioIden == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "No existe el usuario indicado"});
                }

                //Valida el email si fue utilizado por otro usuario del sistema
                var resultado = await _context.Users.Where(x => x.Email == request.Email &&  x.UserName != request.UserName).AnyAsync();
                if(resultado){
                    throw new ManejadorException(HttpStatusCode.InternalServerError, new {mensaje = "Este email pertenece a otro usuario"});
                }

                //Actualizando los datos del usuario
                usuarioIden.Nombre = request.Nombre;
                usuarioIden.Apellidos = request.Apellidos;
                usuarioIden.PasswordHash = _passwordHasher.HashPassword(usuarioIden, request.Password);
                usuarioIden.Email = request.Email;

                //Obtener lista de roles para este usuario que estamos evaluando
                var resultadoRoles = await _userManager.GetRolesAsync(usuarioIden);
                var roles = new List<string>(resultadoRoles);

                var resultadoUpdate = await _userManager.UpdateAsync(usuarioIden);
                if(resultadoUpdate.Succeeded){
                    return new UsuarioData(){
                       // NombreCompleto = usuarioIden.NombreCompleto,
                        Nombre = usuarioIden.Nombre,
                        Apellidos = usuarioIden.Apellidos,
                        UserName = usuarioIden.UserName,
                        Email = usuarioIden.Email,
                        Token = _jwtGnerador.CrearToken(usuarioIden, roles)
                    };
                }

                throw new Exception("No se pudo actualizar los cambios");
            }
        }
    }
}