using AngularBackend.Entities.Data;
using AngularBackend.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Repository.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly GetConnString _getConnString;
        public AccountRepository(GetConnString getConnString)
        {
            _getConnString =  getConnString;
        }

        public string shdfkjs()
        {
            var con = _getConnString.Connection();
            return "dfgdf";
        }
    }
}
