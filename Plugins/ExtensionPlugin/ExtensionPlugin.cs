using Microsoft.Extensions.Logging;
using RawCMS.Library.Core;
using RawCMS.Library.Core.Extension;
using RawCMS.Plugins.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;


namespace ExtensionPlugin
{
    public class ExtensionPlugin : Plugin
    {
        public ExtensionPlugin(AppEngine appEngine, AuthConfig config, ILogger logger) : base(appEngine, logger)
        {

        }

        public override string Name => "ExtensionPlugin";

        public override string Description => "";

        public override void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
            
        }

        public override void ConfigureMvc(Microsoft.Extensions.DependencyInjection.IMvcBuilder builder)
        {
            
        }

        public override void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            
        }

        public override void Init()
        {
            
        }

        public override void Setup(Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
        {
            
        }
    }
}
