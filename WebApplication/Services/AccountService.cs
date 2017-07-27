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
    }
}