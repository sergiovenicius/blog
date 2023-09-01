using blog.ui.Models;
using blog.ui.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace blog.ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogAPIService _blogAPIService;

        public HomeController(ILogger<HomeController> logger, IBlogAPIService blogAPIService)
        {
            _logger = logger;
            _blogAPIService = blogAPIService;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("loggedUser")))
                return RedirectToAction("Login");

            _blogAPIService.SetUser(HttpContext.Session.GetString("loggedUser"), HttpContext.Session.GetString("loggedUser"));
            List <Post> postList = await _blogAPIService.ListAllPublishedPosts();

            return View(postList);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            // esta action trata o post (login)
            if (ModelState.IsValid) //verifica se é válido
            {
                List<string> users = new List<string>() { "puser", "wuser", "euser" };

                if (users.Contains(u.Username) && u.Username == u.Password)
                {
                    HttpContext.Session.SetString("loggedUser", u.Username);
                    
                    return RedirectToAction("Index");
                }
                else
                    ViewBag.Message = "Invalid login";
            }
            return View(u);
        }

        public async Task<IActionResult> PostDetail(int postId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("loggedUser")))
                return RedirectToAction("Login");

            _blogAPIService.SetUser(HttpContext.Session.GetString("loggedUser"), HttpContext.Session.GetString("loggedUser"));
            Post post = await _blogAPIService.GetPostById(postId);

            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}