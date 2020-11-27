using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Persistencia.DapperConexion.Paginacion;

namespace WebApi.Controllers
{
    //Para que sea de tipo Controller lo que necesito es hacer que herede desde la clase ControllerBase
    //http://localhost:5000/api/Cursos -> Este es el endpoint que nosotros vamos a crear para este controller
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : MiControllerBase
    {
       /*  private readonly IMediator _mediator;//IMediator proviene de la libreria MediaTr

        public CursosController(IMediator mediator){
            //Aqui se realiza la injecction, la variable _mediator se tranformo en un objeto en el momento que se contruta la clase CursoController
            _mediator = mediator;
        } */
        
        [HttpGet]
        public async Task<ActionResult<List<CursoDto>>> Cursos(){
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        //http://localhost:5000/api/Cursos/{id}
        //http://localhost:5000/api/Cursos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> CursoUnico(Guid id){
            return await Mediator.Send(new ConsultaId.CursoUnico { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> CrearCurso(Nuevo.Ejecuta data){
            return await Mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> EditarCurso(Guid id, Editar.Ejecuta data){
            data.CursoId = id;
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> EliminarCurso(Guid id){
            return await Mediator.Send(new Eliminar.Ejecuta { Id = id });
        }

        [HttpPost("report")]
        //[AllowAnonymous]
        public async Task<ActionResult<PaginacionModel>> Paginacion(PaginacionCurso.Ejecuta data){
            return await Mediator.Send(data);
        }
    }
}