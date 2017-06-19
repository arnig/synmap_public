using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class AlphabetService
    {
        public AlphabetsViewModel getAlphabets()
        {
            List<AlphabetDemo> lis = new List<AlphabetDemo>();

            lis.Add(new AlphabetDemo { Id = 1337, Description = "Brolandic" });

            return new AlphabetsViewModel { alphabets = lis };
        }
    }
}