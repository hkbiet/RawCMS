using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RawCMS.Library.GraphQL.Types
{
    public class JObjectFieldResolver : IFieldResolver
    {
        public object Resolve(ResolveFieldContext context)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"http://localhost:57564/api/CRUD/{context.FieldName.ToPascalCase()}/5c208311be03fb245cd6fde5", HttpCompletionOption.ResponseContentRead).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            return JObject.Parse(result)["data"]["items"];
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
    }
}
