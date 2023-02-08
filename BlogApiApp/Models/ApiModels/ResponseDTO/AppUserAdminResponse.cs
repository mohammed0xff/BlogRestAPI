using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ApiModels.ResponseDTO
{
    public class AppUserAdminResponse : AppUserResponse
    {
        public bool IsSuspended {get; set;}
    }
}
