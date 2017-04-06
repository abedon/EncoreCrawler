using ASPNETCore_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_Core.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(EncoreContext context, ApplicationDbContext usercontext, UserManager<ApplicationUser> userManager)
        {
            // Look for new roles
            string[] roles = new string[] { "Admin", "User" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(usercontext);

                if (!usercontext.Roles.Any(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new IdentityRole(role)
                    {
                        //unable to assign roles if NormalizedName is not set 
                        NormalizedName = role.ToUpper()
                    });
                }
            }

            var name = "abedon";
            var email = "d@ve.gs";
            var phone = "+1234567890";

            // create a new user
            var user = new ApplicationUser
            {
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                PhoneNumber = phone,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            var userStore = new UserStore<ApplicationUser>(usercontext);

            if (!usercontext.Users.Any(u => u.Email == user.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "Ve.gs777");
                user.PasswordHash = hashed;                
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, roles.First());
            }
            else
            {
                var existingUser = usercontext.Users.Where(u => u.NormalizedEmail == user.NormalizedEmail).SingleOrDefault();
                //another way to assign roles
                await userManager.AddToRolesAsync(existingUser, roles);
            }

            await usercontext.SaveChangesAsync();
            
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            try
            {
                // Look for any encore.
                if (context.Encores.Any())
                {
                    return;   // DB has been seeded
                }
            }
            catch (Exception ex)
            {
                var aaa = ex.Message;
            }
            

            context.Encores.Add(new Encore
            {
                LottoType = LottoTypeEnum.LottoMAX,
                DrawType = DrawTypeEnum.Evening,
                WinninNumber = "0000000",
                DrawDate = DateTime.Parse("2000-01-01"),
                TotalCashWon = 541254
            });
            
            context.SaveChanges();

            var encoreMatches = new EncoreMatch[]
            {
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.IIIIIII, TicketsWon=0, Prize=1000000.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OIIIIII, TicketsWon=1, Prize=100000.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OOIIIII, TicketsWon=10, Prize=1000.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OOOIIII, TicketsWon=128, Prize=100.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OOOOIII, TicketsWon=1257, Prize=10.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OOOOOII, TicketsWon=12260, Prize=5.00 },
            new EncoreMatch{ EncoreID=1, MatchType=MatchTypeEnum.OOOOOOI, TicketsWon=121468, Prize=2.00 },
            };
            foreach (EncoreMatch match in encoreMatches)
            {
                context.EncoreMatches.Add(match);
            }
            context.SaveChanges();            
        }
    }
}
