using CurrencyRepo.Abstracts;

namespace CurrencyRepo.Infrastructure
{
    public class CacheKeysStore : ICacheKeyStore
    {
        private List<string> _cacheKeys = new();

        public List<string> CacheKeys
        {
            get => _cacheKeys;
            set
            {
                _cacheKeys = value;
            }
        }

        public CacheKeysStore() { }
        
        public string this[int index] 
        {
            get {  return _cacheKeys[index]; }
            set { _cacheKeys[index] = value; }
        }

        public void Remove(string key) 
        {
            _cacheKeys.Remove(key);
        }

        public int Count() => _cacheKeys.Count;

        public void Clear() 
        {
            _cacheKeys.Clear();
        }

        public void AddKey(string key) 
        {
            _cacheKeys.Add(key);
        }

        public bool Contains(string key) 
        {
            return _cacheKeys.Any(x => x.Equals(key));
        }
    }
}
