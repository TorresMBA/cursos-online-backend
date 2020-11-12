using System;
using Dominio;

namespace Aplicacion.Cursos
{
    public class CursoInstructorDto
    {
        public Curso CursoId { get; set; }

        public Guid InstructorId { get; set; }

    }
}