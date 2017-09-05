using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Services;
using WebApplication.Models;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using WebApplication.Scripts;

namespace WebApplication.Controllers
{
    public class AlphabetController : Controller
    {
        private AlphabetService service = new AlphabetService();
        // GET: Alphabet
        public ActionResult Index()
        {
            AlphabetIndexViewModel alphabets = service.getAlphabets(User.Identity.GetUserId());

            return View(alphabets);
        }

        public ActionResult Participate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Alphabet");
            }


            string currentUser = User.Identity.GetUserId();
            string currentSession = Session.SessionID;

            service.PostSurvey(currentUser, currentSession);
            
            service.PostAlphabetResult(id.Value, currentUser, currentSession);

            AlphabetParticipateViewModel alphabet = service.GetAlphabetParticipateModel(id.Value);

            return View(alphabet);
        }

        [HttpPost]
        public void SurveyInformation(SurveyInfoViewModel viewModel)
        {
            string currentSession = this.Session.SessionID;

            int surveyId = service.GetLatestSurveyBySession(currentSession);

            service.AddAnonIdentity(viewModel, surveyId);
        }

        [HttpPost]
        public bool AlphabetResult(AlphabetResultViewModel viewModel)
        {
            string currentSession = this.Session.SessionID;

            int abResultId = service.GetLatestAlphabetResultBySession(currentSession);

            service.PostAsciiResults(viewModel, abResultId);

            service.FinishSurveyByAlphabetResult(abResultId);

            return true;
        }

        public ActionResult Result(int? surveyId)
        {
            SurveyResultViewModel viewModel;

            if (surveyId == null)
            {
                string currentSession = this.Session.SessionID;

                viewModel = service.GetLatestAsciiResultsBySession(currentSession);
            }
            else
            {
                viewModel = service.GetAsciiResultsBySurveyId(surveyId.Value);
            }

            if (viewModel == null)
            {
                return RedirectToAction("Index", "Alphabet");
            }
            else
            {
                return View(viewModel);
            }
        }

        //TODO: Authorize admin
        public ActionResult Create()
        {
            AlphabetCreateViewModel vm = service.GetAlphabetCreateModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AlphabetCreateViewModel newAlphabet)
        {
            if (ModelState.IsValid)
            {
                if (!service.Add(newAlphabet))
                {
                    ModelState.AddModelError("", "Alphabet could not be saved!");
                    return View(newAlphabet);
                }

                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Submission invalid!");
            return View(newAlphabet);
        }

        //TODO: Authorize admin
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Index");
            }

            AlphabetViewModel alphabetViewModel = service.getAlphabet(id.Value);

            string characters = "";

            foreach (int ascii in alphabetViewModel.Ascii)
            {
                characters += (char)ascii;
            }

            AlphabetCreateViewModel vm = new AlphabetCreateViewModel
            {
                Nation = alphabetViewModel.Alphabet.Nation,
                Description = alphabetViewModel.Alphabet.Description,
                Characters = characters,
                BackgroundColor = alphabetViewModel.Alphabet.BackgroundARGB,
                Font = alphabetViewModel.Alphabet.Font
            };

            service.PopulateFonts(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, AlphabetCreateViewModel editAlphabet)
        {
            if (id != null)
            {
                if (!service.Edit(id.Value, editAlphabet))
                {
                    ModelState.AddModelError("", "No changes have been made!");
                    service.PopulateFonts(editAlphabet);
                    return View(editAlphabet);
                }

                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpPost]
        public String Download(DateTime start, DateTime end)
        {
            var surveys = service.GetCompletedSurveys(start, end);

            return surveys.ToJSON();
        }
    }
}
