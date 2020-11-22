using System;

namespace Dominio
{
    public class Documento
    {
        public Guid DocumentoId { get; set; }

        //Este campo nos servira para saber el documento a que entidad pertenece, a que perfil
        //de usuario pertenece, esta es la propiedad que me va a vincular la imagen con el perfil de usuario
        public Guid ObjetoReferencia { get; set; }

        public string Nombre { get; set; }

        public string Extension { get; set; }

        public byte[] Contenido { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}