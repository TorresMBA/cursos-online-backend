using System.Security.Cryptography;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Aplicacion.Contratos;
using Dominio;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace Seguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario, List<string> roles)
        {
            //Indicarle la lista de los claims
            //Los Claims es la data del usuario que tu quieres compartir con el cliente
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
            };
            //Solamente agregar los roles como claims dentro del token
            //Validar si la lista de roles no es null
            if(roles != null){
                
                //Bucle que me permita crear por cada elemento de la Lista un claim
                foreach(var rol in roles){
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }
            }
            //

            //Crear las credenciales de acceso
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi Palabra secreta"));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //descripcion del token
            var tokenDescripcion = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                
                //Tiempo de vida del token
                Expires = DateTime.Now.AddDays(30),

                //Tipo de acceso a las credenciales
                SigningCredentials = credenciales
            };

            var tokenManejador = new JwtSecurityTokenHandler();
            var token = tokenManejador.CreateToken(tokenDescripcion);

            return tokenManejador.WriteToken(token);
        }
    }
}