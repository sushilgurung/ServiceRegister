using Gurung.ServiceRegister.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister.Services;

public class RegisterServices
{
    internal void DiscoverAndRegisterServices(IEnumerable<Assembly> assemblies, IServiceCollection services, IConfiguration configuration)
    {
        var identityType = typeof(IIdentityServicesRegistration);
        var dbServiceRegistrationType = typeof(IDbServiceRegistration);
        var serviceRegistrationWithConfigType = typeof(IServicesRegistrationWithConfig);

        var registrationType = typeof(IServicesRegistration);
        var repositoriesType = typeof(IRepositoriesRegistration);


        try
        {
            var types = assemblies.SelectMany(x => x.DefinedTypes);

            var dbTypesConfig = types.Where(type => type.ImplementedInterfaces.Contains(dbServiceRegistrationType));
            if (dbTypesConfig is not null)
            {
                foreach (var type in dbTypesConfig)
                {
                    services.InvokeServiceRegistrationWithConfig(type, configuration, nameof(IDbServiceRegistration.AddServices));
                }
            }

            var identityTypesConfig = types.Where(type => type.ImplementedInterfaces.Contains(identityType));
            if (identityTypesConfig is not null)
            {
                foreach (var type in identityTypesConfig)
                {
                    services.InvokeServiceRegistrationWithConfig(type, configuration, nameof(IIdentityServicesRegistration.AddServices));
                }
            }

            var typesWithConfig = types.Where(type => type.ImplementedInterfaces.Contains(serviceRegistrationWithConfigType));
            if (typesWithConfig is not null)
            {
                foreach (var type in typesWithConfig)
                {
                    services.InvokeServiceRegistrationWithConfig(type, configuration, nameof(IServicesRegistrationWithConfig.AddServices));
                }
            }

            var typesWithoutConfig = types.Where(type => type.ImplementedInterfaces.Contains(registrationType));
            if (typesWithoutConfig is not null)
            {
                foreach (var type in typesWithoutConfig)
                {
                    services.InvokeServiceRegistration(type, nameof(IServicesRegistration.AddServices));
                }
            }

            var repositoriesTypesConfig = types.Where(type => type.ImplementedInterfaces.Contains(repositoriesType));
            if (repositoriesTypesConfig is not null)
            {
                foreach (var type in repositoriesTypesConfig)
                {
                    services.InvokeServiceRegistration(type, nameof(IRepositoriesRegistration.AddServices));
                }
            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during service discovery: {ex.Message}", ex);
        }
    }



}

