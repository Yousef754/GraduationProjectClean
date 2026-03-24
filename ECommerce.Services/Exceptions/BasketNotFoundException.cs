using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Exceptions
{
    public class BasketNotFoundExceptions : Exception
    {
        public BasketNotFoundExceptions() { }

        public BasketNotFoundExceptions(string basketId)
            : base($"Basket with ID '{basketId}' was not found.")
        {
        }
    }
}
