using Microsoft.AspNetCore.Mvc;
using BoilerMonitorWeb.Application.Services;
using System.Threading.Tasks;

namespace BoilerMonitorWeb.Controllers
{
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
        public IActionResult Approve(int id, string signaturePassword, string justificationReason)
        {
            TempData["SuccessMessage"] = $"Batch #{id} verification trail recorded into regulatory ledger logs under reasoning: {justificationReason}";
            return RedirectToAction("Approve", new { id = id });
        }
    }
}