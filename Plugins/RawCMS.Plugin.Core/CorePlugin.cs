using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RawCMS.Library.Core;
using RawCMS.Library.Core.Extension;
using RawCMS.Library.DataModel;
using RawCMS.Library.Service;
using System;

namespace RawCMS.Plugins.Core
{
    public class CorePlugin : RawCMS.Library.Core.Extension.Plugin
    {
        public CorePlugin()
        {
           
        }

        public override string Name => "Core";

        public override string Description => "Add core CMS capabilities";

        public override void Init()
        {
            Logger.LogInformation("Core plugin loaded");
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Logger.LogInformation("Core plugin ConfigureServices");

            base.ConfigureServices(services);

            services.Configure<MongoSettings>(x =>
            {
                x = appSettings.DatabaseConnection;
            });

            IOptions<MongoSettings> settingsOptions = Options.Create<MongoSettings>(appSettings.DatabaseConnection);
            MongoService mongoService = new MongoService(settingsOptions);
            CRUDService crudService = new CRUDService(mongoService, settingsOptions);

            Engine.Service = crudService;

            services.AddSingleton<MongoService>(mongoService);
            services.AddSingleton<CRUDService>(crudService);
            services.AddSingleton<AppEngine>(Engine);
            services.AddHttpContextAccessor();

            crudService.EnsureCollection("_configuration");

            Engine.Plugins.ForEach(x => SetConfiguration(x, crudService));

            crudService.EnsureCollection("_schema");
            
        }



        private void SetConfiguration(Plugin plugin, CRUDService crudService)
        {
            Logger.LogInformation("Core plugin SetConfiguration");

            Type confitf = plugin.GetType().GetInterface("IConfigurablePlugin`1");
            if (confitf != null)
            {
                Type confType = confitf.GetGenericArguments()[0];
                Type pluginType = plugin.GetType();

                ItemList confItem = crudService.Query("_configuration", new DataQuery()
                {
                    PageNumber = 1,
                    PageSize = 1,
                    RawQuery = @"{""plugin_name"":""" + pluginType.FullName + @"""}"
                });

                JObject confToSave = null;

                if (confItem.TotalCount == 0)
                {
                    confToSave = new JObject
                    {
                        ["plugin_name"] = plugin.GetType().FullName,
                        ["data"] = JToken.FromObject(pluginType.GetMethod("GetDefaultConfig").Invoke(plugin, new object[] { }))
                    };
                    crudService.Insert("_configuration", confToSave);
                }
                else
                {
                    confToSave = confItem.Items.First as JObject;
                }

                object objData = confToSave["data"].ToObject(confType);

                pluginType.GetMethod("SetActualConfig").Invoke(plugin, new object[] { objData });
            }
        }

        public override void Configure(IApplicationBuilder app, AppEngine appEngine)
        {
            Logger.LogInformation("Core plugin Configure");

            base.Configure(app, appEngine);
        }

        private readonly AppSettings appSettings = new AppSettings();

        public override void Setup(IConfigurationRoot configuration)
        {
            Logger.LogInformation("Core plugin Setup");
            configuration.GetSection("RawCms").Bind(appSettings);
        }

        public override void OnPluginLoaded()
        {
            base.OnPluginLoaded();
            Logger.LogInformation("Core plugin Activated!");
        }
    }
}