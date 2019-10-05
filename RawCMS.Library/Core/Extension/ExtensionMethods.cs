using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RawCMS.Library.Core.Extension
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddSingletonWithOverride<TService, TImplementation>(this IServiceCollection services, AppEngine appEngine)
            where TService : class
            where TImplementation:  class, TService
        {
            var replacedType = typeof(TImplementation);
            var interfaceType = typeof(TService);

            var replacement=appEngine.Lambdas.Where(x => x is FactoryLambda 
            && (replacedType.FullName == ((FactoryLambda)x).OriginalType.FullName 
             || interfaceType.FullName == ((FactoryLambda)x).OriginalType.FullName)// coming from different plugin: direct comparison cannot work
            ).Cast<FactoryLambda>().ToList();

            if (replacement.Count > 0)
            {
                replacedType = replacement.First().ReplacedWith;
            }
            else
            {
                appEngine.ReflectionManager.InvokeGenericMethod(null, typeof(ServiceCollectionServiceExtensions), "AddSingleton", new Type[] { typeof(TService), replacedType }, new object[] { services });
            }

            return services;
        }

    }
}
