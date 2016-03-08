using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class AppSettings
    {
        public static T GetValue<T>(string key, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            T value = defaultValue;

            try
            {
                string configValue = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    value = (T)(Convert.ChangeType(configValue, typeof(T)));
                }
            }
            catch {}

            return value;
        }
    }
}
