using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    //El Proyecto Aplicacion se va a encargar de manejar la reglas negocio, la logica de negocio intermedia
    public class Consulta
    {
        //Esta clase representa la lista de elementos que va a retornar desde la db

        //Lo que nos va a devolver es una lista de Objetos de tipo IRequest envolviendo una lista de Clase Curso
        public class ListaCursos : IRequest<List<CursoDto>>{ //IRequest clase es propio de la libreia MediaTr Injection
        //<List<Curso>>Indica que lo que devolvera sera un valor de tipo List<Curso> cuando finalize la transaccion.

        }

        //Y esta clase que es la que va a manejar la logica propia de la operacion
        //IRequestHandler se le pasa dos parametros 
        //1 Que tipo de data quieres que devuelva el manejador
        //2 Indicarle el formato en que lo va a devolver
        public class Manejador : IRequestHandler<ListaCursos, List<CursoDto>>{//Esto Proviene de la libreria MediaTr
        //La clase IRequestHandler lo que hace es procesar esos parametros, dentro de esta clase se invoca a las instancias de la base de datos con Entity Framework.
            public CursosOnlineContext _context { get; set; }

            public IMapper _mapper {get; set;}

            //Se creara una instancia de CursosOnlineContext que representa la DB y la instancia a EntityFramework
            //CursosOnlineContext proviene desde el proyecto Persistencia
            public Manejador(CursosOnlineContext context, IMapper mapper){
                _context = context;//Entonces ya tengo inyectado la propiedad _context como objeto dentro del constructor de esta clase manejador
                _mapper = mapper;
            }

            //Metodo que proviene de la interfaz IRequestHandler
            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                //Normalmente las transaciones en los servidores son de ida y vuelta por eso se trabaja con procesos asincronos 
                //De esta forma podemos devolver la data de los cursos y de los instructores relacionado a este curso
                var cursos =  await _context.Curso
                                .Include(x => x.ComentarioLista)
                                .Include(x => x.PrecioPromocion)
                                .Include(x => x.InstructoresLink)//Incluye a InstructorLink proviene de Dominio/Curso, esto representa la tabla CursoInstructor
                                                                //Lo que acabo de hacer es vincular Curso con CursoInstructor
                                .ThenInclude(x => x.Instructor)//este meto me pregunta a quien quieres incluiir, incluire a  Instructor
                                .ToListAsync();
                //Este resultado(cursos) es de formato Entity Core y necesitaba pasarlo a un
                //formato dto

                //Metodo Map(de la libreria AutorMapper) pedira 2 parametros
                //1 Tipo de dato origen
                //2 tipo de datos destino(en lo que quieres que se convierta)
                //en este caso dto, y luego la data que convertiras en este caso cursos 
                var cursosDto = _mapper.Map<List<Curso>, List<CursoDto>>(cursos);
                return cursosDto;
            }
        }
    }
}