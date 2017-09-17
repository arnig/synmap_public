using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Drawing;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication.Scripts;

namespace WebApplication.Services
{
    public class AlphabetService
    {
        private ApplicationDbContext db;

        public AlphabetService()
        {
            db = new ApplicationDbContext();
        }

        public AlphabetIndexViewModel getAlphabets(string userId)
        {
            List<Alphabet> alphabetList = (from ab in db.Alphabets
                                           orderby ab.Description.ToLower()
                                select ab).ToList();

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

            return new AlphabetIndexViewModel { Alphabets = alphabetList, UserRoles = userRoles };
        }

        public AlphabetParticipateViewModel GetAlphabetParticipateModel(int value)
        {
            AlphabetParticipateViewModel vm = new AlphabetParticipateViewModel();

            vm.Alphabet = getAlphabetById(value);

            vm.Ascii = getAsciiByAlphabet(value);

            return vm;
        }

        public bool AddAnonIdentity(SurveyInfoViewModel viewModel, int surveyId)
        {
            Survey survey = (from sv in db.Surveys
                             where sv.Id == surveyId
                             select sv).SingleOrDefault();

            survey.AnonCode = viewModel.AnonCode;
            survey.RecipientAddress = viewModel.Email;
            survey.AnonAge = viewModel.AnonAge;

            return Convert.ToBoolean(db.SaveChanges());
        }

        public int GetLatestSurveyBySession(string sessionId)
        {
            int surveyId = (from surveys in db.Surveys
                            where surveys.SessionId == sessionId
                            orderby surveys.DateStarted descending
                            select surveys.Id).FirstOrDefault();

            return surveyId;
        }

        private List<int> getAsciiByAlphabet(int id)
        {
            return (from x in db.AsciiAlphabets
                    where x.AlphabetId == id
                    orderby x.Index ascending
                    select x.Ascii).ToList();
        }

        public AlphabetViewModel getAlphabet(int id)
        {
            Alphabet alphabet = getAlphabetById(id);

            List<int> ascii = getAsciiByAlphabet(id);

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

        public void FinishSurveyByAlphabetResult(int abResultId)
        {
            int surveyId = (from abr in db.AlphabetResults
                            where abr.Id == abResultId
                            select abr.SurveyId).SingleOrDefault();

            Survey survey = (from sv in db.Surveys
                             where sv.Id == surveyId
                             select sv).SingleOrDefault();

            survey.DateFinished = DateTime.Now;

            db.SaveChanges();

            if (survey != null)
                sendResultEmail(survey);
        }

        //TODO: Make async
        private void sendResultEmail(Survey survey)
        {
            List<DownloadViewModel> results = getResultEmail(survey.Id);

            string body = "A user has finished a survey, here are the results: <br /><br />";

            body += "User;Survey;Language;AsciiCharacter;AttemptNumber;CharR;CharG;CharB;BackgroundR;BackgroundG;BackgroundB;TimeStamp<br />";

            foreach (var result in results)
            {
                body +=
                    survey.AnonCode + ";" +
                    result.Survey + ";" +
                    result.Language + ";" +
                    result.AsciiCharacter + ";" +
                    result.AttemptNumber + ";" +
                    result.CharR + ";" +
                    result.CharG + ";" +
                    result.CharB + ";" +
                    result.BackgroundR + ";" +
                    result.BackgroundG + ";" +
                    result.BackgroundB + ";" +
                    result.TimeStamp + "<br />";
            }

            body += "<br /><br />This mail is autogenerated.";

            SmtpClient client = new SmtpClient();
            var mail = new MailMessage("Synesthesia Web synwebinfo@gmail.com", survey.RecipientAddress);
            mail.IsBodyHtml = true;
            mail.Subject = "Results from a Survey " + survey.AnonCode;
            mail.Body = body;
            client.Send(mail);
        }

        public bool Remove(int id)
        {
            return true;
        }

        private List<DownloadViewModel> getResultEmail(int id)
        {
            List<DownloadViewModel> result = new List<DownloadViewModel>();

            var asciiResults = (from sv in db.Surveys
                                where sv.Id == id
                                join abr in db.AlphabetResults
                                on sv.Id equals abr.SurveyId
                                join asr in db.AsciiResults
                                on abr.Id equals asr.AlphabetResultId
                                join ab in db.Alphabets
                                on abr.AlphabetId equals ab.Id
                                orderby sv.Id, asr.AttemptNumber, asr.Ascii
                                select asr);

            AlphabetResult ABResult = getAlphabetResultBySurveyId(id);
            Color bkg = ColorTranslator.FromHtml(ABResult.Alphabet.BackgroundARGB);

            foreach (var asr in asciiResults)
            {
                result.Add(new DownloadViewModel
                {
                    User = ABResult.UserId,
                    Survey = ABResult.SurveyId.ToString(),
                    Language = ABResult.Alphabet.Nation,
                    AsciiCharacter = asr.Ascii.ToString(),
                    AttemptNumber = asr.AttemptNumber.ToString(),
                    CharR = asr.R.HasValue ? asr.R.ToString() : "NaN",
                    CharG = asr.G.HasValue ? asr.G.ToString() : "NaN",
                    CharB = asr.B.HasValue ? asr.B.ToString() : "NaN",
                    BackgroundR = bkg.R.ToString(),
                    BackgroundG = bkg.G.ToString(),
                    BackgroundB = bkg.B.ToString(),
                    TimeStamp = ABResult.Survey.DateFinished.ToString()
                });

            }

            return result;
        }

        private AlphabetResult getAlphabetResultBySurveyId(int id)
        {
            return (from abr in db.AlphabetResults
                    where abr.SurveyId == id
                    select abr).SingleOrDefault();
        }

        public void PopulateFonts(AlphabetCreateViewModel vm)
        {
            List<string> fontlist = new List<string>(new string[] { "Arial Black", "Lucida Console", "Times New Roman" });
            List<int> flaglist = new List<int>(new int[] { });

            for (int i = 0; i < 6; i++)//TODO: make constant somewhere
            {
                flaglist.Add(i);
            }

            IEnumerable<SelectListItem> fonts = fontlist.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            });

