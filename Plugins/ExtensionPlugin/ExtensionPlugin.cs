using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawCMS.Library.Core;
using RawCMS.Library.Core.Extension;
using RawCMS.Plugins.KeyStore;
using RawCMS.Plugins.KeyStore.Contracts;
using System;
using System.Collections.Generic;
using System.Text;


namespace ExtensionPlugin
{
    public class ExtensionPlugin : Plugin
    {
        public ExtensionPlugin(AppEngine appEngine, ILogger logger) : base(appEngine, logger)
        {

        }

        public override string Name => "ExtensionPlugin";

        public override string Description => "";

        public override int Priority => 10000;

        public override void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
            
        }

        public override void ConfigureMvc(Microsoft.Extensions.DependencyInjection.IMvcBuilder builder)
        {
            
        }

        public override void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddSingleton<IKeyStoreService, MyFakeStore>((x) =>  new MyFakeStore());
        }


        public override Dictionary<Type,Type> GetActivationMap()
        {
            var dict= base.GetActivationMap();
            dict[typeof(IKeyStoreService)] = typeof(MyFakeStore);
            return dict;
        }


        public override void Setup(Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
        {
            
        }
    }
}
