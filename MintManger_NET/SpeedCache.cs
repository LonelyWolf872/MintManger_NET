using System;
using System.Collections.Generic;
using System.Text;

namespace MintManger_NET
{
    public class SpeedCache_Options
    {
        public bool Enabled;
    }
    public class SpeedCache
    {
        Dictionary<string, object> _speedCache;
        public SpeedCache()
        {
            _speedCache = new Dictionary<string, object>();
        }
        public T GetCached<T>(string c_key)
        {
            return (T)_speedCache[c_key];
        }
        public bool CheckCached(object c_obj)
        {
            try
            {
                return _speedCache.ContainsValue(c_obj);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckCached(string c_key)
        {
            try
            {
                return _speedCache.ContainsKey(c_key);
            }
            catch
            {
                return false;
            }
        }
    }
}
