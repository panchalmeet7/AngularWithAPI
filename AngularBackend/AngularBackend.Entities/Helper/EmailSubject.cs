using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.Helper
{
    public static class EmailSubject
    {
        public static string EmailStringBody(string email, string token)
        {
            return
                $@"<html>
                      <body>
                            <div>
                              <h1>Reset your password!!</h1>
                              <hr />
                              <p>password change link to reset your password!</p>
                              <p>click the link given below to reset your password!</p>
                              <a href=""http://localhost:4200/reset-password?email={email}&token={token}"" target=""_blank"">reset password</a> <p>kind regards<br /><br /> XYZ!</p>
                            </ div >
                    </ body >
                 </ html > ";
        }
    }
}
