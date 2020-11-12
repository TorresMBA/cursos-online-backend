using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Comentarios
{
    public class Eliminar
    {
        public class Ejecuta : IRequest{
            public Guid ComentarioId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>{

            public readonly CursosOnlineContext _context; 

            public Manejador(CursosOnlineContext context){
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var comentario = await _context.Comentario.FindAsync(request.ComentarioId);
                if(comentario == null){
                    throw new ManejadorException(System.Net.HttpStatusCode.NotFound, new { mensaje = "No se encontro el comentario" });
                }

                _context.Remove(comentario);
                var resul = await _context.SaveChangesAsync();
                if(resul > 0){
                    return Unit.Value;
                }
                throw new Exception("No se pudo eliminar el comentario");
            }
        }
    }
}