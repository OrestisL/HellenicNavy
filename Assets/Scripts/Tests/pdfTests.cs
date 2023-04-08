using UnityEngine;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using TMPro;
using System.IO;
using PdfSharpCore.Drawing.Layout;
using System;
using System.Collections.Generic;
using System.Threading;
using UnitySQLite.Utilities;

public class pdfTests : MonoBehaviour
{
    public TMP_InputField input;
    public int margin = 30;
    public int fontSize = 12;
    public PdfPage currentPage;
    public XGraphics gfx;
    public PdfDocument pdfDocument;
    private void Start()
    {
        if (input.text.Length == 0)
            input.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu.";

        input.onDeselect.AddListener((string s) =>
        {
            // Create a new PDF document
            pdfDocument = new PdfDocument();
            //document.Info.Title = "Report xx-xx-xx";

            // Create an empty page
            currentPage = pdfDocument.AddPage();

            int width = (int)currentPage.Width;
            int height = (int)currentPage.Height;

            // Get an XGraphics object for drawing
            gfx = XGraphics.FromPdfPage(currentPage);
            XTextFormatter textFormatter = new XTextFormatter(gfx);

            // Create a font
            XFont font = new XFont("Verdana", fontSize, XFontStyle.Regular);

            //add an image
            //DrawImageOriginalSize(gfx, page, SettingsHolder.Instance.settings.FullPath);
            //Vector2 imgSize = DrawImageScaled(gfx, page, SettingsHolder.Instance.settings.BadgeFullPath, 150, 150);
            string badgePath = SettingsHolder.Instance.settings.BadgeFullPath;
            string hnBadgePath = SettingsHolder.Instance.settings.HNFullPath;
            string shipName = SettingsHolder.Instance.settings.shipName;
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>
            {
                { "test1", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test2", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test3", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test4", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test5", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test6", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test7", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test8", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test9", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test10", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test11", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test12", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test13", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test14", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test15", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test16", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test17", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test18", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test19", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
                { "test20", new List<string> { "test", "test1", "test3", "test", "test1", "test3" } },
            };
            ThreadedCreatePDF(75, badgePath, hnBadgePath, shipName, dict);
            //(double maxW, double maxH) = CreateHeaderTemplate(gfx, page, 75, badgePath, hnBadgePath, shipName);
            //create the text rect
            //XRect textRect = new XRect(margin, margin / 2 + maxH, page.Width - 2 * margin, page.Height - 2 * margin);
            //gfx.DrawRectangle(XBrushes.AntiqueWhite, textRect);
            //textFormatter.Alignment = XParagraphAlignment.Justify;
            // Draw the text
            //textFormatter.DrawString(s, font, XBrushes.Black, textRect, XStringFormats.TopLeft);

            //CreateTableInDocument(gfx, page, margin, maxH + 10, dict);
            // Save the document           
            //SavePdfDocument(document);
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

    void ThreadedCreatePDF(double biggestEdge, string badgePath, string hnBadgePath, string shipName, Dictionary<string, List<string>> contents)
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
        {
            showLoadingIndicator = true,
            useLeftButton = false,
            useRightButton = false,
            mainText = "Παρακαλώ περιμένετε...",
            showLabel = false,
        }, -1);
        Thread worker = new Thread(() =>
        {
            //create header
            (double maxW, double maxH) = CreateHeaderTemplate(gfx, biggestEdge, badgePath, hnBadgePath, shipName);
            //create table
            CreateTableInDocument(margin, maxH, contents);
            //save
            string path = SavePdfDocument(pdfDocument);
            //hide message box
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                MessageBox.Instance.HideMessageBox();
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
                {
                    showLoadingIndicator = false,
                    useLeftButton = false,
                    useRightButton = false,
                    mainText = string.Format("Η αναφορά αποθηκεύτηκε επιτυχώς στην τοποθεσία {0}", path),
                    showLabel = false,
                });
            });
        });

