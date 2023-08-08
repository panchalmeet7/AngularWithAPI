using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Email { get; set; }
        public string? EmailToken { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
