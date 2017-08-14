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

            return new AccountIndexViewModel { UserRoles = userRoles };
        }
    }
}