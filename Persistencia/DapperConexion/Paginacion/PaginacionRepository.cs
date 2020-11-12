using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepository : IPaginacion
    {
        public readonly IFactoryConnection _factoryConnection;

        public PaginacionRepository(IFactoryConnection factoryConnection){
            _factoryConnection = factoryConnection;
        }   

        public async Task<PaginacionModel> DevolverPaginacion(string storeProcedure, int numPagina, int cantidadElemento, IDictionary<string, object> parametroFiltro, string ordenamientoColumna)
        {
            PaginacionModel paginacionModel = new PaginacionModel();
            List<IDictionary<string, object>> listaReporte = null;
            int totalRecords = 0, totalPaginas = 0;
            try{    
                var connection = _factoryConnection.GetConnection();

                DynamicParameters parameters =  new DynamicParameters();

                foreach(var param in parametroFiltro){
                    parameters.Add("@" + param.Key, param.Value);
                }

                parameters.Add("@NumeroPagina", numPagina);
                parameters.Add("@CantidadElementos", cantidadElemento);
                parameters.Add("@Ordenamiento", ordenamientoColumna);

                parameters.Add("@TotalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parameters.Add("@TotalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);

                var result = await connection.QueryAsync(storeProcedure, parameters, commandType: CommandType.StoredProcedure);
                listaReporte = result.Select(x => (IDictionary<string, object>) x).ToList();
                paginacionModel.ListaRecords = listaReporte;
                paginacionModel.NumeroPaginas = parameters.Get<int>("@TotalPaginas");
                paginacionModel.TotalRecords = parameters.Get<int>("@TotalRecords");
            }catch(Exception e){
                throw new Exception("No se pudo ejecutar el procedimiento  almacenado", e);
            }finally{
                _factoryConnection.CloseConnection();
            }
            return paginacionModel;
        }
    }
}