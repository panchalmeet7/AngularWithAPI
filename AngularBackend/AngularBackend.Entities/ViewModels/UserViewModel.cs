using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.Models.ViewModels
{
    public class UserViewModel
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
