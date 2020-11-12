using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public interface IPaginacion
    {
         Task<PaginacionModel> DevolverPaginacion(string storeProcedure, 
                                                    int numPagina, 
                                                    int cantidadElemento, 
                                                    IDictionary<string, object> parametroFiltro, 
                                                    string ordenamientoColumna);
    }
}