        worker.Start();
    }

    (double, double) CreateHeaderTemplate(XGraphics gfx, double biggestEdge, string badgePath, string hnBadgePath, string shipName)
    {
        //load images
        XImage badgeImg = XImage.FromFile(badgePath);
        XImage hnImg = XImage.FromFile(hnBadgePath);
        //string shipName = SettingsHolder.Instance.settings.shipName;

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
        gfx.DrawImage(hnImg, currentPage.Width - finalWidthHN - margin / 2, margin / 2, finalWidthHN, finalHeightHN);

        //draw text between images
        XTextFormatter tf = new XTextFormatter(gfx);
        tf.Alignment = XParagraphAlignment.Center;
        XFont font = new XFont("Verdana", 25, XFontStyle.Bold);

        double verticalPos = margin - 5;
        double horzSize = currentPage.Width - finalWidthHN - finalWidthBadge - 2 * margin;

        XRect titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 25);
        tf.Alignment = XParagraphAlignment.Center;
        tf.DrawString(shipName, font, XBrushes.Black, titleRect);

        font = new XFont("Verdana", 18, XFontStyle.Regular);
        verticalPos += 30;
        titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 18);
        tf.DrawString("ΑΝΑΦΟΡΑ ΕΠΙΣΚΕΥΩΝ", font, XBrushes.Black, titleRect);

        font = new XFont("Verdana", 16, XFontStyle.Regular);
        verticalPos += 20;
        titleRect = new XRect(finalWidthBadge + margin, verticalPos, horzSize, 16);
        tf.DrawString(string.Format("Ημερομηνία {0}", DateTime.Now.ToString("dd-MM-yy")), font, XBrushes.Black, titleRect);

        return (Math.Max(finalWidthHN, finalWidthBadge) + margin / 2, Math.Max(finalHeightBadge, finalHeightHN) + (float)margin * 0.66f);
    }

    string SavePdfDocument(PdfDocument document)
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
            return Path.Combine(reportsPath, string.Format("{0}.pdf", filename));
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
                    return Path.Combine(reportsPath, string.Format("{0}.pdf", currentName));
                }
            }

        }
    }

    //the dictionary should change to a json file probably, and the loop should also change
    void CreateTableInDocument(double offsetX, double offsetY, Dictionary<string, List<string>> contents)
    {
        // Text format
        XStringFormat format = new XStringFormat();
        format.LineAlignment = XLineAlignment.Near;
        format.Alignment = XStringAlignment.Near;
        XTextFormatter tf = new XTextFormatter(gfx);

        //fonts
        XFont cellFont = new XFont("Verdana", 6, XFontStyle.Regular);
        XFont headerFont = new XFont("Verdana", 7, XFontStyle.Bold);

        //element dimensions
        int elementWidth = (int)(currentPage.Width - margin) / 2; //282
        int doubleElementWidth = 2 * elementWidth;
        ////TODO change height according to how many lines there are per machinery, because 120 is too big
        ////or ask if it looks ok
        int elementHeight = 40; //consider changing? with this height page can hold a table of 6x2 cells
                                //add new page after 6
                                //also need new page for comments

        //offset between lines
        double lineOffset = 1;
        double doubleLineOffset = 2 * lineOffset;

        //color of squares
        XSolidBrush rectStyle = new XSolidBrush(XColors.White);

        //first draw the background black rect
        int amountElements = contents.Keys.Count; //keys will be machinery names
        gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY, doubleElementWidth + doubleLineOffset + lineOffset - margin, amountElements * (elementHeight + lineOffset) + lineOffset);

        int i = -1;
        foreach (KeyValuePair<string, List<string>> item in contents)
        {
            double currentYOffset = offsetY + lineOffset * (i + 2) + elementHeight * (i + 1);
            double currentXOffset = margin;
            //table should probably be like this
            //  NAME    ID  SYSTEM  DEPT    ServiceDescr    Status
            //  82      40  50      20      290             82        (widths)
            //used to be 2 cells 282x120 each = 564x120 total (check into these)
            double nameWidth = 75;
            double idWidth = 42;
            double systemWidth = 50;
            double deptWidth = 20;
            double serviceDescrWidth = 262;
            double statusWidth = 82;

            //name box
            XRect nameRect = new XRect(currentXOffset, currentYOffset, nameWidth, elementHeight);
            gfx.DrawRectangle(rectStyle, nameRect);
            //id box
            currentXOffset += nameWidth + lineOffset;
            XRect idRect = new XRect(currentXOffset, currentYOffset, idWidth, elementHeight);
            gfx.DrawRectangle(rectStyle, idRect);
            //system box
            currentXOffset += idWidth + lineOffset;
            //XRect systemRect = new XRect(currentXOffset, currentYOffset, systemWidth, elementHeight);
            //gfx.DrawRectangle(rectStyle, systemRect);
            ////dept box
            //currentXOffset += systemWidth + lineOffset;
            //XRect deptRect = new XRect(currentXOffset, currentYOffset, deptWidth, elementHeight);
            //gfx.DrawRectangle(rectStyle, deptRect);
            XRect deptAndSystemRect = new XRect(currentXOffset, currentYOffset, deptWidth + systemWidth, elementHeight);
            gfx.DrawRectangle(rectStyle, deptAndSystemRect);
            //service box
            currentXOffset += systemWidth + deptWidth + lineOffset;
            XRect serviceDescrRect = new XRect(currentXOffset, currentYOffset, serviceDescrWidth, elementHeight);
            gfx.DrawRectangle(rectStyle, serviceDescrRect);
            //status box
            currentXOffset += serviceDescrWidth + lineOffset;
            XRect statusRect = new XRect(currentXOffset, currentYOffset, statusWidth, elementHeight);
            gfx.DrawRectangle(rectStyle, statusRect);


            //write inside boxes
            if (i == -1)
            {
                tf.DrawString("ΟΝΟΜΑ ΜΗΧΑΝΗΜΑΤΟΣ", headerFont, XBrushes.Black, nameRect);
                tf.DrawString("ΚΩΔΙΚΟΣ", headerFont, XBrushes.Black, idRect);
                //tf.DrawString("ΣΥΣΤΗΜΑ", headerFont, XBrushes.Black, systemRect);
                //tf.DrawString("ΕΠΙΣΤΑΣΙΑ", headerFont, XBrushes.Black, deptRect);
                tf.DrawString("ΣΥΣΤΗΜΑ ΚΑΙ\nΕΠΙΣΤΑΣΙΑ", headerFont, XBrushes.Black, deptAndSystemRect);
                tf.DrawString("ΠΕΡΙΓΡΑΦΗ ΕΠΙΣΚΕΥΗΣ", headerFont, XBrushes.Black, serviceDescrRect);
                tf.DrawString("ΚΑΤΑΣΤΑΣΗ", headerFont, XBrushes.Black, statusRect);
                i++;
                continue;
            }

            //machinery box
            tf.DrawString(item.Key, cellFont, XBrushes.Black, nameRect, format);
            //info box
            string infos = string.Empty;
            for (int j = 0; j < item.Value.Count; j++)
            {
                infos += string.Format("{0}\n", item.Value[j]);
            }
            infos.Remove(infos.Length - 2, 1);
            tf.DrawString(infos, cellFont, XBrushes.Black, serviceDescrRect, format);
            i++;

            if (i % 17 == 0 & i != 0)
            {
                currentPage.Close();
                gfx.Dispose();
                currentPage = pdfDocument.AddPage();
                gfx = XGraphics.FromPdfPage(currentPage);
                tf = null;
                tf = new XTextFormatter(gfx);
            }
            //TODO change how changing pages works
            //resize black box for background
            //atm first page holds up to 18 entries, other pages can hold more because no header
            //also after table add a box for remarks

        }
    }
}
