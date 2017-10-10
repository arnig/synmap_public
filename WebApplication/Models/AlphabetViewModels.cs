using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models.Entities;

namespace WebApplication.Models
{
    public class AlphabetIndexViewModel
    {
        public List<Alphabet> Alphabets { get; set; }
        public List<string> UserRoles { get; set; }
    }

    public class AlphabetViewModel
    {
        public Alphabet Alphabet{ get; set; }
        public List<int> Ascii { get; set; }
    }

    public class SurveyInfoViewModel
    {
        public string AnonCode { get; set; }
        public string Email { get; set; }
        public int AnonAge { get; set; }
    }

    public class AlphabetResultViewModel
    {
        public List<AsciiResult> results { get; set; }
        public string userId { get; set; }
    }

    public class SurveyResultViewModel
    {
        public List<AsciiResult> firstAttempt { get; set; }
        public List<AsciiResult> secondAttempt { get; set; }
        public List<AsciiResult> thirdAttempt { get; set; }
        public List<float> calculations { get; set; }
        public float score { get; set; }
    }

    public class AlphabetCreateViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Nation { get; set; }
        public string Characters { get; set; }
        public string BackgroundColor { get; set; }
        public string Font { get; set; }
        public int Flag { get; set; }
        public IEnumerable<SelectListItem> AvailableFonts { get; set; }
        public IEnumerable<SelectListItem> AvailableFlags { get; set; }
    }

    public class AlphabetParticipateViewModel
    {
        public Alphabet Alphabet { get; set; }
        public List<int> Ascii { get; set; }

    }

    public class DownloadViewModel
    {
        public string User { get; set; }
        public string Survey { get; set; }
        public string AnonIdentity { get; set; }
        public string AnonYOB { get; set; }
        public string Language { get; set; }
        public string UnicodeCharacter { get; set; }
        public string AttemptNumber { get; set; }
        public string CharR { get; set; }
        public string CharG { get; set; }
        public string CharB { get; set; }
        public string BackgroundR { get; set; }
        public string BackgroundG { get; set; }
        public string BackgroundB { get; set; }
        public string TimeStamp { get; set; }
    }
}