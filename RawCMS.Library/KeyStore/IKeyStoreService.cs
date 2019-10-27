using System;
using System.Collections.Generic;
using System.Text;

namespace RawCMS.Library.KeyStore
{
    public interface IKeyStoreService
    {
        object Get(string key);

        void Set(KeyStoreInsertModel insert);
    }
}
