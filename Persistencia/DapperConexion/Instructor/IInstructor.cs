using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
         Task<IEnumerable<InstructorModel>> ObtenerLista();

         Task<InstructorModel> ObtenerId(Guid id);

         Task<int> Nuevo(string nombre, string apellidos, string grado);

         Task<int> Actualiza(Guid id, string nombre, string apellido, string grado);

         Task<int> Eliminar(Guid id);
    }
}