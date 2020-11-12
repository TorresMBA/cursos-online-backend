using System.Net;
using System;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApi.Middleware
{
    //El ManejadorErrorMiddleware es como un interceptador de reqirmientos
    //en el momento que nosotros estamos trantando de enviar un pedido de 
    //insert un nuevo curso lo que va a despirarse primer son las validaciones 
    //pra verificar que la data que se envia sea correcta 
    //Si se detecta que el titulo esta vacio en ese momento la clase que se va a disparar 
    //es la clase ManejadorErrorMiddleware atravez del metodo Invocar...
    public class ManejadorErrorMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ManejadorErrorMiddleware> _logger;

        public ManejadorErrorMiddleware(RequestDelegate next, ILogger<ManejadorErrorMiddleware> logger){
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context){
            try{
                //Si tiene el titulo y la descrpcion todo esta corecto
                //entonces pasa al siguiente procedimiento que es la
                //transacion es porque que le ponemos nombre _next y que
                //lleve todo el contexto, dentro del contexto estan los 
                //prarametros requerimientos, toda la data que el usuario quiere
                //insertar en la db. si todo es correcta pasa para allá
                await _next(context);//Pero si el titulo esta vacio
            }catch (Exception ex){
                //en este momento es donde se crea un exception y al crearse la excepion
                //va a dispirar llamar al metodo ManejadorExceptionAsincrono y que hace
                //este metodo me esta indicanddo que tipo de exception es lo que va a lanzar
                await ManejadorExceptionAsincrono(context, ex , _logger);
            }
        }

        private async Task ManejadorExceptionAsincrono(HttpContext context, Exception exception, ILogger<ManejadorErrorMiddleware> logger){
            object errores = null;
            switch(exception){
                //Si es de tipo Http 
                case ManejadorException me:
                    // entonces quiero que me imprima con detalle el log del error  
                    logger.LogError(exception, "Manejador Error");
                    //Quiero que los errores que se estan haciendo una collecion en este objeto
                    errores = me.Errores;
                    //Y tambien quiero que me digas que codigo de error es
                    context.Response.StatusCode = (int)me.Codigo;
                    break;
                case Exception e://En cass sea un error generico de exception propio de C#
                    logger.LogError(exception, "Error de Servidor");//logger con al excepcion
                    //tambien la lista de errores que se an producido en detalle cual es error
                    errores = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            //Aca le estoy indicando en que formato va a devolver este error 
            context.Response.ContentType = "application/json";
            if(errores != null){
                var resultados = JsonConvert.SerializeObject(new { errores });
                await context.Response.WriteAsync(resultados);
            }
        }
    }
}

//Logger
/* Un logger es usado para crear mensajes de error personalizados, lo bueno de hacer logs es que estos pueden registrarse a invel del mismo sistema operativo/ container que esta ejecutando tu app. Si estas en Windows a traves del Event Log.

En mi caso personal uso Event Log para monitear algunos eventos que estan en alto trafico. Como ves no siempre el Logger se usa para disparar o registrar un error, tambien sirve para warnings, advertencias o tambien para enviar un mensaje que la secuencia logica de tu codigo fue correcta. Para casos de logica sensible como transacciones bancarias en modo TEST. */

//Qué hacen las clases invocadas en el constructor ManejadorError, RequestDelegate y ILogger.
/* Usamos la clase ManejorErrorMiddleware para interceptar los mensajes del Request y del HttpContext que suceden entre el controller y la capa de aplicacion

Para poder interceptar estos mensajes usamos el RequestDelegate, para evaluar si se disparo un error en la logica de alguna clase en el proyecto  Aplicacion

Un ejemplo de este disparo de error en alguna clase del proyecto Aplicacion seria cuando por ejemplo buscas un usuario y este no existe, entonces disparas un throw error :

 if (usuario == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
                }
Alli esta disparando un error que esta siendo detectado por el RequestDelegate, en caso no existiera ningun disparo de error simplemente la logica continuaria, por ello en el metodo Invoke del ManejadorErrorMiddleware simplemente le ponemos  await _next(context); 

Esto es muy importante, por que en esa linea le estamos diciendo que continue con mi logica del contexto.



Ahora volviendo al punto de que se dispare ese error, lo que hace entonces el RequestDelegate es evaluar que hacer con este "throw error",  y puede haber muchas variantes aqui respecto que hacer con el error, si lo deseas puedes guardarlo en la base de datos, si deseas puedes llamar a otro servicio que evalue el error, pero lo que nosotros estamos haciendo es imprimir en la consola el error por ello invocamos al ILogger, esta herramienta como ya lo dije puede imprimir errores, warnings, tambien flags, etc.

Creo que con eso respondi tu pregunta de por que usamos el ILogger.



Tu segunda pregunta es si podemos usar otras alternativas,  la respuesta es si, dependiendo de que es lo que quieres hacer cuando el RequestDelegate detecte un error, si ya no quieres imprimirlo en la consola si no mas bien almacenarlo en una tabla de tu base de datos, entonces tendrias en el constructor que llamar al context de CursosOnlineContext.

Si deseas llamar a una externa web service para enviarle ese log a otro sistema, tendrias que usar la libreria HttpClient.

Y asi sucesivamente puedes tener muchas opciones dependiendo de los requerimientos o definiciones en la arquitectura de tu proyecto.  */