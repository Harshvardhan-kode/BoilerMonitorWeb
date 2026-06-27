using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BoilerMonitorWeb.Application.Services;
using System.Threading.Tasks;

namespace BoilerMonitorWeb.Controllers
{
    // Restrict the entire controller to authorized users with Engineer or Admin roles
    [Authorize(Roles = "Engineer,Admin")]
    public class BatchApprovalController : Controller
    {
        private readonly IBatchValidationService _validationService;

        public BatchApprovalController(IBatchValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _validationService.ValidateAsync(id);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Recommended for POST actions to prevent CSRF attacks
        public async Task<IActionResult> Approve(int id, string signaturePassword, string justificationReason)
        {
            // TODO: Implement actual password/signature validation against your user/security service
            bool isSignatureValid = await _validationService.VerifySignatureAsync(User.Identity.Name, signaturePassword);

            if (!isSignatureValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid signature password.");
                var result = await _validationService.ValidateAsync(id);
                return View(result);
            }

            // Perform the approval logic
            await _validationService.RecordApprovalAsync(id, justificationReason);

            TempData["SuccessMessage"] = $"Batch #{id} verification trail recorded into regulatory ledger logs under reasoning: {justificationReason}";
            return RedirectToAction("Approve", new { id = id });
        }
    }
}
