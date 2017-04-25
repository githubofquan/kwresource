using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kwresource.Models;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace kwresource.Controllers
{
    public class HomeController : Controller
    {
        private kwresourceEntities db = new kwresourceEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Test";

            return View();
        }


        public async Task AsyncProcess(string email, string emailCC)
        {


            const string HOST = "email-smtp.us-west-2.amazonaws.com";

            const string SMTP_USERNAME = "AKIAIF7OJHCLUJAFZBKA";  // Replace with your SMTP username. 
            const string SMTP_PASSWORD = "AnERuonOwPouCRNEVgyeNcVealf2mCidASUstfGAry5L";  // Replace with your SMTP password.

            const int PORT = 587;

            using (SmtpClient client = new SmtpClient(HOST, PORT))
            {
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                client.EnableSsl = true;


                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("info@eduu.vn");
                msg.Subject = "kwresource done";
                msg.Body = "done";
                msg.IsBodyHtml = true;
                msg.To.Add(email);
                msg.CC.Add(emailCC);
                client.Send(msg);

            }

        }

        public async Task<string> GetForm(FormData formData)
        {
            var result = formData.field1 + "_" + formData.field2;
            string email = formData.field1;
            string emailCC = formData.field2;
            var kw = new keyword()
            {
                Id = Guid.NewGuid().ToString(),
                kw = result
            };
            db.keywords.Add(kw);
            db.SaveChanges();

            try
            {
                await AsyncProcess(email, emailCC).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
        }

        public class FormData
        {
            public string field1 { get; set; }
            public string field2 { get; set; }
        }
    }
}
