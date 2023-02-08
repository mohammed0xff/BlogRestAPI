using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions.Users
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string username)
            : base($"User with username : {username} Not found!")
        {
        }

    }
}
