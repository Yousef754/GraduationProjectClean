using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Settings
{
    public class EmailSettings
    {
        public string From { get; set; } = default!;
        public string SmtpHost { get; set; } = default!;
        public int SmtpPort { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
