using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Dominio;
using Persistencia;
using System;

namespace Aplicacion.Comentarios
{
    public class Nuevo
    {
        public class Ejecuta : IRequest{
            public string Alumno { get; set; }

            public int Puntaje { get; set; }

            public string Comentario { get; set; }

            public Guid CursoId { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            
            public EjecutaValidacion(){
                RuleFor(x => x.Alumno).NotEmpty();
                RuleFor(x => x.Puntaje).NotEmpty();
                RuleFor(x => x.Comentario).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            public readonly CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context){
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var comentario = new Comentario(){
                    ComentarioId = Guid.NewGuid(),
                    Alumno = request.Alumno,
                    ComentarioTexto = request.Comentario,
                    Puntaje = request.Puntaje,
                    CursoId = request.CursoId,
                    FechaCreacion = DateTime.UtcNow
                };

                 _context.Comentario.Add(comentario);
                 var resul = await _context.SaveChangesAsync();
                 if(resul > 0){
                     return Unit.Value;
                 }
                throw new Exception("No se pudo insertar el comentario");
            }
        }
    }
}