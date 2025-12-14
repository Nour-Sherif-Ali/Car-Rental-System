using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.User
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
