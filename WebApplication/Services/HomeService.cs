using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.Entities;

namespace WebApplication.Services
{
    public class HomeService
    {
        private ApplicationDbContext db;

        public HomeService()
        {
            db = new ApplicationDbContext();
        }

        public HomeViewModel GetViewModel()
        {
            List<Alphabet> alphabetList = (from ab in db.Alphabets
                                           select ab).Take(3).ToList();

            return new HomeViewModel { alphabets = alphabetList };
        }
    }
}