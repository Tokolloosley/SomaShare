using Microsoft.AspNetCore.Identity;
using SomaShare.Models;

namespace SomaShare.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Create roles
            string[] roles = { "Admin", "Seller", "Buyer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default users
            var admin = new ApplicationUser
            {
                UserName = "admin@somashare.com",
                Email = "admin@somashare.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                RegistrationDate = DateTime.UtcNow
            };

            var seller = new ApplicationUser
            {
                UserName = "seller@somashare.com",
                Email = "seller@somashare.com",
                EmailConfirmed = true,
                FirstName = "Thabo",
                LastName = "Mkhize",
                RegistrationDate = DateTime.UtcNow,
                AverageRating = 4.5
            };

            var buyer = new ApplicationUser
            {
                UserName = "buyer@somashare.com",
                Email = "buyer@somashare.com",
                EmailConfirmed = true,
                FirstName = "Nomsa",
                LastName = "Dlamini",
                RegistrationDate = DateTime.UtcNow,
                AverageRating = 4.0
            };

            // Add users if they don't exist
            if (await userManager.FindByEmailAsync(admin.Email) == null)
            {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync(seller.Email) == null)
            {
                await userManager.CreateAsync(seller, "Seller@123");
                await userManager.AddToRoleAsync(seller, "Seller");
            }

            if (await userManager.FindByEmailAsync(buyer.Email) == null)
            {
                await userManager.CreateAsync(buyer, "Buyer@123");
                await userManager.AddToRoleAsync(buyer, "Buyer");
            }

            await context.SaveChangesAsync();

            // Seed sample textbooks if none exist
            if (!context.Textbooks.Any())
            {
                var sellerUser = await userManager.FindByEmailAsync(seller.Email);

                var textbooks = new List<Textbook>
                {
                    new Textbook
                    {
                        Title = "Introduction to Algorithms",
                        Author = "Thomas H. Cormen",
                        ISBN = "978-0262033848",
                        Description = "The leading textbook on computer algorithms",
                        Edition = "3rd",
                        AskingPrice = 899.99m,
                        Condition = "Like New",
                        SellerId = sellerUser.Id,
                        IsAvailable = true,
                        ListedDate = DateTime.UtcNow,
                        ImageUrl = "https://via.placeholder.com/300x400?text=Algorithms"
                    },
                    new Textbook
                    {
                        Title = "Data Structures and Algorithms",
                        Author = "Mark Allen Weiss",
                        ISBN = "978-0132576277",
                        Description = "Comprehensive guide to DSA",
                        Edition = "3rd",
                        AskingPrice = 755.50m,
                        Condition = "Good",
                        SellerId = sellerUser.Id,
                        IsAvailable = true,
                        ListedDate = DateTime.UtcNow,
                        ImageUrl = "https://via.placeholder.com/300x400?text=DataStructures"
                    },
                    new Textbook
                    {
                        Title = "Database Systems",
                        Author = "C.J. Date",
                        ISBN = "978-0321884939",
                        Description = "Complete introduction to databases",
                        Edition = "12th",
                        AskingPrice = 1200.00m,
                        Condition = "New",
                        SellerId = sellerUser.Id,
                        IsAvailable = true,
                        ListedDate = DateTime.UtcNow,
                        ImageUrl = "https://via.placeholder.com/300x400?text=Databases"
                    }
                };

                context.Textbooks.AddRange(textbooks);
                await context.SaveChangesAsync();
            }

            // Seed sample wanted ads if none exist
            if (!context.WantedAds.Any())
            {
                var buyerUser = await userManager.FindByEmailAsync(buyer.Email);

                var wantedAds = new List<WantedAd>
                {
                    new WantedAd
                    {
                        Title = "Operating Systems Concepts",
                        Author = "Silberschatz, Galvin, Gagne",
                        ISBN = "978-1118063261",
                        Description = "Looking for the latest edition",
                        MaxPrice = 800.00m,
                        UserId = buyerUser.Id,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    },
                    new WantedAd
                    {
                        Title = "Computer Networks",
                        Author = "Andrew Tanenbaum",
                        ISBN = "978-0132126953",
                        Description = "Any condition accepted",
                        MaxPrice = 650.00m,
                        UserId = buyerUser.Id,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    }
                };

                context.WantedAds.AddRange(wantedAds);
                await context.SaveChangesAsync();
            }
        }
    }
}