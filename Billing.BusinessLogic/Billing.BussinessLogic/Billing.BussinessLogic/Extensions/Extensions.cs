using Billing.BussinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Billing.BussinessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, Login>();
            services.AddSingleton<IJWTService, JwtService>();

            return services;
        }
    }
}
