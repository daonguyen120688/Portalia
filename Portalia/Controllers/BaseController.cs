using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Portalia.Core.Dtos.User;
using Portalia.Core.Helpers;
using Portalia.Core.Infrastructure;
using Portalia.Extentions;
using Portalia.Models;
using Portalia.ViewModels.WorkContract;

namespace Portalia.Controllers
{
    public abstract class BaseController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnException(ExceptionContext filterContext)
        {
            var currentController = filterContext.RouteData.Values["controller"].ToString();
            var currentAction = filterContext.RouteData.Values["action"].ToString();

            logger.Error($"Controller: {currentController}, Action: {currentAction}, Error: {filterContext.Exception}");
            Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);

            if (filterContext.Exception is HttpAntiForgeryException &&
                currentController == "Account" &&
                currentAction == "Login")
            {
                filterContext.Result = View("~/Views/Account/Login.cshtml",
                    new LoginViewModel
                    {
                        Errors = new List<string>
                        {
                            Resources.Login.LoginProcessError
                        }
                    });
                filterContext.ExceptionHandled = true;
            }
            else
            {
                filterContext.Result = View("~/Views/Home/Error.cshtml");
            }
            //base.OnException(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionHasAllowAnonymousAttribute = filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
                .Any();

            if (actionHasAllowAnonymousAttribute)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var hasAuthorizeAttribute = filterContext.Controller.GetType()
                                            .GetCustomAttributes(typeof(AuthorizeAttribute), false).Length > 0;
            var excludedControllerAndActions = new List<ExcludeControllerAndActionModel>
            {
                new ExcludeControllerAndActionModel {Controller = "Manage", Action = "GetUserPicture"},
                new ExcludeControllerAndActionModel {Controller = "Proposal", Action = "CountMissingField"},
                new ExcludeControllerAndActionModel {Controller = "Account", Action = "ChangePasswordForNewPolicy"},
                new ExcludeControllerAndActionModel {Controller = "Account", Action = "LogOff"},
                new ExcludeControllerAndActionModel {Controller = "Account", Action = "GetUserPicture"},
                new ExcludeControllerAndActionModel {Controller = "Layout", Action = "WorkContractButton"}
            };
            var currentController = filterContext.RouteData.Values["controller"].ToString();
            var currentAction = filterContext.RouteData.Values["action"].ToString();
            var isRequestInExcludedControllerAndAction = excludedControllerAndActions
                .Any(entry => entry.Controller == currentController && entry.Action == currentAction);
            var hasChangedPassword = User.GetHasChangedPasswordClaim().ConvertToBool();

            if (User.Identity.IsAuthenticated &&
                hasAuthorizeAttribute &&
                !isRequestInExcludedControllerAndAction &&
                !hasChangedPassword)
            {
                filterContext.Result = RedirectToAction("ChangePasswordForNewPolicy", "Account");
            }

            base.OnActionExecuting(filterContext);
        }

        protected void AddValidationMessagesFromPasswordToModelState(PasswordValidationMessageDto validationMessages)
        {
            if (validationMessages == null)
            {
                return;
            }

            foreach (var error in validationMessages.Messages)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                    viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                    ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}