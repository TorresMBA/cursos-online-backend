using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;
using System.Collections.Generic;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest{

            //public int CursoId { get; set; } Se Elimina porque se auto genera

            //[Required(ErrorMessage="Por favor ingrese el titulo")]
            public string Titulo { get; set; }

            public string Descripcion { get; set; }

            public DateTime? FechaPublicacion { get; set; }

            public List<Guid> ListaInstructor { get; set; }

            public decimal Precio { get; set; }

            public decimal PrecioPromocion { get; set; }
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
                //Inserta datos a la tabla Curso
                Guid _cursoId = Guid.NewGuid();
                var curso = new Curso{
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow //Que significa utc? Coodinated Universal Time (UTC) es una nomenclatura de tiempo stander 
                };
                _context.Add(curso);

                //Inserta datos a la tabla CursoInstructor
                if(request.ListaInstructor != null){
                    foreach (var id in request.ListaInstructor){
                        var cursoIsntructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        _context.CursoInstructor.Add(cursoIsntructor);
                    }
                }

                //Insertar datos a la tabla Precio
                var precioEntidad = new Precio(){
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    PrecioPromocion = request.PrecioPromocion,
                    PrecioId = Guid.NewGuid()
                };
                _context.Precio.Add(precioEntidad);

                var valor = await _context.SaveChangesAsync();//Devuelte el numero de operaciones que se esta realizando sobre la data en la db 
                //Derrenpete estas insertando solo un curso y ningun instrcutor entonces el valor es 1
                //pero si se inserta un curso y ademas 3 instructor seria 4

                if (valor > 0){
                    return Unit.Value; //Es mandarle un flak una alerta ;
                }                

                throw new Exception("No se Pudo Insertar el Curso");
            }
        }
    }
}