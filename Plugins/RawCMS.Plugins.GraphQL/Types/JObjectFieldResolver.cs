﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Min�</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RawCMS.Library.DataModel;
using RawCMS.Plugins.GraphQL.Classes;
using System;
using System.Collections.Generic;

namespace RawCMS.Plugins.GraphQL.Types
{
    public class JObjectFieldResolver : IFieldResolver
    {
        private readonly GraphQLService _graphQLService;

        public JObjectFieldResolver(GraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        public object Resolve(ResolveFieldContext context)
        {
            ItemList result;
            if (context.Arguments != null && context.Arguments.Count > 0)
            {
                int pageNumber = 1;
                int pageSize = 1000;
                if (context.Arguments.ContainsKey("pageNumber"))
                {
                    pageNumber = int.Parse(context.Arguments["pageNumber"].ToString());
                    if (pageNumber < 1)
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

                result = _graphQLService.service.Query(context.FieldName.ToPascalCase(), new DataQuery()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    RawQuery = BuildMongoQuery(context.Arguments)
                });
            }
            else
            {
                result = _graphQLService.service.Query(context.FieldName.ToPascalCase(), new DataQuery()
                {
                    PageNumber = 1,
                    PageSize = 1000,
                    RawQuery = null
                });
            }

            return result.Items.ToObject<List<JObject>>();
        }

        private string BuildMongoQuery(Dictionary<string, object> arguments)
        {
            string query = null;
            if (arguments != null)
            {
                JsonSerializerSettings jSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                if (arguments.ContainsKey("rawQuery"))
                {
                    query = Convert.ToString(arguments["rawQuery"]);
                }
                else
                {

                    jSettings.ContractResolver = new DefaultContractResolver();
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (string key in arguments.Keys)
                    {
                        if (arguments[key] is string)
                        {
                            JObject reg = new JObject
                            {
                                ["$regex"] = $"/*{arguments[key]}/*",
                                ["$options"] = "si"
                            };
                            dictionary[key.ToPascalCase()] = reg;
                        }
                        else
                        {
                            dictionary[key.ToPascalCase()] = arguments[key];
                        }
                    }
                    query = JsonConvert.SerializeObject(dictionary, jSettings);
                }
            }

            return query;
        }
    }
}