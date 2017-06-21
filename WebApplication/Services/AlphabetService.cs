using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.Entities;

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
            string description = (from ab in db.Alphabets
                                  where ab.Id == id
                                  select ab.Description).SingleOrDefault();
            List<int> ascii = (from x in db.AsciiAlphabets
                               where x.AlphabetId == id
                               select x.Ascii).ToList();

            return new AlphabetViewModel { Ascii = ascii, Description = description };
        }
    }
}