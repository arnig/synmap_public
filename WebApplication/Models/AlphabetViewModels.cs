﻿using System;
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

    public class AlphabetResultViewModel
    {
        public List<AsciiResults> results { get; set; }
        public string userId { get; set; }
        public int attemptNumber { get; set; }
    }

    public class AsciiResults
    {
        public int Ascii { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }
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
        public string Description { get; set; }
        public string Nation { get; set; }
        public string Characters { get; set; }
        public string BackgroundColor { get; set; }
        public string Font { get; set; }
        public IEnumerable<SelectListItem> AvailableFonts { get; set; }
    }

    public class AlphabetParticipateViewModel
    {
        public Alphabet Alphabet { get; set; }
        public List<int> Ascii { get; set; }

    }
}