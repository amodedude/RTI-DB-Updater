﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Interfaces.Download
{
    public interface IUriBuilder
    {
        string BuildUri(string param);
    }
}