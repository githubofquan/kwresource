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

        static int getEditDistance(string word1, string word2)
        {
            int cols = word1.Length + 1,
                rows = word2.Length + 1;
            int[,] matrix = new int[rows, cols];


            for (int i = 0; i < cols; i++) matrix[0, i] = i;
            for (int i = 0; i < rows; i++) matrix[i, 0] = i;


            for (int row = 1; row < rows; row++)
            {
                for (int col = 1; col < cols; col++)
                {
                    int value;

                    if (word1[col - 1] == word2[row - 1])
                    {
                        value = matrix[row - 1, col - 1];
                    }
                    else
                    {
                        value = getMin(matrix[row - 1, col - 1],
                            matrix[row - 1, col],
                            matrix[row, col - 1]) + 1;
                    }

                    matrix[row, col] = value;
                }
            }


            return matrix[rows - 1, cols - 1];
        }

        static int getMin(int n1, int n2, int n3)
        {
            return Math.Min(Math.Min(n1, n2), n3);
        }

        public void SendEmail(string email, string emailCC, string sessionid)
        {
            var dem = db.keywords.Count(t => t.sessionid == sessionid);
            const string HOST = "email-smtp.us-west-2.amazonaws.com";

            const string SMTP_USERNAME = "AKIAIF7OJHCLUJAFZBKA";
            const string SMTP_PASSWORD = "AnERuonOwPouCRNEVgyeNcVealf2mCidASUstfGAry5L";

            const int PORT = 587;

            using (SmtpClient client = new SmtpClient(HOST, PORT))
            {

                client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);


                client.EnableSsl = true;


                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("info@eduu.vn");
                msg.Subject = sessionid + '@' + dem;
                msg.Body = sessionid + '@' + dem;
                msg.IsBodyHtml = true;
                msg.To.Add(email);
                msg.CC.Add(emailCC);
                client.Send(msg);

            }
        }

        public async Task AsyncProcess(string email, string emailCC, string sessionid)
        {

            var form = new FormData();
            form.sessionid = sessionid;
            var cal = await Calculate(form);
            if (cal == "Calculate Done")
            {
                SendEmail(email, emailCC, sessionid);
            }           
        }

        public async Task<string> GetForm(FormData formData)
        {
            var result = "waiting";


            if (formData.kw.Contains('@'))
            {
                result = "All sent";
                try
                {
                    await AsyncProcess(formData.kw.Trim(), "mailofquan@gmail.com", formData.sessionid).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    return result;
                }
            }
            else
            {
                var kw = new keyword()
                {
                    Id = Guid.NewGuid().ToString(),
                    kw = formData.kw,
                    nosignkw = removeVietnameseSign(formData.kw).ToLower(),
                    volume = Int32.Parse(formData.vl),
                    landingpage = formData.lp,
                    currentranking = Int32.Parse(formData.cr),
                    kd = Int32.Parse(formData.kd),
                    cost = Int32.Parse(formData.cost),
                    sessionid = formData.sessionid,
                    stt = Int32.Parse(formData.stt)
                };
                db.keywords.Add(kw);
                db.SaveChanges();
            }


            return result;
        }

        public async Task<string> GetResult(FormData formData)
        {
            var newcost = "0000000";

            var stt = Int32.Parse(formData.stt);
            var sessionid = formData.sessionid;
            
            var kw = db.keywords.FirstOrDefault(t => t.sessionid == sessionid && t.stt == stt);

            if ((kw != null) && (kw.newcost != null))
            {
                newcost = kw.newcost.ToString();
            }

            return newcost;
        }

        public async Task<string> Calculate(FormData formData)
        {
            var sessionid = formData.sessionid;
            var listkw = db.keywords.Where(r => r.sessionid == sessionid).ToList();
            var listkwlength = listkw.Count;
            var Similarity = new double[listkwlength, listkwlength];            
            for (var i = 0; i < listkwlength - 1; i++)
                for (var j = i + 1; j < listkwlength; j++)
                {
                    var simi = (double)getEditDistance(listkw[i].kw, listkw[j].kw);
                    if (listkw[i].landingpage == listkw[j].landingpage)
                    {
                        simi = simi / 2.0;
                    }
                    Similarity[i, j] = simi;
                    Similarity[j, i] = simi;                   
                }

            var SimilarityEachWord = new double[listkwlength];
            
            var ListOfLandingPage = new List<string>();
            var SumAllKWCost = 0;
            
            for (var i = 0; i < listkwlength; i++)
            {
                var sum = 0.0;
                for (var j = 0; j < listkwlength; j++)
                        sum = sum + Similarity[i, j];

                SimilarityEachWord[i] = (double)(sum) / (listkwlength - 1);
                if (!ListOfLandingPage.Contains(listkw[i].landingpage))
                {
                    ListOfLandingPage.Add(listkw[i].landingpage);
                }
                SumAllKWCost = SumAllKWCost + (int)listkw[i].cost;
                
            }
            var NoOfLandingPage = ListOfLandingPage.Count;
            var SimilarityAverage = SimilarityEachWord.Average();

            //var NewCostOfKWList = SumAllKWCost * listkwlength / (SimilarityAverage * NoOfLandingPage*1.0);
            //var NewCostOfKWList = SumAllKWCost * SimilarityAverage / (listkwlength * NoOfLandingPage * 1.0);
            var NewCostOfKWList = SumAllKWCost * SimilarityAverage / (listkwlength * (listkwlength - NoOfLandingPage) * 1.0);

            var PercentCostEachWord = new double[listkwlength];
            var NewCost = new int[listkwlength];
            for (var i = 0; i < listkwlength; i++)
            {
                PercentCostEachWord[i] = ((double)(listkw[i].cost) / SumAllKWCost*1.0);
                NewCost[i] = (int)(PercentCostEachWord[i] * NewCostOfKWList);
                listkw[i].newcost = NewCost[i];
            }

            db.SaveChanges();

            return "Calculate Done";
        }

        private static readonly string[] VietnameseSigns = new string[]
{

        "aAeEoOuUiIdDyY",

        "áàạảãâấầậẩẫăắằặẳẵ",

        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

        "éèẹẻẽêếềệểễ",

        "ÉÈẸẺẼÊẾỀỆỂỄ",

        "óòọỏõôốồộổỗơớờợởỡ",

        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

        "úùụủũưứừựửữ",

        "ÚÙỤỦŨƯỨỪỰỬỮ",

        "íìịỉĩ",

        "ÍÌỊỈĨ",

        "đ",

        "Đ",

        "ýỳỵỷỹ",

        "ÝỲỴỶỸ"

};

        public static string removeVietnameseSign(string str)
        {
            if (str != null)
            {
                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }
            }
            return str;
        }

        public class FormData
        {
            public string kw { get; set; }
            public string lp { get; set; }
            public string vl { get; set; }
            public string cr { get; set; }
            public string kd { get; set; }
            public string cost { get; set; }
            public string sessionid { get; set; }
            public string stt { get; set; }
        }
    }
}
