using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using blog.common.Service;
using System.Net.Http.Headers;
using System.Text;
using blog.api.Model;

namespace blog.common.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AuthorizeFilter, AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                var username = credentials[0];
                var password = credentials[1];

                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                var user = userService.Authenticate(username, password);

                if (user == null)
                {
                    // not logged in - return 401 unauthorized
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    var requestPermission = context.ActionDescriptor.EndpointMetadata.OfType<HasPermission>().FirstOrDefault();
                    if (requestPermission != null)
                    {
                        if (user.Role.Contains(requestPermission.Role) == false)
                        {
                            // not logged in - return 401 unauthorized
                            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                        else
                        {
                            var currentUser = context.HttpContext.RequestServices.GetService(typeof(CurrentUser)) as CurrentUser;
                            if (currentUser != null)
                            {
                                currentUser.Id = user.ID;
                            }
                        }
                    }
                }
            }
            catch
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

        }
    }
}
