using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiControllerBase : ControllerBase{
        
        private IMediator _mediator;


        //Cuando una variable es protegida signifca los hijos, las clases que herdedn de MiControllerBase podran usar
        //esta propiedad Mediator que es un objeto en si 
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}