using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeControl.Local.Contracts;

namespace HomeControl.Local
{
    public interface IStateReporter
    {
        void Init(IDictionary<string, string> configuration);
        Task ReportPersonState(PersonState personState);
        Task ReportLocationState(IEnumerable<PersonState> personState);
    }
}
