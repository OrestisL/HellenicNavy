using UnityEngine;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using TMPro;
using System.IO;
using System.Text;
using PdfSharpCore.Drawing.Layout;
using System;
using Unity.VisualScripting;

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
            XFont font = new XFont("Verdana", fontSize, XFontStyle.Regular);

            //add an image
            //DrawImageOriginalSize(gfx, page, SettingsHolder.Instance.settings.FullPath);
            //Vector2 imgSize = DrawImageScaled(gfx, page, SettingsHolder.Instance.settings.BadgeFullPath, 150, 150);
            (double maxW, double maxH) = CreateHeaderTemplate(gfx, page, 75);
            //create the text rect
            XRect textRect = new XRect(margin, margin / 2 + maxH, page.Width - 2 * margin, page.Height - 2 * margin);
            gfx.DrawRectangle(XBrushes.AntiqueWhite, textRect);
            textFormatter.Alignment = XParagraphAlignment.Justify;
            // Draw the text
            textFormatter.DrawString(s, font, XBrushes.Black, textRect, XStringFormats.TopLeft);

            // Save the document           
            SavePdfDocument(document);
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
        string shipName = SettingsHolder.Instance.settings.shipName;

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
        XFont font = new XFont("Verdana", 25, XFontStyle.Bold);
        double verticalPos = margin - 5;
        double horzSize = page.Width - finalWidthHN - finalWidthBadge - 2 * margin;
        XRect titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 25);
        gfx.DrawRectangle(XBrushes.Black, titleRect);
        tf.Alignment = XParagraphAlignment.Center;
        tf.DrawString(shipName, font, XBrushes.Black, titleRect);
        font = new XFont("Verdana", 18, XFontStyle.Regular);
        verticalPos += 30;
        titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 18);
        gfx.DrawRectangle(XBrushes.Black, titleRect);
        tf.DrawString("ΑΝΑΦΟΡΑ ΕΠΙΣΚΕΥΩΝ", font, XBrushes.Black, titleRect);
        font = new XFont("Verdana", 16, XFontStyle.Regular);
        verticalPos += 20;
        titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 16);
        gfx.DrawRectangle(XBrushes.Black, titleRect);
        tf.DrawString(string.Format("Ημερομηνία {0}", DateTime.Now.ToString("dd-MM-yy")), font, XBrushes.Black, titleRect);

        return (Math.Max(finalWidthHN, finalWidthBadge) + margin / 2, Math.Max(finalHeightBadge, finalHeightHN) + margin / 2);
    }

    void SavePdfDocument(PdfDocument document)
    {
        string reportsPath = Path.Combine(Directory.GetCurrentDirectory(), "Αναφορές");
        if (!Directory.Exists(reportsPath))
        {
            Directory.CreateDirectory("Αναφορές");
        }

        //check if file already exists
        string filename = string.Format("Αναφορά {0}", DateTime.Now.ToString("dd-MM-yy"));
        //first time creating the file it should not exist
        if (!File.Exists(Path.Combine(reportsPath, string.Format("{0}.pdf", filename))))
        {
            document.Save(Path.Combine(reportsPath, string.Format("{0}.pdf", filename)));
            Debug.Log(string.Format("Successfully saved report {0} at {1}", filename, Path.Combine(reportsPath, filename)));
            return;
        }
        //if multiple files are saved within the same day, check and add a (#) at the end
        else
        {
            for (int i = 1; ; i++)
            {
                string currentName = string.Format("{0} ({1})", filename, i);
                if (!File.Exists(Path.Combine(reportsPath, string.Format("{0}.pdf", currentName))))
                {
                    document.Save(Path.Combine(reportsPath, string.Format("{0}.pdf", currentName)));
                    Debug.Log(string.Format("Successfully saved report {0} at {1}.pdf", currentName, Path.Combine(reportsPath, currentName)));
                    return;
                }
            }

        }
    }


    void CreateTableInDocument(XGraphics gfx, PdfPage page, int amountElements, double offsetX, double offsetY)
    {
        // Text format
        XStringFormat format = new XStringFormat();
        format.LineAlignment = XLineAlignment.Near;
        format.Alignment = XStringAlignment.Near;
        XTextFormatter tf = new XTextFormatter(gfx);

        //fonts
        XFont cellFont = new XFont("Verdana", 10, XFontStyle.Regular);
        XFont headerFont = new XFont("Verdana", 11, XFontStyle.Bold);

        //offset between lines
        double lineHeight = 20;

        //element element dimensions
        int elementWidth = (int)(page.Width - margin) / 2;
        int doubleElementWidth = 2 * elementWidth;
        int elementHeight = 120; //consider changing?

        int lineOffset = 1;
        int doubleLineOffset = 2 * lineOffset;

        //color of squares
        XSolidBrush rect_style1 = new XSolidBrush(XColors.White);

        //first draw the background black rect
        gfx.DrawRectangle(XBrushes.Black, offsetX, offsetY, doubleElementWidth + doubleLineOffset, amountElements * elementHeight);

    }
}
