using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MultiBranding.ApiClient;
using Newtonsoft.Json;
using Portalia.Extentions;
using Portalia.ViewModels;

namespace Portalia.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            string isDevEnv = ConfigurationManager.AppSettings["IsDevEnv"];
            if (!string.IsNullOrEmpty(isDevEnv) && isDevEnv=="1")
                return RedirectToAction("MySpace", "Proposal");

            var url = Request.Url?.GetLeftPart(UriPartial.Authority);
            return Redirect(url);
        }

        public ActionResult About()
        {
            return View();
        }
        
   
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult CallYouBack()
        {
            return PartialView(new CallBackViewModel());
        }

        [HttpPost]
        public ActionResult CallYouBack(CallBackViewModel model)
        {
            if (ModelState.IsValid)
            {
                MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
                var result = MultiBrandingHelper.SendEmail(new SendEmailParameter()
                {
                    ApplicationName = "Multibranding",
                    Holding = "Portalia",
                    TemplateName = "ContactUs",
                    Variable = JsonConvert.SerializeObject(new
                    {
                        EmailContent = $"<p>Name: {model.FirstName} {model.LastName}</p>" +
                               $"<p>Email: {model.Email}</p>" +
                               $"<p>Phone: {model.Phone}</p>",
                        Subject = $"subscribe",
                        SendFrom = ConfigurationManager.AppSettings["MailFrom"],
                        SendTo = ConfigurationManager.AppSettings["ContactUsEmail"]
                    })
                });

            }
            return RedirectToAction("Index");
        }

        public ActionResult PortageFromAtoZ()
        {
            return View();
        }

        public ActionResult Promotion()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetSuggestTime(string dateFrom,string dateTo)
        {
            if (string.IsNullOrEmpty(dateFrom)||string.IsNullOrEmpty(dateTo))
            {
                return PartialView(new Tuple<List<DateTime>, List<DateTime>>(item1: new List<DateTime>(), item2: new List<DateTime>()));
            }
            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
            var listDate = DateTimeExtention.DateRange(DateTime.Parse(dateFrom),DateTime.Parse(dateTo)).ToList();
            var datesRequest = listDate.Select(dateTime => MultiBrandingHelper.GetTimeSuggestion(new MeetingTimeSuggestion()
            {
                DateTime = dateTime,
                Email = "procher@portalia.fr",
                Duration = 60,
                From = 6,
                To = 7,
                TimeZoneId = "Central European Standard Time"
            }).FirstOrDefault()).ToList();
            var model = new Tuple<List<DateTime>, List<DateTime>>(item1: listDate, item2: datesRequest);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult SendRequestAppointment(string date)
        {
            var model = new CreateAppointment
            {
                DateTime = date
            };
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult CreateAppoinment(CreateAppointment model)
        {
            MultiBrandingHelper.BaseUrl = ConfigurationManager.AppSettings["MultiBranding"];
            var stardDate = DateTime.Parse(model.DateTime);
            var endDate = stardDate.AddMinutes(60);
            var result = MultiBrandingHelper.SkypeMeeting(new SkypeMeetingParameter()
            {
                AttendeeMail = model.Email,
                AttendeeName = $"{model.FirstName} {model.LastName}",
                employeeId = 119,
                EndDate = endDate,
                StartDate = stardDate,
                Leader = "procher@portalia.fr",
                MeetingContent = "meeting"
            });
            return Json(new { result = result.Success, message = result.Error }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Timeline()
        {
            return View();
        }

        public ActionResult TimelineFile()
        {
            return new FileContentResult(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/Annexe-1-Timeline-Portage.pdf")), "application/pdf");
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}