namespace Aplicacion.Seguridad
{
    //Esta clase representara la data que le devolvere al cliente
    public class UsuarioData
    {
        public string NombreCompleto { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Imagen { get; set; }
    }
}