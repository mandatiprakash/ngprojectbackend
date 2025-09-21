using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace webapi.Security
{
    public static class ServiceExtensions
    {
        public static void AddJWTService(this IServiceCollection services, IConfiguration configuration)
        {
            string key = configuration.GetSection("Jwt:Key").Value;
            var issuer = configuration.GetSection("Jwt:Issuer").Value;
            var aud = configuration.GetSection("Jwt:Audience").Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = issuer,
                  ValidAudience = issuer,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
              };

              options.Events = new JwtBearerEvents
              {
                  OnAuthenticationFailed = context =>
                  {
                      if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                      {
                          context.Response.Headers.Add("Token-Expired", "true");
                      }
                      return Task.CompletedTask;
                  }
              };
          });
        }

    }
}
