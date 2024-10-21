using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister
{
    public interface IRepositoriesRegistration 
    {
        /// <summary>
        /// Register services
        /// </summary>
        /// <param name="services"></param>
        void AddServices(IServiceCollection services);
    }
}
