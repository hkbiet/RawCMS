using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RawCMS.Library.GraphQL.Classes
{
    public class GraphQLSettings
    {
        public PathString Path => "/api/graphql";
        public Func<HttpContext, object> BuildUserContext { get; set; }
        public bool EnableMetrics { get; set; }
    }
}
