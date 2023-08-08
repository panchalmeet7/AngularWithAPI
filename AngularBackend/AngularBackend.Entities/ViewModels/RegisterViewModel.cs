using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.ViewModels
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public long PhoneNumber { get; set; }

        public string Password { get; set; } = null!;
        public string? Token { get; set; }

        public string? Role { get; set; }
    }
}
