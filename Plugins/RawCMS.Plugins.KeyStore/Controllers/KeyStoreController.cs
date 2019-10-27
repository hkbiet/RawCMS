using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RawCMS.Library.Core.Attributes;
using RawCMS.Library.KeyStore;

namespace RawCMS.Plugins.KeyStore.Controllers
{
    [AllowAnonymous]
    [RawAuthentication]
    [Route("api/[controller]")]
    public class KeyStoreController : Controller
    {
        private IKeyStoreService service;

        public KeyStoreController(IKeyStoreService service)
        {
            this.service = service;
        }

        [HttpHead("{key}")]
        public void Get(string key)
        {
            // var content = new OkResult();
            StringValues result = new StringValues(new string[] { service.Get(key) as string });
            Response.Headers.Add("r", result);
            //return content;
        }

        [HttpPost()]
        public void Set([FromBody]KeyStoreInsertModel insert)
        {
            service.Set(insert);
        }
    }
}