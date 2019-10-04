using RawCMS.Library;
using RawCMS.Plugins.KeyStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionPlugin
{
    public class KeyStoreLambda : FactoryLambda
    {
        public override string PluginName => "RawCMS.Plugins.Core";

        public override Type OriginalType => typeof(KeyStoreService);

        public override Type ReplacedWith => typeof(MyFakeStore);
    }


    public class MyFakeStore : KeyStoreService
    {
        public override object Get(string key)
        {
            return "this was hacked";
        }
    }
}
