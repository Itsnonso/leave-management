using leave_management.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management
{
    public static class SeedData
    {
        public static void Seed(UserManager<Employee> usermanager, RoleManager<IdentityRole> rolemanager)
        {
            SeedRoles(rolemanager);
            SeedUsers(usermanager);
        }
        private static void SeedUsers(UserManager<Employee> usermanager)
        {
            if (usermanager.FindByNameAsync("admin@localhost.com").Result == null)
            {
                var user = new Employee
                {
                    UserName = "admin@localhost.com",
                    Email = "admin@localhost.com"

                };
                var result = usermanager.CreateAsync(user, "Password@1").Result;

                if (result.Succeeded)
                {
                    usermanager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }

        }
        private static void SeedRoles(RoleManager<IdentityRole> rolemanager)
        {
            if (!rolemanager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator",

                };
               var result = rolemanager.CreateAsync(role).Result;
            }

            if (!rolemanager.RoleExistsAsync("Employee").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Employee",

                };
                var result = rolemanager.CreateAsync(role).Result;
            }
        }
    }
}
