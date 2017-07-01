using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.Entities;
using Microsoft.AspNet.Identity;

namespace WebApplication.Services
{
    public class AlphabetService
    {
        private ApplicationDbContext db;

        public AlphabetService()
        {
            db = new ApplicationDbContext();
        }

        public AlphabetsViewModel getAlphabets()
        {
            List<Alphabet> alphabetList = (from ab in db.Alphabets
                                select ab).ToList();

            return new AlphabetsViewModel { alphabets = alphabetList };
        }

        public AlphabetViewModel getAlphabet(int id)
        {
            Alphabet alphabet= (from ab in db.Alphabets
                                  where ab.Id == id
                                  select ab).SingleOrDefault();
            List<int> ascii = (from x in db.AsciiAlphabets
                               where x.AlphabetId == id
                               select x.Ascii).ToList();

            return new AlphabetViewModel { Ascii = ascii, Alphabet = alphabet};
        }

        public bool PostAlphabetResult(int value, string userId)
        {
            AlphabetResult alphabetResult = new AlphabetResult();

            //TODO: Select latest survey from user.
            int surveyId = (from sv in db.Surveys
                            orderby sv.Id descending
                            select sv.Id).FirstOrDefault();

            alphabetResult.SurveyId = surveyId;
            alphabetResult.UserId = userId;
            alphabetResult.AlphabetId = value;
            
            db.AlphabetResults.Add(alphabetResult);
            //TODO: Attach userId to survey and remove from AlphabetResult

            return Convert.ToBoolean(db.SaveChanges());
        }

        public bool PostSurvey()
        {
            Survey survey = new Survey() { Type = 0 };

            db.Surveys.Add(survey);
            //TODO: Attach userId to survey and remove from AlphabetResult

            return Convert.ToBoolean(db.SaveChanges());
        }
    }
}