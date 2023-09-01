using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blog.common.Middleware;
using Org.BouncyCastle.Asn1.Ocsp;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Features;
using blog.common.Service;
using blog.common.Model;
using Microsoft.Extensions.DependencyInjection;
using blog.api.Model;
using blog.common.Exceptions;

namespace blog.api.test
{
    public class AuthorizationFilterTest
    { 

        [Test]
        public void AuthorizeFilterAuthenticationExceptionNoHeader()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        }
                    }), new List<IFilterMetadata>());

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            var unauthorizedResult = context.Result as JsonResult;

            // assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
            Assert.That(unauthorizedResult.Value.ToString(), Is.EqualTo("{ message = Unauthorized }"));
        }

        [Test]
        public void AuthorizeFilterAuthenticationAnonymousAttributeMethod()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "GetByUserName" },
                            { "controller", "UserController"}
                        },
                        EndpointMetadata = new List<object>()
                        {
                            new AllowAnonymousAttribute()
                        }
                    }), new List<IFilterMetadata>());

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public void AuthorizeFilterAuthenticationUserFound()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        }
                    }), new List<IFilterMetadata>());

            context.HttpContext.Request.Headers.Add("Authorization", "Basic YWJjOmFiYw==");

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new User()
                {
                    ID = 1,
                    Email = "any@email.com",
                    Name = "any",
                    Username = "test",
                    Password = "test",
                    Role = new List<UserRole>()
                    {
                        UserRole.Public,
                        UserRole.Writer,
                        UserRole.Editor
                    }
                });

            var requestedServicesMock = new Mock<IServiceProvider>();
            requestedServicesMock.Setup(s => s.GetService(typeof(UserService))).Returns(userService.Object);

            context.HttpContext.RequestServices = requestedServicesMock.Object;

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public void AuthorizeFilterAuthenticationRequestHasPermissionSuccess()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        },
                        EndpointMetadata = new List<object>()
                        {
                            new HasPermission(UserRole.Writer)
                        }
                    }), new List<IFilterMetadata>());

            context.HttpContext.Request.Headers.Add("Authorization", "Basic YWJjOmFiYw==");

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new User()
                {
                    ID = 2,
                    Email = "any@email.com",
                    Name = "any",
                    Username = "test",
                    Password = "test",
                    Role = new List<UserRole>()
                    {
                        UserRole.Public,
                        UserRole.Writer,
                        UserRole.Editor
                    }
                });

            var currentUserScoped = new CurrentUser() { Id = 1 };

            var requestedServicesMock = new Mock<IServiceProvider>();
            requestedServicesMock.Setup(s => s.GetService(typeof(UserService))).Returns(userService.Object);
            requestedServicesMock.Setup(s => s.GetService(typeof(CurrentUser))).Returns(currentUserScoped);

            context.HttpContext.RequestServices = requestedServicesMock.Object;

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            Assert.IsNull(context.Result);
            Assert.That(currentUserScoped.Id, Is.EqualTo(2));
        }

        [Test]
        public void AuthorizeFilterAuthenticationRequestHasPermissionFail()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        },
                        EndpointMetadata = new List<object>()
                        {
                            new HasPermission(UserRole.Writer)
                        }
                    }), new List<IFilterMetadata>());

            context.HttpContext.Request.Headers.Add("Authorization", "Basic YWJjOmFiYw==");

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new User()
                {
                    ID = 1,
                    Email = "any@email.com",
                    Name = "any",
                    Username = "test",
                    Password = "test",
                    Role = new List<UserRole>()
                    {
                        UserRole.Public
                    }
                });

            var requestedServicesMock = new Mock<IServiceProvider>();
            requestedServicesMock.Setup(s => s.GetService(typeof(UserService))).Returns(userService.Object);

            context.HttpContext.RequestServices = requestedServicesMock.Object;

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            var unauthorizedResult = context.Result as JsonResult;

            // assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
            Assert.That(unauthorizedResult.Value.ToString(), Is.EqualTo("{ message = Unauthorized }"));
        }

        [Test]
        public void AuthorizeFilterAuthenticationUserNotFound()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        },
                        EndpointMetadata = new List<object>()
                        {
                            new HasPermission(UserRole.Writer)
                        }
                    }), new List<IFilterMetadata>());

            context.HttpContext.Request.Headers.Add("Authorization", "Basic YWJjOmFiYw==");

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns<User>(null);

            var requestedServicesMock = new Mock<IServiceProvider>();
            requestedServicesMock.Setup(s => s.GetService(typeof(UserService))).Returns(userService.Object);

            context.HttpContext.RequestServices = requestedServicesMock.Object;

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            var unauthorizedResult = context.Result as JsonResult;

            // assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
            Assert.That(unauthorizedResult.Value.ToString(), Is.EqualTo("{ message = Unauthorized }"));
        }

        [Test]
        public void AuthorizeFilterAuthenticationRequestHasInvalidCredential()
        {
            var context = new AuthorizationFilterContext(new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    {
                        RouteValues = new Dictionary<string, string>()
                        {
                            { "action", "mine" },
                            { "controller", "PostController"}
                        },
                        EndpointMetadata = new List<object>()
                        {
                            new HasPermission(UserRole.Writer)
                        }
                    }), new List<IFilterMetadata>());

            context.HttpContext.Request.Headers.Add("Authorization", "Basic 123");

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new User()
                {
                    ID = 1,
                    Email = "any@email.com",
                    Name = "any",
                    Username = "test",
                    Password = "test",
                    Role = new List<UserRole>()
                    {
                        UserRole.Public
                    }
                });

            var requestedServicesMock = new Mock<IServiceProvider>();
            requestedServicesMock.Setup(s => s.GetService(typeof(UserService))).Returns(userService.Object);

            context.HttpContext.RequestServices = requestedServicesMock.Object;

            var filter = new AuthorizeFilter();

            filter.OnAuthorization(context);

            var unauthorizedResult = context.Result as JsonResult;

            // assert
            Assert.IsNotNull(unauthorizedResult);
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
            Assert.That(unauthorizedResult.Value.ToString(), Is.EqualTo("{ message = Unauthorized }"));
        }
    }
}
