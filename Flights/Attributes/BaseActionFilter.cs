using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Attributes
{
    public class BaseActionFilter : ActionFilterAttribute
    {
        // Élément actif dans la sidebar
        public readonly string ActiveAction = "active";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Si l'objet .Controller est de type Controller
            if (filterContext.Controller is Controller controller)
            {
                // Récupération de la route
                string Area = filterContext.RouteData.Values["area"] == null ? null : filterContext.RouteData.Values["area"].ToString();
                string Controller = filterContext.RouteData.Values["controller"] == null ? null : filterContext.RouteData.Values["controller"].ToString();
                string Action = filterContext.RouteData.Values["action"] == null ? null : filterContext.RouteData.Values["action"].ToString();

                // Construction de la chaîne de caractères pour montrer l'élément actif dans la sidebar
                string Builder = "/";
                string ActionBuilder = "/";

                if (!String.IsNullOrEmpty(Area))
                {
                    Builder += Area;
                    ActionBuilder += Area;
                }

                if (!String.IsNullOrEmpty(Controller) && Controller != "Home")
                {
                    Builder += "/" + Controller;
                    ActionBuilder += "/" + Controller;
                }

                if (!String.IsNullOrEmpty(Action) && Action != "Index")
                {
                    ActionBuilder += "/" + Action;
                }

                controller.ViewData[Builder] = ActiveAction;

                // Indication que c'est TV : treeview
                ActionBuilder += "_TV";
                controller.ViewData[ActionBuilder] = ActiveAction;
            }
        }
    }
}
