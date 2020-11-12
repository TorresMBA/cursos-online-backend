using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* Lo que hacemos dentro del Main es crear un procedimiento para que ejecute el archivo de migracion que creamos en el proyecto Persistencia, bueno en otras palabras ejecute los archivos que estan dentro de Persistencia/Migrations
            Cuando se ejecuten estos archivos migrations, se evalua si existe algun cambio pendiente que hacer en las estructuras de la base de datos, por eso ejecutamos el comando :
                dotnet watch run
            Este comando lo que hace es ejecutarse cada vez que un archivo del proyecto cambie. */
            //Este codigo lo que hace es ejecutar el archivo de migracion 
            //ubicado en Persistencia/Migrations
            var hostserver = CreateHostBuilder(args).Build();
            using(var ambiente = hostserver.Services.CreateScope()){
                var services = ambiente.ServiceProvider;
                try{
                    var userManaager = services.GetRequiredService<UserManager<Usuario>>();
                    var context = services.GetRequiredService<CursosOnlineContext>();
                    context.Database.Migrate();
                    DataPrueba.InsertarData(context, userManaager).Wait();
                }catch(Exception ex){
                    var logging = services.GetRequiredService<ILogger<Program>>();
                    logging.LogError(ex, "Ocurrio un error en la migracion");
                }
            }
            hostserver.Run();
        }

        //Carga la configuracion por defecto que yo agregue dentro del
        //appsettingD.Develoment.json cuando estamos en ambiende de
        //de desarrollo y appsettings.json en azure 
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
