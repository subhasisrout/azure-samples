using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SecureAppServiceDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var allowedAppIds = new List<string> { "264dc6bd-f9c2-433c-b8d9-23a06d1dd039", 
                "096f2d18-5f4d-4b9f-8971-0366883d43be",
                "58e610f4-6909-4824-b501-8b4261d9225c", //function app for python
                "appId3" }; // Add your allowed Application IDs here

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opt =>
               {
                   opt.Audience = "api://c7c788a5-f95b-41d1-b677-ca2116fcf073";
                   opt.Authority = $"https://login.microsoftonline.com/ce8ac118-d95e-4cce-a7d9-70febd5764d6";
                   opt.Events = new JwtBearerEvents
                   {
                       OnTokenValidated = context =>
                       {
                           // Get the application ID (client ID) from the token
                           var appId = context.Principal.FindFirst("azp")?.Value
                                       ?? context.Principal.FindFirst("appid")?.Value;

                           // Check if the appId is in the allowed list
                           if (appId == null || !allowedAppIds.Contains(appId))
                           {
                               // If not allowed, reject the token
                               context.Fail("Unauthorized: The application is not allowed.");
                           }

                           return Task.CompletedTask;
                       }
                   };

               });
            builder.Services.AddControllers();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
