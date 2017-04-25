using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kwresource.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace kwresource.Controllers
{
    public class HomeController : Controller
    {
        private kwresourceEntities db = new kwresourceEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public async Task<string> GetForm(FormData formData)
        {          
            var result = formData.field1 + "_" + formData.field2;
            var kw = new keyword()
            {
                Id = Guid.NewGuid().ToString(),
                kw = result
            };
            db.keywords.Add(kw);
            db.SaveChanges();
            return result;
        }

        public class FormData
        {
            public string field1 { get; set; }
            public string field2 { get; set; }
        }
    }
}
