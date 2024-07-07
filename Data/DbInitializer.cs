using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TL_Clothing.Interface;
using TL_Clothing.Models;

namespace TL_Clothing.Data
{
    public class DbInitializer:IDbInitializer
    {
        private readonly SqlDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Customer> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public DbInitializer(SqlDbContext db, RoleManager<IdentityRole> roleManager, UserManager<Customer> userManager, IUnitOfWork unitOfWork)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public void Initialize()
        {
            try
            {

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();




                    _userManager.CreateAsync(new Customer
                    {

                        UserName = "sbongakonkesihle31@gmail.com",
                        Email = "sbongakonkesihle31@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0657290039",
                        TwoFactorEnabled = true,
                        NormalizedUserName = "SBONGAKONKESIHLE31@GMAIL.COM",
                        NormalizedEmail = "SBONGAKONKESIHLE31@GMAIL.COM",
                    }, "Developer@Admin1234").GetAwaiter().GetResult();





                    Customer developer = _userManager.Users.FirstOrDefault(x => x.Email == "sbongakonkesihle31@gmail.com")!;

                    _userManager.AddToRoleAsync(developer, SD.Role_Admin).GetAwaiter().GetResult();

                
                }
            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
