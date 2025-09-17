using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers
{
    public static class DbIntializer
    {

        public static async Task SeedUsersAndRoles(UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            //roles
            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in AppRoles.All)
                {
                    if(!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    }
                }
            }
            //Users with Roles
            if(!userManager.Users.Any(n => !string.IsNullOrEmpty(n.Email)))
            {
                var userPassword = "Asd1234@";
                var newUser = new User()
                {
                    UserName = "mohamed.gamal",
                    Email = "m1@yahoo.com",
                    FullName = "Mohamed Gamal",
                    ProfilePictureUrl = "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?semt=ais_incoming&w=740&q=80",
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(newUser, userPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, AppRoles.User);
                }



                var newAdmin = new User()
                {
                    UserName = "admin.admin",
                    Email = "admin@yahoo.com",
                    FullName = "Mohamed Admin",
                    ProfilePictureUrl = "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?semt=ais_incoming&w=740&q=80",
                    EmailConfirmed = true,
                };
                var resultNewAdmin = await userManager.CreateAsync(newAdmin, userPassword);

                if (resultNewAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
                }
            }
        }
        public async static Task SeedAsync(AppDbContext appDbContext)
        {
            if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            {
                var newUser = new User()
                {
                   FullName = "Mohamed Jamal",
                   ProfilePictureUrl = "https://media.istockphoto.com/id/1682296067/photo/happy-studio-portrait-or-professional-man-real-estate-agent-or-asian-businessman-smile-for.jpg?s=612x612&w=0&k=20&c=9zbG2-9fl741fbTWw5fNgcEEe4ll-JegrGlQQ6m54rg="
                };

                await appDbContext.Users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                var newPostWithoutImage = new Post()
                {
                    Content = "Hi this my first post which is being loaded from database",
                    ImageUrl = "",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UserId = newUser.Id
                };

                var newPostWithImage = new Post()
                {
                    Content = "Hi this my first post which is being loaded from database, " +
                    "this post has an image",
                    ImageUrl = "https://images.unsplash.com/photo-1575936123452-b67c3203c357?fm=jpg&q=60&w=3000&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8aW1hZ2V8ZW58MHx8MHx8fDA%3D",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UserId = newUser.Id
                };

                await appDbContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
                await appDbContext.SaveChangesAsync();
            }
            
        }
    }
}
