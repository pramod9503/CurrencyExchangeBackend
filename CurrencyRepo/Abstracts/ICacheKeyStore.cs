namespace CurrencyRepo.Abstracts
{
    public interface ICacheKeyStore
    {
        List<string> CacheKeys { get; set;}

        void Remove(string key);

        int Count();

        void Clear();

        void AddKey(string key);

        bool Contains(string key);
    }
}
