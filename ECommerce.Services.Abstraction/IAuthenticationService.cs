using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.IdentityDTOs;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IAuthenticationService
    {
        
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);

        
        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);

       

        Task<bool> CheckEmailAsync(string email);

        

        Task<Result<UserDTO>> GetUserByEmailAsync(string email);

       
        Task<Result<AddressDTO>> GetAddressAsync(string email);

        
        Task<Result<AddressDTO>> UpdateUserAddressAsync(string email, AddressDTO addressDTO);
    }
}
