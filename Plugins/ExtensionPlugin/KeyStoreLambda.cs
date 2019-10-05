using RawCMS.Library;
using RawCMS.Plugins.KeyStore;
using RawCMS.Plugins.KeyStore.Contracts;
using RawCMS.Plugins.KeyStore.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionPlugin
{
    public class KeyStoreLambda : FactoryLambda
    {
        public override string PluginName => "RawCMS.Plugins.Core";

        public override Type OriginalType => typeof(IKeyStoreService);

        public override Type ReplacedWith => typeof(MyFakeStore);
    }


    public class MyFakeStore : IKeyStoreService
    {
        public object Get(string key)
        {
            return "fake";
        }

        public void Set(KeyStoreInsertModel insert)
        {
           //FAKE
        }
    }
}
