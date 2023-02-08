using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions.Users
{
    public class UsernameAlreadyExistsException : CustomException
    {
        public UsernameAlreadyExistsException(string username) 
            :base($"Username {username} Already Exits!")
        {
        }

    }
}
