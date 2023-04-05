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
using System;

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
            document.Info.Title = "Report xx-xx-xx";

            // Create an empty page
            PdfPage page = document.AddPage();
            int width = (int)page.Width;
            int height = (int)page.Height;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter textFormatter = new XTextFormatter(gfx);

            // Create a font
            XFont font = new XFont("Times New Roman", fontSize, XFontStyle.Regular);

            //add an image
            //DrawImageOriginalSize(gfx, page, SettingsHolder.Instance.settings.FullPath);
            //Vector2 imgSize = DrawImageScaled(gfx, page, SettingsHolder.Instance.settings.BadgeFullPath, 150, 150);
            (double maxW, double maxH) = CreateHeaderTemplate(gfx, page, 150);
            //create the text rect
            XRect textRect = new XRect(margin / 2, margin / 2 + maxH, page.Width - margin, page.Height - margin);
            //gfx.DrawRectangle(XBrushes.AntiqueWhite, textRect);
            textFormatter.Alignment = XParagraphAlignment.Justify;
            // Draw the text
            //textFormatter.DrawString(s, font, XBrushes.Black, textRect, XStringFormats.TopLeft);

            // Save the document...            
            const string filename = "HelloWorld.pdf";
            document.Save(filename);
            // ...and start a viewer.
            //Process.Start(filename);
        });
    }


    void DrawImageOriginalSize(XGraphics gfx, PdfPage page, string path)
    {
        XImage img = XImage.FromFile(path);
        XPoint imgPoint = new XPoint(-page.Width / 2 - margin, -page.Height / 2 - margin);
        gfx.DrawImage(img, imgPoint);
    }

    Vector2 DrawImageScaled(XGraphics gfx, PdfPage page, string path, double width, double height)
    {
        XImage img = XImage.FromFile(path);
        double max = Math.Max(img.Size.Width, img.Size.Height);

        double finalWidth = img.Size.Width * width / max;
        double finalHeight = img.Size.Height * height / max;
        gfx.DrawImage(img, margin / 2, margin / 2, finalWidth, finalHeight);

        return new Vector2((float)finalWidth + margin / 2, (float)finalHeight + margin / 2);
    }

    (double, double) CreateHeaderTemplate(XGraphics gfx, PdfPage page, double biggestEdge)
    {
        //load images
        XImage badgeImg = XImage.FromFile(SettingsHolder.Instance.settings.BadgeFullPath);
        XImage hnImg = XImage.FromFile(SettingsHolder.Instance.settings.HNFullPath);

        //get max for normalized scaling 
        double maxSizeBadge = Math.Max(badgeImg.Size.Width, badgeImg.Size.Height);
        double maxSizeHN = Math.Max(hnImg.Size.Width, hnImg.Size.Height);

        //calculate final dimensions
        double finalWidthBadge = badgeImg.Size.Width * biggestEdge / maxSizeBadge;
        double finalHeightBadge = badgeImg.Size.Height * biggestEdge / maxSizeBadge;

        double finalWidthHN = hnImg.Size.Width * biggestEdge / maxSizeHN;
        double finalHeightHN = hnImg.Size.Height * biggestEdge / maxSizeHN;

        //draw images
        //badge top left, HN top right
        gfx.DrawImage(badgeImg, margin / 2, margin / 2, finalWidthBadge, finalHeightBadge);
        gfx.DrawImage(hnImg, page.Width - finalWidthHN - margin / 2, margin / 2, finalWidthHN, finalHeightHN);

        //draw text between images
        XTextFormatter tf = new XTextFormatter(gfx);
        tf.Alignment = XParagraphAlignment.Center;
        XFont font = new XFont("Times New Roman", 25, XFontStyle.Bold);
        double verticalPos = 50 + margin;
        XRect titleRect = new XRect(0, verticalPos, page.Width, 25);
        tf.Alignment = XParagraphAlignment.Center;
        tf.DrawString(SettingsHolder.Instance.settings.shipName, font, XBrushes.Black, titleRect);
        font = new XFont("Times New Roman", 18, XFontStyle.Regular);
        verticalPos += 25;
        tf.DrawString("ΑΝΑΦΟΡΑ ΕΠΙΣΚΕΥΩΝ", font, XBrushes.Black, new XRect(0, verticalPos, page.Width, 18));
        font = new XFont("Times New Roman", 16, XFontStyle.Regular);
        verticalPos += 30;
        tf.DrawString(string.Format("Ημερομηνία {0}", DateTime.Now.ToString("dd-MM-yy")), font, XBrushes.Black, new XRect(0, verticalPos, page.Width, 16));


        return (Math.Max(finalWidthHN, finalWidthBadge) + margin / 2, Math.Max(finalHeightBadge, finalHeightHN) + margin / 2);
    }



}
