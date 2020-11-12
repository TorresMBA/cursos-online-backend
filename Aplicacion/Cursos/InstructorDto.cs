using System;

namespace Aplicacion.Cursos
{
    public class InstructorDto
    {
        public Guid InstructorId { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string Grado { get; set; }

        public byte[] FotoPerfil { get; set; }

        public PrecioDto Precio { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public System.Collections.Generic.ICollection<ComentarioDto> Comentarios { get; set; }
    }
}