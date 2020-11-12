using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    //IdentityUser es la clase la cual estoy heredando para usuario
    //esta clase contiene las propiedades que core identity va a manejar
    //de la persona que va a ingresar dentro de mi aplicacion 
    //Propieades como email password telefono etc
    public class Usuario : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}