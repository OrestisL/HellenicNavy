using UnityEngine;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using TMPro;
using System.IO;
using System.Text;
using System.Diagnostics;
using PdfSharpCore.Drawing.Layout;

public class pdfTests : MonoBehaviour
{
    public TMP_InputField input;
    public int margin = 30;
    public int fontSize = 12;
    private void Start()
    {
        if (input.text.Length == 0)
            input.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.";

        input.onDeselect.AddListener((string s) =>
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            //document.Info.Title = "Created with PDFsharp";

            // Create an empty page
            PdfPage page = document.AddPage();
            int width = (int)page.Width;
            int height = (int)page.Height;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter textFormatter = new XTextFormatter(gfx);

            // Create a font
            XFont font = new XFont("Times New Roman", fontSize, XFontStyle.Regular);

            //create the rect
            XRect textRect = new XRect(margin / 2, margin / 2, page.Width - margin, page.Height - margin);
            gfx.DrawRectangle(XBrushes.AntiqueWhite, textRect);
            textFormatter.Alignment = XParagraphAlignment.Justify;
            // Draw the text
            textFormatter.DrawString(s, font, XBrushes.Black,
              textRect, XStringFormats.TopLeft);

            // Save the document...            
            const string filename = "HelloWorld.pdf";
            document.Save(filename);
            // ...and start a viewer.
            //Process.Start(filename);
        });
    }

}
