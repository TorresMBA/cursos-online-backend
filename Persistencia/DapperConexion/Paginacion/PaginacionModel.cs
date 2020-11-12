using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel{
        /* Ejemplo de un diccionario en C# 
            var diccionario = new Dictionary<int, string>{
                {1,"Domingo"},
                {2,"Lunes"},
                {3,"Martes"},
                {4,"Miercoles"},
                {5,"Jueves"},
                {6,"Viernes"},
                {7,"Sabado"}
            };
        */
        public List<IDictionary<string, object>> ListaRecords { get; set; }

        public int NumeroPaginas { get; set; }

        public int TotalRecords { get; set; }
    }
}