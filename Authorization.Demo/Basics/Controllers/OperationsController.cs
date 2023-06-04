using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        public async Task<IActionResult> Open()
        {
            // Get cookieJar from DB
            var cookieJar = new CookieJar();
            await _authorizationService.AuthorizeAsync(User, cookieJar, CookieJarOperations.Open);
            return View();
        }
    }

    public class CookieJarAuthorizationHandle : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            CookieJar resource)
        {
            if(requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
                else if(requirement.Name == CookieJarOperations.ComeNear)
                {
                    if(context.User.HasClaim("Friend", "Good"))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement()
        {
            Name = CookieJarOperations.Open
        };
    }

    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }

    public class CookieJar
    {
        public string? Name { get; set; }
    }
}
