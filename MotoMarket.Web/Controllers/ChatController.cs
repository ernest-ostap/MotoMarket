using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Services.Chat;
using System.Security.Claims;

namespace MotoMarket.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IChatService _chatService;

        public ChatController(IConfiguration configuration, IChatService chatService)
        {
            _configuration = configuration;
            _chatService = chatService;
        }

        // GET [index]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var conversations = await _chatService.GetMyConversations();
            return View(conversations);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _chatService.GetUnreadCount();
            return Json(count);
        }

        // GET [conversation]
        public IActionResult Conversation(string recipientId, int? listingId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recipientId == currentUserId)
            {
                return RedirectToAction("Index", "Listings");
            }

            var token = Request.Cookies["JWT"];
            ViewBag.JwtToken = token;

            ViewBag.RecipientId = recipientId;
            ViewBag.ListingId = listingId;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ApiUrl = _configuration["ApiUrl"] ?? "https://localhost:7072";

            return View();
        }
    }
}