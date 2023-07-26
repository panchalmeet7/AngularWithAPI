using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.Data
{
    public class GetConnString
    {
        public readonly IConfiguration _configManager;


        public GetConnString(IConfiguration configurationManager)
        {
            _configManager = configurationManager;
        }

        public string? Connection()
        {
            return _configManager.GetConnectionString("conn");
        }
    }
}
