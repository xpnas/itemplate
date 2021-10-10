using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Project.Data.Models.System
{
    public class SystemDBStore
    {
        private static readonly Dictionary<string, string> m_systemInfos;

        static SystemDBStore()
        {
            m_systemInfos = SystemDBManager.Instance.DBase.Query<SystemConfig>().ToList().ToDictionary(e => e.key, e => e.Value);
        }

        public static string GetSystemValue(string key)
        {
            var result = SystemDBManager.Instance.DBase.Query<SystemConfig>().Where(e => e.key == key).FirstOrDefault();
            if (result == null) return "";
            return result.Value;

        }

        public static void SetSystemValue(string key, string value)
        {
            m_systemInfos[key] = value ?? "";
            var systemInfo = new SystemConfig(key, m_systemInfos[key]);
            if (SystemDBManager.Instance.DBase.Query<SystemConfig>().Where(e => e.key == key).Any())
            {
                SystemDBManager.Instance.DBase.Delete(systemInfo);
            }
            SystemDBManager.Instance.DBase.Insert(systemInfo);
        }
    }
}
