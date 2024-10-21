using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gurung.ServiceRegister.Extension
{
    internal static class RegistrationExtensions
    {
        /// <summary>
        /// Registers all service types found in the given assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceRegistrationType"></param>
        /// <param name="configuration"></param>
        /// <param name="registrationMethodName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void InvokeServiceRegistrationWithConfig(this IServiceCollection services, Type serviceRegistrationType, IConfiguration configuration, string registrationMethodName)
        {
            try
            {
                if (!serviceRegistrationType.IsClass)
                {
                    throw new InvalidOperationException($"Cannot create an instance of type '{serviceRegistrationType.FullName}' because it is not a class.");
                }

                var serviceInstance = Activator.CreateInstance(serviceRegistrationType);
                var methodInfo = serviceRegistrationType.GetMethod(registrationMethodName);
                if (methodInfo == null)
                {
                    throw new InvalidOperationException($"Method '{registrationMethodName}' not found on type '{serviceRegistrationType.FullName}'.");
                }

                methodInfo.Invoke(serviceInstance, new object[] { services, configuration });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error invoking method '{registrationMethodName}' on type '{serviceRegistrationType.FullName}': {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Registers all service types found in the given assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceRegistrationType"></param>
        /// <param name="registrationMethodName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void InvokeServiceRegistration(this IServiceCollection services, Type serviceRegistrationType, string registrationMethodName)
        {
            try
            {
                if (!serviceRegistrationType.IsClass)
                {
                    throw new InvalidOperationException($"Cannot create an instance of type '{serviceRegistrationType.FullName}' because it is not a class.");
                }

                var serviceInstance = Activator.CreateInstance(serviceRegistrationType);
                var methodInfo = serviceRegistrationType.GetMethod(registrationMethodName);
                if (methodInfo == null)
                {
                    throw new InvalidOperationException($"Method '{registrationMethodName}' not found on type '{serviceRegistrationType.FullName}'.");
                }

                methodInfo.Invoke(serviceInstance, new object[] { services });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error invoking method '{registrationMethodName}' on type '{serviceRegistrationType.FullName}': {ex.Message}", ex);
            }
        }
    }
}
