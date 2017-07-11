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

        public int GetLatestAlphabetResultBySession(string sessionId)
        {
            int surveyId = (from surveys in db.Surveys
                            where surveys.SessionId == sessionId
                            orderby surveys.DateStarted descending
                            select surveys.Id).FirstOrDefault();

            int abResultId = (from abr in db.AlphabetResults
                            where abr.SurveyId == surveyId
                            orderby abr.Id descending
                            select abr.Id).FirstOrDefault();

            return abResultId;
        }

        public bool FinishSurveyByAlphabetResult(int abResultId)
        {
            int surveyId = (from abr in db.AlphabetResults
                            where abr.Id == abResultId
                            select abr.SurveyId).SingleOrDefault();

            Survey survey = (from sv in db.Surveys
                             where sv.Id == surveyId
                             select sv).SingleOrDefault();

            survey.DateFinished = DateTime.Now;

            return Convert.ToBoolean(db.SaveChanges());
        }

        public SurveyResultViewModel GetLatestAsciiResultsBySession(string sessionId)
        {
            Survey survey = (from sv in db.Surveys
                             where sv.SessionId == sessionId
                             orderby sv.DateStarted descending
                             select sv).FirstOrDefault();

            if (survey == null)
            {
                return null;
            }

            AlphabetResult alphabetResult = (from abr in db.AlphabetResults
                                             where abr.SurveyId == survey.Id
                                             select abr).SingleOrDefault();

            IQueryable<AsciiResult> asciiResults = (from ar in db.AsciiResults
                                                    where ar.AlphabetResultId == alphabetResult.Id
                                                    select ar);

            //TODO: make foreach loop for the following..

            List<AsciiResult> firstAttempt = (from ar in asciiResults
                                              where ar.AttemptNumber == 1
                                              select ar).ToList();

            List<AsciiResult> secondAttempt = (from ar in asciiResults
                                               where ar.AttemptNumber == 2
                                               select ar).ToList();

            List<AsciiResult> thirdAttempt = (from ar in asciiResults
                                              where ar.AttemptNumber == 3
                                              select ar).ToList();

            List<float> calculations = new List<float>();

            //TODO: Create database entry for these values
            for (int i = 0; i < firstAttempt.Count; i++)
            {
                float v_r = (
                    (float)
                    (Math.Abs(firstAttempt.ElementAt(i).R - secondAttempt.ElementAt(i).R)
                    + Math.Abs(secondAttempt.ElementAt(i).R - thirdAttempt.ElementAt(i).R)
                    + Math.Abs(thirdAttempt.ElementAt(i).R - firstAttempt.ElementAt(i).R))
                    ) / 255;
                float v_g = (
                    (float)
                    (Math.Abs(firstAttempt.ElementAt(i).G - secondAttempt.ElementAt(i).G)
                    + Math.Abs(secondAttempt.ElementAt(i).G - thirdAttempt.ElementAt(i).G)
                    + Math.Abs(thirdAttempt.ElementAt(i).G - firstAttempt.ElementAt(i).G))
                    ) / 255;
                float v_b = (
                    (float)
                    (Math.Abs(firstAttempt.ElementAt(i).B - secondAttempt.ElementAt(i).B)
                    + Math.Abs(secondAttempt.ElementAt(i).B - thirdAttempt.ElementAt(i).B)
                    + Math.Abs(thirdAttempt.ElementAt(i).B - firstAttempt.ElementAt(i).B))
                    ) / 255;

                calculations.Add(v_r + v_g + v_b);
            }

            float score = calculations.Sum() / ((float) calculations.Count);

            return new SurveyResultViewModel {
                firstAttempt = firstAttempt,
                secondAttempt = secondAttempt,
                thirdAttempt = thirdAttempt,
                calculations = calculations,
                score = score
            };
        }

        public bool Edit(int id, AlphabetCreateViewModel editAlphabet)
        {
            Alphabet oldAlphabet = getAlphabetById(id);

            if ( (editAlphabet.Nation != oldAlphabet.Nation) || (editAlphabet.Description != oldAlphabet.Description))
            {
                oldAlphabet.Nation = editAlphabet.Nation;
                oldAlphabet.Description = editAlphabet.Description;

                if (!Convert.ToBoolean(db.SaveChanges()))
                {
                    return false;
                }
            }

            // Delete old values
            List<AsciiAlphabet> oldAsciiAlphabet = (from aa in db.AsciiAlphabets
                                 where aa.AlphabetId == id
                                 select aa).ToList();

            if (oldAsciiAlphabet.Count > 0)
            {
                db.AsciiAlphabets.RemoveRange(oldAsciiAlphabet);

                if (!Convert.ToBoolean(db.SaveChanges()))
                {
                    return false;
                }
            }

            // Insert new values
            for (int i = 0; i < editAlphabet.Characters.Length; i++)
            {
                char temp = editAlphabet.Characters.ElementAt(i);

                AsciiAlphabet asciiAlphabet = new AsciiAlphabet { AlphabetId = id, Ascii = (int)temp };

                db.AsciiAlphabets.Add(asciiAlphabet);
            }

            return Convert.ToBoolean(db.SaveChanges());
        }

        private Alphabet getAlphabetById(int id)
        {
            return (from ab in db.Alphabets
                    where ab.Id == id
                    select ab).SingleOrDefault();
        }

        public AlphabetCreateViewModel GetAlphabetCreateModel(int value)
        {
            throw new NotImplementedException();
        }

        public bool Add(AlphabetCreateViewModel newAlphabet)
        {
            Alphabet alphabet = new Alphabet { Nation = newAlphabet.Nation, Description = newAlphabet.Description };

            db.Alphabets.Add(alphabet);

            if (!Convert.ToBoolean(db.SaveChanges()))
            {
                return false;
            }

            int alphabetId = (from ab in db.Alphabets
                              orderby ab.Id descending
                              select ab.Id).FirstOrDefault();

            for (int i = 0; i < newAlphabet.Characters.Length; i++)
            {
                char temp = newAlphabet.Characters.ElementAt(i);

                AsciiAlphabet asciiAlphabet = new AsciiAlphabet { AlphabetId = alphabetId, Ascii = (int)temp };

                db.AsciiAlphabets.Add(asciiAlphabet);
            }

            return Convert.ToBoolean(db.SaveChanges());
        }

        internal SurveyResultViewModel GetAsciiResultsBySurveyId(int value)
        {
            throw new NotImplementedException();
        }

        public bool PostAsciiResults(AlphabetResultViewModel viewModel, int alphabetResultId)
        {
            foreach (var result in viewModel.results)
            {
                AsciiResult temp = new AsciiResult();

                temp.AttemptNumber = viewModel.attemptNumber;
                temp.Ascii = result.Ascii;
                temp.AlphabetResultId = alphabetResultId;
                temp.R = result.R;
                temp.G = result.G;
                temp.B = result.B;
                temp.A = result.A;

                db.AsciiResults.Add(temp);
            }

            return Convert.ToBoolean(db.SaveChanges());
        }

        public bool PostAlphabetResult(int value, string userId, string sessionId)
        {
            AlphabetResult alphabetResult = new AlphabetResult();

            //TODO: Select latest survey by session.
            int surveyId = (from sv in db.Surveys
                            where sv.SessionId == sessionId
                            orderby sv.DateStarted descending
                            select sv.Id).FirstOrDefault();

            alphabetResult.SurveyId = surveyId;
            alphabetResult.UserId = userId;
            alphabetResult.AlphabetId = value;
            
            db.AlphabetResults.Add(alphabetResult);
            //TODO: Attach userId to survey and remove from AlphabetResult

            return Convert.ToBoolean(db.SaveChanges());
        }

        public bool PostSurvey(string userId, string sessionId)
        {
            Survey survey = new Survey() { Type = 0, UserId = userId, SessionId = sessionId, DateStarted = DateTime.Now };

            db.Surveys.Add(survey);
            //TODO: Attach userId to survey and remove from AlphabetResult

            return Convert.ToBoolean(db.SaveChanges());
        }
    }
}