using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MotoMarket.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;

        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: Chat/Conversation?recipientId=xyz&listingId=123
        public IActionResult Conversation(string recipientId, int? listingId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recipientId == currentUserId)
            {
                return RedirectToAction("Index", "Listings"); // Nie można czatować samemu ze sobą
            }

            // --- NOWOŚĆ: Wyciągamy Token z Ciasteczka ---
            // "JWT" to nazwa ciasteczka, którą ustawiłeś przy logowaniu.
            // Jeśli nazwałeś je inaczej (np. "X-Access-Token"), zmień tutaj nazwę.
            var token = Request.Cookies["JWT"];
            ViewBag.JwtToken = token;
            // --------------------------------------------

            // Przekazujemy dane do widoku przez ViewBag
            ViewBag.RecipientId = recipientId;
            ViewBag.ListingId = listingId;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ApiUrl = _configuration["ApiUrl"] ?? "https://localhost:7072"; // Potrzebne dla SignalR

            return View();
        }
    }
}