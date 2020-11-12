using System.Linq;
using Aplicacion.Cursos;
using AutoMapper;
using Dominio;

namespace Aplicacion
{
    //Esta clase va a manejar los mapeos entre las clases
    //Entity Core como Curso y el mapeo como las clases CursoDto
    public class MappingProfile : Profile
    {
        public MappingProfile(){
            //Se pone la clase de entity core y la clase con la que se mapeara
            CreateMap<Curso, CursoDto>()
                //Esta linea me permite mapear dentro de Dto(x => x.Instructores)
                //quiero mappear toda la propiedad Instructores del CursoDto desde
                //(y => y.MapFrom(z => z.InstructoresLink.Select(a => a.Instructor)
                //este origin proviene desde entity core usando sentencias linq
                .ForMember(x => x.Instructores, y => y.MapFrom(
                    z => z.InstructoresLink.Select(a => a.Instructor).ToList()
                ))//segundo parametro es quien llenara de donde va a prevenir
                .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista))
                .ForMember(x => x.Precio, y => y.MapFrom(z => z.PrecioPromocion));
            CreateMap<CursoInstructor, CursoInstructorDto>();
            CreateMap<Instructor, InstructorDto>();
            CreateMap<Comentario, ComentarioDto>();
            CreateMap<Precio, PrecioDto>();
        }
    }
}