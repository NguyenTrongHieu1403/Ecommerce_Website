using MailChimp.Net.Core;
using Microsoft.AspNetCore.Mvc;
using ecommerce_final.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web_Ecommerce.Controllers
{
    public class EmailController : Controller
    {
        private readonly MailChimpService _mailChimpService;

        public EmailController(MailChimpService mailChimpService)
        {
            _mailChimpService = mailChimpService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SendEmail()
        {

            var subject = "Chào bạn, đây là email tự động!";
            var body = "<h1>Chào bạn!</h1><p>Đây là email thử nghiệm từ MailChimp.</p>";
            var emailAddresses = new List<string> { "recipient1@example.com", "recipient2@example.com","nguyentronghieu11a13anlac@gmail.com" };
            await _mailChimpService.SendEmailAsync(subject,body, emailAddresses);

            TempData["Message"] = "Email đã được gửi thành công!";
            return RedirectToAction("Index");
        }
    }
}
