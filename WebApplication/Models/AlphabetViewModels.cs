using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models.Entities;

namespace WebApplication.Models
{
    public class AlphabetsViewModel
    {
        public List<Alphabet> alphabets { get; set; }
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
        public List<AsciiResult> calculations { get; set; }
    }
}