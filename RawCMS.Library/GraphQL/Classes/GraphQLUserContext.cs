using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RawCMS.Library.GraphQL.Classes
{
    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
    }
}
