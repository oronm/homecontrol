using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Cloud.Contracts.Models;

namespace HomeControl.Cloud.Contracts
{
    public interface ITokensStore
    {
        string CreateToken(StateIdentity identity);
        StateIdentity GetIdentity(string token);
        StateIdentity RemoveToken(string token);
    }
}
