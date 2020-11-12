using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta : IRequest{
            public Guid CursoId { get; set; }

            public string Titulo { get; set; }

            public string Descripcion { get; set; }

            public DateTime? FechaPublicacion { get; set; } // ? => Hace que permita nulos en la variable

            public List<Guid> ListaInstructor { get; set;}

            public decimal? Precio { get; set; }

            public decimal? PrecioPromocion { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{ //Este metodo proviene de la libreia
        //FluentValidation y este se la pasa la clase con la cual va a trabajar
            public EjecutaValidacion(){
                //Dentro del constructor de esta clase es donde se pone las reglas de validacion
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context){
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var curso = await _context.Curso.FindAsync(request.CursoId);

                if(curso == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {
                        mensaje = "No se encontro el curso"
                    });
                }

                curso.Titulo = request.Titulo ?? curso.Titulo; // El operador ??  => lo que hace es validar si esta varibale(request.Titulo  es indifinida o null pone el valor que se le asigne a la rderecha
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow;

                /*Actualizar el precio del curso*/
                var precioEntidad = _context.Precio.Where(x => x.CursoId == curso.CursoId).FirstOrDefault();
                if(precioEntidad != null){
                    precioEntidad.PrecioPromocion = request.PrecioPromocion ?? precioEntidad.PrecioPromocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioPromocion; 
                }else{//En caso no tenga precio se ingresara el precio
                    precioEntidad = new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0,
                        PrecioPromocion = request.PrecioPromocion ?? 0,
                        CursoId = curso.CursoId
                    };
                    await _context.Precio.AddAsync(precioEntidad);
                }


                if (request.ListaInstructor != null){
                    if (request.ListaInstructor.Count > 0) {
                        /*Eliminar  los instructores actuales  del curso en la base de datos*/

                        //Esto devuleve un arreglo de codigos guid de instructor que estan en la db
                        var instructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                        foreach (var instructoEliminar in instructoresDB){
                            _context.CursoInstructor.Remove(instructoEliminar);
                        }

                        /*Procedimento para agregar instructores que viene del cliente*/
                        foreach(var id in request.ListaInstructor){
                            var nuevoInstructor = new CursoInstructor{
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                    }
                }

                var valor = await _context.SaveChangesAsync();
                if (valor > 0){
                    return Unit.Value;
                }
                throw new Exception("No se pudieron guardar los cambios al curso");
            }
        }
    }
}