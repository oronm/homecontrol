using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Contracts.Models;

namespace HomeControl.Cloud.Contracts
{
    [ServiceContract]
    public interface IStateFeed
    {
        [OperationContract]
        Task Feed(UpdateLocationState newState);

        [OperationContract]
        Task Feed(UpdatePersonState newPersonState);
    }
}
