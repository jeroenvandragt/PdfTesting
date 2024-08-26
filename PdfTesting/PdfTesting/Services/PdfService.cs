using iText.Forms;
using iText.Kernel.Pdf;

public class PdfService
{
    //private byte[] GeneratePdfFromHtml ( string htmlContent )
    //{
    //    var converter = new BasicConverter( new PdfTools() );
    //    var document = new HtmlToPdfDocument
    //    {
    //        GlobalSettings = {
    //            PaperSize = PaperKind.A4,
    //            Orientation = Orientation.Portrait,
    //        } ,
    //        Objects = {
    //            new ObjectSettings
    //            {
    //                HtmlContent = htmlContent
    //            }
    //        }
    //    };

    //    using (var stream = new MemoryStream())
    //    {
    //        var pdfBytes = converter.Convert( document );
    //        return pdfBytes;
    //    }
    //}

    public Dictionary<string , string> GetFormFields ( string pdfPath )
    {
        var formFields = new Dictionary<string , string>();

        using (var pdfReader = new PdfReader( pdfPath ))
        using (var pdfDocument = new PdfDocument( pdfReader ))
        {
            // Get the AcroForm from the document
            var acroForm = PdfAcroForm.GetAcroForm( pdfDocument , false );
            if (acroForm != null)
            {
                // Iterate over all the fields in the form
                var fieldNames = acroForm.GetAllFormFields();
                foreach (var fieldName in fieldNames)
                {
                    var field = acroForm.GetField( fieldName.Key );
                    var fieldValue = field.GetValueAsString();
                    formFields[fieldName.Key] = fieldValue;
                }
            }
        }

        return formFields;
    }

    public byte[] FillPdfForm ( string inputPdfPath , Dictionary<string , (string Value, float FontSize)> formFields , bool editable )
    {
        using (var inputPdfStream = new FileStream( inputPdfPath , FileMode.Open , FileAccess.Read ))
        using (var outputPdfStream = new MemoryStream())
        {
            // Load the existing PDF
            using (var pdfDocument = new PdfDocument( new PdfReader( inputPdfStream ) , new PdfWriter( outputPdfStream ) ))
            {
                var acroForm = PdfAcroForm.GetAcroForm( pdfDocument , true );

                if (acroForm != null)
                {
                    foreach (var entry in formFields)
                    {
                        var fieldName = entry.Key;
                        var fieldValue = entry.Value.Value;
                        var fontSize = entry.Value.FontSize;

                        var field = acroForm.GetField( fieldName );
                        if (field != null)
                        {
                            field.SetValue( fieldValue );
                            field.SetFontSize( fontSize );
                        }

                    }
                    if (!editable) acroForm.FlattenFields();
                }

                pdfDocument.Close();
            }

            return outputPdfStream.ToArray();
        }
    }

    //public byte[] OverlayPdfWithHtml ( string htmlContent )
    //{
    //    var htmlPdfBytes = GeneratePdfFromHtml( htmlContent );

    //    using (var inputStream = new MemoryStream( File.ReadAllBytes( "CMR_Template.pdf" ) ))
    //    using (var document = PdfReader.Open( inputStream , PdfDocumentOpenMode.Modify ))
    //    using (var htmlStream = new MemoryStream( htmlPdfBytes ))
    //    using (var htmlDocument = PdfReader.Open( htmlStream , PdfDocumentOpenMode.Import ))
    //    {
    //        // Overlay each page from the HTML PDF onto the existing PDF
    //        for (int i = 0; i < document.PageCount && i < htmlDocument.PageCount; i++)
    //        {
    //            var existingPage = document.Pages[i];
    //            var htmlPage = htmlDocument.Pages[i];

    //            using (var gfx = XGraphics.FromPdfPage( existingPage ))
    //            {
    //                // Overlay logic here
    //                // For example, you might draw the HTML page as an image or use other methods
    //            }
    //        }

    //        // Save the modified PDF to a new memory stream
    //        using (var outputStream = new MemoryStream())
    //        {
    //            document.Save( outputStream , false );
    //            return outputStream.ToArray();
    //        }
    //    }
    //}

}
