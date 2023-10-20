using System.Runtime.Caching;

namespace WebApi.Services.CacheServices
{
    public class CacheService : ICacheService
    {
        private ObjectCache _memoryCache = MemoryCache.Default;
        public T GetData<T>(string key)
        {
            try
            {
                T item = (T)_memoryCache.Get(key);

                return item;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public object RemoveData(string key)
        {
            var result = true;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                   var res = _memoryCache.Remove(key);
                }
                else
                    result = false;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SetData<T>(string key, T entity, DateTimeOffset exprirationTime)
        {
            var result = true;
            try
            {
                if(!string.IsNullOrEmpty(key) && entity!=null)
                    _memoryCache.Set(key, entity, exprirationTime);
                else
                   result = false;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
