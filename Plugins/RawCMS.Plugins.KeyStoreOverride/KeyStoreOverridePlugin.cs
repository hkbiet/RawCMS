using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RawCMS.Library.Core;
using RawCMS.Library.KeyStore;

namespace RawCMS.Plugins.KeyStoreOverride
{
    public class KeyStoreOverridePlugin : Library.Core.Extension.Plugin
    {
        public KeyStoreOverridePlugin(AppEngine engine, ILogger logger) : base(engine, logger)
        {
        }

        public override string Name => "KeyStoreOverridePlugin";

        public override string Description => "override KeyStorePlugin";
        public override int Priority => int.MaxValue; //ensure loading befor original plugin

        public override void Configure(IApplicationBuilder app)
        {
            
        }

        public override void ConfigureMvc(IMvcBuilder builder)
        {
            
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IKeyStoreService, KeyStoreServiceOverride>());
        }

        public override void Setup(IConfigurationRoot configuration)
        {
            
        }
    }
}
