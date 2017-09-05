using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ManageService
    {

        private ApplicationDbContext db;

        public ManageService()
        {
            db = new ApplicationDbContext();
        }
        
        public ManageAccountsViewModel GetManageAccountsViewModel(string userId)
        {
            ManageAccountsViewModel vm = new ManageAccountsViewModel() { Users = new List<ManageAccount>() };

            List<ApplicationUser> users = (from u in db.Users
                         where u.Id != userId
                         orderby u.Email
                         select u).ToList();

            var roles = (from r in db.Roles
                         select r).ToList();

            foreach (var user in users)
            {
                ManageAccount temp = new ManageAccount() { Roles = new List<string>() };

                temp.User = user;

                foreach (var ur in roles)
                {
                    foreach (var u in ur.Users)
                    {
                        if (u.UserId == user.Id)
                        {
                            temp.Roles.Add(ur.Name);
                        }
                    }
                }

                vm.Users.Add(temp);
            }

            return vm;
        }

        public bool PromoteAdmin(string email)
        {
            var user = (from u in db.Users
                        where u.Email == email
                        select u).SingleOrDefault();

            if (user == null)
                return false;

            var role = (from ur in db.Roles
                         where ur.Name == "Admin"
                        select ur).SingleOrDefault();

            IdentityUserRole newRole = new IdentityUserRole() { RoleId = role.Id, UserId = user.Id };

            role.Users.Add(newRole);

            return Convert.ToBoolean(db.SaveChanges());
        }

        public bool RevokeAdmin(string email)
        {
            var user = (from u in db.Users
                        where u.Email == email
                        select u).SingleOrDefault();

            if (user == null)
                return false;

            var role = (from ur in db.Roles
                        where ur.Name == "Admin"
                        select ur).SingleOrDefault();

            foreach (var u in role.Users)
            {
                if (u.UserId == user.Id)
                {
                    role.Users.Remove(u);
                    break;
                }
            }

            return Convert.ToBoolean(db.SaveChanges());
        }
    }
}