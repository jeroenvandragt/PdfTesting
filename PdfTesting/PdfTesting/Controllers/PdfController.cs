using Microsoft.AspNetCore.Mvc;

namespace PdfTesting.Controllers;

[ApiController]
[Route( "api/[controller]/[action]" )]
public class PdfController : ControllerBase
{
    private readonly PdfService _pdfService;

    public PdfController ( PdfService pdfService )
    {
        _pdfService = pdfService;
    }

    [HttpGet]
    public IActionResult GeneratePdf ( [FromQuery] bool editable = false )
    {
        string pdfTemplatePath = "CMR_Template.pdf";

        var formFields = new Dictionary<string , (string Value, float FontSize)>
        {
            { "VakRood01", ("Business 1,\r\n123 Commerce Street,\r\nCityville, 45678,\r\nUnited Kingdom", 7f) },
            { "VakRood02", ("Business 2,\r\n123 Commerce Street,\r\nCityville, 45678,\r\nUnited Kingdom", 7f) },
            { "VakRood16", ("Business 3,\r\n123 Commerce Street,\r\nCityville, 45678,\r\nUnited Kingdom", 7f) }
        };

        byte[] pdfBytes = _pdfService.FillPdfForm( pdfTemplatePath , formFields , editable );

        // Return the PDF as a file result
        return File( pdfBytes , "application/pdf" , "FilledForm.pdf" );
    }

    [HttpGet]
    public IActionResult ReadPdfFields ( )
    {
        try
        {
            string pdfTemplatePath = "CMR_Template.pdf";

            Dictionary<string , string> formFields = _pdfService.GetFormFields( pdfTemplatePath );

            return Ok( formFields );
        }
        catch (Exception ex)
        {
            return StatusCode( 500 , $"Internal server error: {ex.Message}" );
        }
    }
}
