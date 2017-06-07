using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class AlphabetController : Controller
    {
        // GET: Alphabet
        public ActionResult Index()
        {
            return View();
        }

        // GET: Alphabet/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Alphabet/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Alphabet/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Alphabet/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Alphabet/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Alphabet/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Alphabet/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
