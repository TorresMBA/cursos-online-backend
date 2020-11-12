using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;
using System.Linq;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest{
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            public CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context){
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Conseguir todos los intructores relaciones del curso dentro de la tabla CursoInstructor
                var instructorDB = _context.CursoInstructor.Where(x => x.CursoId == request.Id);
                foreach(var instructor in instructorDB){
                    _context.CursoInstructor.Remove(instructor);
                }

                var comentariosDB = _context.Comentario.Where(x => x.CursoId == request.Id);
                foreach (var comentario in comentariosDB) {
                    _context.Comentario.Remove(comentario);
                }

                var precioDB = _context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();
                if(precioDB != null){
                    _context.Precio.Remove(precioDB);
                }


                //Busca el curso por al id antes de eliminar 
                var curso =  await _context.Curso.FindAsync(request.Id);
                
                //Si la busqueda no es correcta significa que curso sera null
                //se cumplira la condiciones y lanzara una exception
                if (curso == null){
                    //throw new Exception("Nose Puede Eliminar el curso");
                    //Y lo que queremos es reemplazaar la expceion regular que tiene C#
                    //para usar nuestra excepcion que nosotros hemos construido
                    //1r Parametro - Sino encuentra el curso significa que no hay un NotFound
                    //2d Parametro - indicar que tipo de mensaj devolveremos al usuario
                    //es como indicarle un objeto de tipo error que pueden haber varios
                    //Estamos creadno un nuevo objeto new { xxx: "mensaje"}
                    throw new ManejadorException(HttpStatusCode.NotFound, new {
                        mensaje = "No se encontro el curso"
                    });
                }
                _context.Remove(curso);

                var valor = await _context.SaveChangesAsync();
                if(valor > 0){
                    return Unit.Value;
                }
                throw new Exception("No se pudieron los guardas los cambios");
            }
        }
    }
}