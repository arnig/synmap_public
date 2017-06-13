using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Entities
{
    public class Alphabet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool[] Characters { get; set; }
    }
}