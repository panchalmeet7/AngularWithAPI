﻿using AngularBackend.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Repository.Interface
{
    public interface IEmailRepository
    {
        void SendEmail( string recipient, string subject, string body);
    }
}