            IEnumerable<SelectListItem> flags = flaglist.Select(x => new SelectListItem
            {
                Text = "Flag " + (x + 1).ToString(),
                Value = x.ToString()
            });

            vm.AvailableFonts = fonts;
            vm.AvailableFlags = flags;
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

            return getSurveyResultViewModel(asciiResults);
        }

        private SurveyResultViewModel getSurveyResultViewModel(IQueryable<AsciiResult> asciiResults)
        {
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
                //Check if there is null (no color)
                if (!firstAttempt.ElementAt(i).R.HasValue ||
                    !firstAttempt.ElementAt(i).G.HasValue ||
                    !firstAttempt.ElementAt(i).B.HasValue ||
                    !secondAttempt.ElementAt(i).R.HasValue ||
                    !secondAttempt.ElementAt(i).G.HasValue ||
                    !secondAttempt.ElementAt(i).B.HasValue ||
                    !thirdAttempt.ElementAt(i).R.HasValue ||
                    !thirdAttempt.ElementAt(i).G.HasValue ||
                    !thirdAttempt.ElementAt(i).B.HasValue )
                {
                    calculations.Add(-1);
                }
                else
                {
                    float v_r = (
                    (float)
                    (Math.Abs(firstAttempt.ElementAt(i).R.Value - secondAttempt.ElementAt(i).R.Value)
                    + Math.Abs(secondAttempt.ElementAt(i).R.Value - thirdAttempt.ElementAt(i).R.Value)
                    + Math.Abs(thirdAttempt.ElementAt(i).R.Value - firstAttempt.ElementAt(i).R.Value))
                    ) / 255;
                    float v_g = (
                        (float)
                        (Math.Abs(firstAttempt.ElementAt(i).G.Value - secondAttempt.ElementAt(i).G.Value)
                        + Math.Abs(secondAttempt.ElementAt(i).G.Value - thirdAttempt.ElementAt(i).G.Value)
                        + Math.Abs(thirdAttempt.ElementAt(i).G.Value - firstAttempt.ElementAt(i).G.Value))
                        ) / 255;
                    float v_b = (
                        (float)
                        (Math.Abs(firstAttempt.ElementAt(i).B.Value - secondAttempt.ElementAt(i).B.Value)
                        + Math.Abs(secondAttempt.ElementAt(i).B.Value - thirdAttempt.ElementAt(i).B.Value)
                        + Math.Abs(thirdAttempt.ElementAt(i).B.Value - firstAttempt.ElementAt(i).B.Value))
                        ) / 255;

                    calculations.Add(v_r + v_g + v_b);
                }
            }

            var validNumbers = calculations.Where(x => !(x < 0));

            float score = validNumbers.Count() > 0 ? validNumbers.Average() : -1;

