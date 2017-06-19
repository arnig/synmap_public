using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AlphabetsViewModel
    {
        public List<AlphabetDemo> alphabets { get; set; }
    }

    public class AlphabetDemo
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}