using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RawCMS.Library.Core;
using RawCMS.Library.DataModel;
using RawCMS.Library.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RawCMS.Library.GraphQL.Types
{
    public class JObjectFieldResolver : IFieldResolver
    {

        private readonly CRUDService service;

        public JObjectFieldResolver(AppEngine manager)
        {
            service = manager.Service;
        }

        public object Resolve(ResolveFieldContext context)
        {
            ItemList result;
            if (context.Arguments != null && context.Arguments.Count > 0)
            {
                var pageNumber = 1;
                var pageSize = 1000;
                if(context.Arguments.ContainsKey("pageNumber"))
                {
                    pageNumber = int.Parse(context.Arguments["pageNumber"].ToString());
                    if(pageNumber < 1)
                    {
                        pageNumber = 1;
                    }
                    context.Arguments.Remove("pageNumber");
                }

                if (context.Arguments.ContainsKey("pageSize"))
                {
                    pageSize = int.Parse(context.Arguments["pageSize"].ToString());
                    context.Arguments.Remove("pageSize");
                }

                result = service.Query(context.FieldName.ToPascalCase(), new DataQuery()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    RawQuery = BuildMongoQuery(context.Arguments)
                });

            }
            else
            {
                result = service.Query(context.FieldName.ToPascalCase(), new DataQuery()
                {
                    PageNumber = 1,
                    PageSize = 1000,
                    RawQuery = null
                });
            }

            return result.Items.ToObject<List<JObject>>();
            ////return new RestMessage<ItemList>(result);
            //HttpClient client = new HttpClient();
            //var response = client.GetAsync($"http://localhost:57564/api/CRUD/{context.FieldName.ToPascalCase()}/5c208311be03fb245cd6fde5", HttpCompletionOption.ResponseContentRead).Result;
            ////var response = client.GetAsync($"http://localhost:57564/api/CRUD/{context.FieldName.ToPascalCase()}", HttpCompletionOption.ResponseContentRead).Result;
            //var result = response.Content.ReadAsStringAsync().Result;
            //var tt = JObject.Parse(result)["data"]["items"];
            //if(tt == null)
            //{
            //    tt = JObject.Parse(result)["data"];
            //    var res = new List<JObject>();
            //    res.Add(tt.ToObject<JObject>());
            //    return res;
            //}
            ////var result2 = new List<JObject>();
            ////result2.Add(JObject.Parse(tt.ToString()));
            //return tt.ToObject<List<JObject>>();//JObject.Parse(result)["data"];
            //var result = new List<JObject>();
            //if (context.FieldName.Equals("testcollection", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    result.Add(JObject.Parse(@"{ 'Key': 1, 'Description' : 'test1'}"));
            //    result.Add(JObject.Parse(@"{ 'Key': 2, 'Description' : 'test 123'}"));
            //}
            //else if (context.FieldName.Equals("user", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    result.Add(JObject.Parse(@"{'Key' : 1, 'DisplayName' : 'pippo', 'Email': 'pippo@pippo.com'}"));
            //    result.Add(JObject.Parse(@"{'Key' : 2, 'DisplayName' : 'pippo 2', 'Email': 'pippo2@pippo.com'}"));
            //    result.Add(JObject.Parse(@"{'Key' : 3, 'DisplayName' : 'pippo 3', 'Email': 'pippo3@pippo.com'}"));
            //    result.Add(JObject.Parse(@"{'Key' : 4, 'DisplayName' : 'pippo 4', 'Email': 'pippo4@pippo.com'}"));
            //}
            //return result;
        }

        private string BuildMongoQuery(Dictionary<string,object> arguments)
        {
            string query = null;
            if (arguments != null)
            {
                JsonSerializerSettings jSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                jSettings.ContractResolver = new DefaultContractResolver();
                var dictionary = new Dictionary<string, object>();
                foreach (var key in arguments.Keys)
                {
                    if (arguments[key] is string)
                    {
                        var reg = new JObject();
                        reg["$regex"] = $"/*{arguments[key]}/*";
                        reg["$options"] = "si";
                        dictionary[key.ToPascalCase()] = reg;
                    }else
                    {
                        dictionary[key.ToPascalCase()] = arguments[key];
                    }
                }
                query = JsonConvert.SerializeObject(dictionary, jSettings);
            }

            return query;
        }
    }
}
