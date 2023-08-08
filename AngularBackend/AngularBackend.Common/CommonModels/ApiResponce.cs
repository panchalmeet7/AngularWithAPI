﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Common.CommonModels
{
    public class ApiResponce<T>
    {
        public T? Data { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
