using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HomeControl
{
    public class LocalFileResidentsRepository : IResidentsRepository
    {
        private readonly string connectionString;
        private readonly StreamReader repositoryHandle;

        public LocalFileResidentsRepository(string connectionString)
        {
            this.connectionString = connectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            if (!File.Exists(connectionString))
            {
                throw new ArgumentException(string.Format("File not found: {0}",connectionString), "connectionString");
            }
            repositoryHandle = loadRepository(connectionString); 
        }

        private StreamReader loadRepository(string connectionString)
        {
            return new StreamReader(new FileStream(connectionString, FileMode.Open));
        }

        public IEnumerable<IdentifyingDevice> GetIdentifyingDevices()
        {
            return JsonConvert.DeserializeObject<IdentifyingDevice[]>(repositoryHandle.ReadToEnd());
        }
    }
}
