﻿using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RawCMS.Library.Core;
using RawCMS.Library.Core.Exceptions;
using RawCMS.Library.Core.Interfaces;
using RawCMS.Library.DataModel;
using RawCMS.Library.Lambdas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RawCMS.Library.Service
{
    public class CRUDService : IRequireApp
    {
        private readonly MongoService _mongoService;
        private readonly MongoSettings _settings;
        private readonly List<string> collectionNames= new List<string>();
        private AppEngine lambdaManager;

        private readonly JsonWriterSettings js = new JsonWriterSettings()
        {
            OutputMode = JsonOutputMode.Strict,
            GuidRepresentation = GuidRepresentation.CSharpLegacy
        };

        public CRUDService(MongoService mongoService, IOptions<MongoSettings> settings)
        {
            _mongoService = mongoService;
            _settings = settings.Value;
            LoadCollectionNames();
            
        }

        private void LoadCollectionNames()
        {
            foreach (var collection in this._mongoService.GetDatabase().ListCollections().ToEnumerable())
            {
                this.collectionNames.Add(collection["name"].AsString);
            }
        }

        public JObject Insert(string collection, JObject newitem)
        {
            var dataContext = new Dictionary<string, object>();
            InvokeValidation(newitem, collection);

            EnsureCollection(collection);

            InvokeProcess(collection, ref newitem, SavePipelineStage.PreSave, dataContext);

            string json = newitem.ToString();
            BsonDocument itemToAdd = BsonSerializer.Deserialize<BsonDocument>(json);
            if (itemToAdd.Contains("_id"))
            {
                itemToAdd.Remove("_id");
            }

            _mongoService.GetCollection<BsonDocument>(collection).InsertOne(itemToAdd);

            //sanitize
            itemToAdd["_id"] = itemToAdd["_id"].ToString();

            var addedItem = JObject.Parse(itemToAdd.ToJson(js));

            InvokeProcess(collection, ref addedItem, SavePipelineStage.PostSave, dataContext);

            return addedItem;
        }

        private void InvokeValidation(JObject newitem, string collection)
        {
            List<Error> errors = Validate(newitem, collection);
            if (errors.Count > 0)
            {
                throw new ValidationException(errors, null);
            }
        }

        private void InvokeProcess(string collection, ref JObject item, SavePipelineStage save, Dictionary<string, object> dataContext)
        {
            List<Lambda> processhandlers = lambdaManager.Lambdas
                .Where(x => x is DataProcessLambda)
                .Where(x => ((DataProcessLambda)x).Stage == save)
                .ToList();

            foreach (DataProcessLambda h in processhandlers)
            {
                h.Execute(collection, ref item, ref dataContext);
            }
        }

        private void InvokeAlterQuery(string collection,  FilterDefinition<BsonDocument>  query)
        {
            List<Lambda> genericAlter = lambdaManager.Lambdas
                .Where(x => x is AlterQueryLambda)
                .ToList();

            foreach (AlterQueryLambda h in genericAlter)
            {
                h.Alter(collection,  query);
            }


            List<CollectionAlterQueryLambda> collectionAlter = lambdaManager.Lambdas
                .Where(x => x is CollectionAlterQueryLambda)
                .Cast<CollectionAlterQueryLambda>()
                .Where(x=> Regex.IsMatch(collection,x.Collection))
                .ToList();

            foreach (CollectionAlterQueryLambda h in collectionAlter)
            {
                h.Alter(  query);
            }
        }

        public JObject Update(string collection, JObject item, bool replace)
        {
            var dataContext = new Dictionary<string, object>();
            //TODO: why do not manage validation as a simple presave process?
            InvokeValidation(item, collection);

            //TODO: create collection if not exists
            EnsureCollection(collection);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", BsonObjectId.Create(item["_id"].Value<string>()));

            InvokeProcess(collection, ref item, SavePipelineStage.PreSave, dataContext);

            var id = item["_id"].Value<string>();
            //insert id (mandatory)
            BsonDocument doc = BsonDocument.Parse(item.ToString());
            doc["_id"] = BsonObjectId.Create(id);

            UpdateOptions o = new UpdateOptions()
            {
                IsUpsert = true,
                BypassDocumentValidation = true
            };

            if (replace)
            {
                _mongoService.GetCollection<BsonDocument>(collection).ReplaceOne(filter, doc, o);
            }
            else
            {
                BsonDocument dbset = new BsonDocument("$set", doc);
                _mongoService.GetCollection<BsonDocument>(collection).UpdateOne(filter, dbset, o);
            }
            var fullSaved = Get(collection, id);
            InvokeProcess(collection, ref fullSaved, SavePipelineStage.PostSave, dataContext);
            return JObject.Parse(fullSaved.ToJson(js));
        }

        public void EnsureCollection(string collection)
        {
           
                if (!this.collectionNames.Any(x => x.Equals(collection)))
                {
                    _mongoService.GetDatabase().CreateCollection(collection);
                    this.collectionNames.Add(collection);
                }
        }

        public bool Delete(string collection, string id)
        {
            EnsureCollection(collection);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", BsonObjectId.Create(id));

            DeleteResult result = _mongoService.GetCollection<BsonDocument>(collection).DeleteOne(filter);

            return result.DeletedCount == 1;
        }

        public JObject Get(string collection, string id)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", BsonObjectId.Create(id));

            IFindFluent<BsonDocument, BsonDocument> results = _mongoService
                .GetCollection<BsonDocument>(collection)
                .Find<BsonDocument>(filter);

            List<BsonDocument> list = results.ToList();

            BsonDocument item = list.FirstOrDefault();
            string json = "{}";
            //sanitize id format
            if (item != null)
            {
                item["_id"] = item["_id"].ToString();
                json = item.ToJson(js);
            }

            return JObject.Parse(json);
        }

        public long Count(string collection,string query)
        {
            FilterDefinition<BsonDocument> filter = FilterDefinition<BsonDocument>.Empty;
            if (!string.IsNullOrWhiteSpace(query))
            {
                filter = new JsonFilterDefinition<BsonDocument>(query);
            }
            return Count(collection, filter);
        }

            public long Count(string collection, FilterDefinition<BsonDocument> filter)
        {
            InvokeAlterQuery(collection, filter);
            long count = _mongoService
               .GetCollection<BsonDocument>(collection).Find<BsonDocument>(filter).Count();
            return count;
        }

        public ItemList Query(string collection, DataQuery query)
        {
            FilterDefinition<BsonDocument> filter = FilterDefinition<BsonDocument>.Empty;
            if (query.RawQuery != null)
            {
                filter = new JsonFilterDefinition<BsonDocument>(query.RawQuery);
            }

            InvokeAlterQuery(collection, filter);

            IFindFluent<BsonDocument, BsonDocument> results = _mongoService
                .GetCollection<BsonDocument>(collection).Find<BsonDocument>(filter)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Limit(query.PageSize);

            long count =Count(collection, filter);

            List<BsonDocument> list = results.ToList();

            //sanitize id format
            foreach (BsonDocument item in list)
            {
                item["_id"] = item["_id"].ToString();
            }

            string json = list.ToJson(js);

            return new ItemList(JArray.Parse(json), (int)count, query.PageNumber, query.PageSize);
        }

        public List<Error> Validate(JObject item, string collection)
        {
            List<Error> result = new List<Error>();
            result.AddRange(ValidateGeneric(item, collection));
            result.AddRange(ValidateSpecific(item, collection));

            return result;
        }

        private List<Error> ValidateSpecific(JObject item, string collection)
        {
            List<Error> result = new List<Error>();

            List<Lambda> labdas = lambdaManager.Lambdas
                .Where(x => x is CollectionValidationLambda)
                .Where(x => ((CollectionValidationLambda)x).TargetCollections.Contains(collection))
                .ToList();

            foreach (CollectionValidationLambda lambda in labdas)
            {
                List<Error> errors = lambda.Validate(item);
                result.AddRange(errors);
            }

            return result;
        }

        private List<Error> ValidateGeneric(JObject item, string collection)
        {
            List<Error> result = new List<Error>();

            List<Lambda> labdas = lambdaManager.Lambdas
                .Where(x => x is SchemaValidationLambda).ToList();

            foreach (SchemaValidationLambda lambda in labdas)
            {
                List<Error> errors = lambda.Validate(item, collection);
                result.AddRange(errors);
            }

            return result;
        }

        public void SetAppEngine(AppEngine manager)
        {
            lambdaManager = manager;
        }
    }
}