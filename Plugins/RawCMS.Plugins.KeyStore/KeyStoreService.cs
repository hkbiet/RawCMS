using RawCMS.Plugins.KeyStore.Model;
using System.Collections.Generic;

namespace RawCMS.Plugins.KeyStore
{
    public class KeyStoreService: IKeyStoreService
    {
        private static readonly Dictionary<string, object> db = new Dictionary<string, object>();

        public virtual object Get(string key)
        {
            return db[key];
        }

        public virtual void Set(KeyStoreInsertModel insert)
        {
            db[insert.Key] = insert.Value;
        }
    }
}