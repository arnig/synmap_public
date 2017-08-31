using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.Entities;

namespace WebApplication.Services
{
    public class AccountService
    {
        private ApplicationDbContext db;

        public AccountService()
        {
            db = new ApplicationDbContext();
        }

        public AccountABSurveysViewModel GetSurveysByUserId(string userId)
        {
            List<Survey> surveys = (from sv in db.Surveys
                                    where (sv.UserId == userId) && (sv.DateFinished != null) 
                                    orderby sv.DateFinished ascending
                                    select sv).ToList();

            return new AccountABSurveysViewModel { Surveys = surveys }; 
        }

        public AccountIndexViewModel GetIndexViewModel(string userId)
        {
            var user = (from u in db.Users
                               where u.Id == userId
                               select u).SingleOrDefault();

            var roles = (from ur in db.Roles
                         select ur).ToList();

            var surveys = (from sv in db.Surveys
                           where sv.DateFinished.HasValue
                           orderby sv.DateFinished descending
                           select sv).Take(5).ToList();

            List<string> userRoles = new List<string>();

            foreach (var ur in roles)
            {
                foreach (var u in ur.Users)
                {
                    if (u.UserId == userId)
                    {
                        userRoles.Add(ur.Name);
                    }
                }
            }

            return new AccountIndexViewModel { User = user, UserRoles = userRoles, Surveys = surveys };
        }

        public AccountEditViewModel GetEditViewModel(string userId)
        {
            var user = (from u in db.Users
                        where u.Id == userId
                        select u).SingleOrDefault();

            return new AccountEditViewModel { UserName = user.UserName, Email = user.Email, PhoneNumber = user.PhoneNumber };
        }

        public bool Edit(string userId, AccountEditViewModel model)
        {
            var user = (from u in db.Users
                        where u.Id == userId
                        select u).SingleOrDefault();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            return Convert.ToBoolean(db.SaveChanges());
        }
    }
}