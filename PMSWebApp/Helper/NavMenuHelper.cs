namespace PMSWebApp.Helper
{
    public static class NavMenuHelper
    {
       

        public static string IsActive(HttpContext httpContext, string controllers = null, string actions = null, string cssClass = "active")
        {
            string currentAction = httpContext.Request.RouteValues["action"]?.ToString();
            string currentController = httpContext.Request.RouteValues["controller"]?.ToString();

            // If controller or action is missing, fallback to route template or simply return nothing
            if (string.IsNullOrEmpty(currentController) || string.IsNullOrEmpty(currentAction))
                return string.Empty;

            var acceptedActions = (actions ?? currentAction)?.Split(',') ?? new string[] { };
            var acceptedControllers = (controllers ?? currentController)?.Split(',') ?? new string[] { };

            //return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController)
            //    ? cssClass
            //    : string.Empty;

            return acceptedControllers.Contains(currentController)
              ? cssClass
              : string.Empty;
        }

        public static string IsMenuOpen(HttpContext httpContext, string controllers, string actions = null, string cssClass = "show")
        {
            return IsActive(httpContext, controllers, actions, cssClass);
        }

        public static string IsAriaExpanded(HttpContext httpContext, string controllers, string actions = null)
        {
            return IsActive(httpContext, controllers, actions) == "active" ? "true" : "false";
        }

    }

}
