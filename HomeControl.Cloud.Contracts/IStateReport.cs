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
    public interface IStateReport
    {
        [OperationContract]
        IEnumerable<PersonState> GetLocationState(string Realm, string Group, string Location);
    }
}
