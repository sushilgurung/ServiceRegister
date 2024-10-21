using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister
{
    public interface IServicesRegistrationWithConfig
    {
        /// <summary>
        /// Add services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        void AddServices(IServiceCollection services, IConfiguration configuration);
    }
}
