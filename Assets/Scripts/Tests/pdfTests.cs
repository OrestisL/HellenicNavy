using UnityEngine;
using sharpPDF;
using TMPro;
using System.IO;
using System.Text;

public class pdfTests : MonoBehaviour
{
    public TMP_InputField input;

    private void Start()
    {
        input.onDeselect.AddListener((string s) =>
        {
            pdfDocument document = new pdfDocument("Test", "no one");
            pdfPage page = document.addPage();
            int widths = s.Length / page.width + 1;

            //for (int i = 1; i < widths; i++)
            //{
            //    //int start = (i - 1) * page.width;
            //    //int length = page.width;
            //    //if (length >= s.Length)
            //    //    length = s.Length - 1;
            //    //string write = s.Substring(start, length);
            //    //page.addText(write, 0, 20 * i, sharpPDF.Enumerators.predefinedFont.csTimesBold, 20, sharpPDF.Enumerators.predefinedColor.csBlack);

            //}
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                builder.Append(s[i]);
                if (i % page.width == 0)
                    builder.Append("\r\n");
            }
            string write = builder.ToString();
            page.addText(write, 0, 250, sharpPDF.Enumerators.predefinedFont.csTimesBold, 20, sharpPDF.Enumerators.predefinedColor.csBlack);

            //<b><i> etc modifiers do not work so it would have to be done on the fly using sharpPDF's things
            document.createPDF(Path.Combine(Directory.GetCurrentDirectory(), "test.pdf"));
            page = null;
            document = null;
        });
    }

}
