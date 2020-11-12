using System.Collections.Immutable;
using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistencia;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Aplicacion.Cursos;
using FluentValidation.AspNetCore;
using WebApi.Middleware;
using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Aplicacion.Contratos;
using Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructor;
using Microsoft.OpenApi.Models;
using Persistencia.DapperConexion.Paginacion;

namespace WebApi
{
    public class Startup
    {
        //Lo que hace este constructo es injectar o creando una, 
        //ingresando como parametro el IConfiguration que proviene
        //de la clase Program.cs
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*Se agrega los Cors para que nuestro servicio pueda ser comsumido por cualquier cliente*/
            services.AddCors(o => o.AddPolicy("corsApp", builder => {
                /* metodo largo
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                
                metodo corto*/
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddDbContext<CursosOnlineContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddOptions();
            services.Configure<ConexionConfiguracion>(Configuration.GetSection("ConnectionStrings"));

            services.AddMediatR(typeof(Consulta.Manejador).Assembly);

            //Se agrego para que nuestros controller tengan autorizacion antes de recibir un request de un cliente 
            //antes de procesar un request de un cliente 
            services.AddControllers(opt => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());//Con esto se agrega la validacion de FluenValitadion para que trabaje en los controllers

            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            //Con estas dos lineas estariamos instanciando el servicio de RolManager, en este caso el objeto va a estar listo
            //cuando arranque mi aplicacion
            identityBuilder.AddRoles<IdentityRole>();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Usuario, IdentityRole>>();

            identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>();
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();

            /* 
            El service services.TryAddSingleton usualmente se utiliza para inyectar un objeto dependiendo si existe, 
            si no existe procedera a la inyeccion caso contrario se usara la instancia que tiene en ese momento la app en tiempo de ejecucion.
            Hay un pequeno bug dentro del registro de nuevos usuarios en el Core Identity propio del framework, cuando sucede este evento 
            se debe registrar tambien la hora y el tiempo del registro, pero para hacerlo el core identity se apoya en la clase SystemClock,  
            por algun motivo, en el momento del registro esta clase no esta siendo instanciada correctamente por ello agregamos esta linea, 
            para forzarla aque haga la instancia de esta clase si es que no existe: services.TryAddSingleton<ISystemClock, SystemClock>();
            */
            services.TryAddSingleton<ISystemClock, SystemClock>();
            //que diferencia existe  tiene TryAddSingleton con el AddSingleton normal y por el que el uso de SystemClock?
            /* La principal diferencia es que el TryAddSingleton evalua la existencia del objeto, si no existe entonces fuerza 
            la instancia de la clase.
            Sobre tu segunda pregunta, existe un bug dentro del Core Identity a la hora de registrar un nuevo usuario, ya que esta 
            intentando registrar la fecha de creacion del nuevo usuario, pero para hacer esto se apoya en la clase SystemClock, y al 
            tratar de utilizar el objeto, este devuelve un null, por ello hacemos la instancia de esta clase con el TryAddSingleton.
            */

            //Porque estamos injectando estas interfax y clas lo hago porque de esta forma podamos acceder a los metodos que van a generar
            //los tokens en Seguridad, es decir WebApi va a poder acceder a esos token haciendo esta inyeccion de servicios
            services.AddScoped<IJwtGenerador, JwtGenerador>();

            services.AddScoped<IUsuarioSesion, UsuarioSesion>();

            services.AddAutoMapper(typeof(Consulta.Manejador));

            services.AddTransient<IFactoryConnection, FactoryConnection>();

            services.AddScoped<IInstructor, InstructorRepositorio>();

            services.AddScoped<IPaginacion, PaginacionRepository>();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi Palabra secreta"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false, //Que quien podra crear estos tokens
                    ValidateIssuer = false //Es como el envio del token
                };
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("V1", new OpenApiInfo {
                    Title = "Servicios Para Mantenimiento de Cursos",
                    Version = "V1"
                });
                c.CustomSchemaIds(c => c.FullName);
            });


        } 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //Este metodo es el que va a determeniar como va a trbajar mi proyecto mis funcionalidad dependiendo del Environment del ambiente cual estoy trabajando
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("corsApp");

            //Aca usamos nuestro propio middlware que creamos para manejar las excepciones
            //y para que arranque junto con nuestro proyecto
            //Ya inscrustamos al ManejadorErrorMiddleware como un tipo middlware dentro de
            //nuestro proyecto WebApi
            app.UseMiddleware<ManejadorErrorMiddleware>();
            if (env.IsDevelopment())
            {
                //Gracias a este serivioc se puede ver actualmente la excepion cada que 
                //lo llamamos desde postman pero nosotros middleware para manejar las 
                //excepciones y vamos a incluir nuestro propio midddlware
                //app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection(); //Se comenta esta linea porque usar https es en un ambiente de producction
            //en un ambiente de desarrollo no es necesario por ahora
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursos Online Version 1");
            });
        }
    }
}
