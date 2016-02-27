using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Contracts.Models;

namespace HomeControl.Cloud.Contracts
{
    public class UglyTokenStore : ITokensStore
    {
        private readonly ConcurrentDictionary<string, StateIdentity> _store;

        public UglyTokenStore()
        {
            _store = new ConcurrentDictionary<string, StateIdentity>();
        }

        public string CreateToken(Models.StateIdentity identity)
        {
            string token = null;
            if (identity != null)
            {
                var entry = _store.Select(kvp => kvp).FirstOrDefault(kv => kv.Value.Group == identity.Group && kv.Value.Location == identity.Location && kv.Value.Realm == identity.Realm);
                if (!string.IsNullOrWhiteSpace(entry.Key))
                {
                    token = entry.Key;
                }
            }

            if (token == null)
            {
                token = Guid.NewGuid().ToString();

                if (!_store.TryAdd(token, identity))
                    token = null;
            }

            return token;
        }

        public Models.StateIdentity GetIdentity(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            StateIdentity result = null;
            _store.TryGetValue(token, out result);

            return result;
        }

        public Models.StateIdentity RemoveToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            StateIdentity removed = null;
            _store.TryRemove(token, out removed);

            return removed;
        }
    }
}
