using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Settings
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = default!;
        public string IntegrationId { get; set; } = default!;
        public string IframeId { get; set; } = default!;
        public string HmacSecret { get; set; } = default!;
    }
}
