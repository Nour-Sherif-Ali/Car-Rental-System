using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.User
{
   
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
