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
}