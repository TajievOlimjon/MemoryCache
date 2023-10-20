namespace WebApi.Services.CacheServices
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key,T entity,DateTimeOffset exprirationTime);
        object RemoveData(string key);
    }
}

