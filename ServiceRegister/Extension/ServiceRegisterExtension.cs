using Gurung.ServiceRegister;
using Gurung.ServiceRegister.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister
{
    /// <summary>
    /// Extension methods for IServiceCollection.
    /// </summary>
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Registers all service types found in the given assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddServiceRegister(this IServiceCollection services, IConfiguration configuration = null)
        {
            RegisterServices serviceRegistrar = new RegisterServices();
            serviceRegistrar.DiscoverAndRegisterServices(AppDomain.CurrentDomain.GetAssemblies(), services, configuration);
        }
    }
}
