using Newtonsoft.Json;
using RawCMS.Library.KeyStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RawCMS.Plugins.KeyStoreOverride
{
    public class KeyStoreServiceOverride : IKeyStoreService
    {
        public object Get(string key)
        {
            return File.ReadAllText(@"c:\temp\key.txt");
        }

        public void Set(KeyStoreInsertModel insert)
        {
            File.WriteAllText(@"c:\temp\key.txt", JsonConvert.SerializeObject(insert));
        }
    }
}
