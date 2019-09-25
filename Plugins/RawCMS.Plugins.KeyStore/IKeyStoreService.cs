using RawCMS.Plugins.KeyStore.Model;

namespace RawCMS.Plugins.KeyStore
{
    public interface IKeyStoreService
    {
        object Get(string key);
        void Set(KeyStoreInsertModel insert);
    }
}