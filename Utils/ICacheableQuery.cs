namespace KibeberiUniversity.Utils
{
    public interface ICacheableQuery
    {
         bool BypassCache{get;}
         string CacheKey{get;}
         bool RefreshCacheEntry{get;}
         bool ReplaceCacheEntry{get;}
    }
}