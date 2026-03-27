using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Persistence.IdentityData.DataSeed
{
    public class IdentityDataIntializer : IDataIntializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataIntializer> _logger;

        public IdentityDataIntializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataIntializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task IntializeAsync()
        {
            try
            {
                _logger.LogInformation("Identity Seeding Started");

                // =======================
                // 1️⃣ Roles Seed
                // =======================
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                // =======================
                // 2️⃣ User 1 (Omar)
                // =======================
                var user1 = await _userManager.FindByEmailAsync("OmarAhmed@gmail.com");

                if (user1 == null)
                {
                    user1 = new ApplicationUser
                    {
                        DisplayName = "Omar Ahmed",
                        UserName = "OmarAhmed",
                        Email = "OmarAhmed@gmail.com",
                        PhoneNumber = "01070965586",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user1, "P@ssw0rd");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user1, "SuperAdmin");
                    }
                    else
                    {
                        _logger.LogError("Failed to create Omar user");
                    }
                }

                // =======================
                // 3️⃣ User 2 (Farida)
                // =======================
                var user2 = await _userManager.FindByEmailAsync("userone@gmail.com");

                if (user2 == null)
                {
                    user2 = new ApplicationUser
                    {
                        DisplayName = "user ",
                        UserName = "user",
                        Email = "userone@gmail.com",
                        PhoneNumber = "01070865586",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user2, "P@ssw0rd");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user2, "Admin");
                    }
                    else
                    {
                        _logger.LogError("Failed to create user1 user");
                    }
                }

                _logger.LogInformation("Identity Seeding Finished Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Seeding Identity Database");
            }
        }
    }
}
