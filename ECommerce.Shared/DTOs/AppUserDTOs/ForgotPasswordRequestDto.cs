using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.AppUserDTOs
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = default!;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmNewPassword { get; set; } = default!;
    }
}
