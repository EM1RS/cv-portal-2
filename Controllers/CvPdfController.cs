using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using CvAPI2.Models; // Endre til din faktiske namespace
using CvAPI2.Services; // Der du henter ut CV-data
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CvPdfController : ControllerBase
{
    private readonly ICvService _cvService;

    public CvPdfController(ICvService cvService)
    {
        _cvService = cvService;
    }

    [HttpGet("{cvId}")]
    public async Task<IActionResult> GetPdf(string cvId)
    {
        var cv = await _cvService.GetCvById(cvId);

        if (cv == null)
            return NotFound("CV not found");

        var document = new CvPdfDocument(cv);
        byte[] pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"CV_{cv.User.FullName ?? "user"}.pdf");
    }
}
