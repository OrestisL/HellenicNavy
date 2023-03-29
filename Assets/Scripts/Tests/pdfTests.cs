using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class pdfTests : MonoBehaviour
{
    public enum TextTypes
    {
        body = 0,
        heading1 = 10,
        heading2,
        heading3,
        header = 20,
        footer,
    }
    public class TextWithType
    {
        public string text;
        public TextTypes type;
        public XFontStyle modifier;

        public TextWithType(string text, TextTypes type, XFontStyle modifier)
        {
            this.text = text;
            this.type = type;
            this.modifier = modifier;
        }

        public TextWithType(string text)
        {
            this.text = text;
            type = TextTypes.body;
            modifier = XFontStyle.Regular;
        }
    }

    public TMP_InputField inputField;
    public List<TextWithType> finalText;

    private PdfDocument document;

    private void Start()
    {
        document = new PdfDocument();

        inputField.onEndEdit.AddListener((string s) => {
            finalText.Add(new TextWithType(s));
            for (int i = 0; i < finalText.Count; i++)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20.0, finalText[i].modifier);
            }
        });


    }

}
