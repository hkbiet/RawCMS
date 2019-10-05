

using RawCMS.Plugins.KeyStore.Contracts.Model;

namespace RawCMS.Plugins.KeyStore.Contracts
{
    public interface IKeyStoreService
    {
        object Get(string key);
        void Set(KeyStoreInsertModel insert);
    }
}