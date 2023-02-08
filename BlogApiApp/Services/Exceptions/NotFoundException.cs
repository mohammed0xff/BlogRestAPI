using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public abstract class NotFoundException : CustomException
    {
        protected NotFoundException(string message)
        : base(message)
        { 
            StatusCode = HttpStatusCode.NotFound;
        }

    }
}
