using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister
{
    public interface IServicesRegistration
    {
        /// <summary>
        /// Add services
        /// </summary>
        /// <param name="services"></param>
        void AddServices(IServiceCollection services);
    }
}
