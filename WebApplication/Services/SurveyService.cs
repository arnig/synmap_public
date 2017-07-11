using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class SurveyService
    {
        private ApplicationDbContext db;

        public SurveyService()
        {
            db = new ApplicationDbContext();
        }

        public SurveyIndexViewModel GetIndexViewModel(string userId)
        {
            var roles = (from ur in db.Roles
                             select ur).ToList();

            List<string> userRoles = new List<string>();

            foreach (var ur in roles)
            {
                foreach (var user in ur.Users)
                {
                    if (user.UserId == userId)
                    {
                        userRoles.Add(ur.Name);
                    }
                }
            }

            return new SurveyIndexViewModel { UserRoles = userRoles };
        }
    }
}