using System;
using System.Threading.Tasks;
using Aplicacion.Documentos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class DocumentoController : MiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>>   GuardarArchivo(SubirArchivo.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivoGenerico>>ObtenerArchivo(Guid id){
            return await Mediator.Send(new ObtenerArchivo.Ejecuta{ Id = id});
        }
    }
}