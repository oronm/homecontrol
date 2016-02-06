using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl.Cloud.Contracts
{
    public class StateFeedClient : ClientBase<IStateFeed>, IStateFeed
    {
        public Task Feed(Models.UpdateLocationState newState)
        {
            return Channel.Feed(newState);
        }

        public Task Feed(Models.UpdatePersonState newPersonState)
        {
            return Channel.Feed(newPersonState);
        }


        public void test(string name)
        {
        }
    }
}
