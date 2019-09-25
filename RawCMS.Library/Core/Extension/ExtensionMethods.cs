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

            var replacement=appEngine.Lambdas.Where(x => x is FactoryLambda && replacedType == ((FactoryLambda)x).OriginalType).Cast<FactoryLambda>().ToList();

            if (replacement.Count > 0)
            {
                replacedType = replacement.First().ReplacedWith;
            }

            appEngine.ReflectionManager.InvokeGenericMethod(services,"AddSingleton", new Type[] { typeof(TService), replacedType }, new object[] { });

            return services;
        }

    }
}