            return new SurveyResultViewModel
            {
                firstAttempt = firstAttempt,
                secondAttempt = secondAttempt,
                thirdAttempt = thirdAttempt,
                calculations = calculations,
                score = score
            };
        }

        public List<DownloadViewModel> GetCompletedSurveys(DateTime start, DateTime end)
        {
            List<DownloadViewModel> viewModel = new List<DownloadViewModel>();

            var asciiResults = (from sv in db.Surveys
                           where sv.DateFinished.HasValue
                           && sv.DateFinished >= start
                           && sv.DateFinished <= end
                           join abr in db.AlphabetResults
                           on sv.Id equals abr.SurveyId
                           join asr in db.AsciiResults
                           on abr.Id equals asr.AlphabetResultId
                           join ab in db.Alphabets
                           on abr.AlphabetId equals ab.Id
                           orderby sv.Id, asr.AttemptNumber, asr.Ascii 
                           select asr).ToList();

            foreach (var asr in asciiResults)
            {
                AlphabetResult abr = asr.AlphabetResult;
                Color bkg = ColorTranslator.FromHtml(abr.Alphabet.BackgroundARGB);
                
                viewModel.Add(new DownloadViewModel {
                    User = abr.UserId,
                    Survey = abr.SurveyId.ToString(),
                    Language = abr.Alphabet.Nation,
                    AsciiCharacter = asr.Ascii.ToString(),
                    AttemptNumber = asr.AttemptNumber.ToString(),
                    CharR = asr.R.HasValue ? asr.R.ToString() : "NaN",
                    CharG = asr.G.HasValue ? asr.G.ToString() : "NaN",
                    CharB = asr.B.HasValue ? asr.B.ToString() : "NaN",
                    BackgroundR = bkg.R.ToString(),
                    BackgroundG = bkg.G.ToString(),
                    BackgroundB = bkg.B.ToString(),
                    TimeStamp = abr.Survey.DateFinished.ToString()
                });
                
            }

            return viewModel;
        }

        public bool Edit(int id, AlphabetCreateViewModel editAlphabet)
        {
            Alphabet oldAlphabet = getAlphabetById(id);

            //TODO: Make Alphabet and compare in single statement
            if ( 
                (editAlphabet.Nation != oldAlphabet.Nation)
                || (editAlphabet.Description != oldAlphabet.Description)
                || (editAlphabet.BackgroundColor != oldAlphabet.BackgroundARGB)
                || (editAlphabet.Font != oldAlphabet.Font)
                || (editAlphabet.Flag != oldAlphabet.Flag)
                )
            {
                oldAlphabet.Nation = editAlphabet.Nation;
                oldAlphabet.Description = editAlphabet.Description;
                oldAlphabet.BackgroundARGB = editAlphabet.BackgroundColor;
                oldAlphabet.Font = editAlphabet.Font;
                oldAlphabet.Flag = editAlphabet.Flag;

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

                AsciiAlphabet asciiAlphabet = new AsciiAlphabet { AlphabetId = id, Index = i, Ascii = (int)temp };

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

        public AlphabetCreateViewModel GetAlphabetCreateModel()
        {
            AlphabetCreateViewModel vm = new AlphabetCreateViewModel();

            PopulateFonts(vm);

            vm.BackgroundColor = "#ffffff";

            return vm;
        }

        public bool Add(AlphabetCreateViewModel newAlphabet)
        {
            Alphabet alphabet = new Alphabet {
                Nation = newAlphabet.Nation,
                Description = newAlphabet.Description,
                BackgroundARGB = newAlphabet.BackgroundColor,
                Font = newAlphabet.Font
                };

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

                AsciiAlphabet asciiAlphabet = new AsciiAlphabet { AlphabetId = alphabetId, Index = i, Ascii = (int)temp };

                db.AsciiAlphabets.Add(asciiAlphabet);
            }

            return Convert.ToBoolean(db.SaveChanges());
        }

        public SurveyResultViewModel GetAsciiResultsBySurveyId(int id)
        {
            AlphabetResult alphabetResult = (from abr in db.AlphabetResults
                                             where abr.SurveyId == id
                                             select abr).SingleOrDefault();

            IQueryable<AsciiResult> asciiResults = (from ar in db.AsciiResults
                                                    where ar.AlphabetResultId == alphabetResult.Id
                                                    select ar);

            return getSurveyResultViewModel(asciiResults);
        }

        public bool PostAsciiResults(AlphabetResultViewModel viewModel, int alphabetResultId)
        {
            foreach (var result in viewModel.results)
            {
                AsciiResult temp = new AsciiResult();

                temp.AttemptNumber = result.AttemptNumber;
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

            return Convert.ToBoolean(db.SaveChanges());
        }
    }
}