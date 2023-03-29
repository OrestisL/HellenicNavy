using UnityEngine;
using sharpPDF;
using TMPro;
using System.IO;

public class pdfTests : MonoBehaviour
{
    public TMP_InputField input;

    private void Start()
    {
        input.onDeselect.AddListener((string s) => 
        {
            pdfDocument document = new pdfDocument("Test", "no one");
            pdfPage page = document.addPage();
            page.addText(s, 200, 450, sharpPDF.Enumerators.predefinedFont.csHelvetica, 2, sharpPDF.Enumerators.predefinedColor.csBlack);
            document.createPDF(Path.Combine(Directory.GetCurrentDirectory(), "test.pdf"));
            page = null;
            document = null;
        });
    }

}
