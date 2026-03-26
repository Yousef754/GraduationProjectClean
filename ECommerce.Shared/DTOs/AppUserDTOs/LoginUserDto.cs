using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.AppUserDTOs
{
    public class LoginUserDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;


    }
}
