using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;

        public InstructorRepositorio(IFactoryConnection factory){
            _factoryConnection = factory;
        }

        public async Task<int> Actualiza(Guid id, string nombre, string apellido, string grado)
        {
            var storeProcedure = "usp_Instructor_Editar";
            try{
                var connection = _factoryConnection.GetConnection();
                var resultado = await connection.ExecuteAsync(
                    storeProcedure, 
                    new { 
                        InstructorId = id, 
                        Nombre = nombre,
                        Apellidos = apellido,
                        Grado = grado
                    },
                    commandType: CommandType.StoredProcedure
                );  
                _factoryConnection.CloseConnection();
                return resultado;
            }catch(Exception ex){
                throw new Exception("" + ex);
            }
        }

        public async Task<int> Eliminar(Guid id)
        {
            var storeProcedure = "usp_Instructor_Elimina";
            try{
                var connection = _factoryConnection.GetConnection();
                var resultado = await connection.ExecuteAsync(
                        storeProcedure, 
                        new { 
                            InstructorId = id,
                        }, 
                        commandType: CommandType.StoredProcedure
                    );
                    _factoryConnection.CloseConnection();
                    return resultado;
            }catch(Exception ex){
                throw new Exception("No se pudo eliminar el instructor", ex);
            }
        }

        public async Task<int> Nuevo(string nombre, string apellidos, string grado)
        {
            var storeProcedure = "usp_instructor_nuevo";
            try{
                var connection = _factoryConnection.GetConnection();
                                      //nombre del sp, Lista, arreglo o enmuneracion de parametros del sp, que tipo de dato es mejor dicho el tipo de operacion
                              //Al finalizar el metodo executeasyn devolvera un metodo entero indicando cuantas transciones se realizo al llamar a este sp        
                var resultado = await connection.ExecuteAsync(storeProcedure, 
                        new {
                            InstructorId = Guid.NewGuid(),
                            Nombre = nombre,
                            Apellidos = apellidos,
                            Grado = grado
                        }, 
                        commandType: CommandType.StoredProcedure
                    );
                    _factoryConnection.CloseConnection();
                    return resultado;
            }catch(Exception ex){
                throw new Exception("No se pudo guardar el nuevo instructor", ex);
            }
        }

        public async Task<InstructorModel> ObtenerId(Guid id)
        {
            var storeProcedure = "usp_Instructor_Unico";
            InstructorModel instructor = null;
            try{
                var conection = _factoryConnection.GetConnection();
                instructor = await conection.QueryFirstAsync<InstructorModel>(
                    storeProcedure, 
                    new { 
                        Id = id
                    }, 
                    commandType: CommandType.StoredProcedure
                );
                _factoryConnection.CloseConnection();
                return instructor;
            }catch(Exception ex){
                throw new Exception("No se pudo encontrar el instructor", ex);
            }
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList = null;
            var storeProcedure = "usp_Obtener_Instructores";

            try{
                var connection = _factoryConnection.GetConnection();
                instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType : CommandType.StoredProcedure);
            }catch(Exception e){
                throw new Exception("Error en la consulta de datos ", e);
            }finally{
                _factoryConnection.CloseConnection();
            }
            return instructorList;
        }
    }
}