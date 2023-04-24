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
using SPS;
using System.Runtime.Remoting.Contexts;
using static UnityEngine.EventSystems.EventTrigger;

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
            List<ReportEntry> entries = new List<ReportEntry>()
            {
                new ReportEntry("test 1","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 2","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 3","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 4","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 5","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 6","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 7","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 8","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 9","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 10","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 11","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 12","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 13","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 14","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 15","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 16","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 17","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 18","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 19","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 20","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 21","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 22","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 23","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 1","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 2","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 3","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 4","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 5","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 6","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 7","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 8","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 9","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 10","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 11","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 12","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 13","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 14","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 15","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 16","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 17","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 18","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 19","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 20","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 21","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 22","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 23","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"), new ReportEntry("test 1","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 2","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 3","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 4","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 5","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 6","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 7","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 8","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 9","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 10","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 11","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 12","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 13","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 14","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 15","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 16","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 17","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 18","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 19","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 20","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 21","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 22","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 23","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 1","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 2","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 3","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 4","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 5","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 6","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 7","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 8","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 9","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 10","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 11","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 12","","system","dept", "some description 123","ΟΛΟΚΛΗΡΩΘΗΚΕ"),
                new ReportEntry("test 13","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 14","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 15","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 16","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 17","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 18","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 19","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 20","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 21","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 22","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),
                new ReportEntry("test 23","","system","dept", "some description 123","ΑΝΑΒΛΗΘΗΚΕ"),

            };

            Report.AddReportEntries(entries);

            string remarks = "Δοκιμη παρατηρησεων υπολογου\nδευτερη γραμμη κλπ";
            ThreadedCreatePDF(75, badgePath, hnBadgePath, shipName, remarks);
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
    void ThreadedCreatePDF(double biggestEdge, string badgePath, string hnBadgePath, string shipName, string remarks)
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
            CreateReport(margin, maxH, remarks);
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

    //the dictionary should change to a json file (String) probably, and the loop should also change
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
        int elementHeight = 40; //consider changing? with this height page can hold a table of 18 rows

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
            //  NAME    ID  SYSTEM  &   DEPT    ServiceDescr    Status
            //  75      42  50          20      262             82        (widths)
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
                i = 0;
            }
            //TODO change how changing pages works
            //resize black box for background
            //atm first page holds up to 18 entries, other pages can hold more because no header
            //also after table add a box for remarks

        }
    }

    void CreateReport(double offsetX, double offsetY, string remarks)
    {
        //first check how many entries the current report has
        List<ReportEntry> reportEntries = Report.ConsumeData();
        if (reportEntries.Count == 0)
        {
            //there is no data, so we should just return
            //there will be a warning shown from the ConsumeData function
            //return here to avoid unecessary memory allocation
            return;
        }

        //all pages will have the header
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
        int elementHeight = 40; //consider changing? with this height page can hold a table of 18 rows

        //offset between lines
        double lineOffset = 1;
        double doubleLineOffset = 2 * lineOffset;
        double currentYPosition = 0;
        //color of squares
        XSolidBrush rectStyle = new XSolidBrush(XColors.White);

        int amountElements = reportEntries.Count;
        //draw black background square
        if (amountElements <= 18)
        {
            //each page holds a table with 18 rows
            //if we have <= 18, then there's no need for more than 1 black square background
            gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY,
                doubleElementWidth + doubleLineOffset + lineOffset - margin,
                amountElements * (elementHeight + lineOffset) + lineOffset);

            int i = -1;
            foreach (ReportEntry entry in reportEntries)
            {
                double currentYOffset = offsetY + lineOffset * (i + 2) + elementHeight * (i + 1);
                double currentXOffset = margin;
                //table should probably be like this
                //  NAME    ID  SYSTEM  &   DEPT    ServiceDescr    Status
                //  75      42  50          20      262             82        (widths)
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
                XRect systemAndDeptRect = new XRect(currentXOffset, currentYOffset, deptWidth + systemWidth, elementHeight);
                gfx.DrawRectangle(rectStyle, systemAndDeptRect);
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
                    tf.DrawString("ΣΥΣΤΗΜΑ ΚΑΙ\nΕΠΙΣΤΑΣΙΑ", headerFont, XBrushes.Black, systemAndDeptRect);
                    tf.DrawString("ΠΕΡΙΓΡΑΦΗ ΕΠΙΣΚΕΥΗΣ", headerFont, XBrushes.Black, serviceDescrRect);
                    tf.DrawString("ΚΑΤΑΣΤΑΣΗ", headerFont, XBrushes.Black, statusRect);
                    i++;
                    continue;
                }

                //machinery name box
                tf.DrawString(entry.MachineryName, cellFont, XBrushes.Black, nameRect, format);
                //id box
                tf.DrawString(entry.MachineryID, cellFont, XBrushes.Black, idRect, format);
                //system & dept box
                tf.DrawString(string.Format("{0}\n{1}", entry.System, entry.Department), cellFont, XBrushes.Black, systemAndDeptRect, format);
                //service description box
                tf.DrawString(entry.ServiceDescription, cellFont, XBrushes.Black, serviceDescrRect, format);
                //status box
                tf.DrawString(entry.ServiceStatus, cellFont, XBrushes.Black, statusRect, format);

                i++;
            }

        }
        else
        {
            //more than 1 page
            //draw the background for the first one
            gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY,
               doubleElementWidth + doubleLineOffset + lineOffset - margin,
               18 * (elementHeight + lineOffset) + lineOffset);

            int currentPageNo = 0;
            int j = -1;
            for (int i = 0; i < amountElements; i++)
            {
                ReportEntry entry = reportEntries[i];
                double currentYOffset = offsetY + lineOffset * (i % 18 + 1) + elementHeight * (i % 18 + 0);
                currentYPosition = currentYOffset;
                double currentXOffset = margin;

                //first check if page full
                if (i % 18 == 0 & i != 0)
                {
                    currentPageNo++;
                    currentPage.Close();
                    gfx.Dispose();
                    currentPage = pdfDocument.AddPage();
                    gfx = XGraphics.FromPdfPage(currentPage);
                    tf = null;
                    tf = new XTextFormatter(gfx);
                    //reset j to -1 to rewrite headers when changing pages
                    j = -1;
                    //insert empty here to avoid losing the first entry
                    reportEntries.Insert(i, new ReportEntry());
                    //draw new background
                    int remaining = amountElements - currentPageNo * 18;
                    if (remaining == 0)
                        return;

                    if (remaining > 18)
                    {
                        gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY,
                            doubleElementWidth + doubleLineOffset + lineOffset - margin,
                            18 * (elementHeight + lineOffset) + lineOffset);
                    }
                    else
                    {
                        gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY,
                            doubleElementWidth + doubleLineOffset + lineOffset - margin,
                            remaining * (elementHeight + lineOffset) + lineOffset);
                    }
                }

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
                XRect systemAndDeptRect = new XRect(currentXOffset, currentYOffset, deptWidth + systemWidth, elementHeight);
                gfx.DrawRectangle(rectStyle, systemAndDeptRect);
                //service box
                currentXOffset += systemWidth + deptWidth + lineOffset;
                XRect serviceDescrRect = new XRect(currentXOffset, currentYOffset, serviceDescrWidth, elementHeight);
                gfx.DrawRectangle(rectStyle, serviceDescrRect);
                //status box
                currentXOffset += serviceDescrWidth + lineOffset;
                XRect statusRect = new XRect(currentXOffset, currentYOffset, statusWidth, elementHeight);
                gfx.DrawRectangle(rectStyle, statusRect);

                //write inside boxes
                if (j == -1)
                {
                    tf.DrawString("ΟΝΟΜΑ ΜΗΧΑΝΗΜΑΤΟΣ", headerFont, XBrushes.Black, nameRect);
                    tf.DrawString("ΚΩΔΙΚΟΣ", headerFont, XBrushes.Black, idRect);
                    //tf.DrawString("ΣΥΣΤΗΜΑ", headerFont, XBrushes.Black, systemRect);
                    //tf.DrawString("ΕΠΙΣΤΑΣΙΑ", headerFont, XBrushes.Black, deptRect);
                    tf.DrawString("ΣΥΣΤΗΜΑ ΚΑΙ\nΕΠΙΣΤΑΣΙΑ", headerFont, XBrushes.Black, systemAndDeptRect);
                    tf.DrawString("ΠΕΡΙΓΡΑΦΗ ΕΠΙΣΚΕΥΗΣ", headerFont, XBrushes.Black, serviceDescrRect);
                    tf.DrawString("ΚΑΤΑΣΤΑΣΗ", headerFont, XBrushes.Black, statusRect);
                    j++;
                    continue;
                }

                //machinery name box
                tf.DrawString(entry.MachineryName, cellFont, XBrushes.Black, nameRect, format);
                //id box
                tf.DrawString(entry.MachineryID, cellFont, XBrushes.Black, idRect, format);
                //system & dept box
                tf.DrawString(string.Format("{0}\n{1}", entry.System, entry.Department), cellFont, XBrushes.Black, systemAndDeptRect, format);
                //service description box
                tf.DrawString(entry.ServiceDescription, cellFont, XBrushes.Black, serviceDescrRect, format);
                //status box
                tf.DrawString(entry.ServiceStatus, cellFont, XBrushes.Black, statusRect, format);
            }

        }

        //remarks 
        double remarkWidth = currentPage.Width - 2 * margin;
        double remarkHeaderHeight = 12;
        double remarkHeight = 200 - remarkHeaderHeight; //might change
        currentYPosition += 2 * elementHeight;
        //check if box fits
        if (currentYPosition + remarkHeight + remarkHeaderHeight + 20 > currentPage.Height)
        {
            //box does not fit
            currentPage.Close();
            gfx.Dispose();
            currentPage = pdfDocument.AddPage();
            gfx = XGraphics.FromPdfPage(currentPage);
            tf = null;
            tf = new XTextFormatter(gfx);

            XRect remarksBG =
                new XRect(margin - lineOffset, offsetY - lineOffset, remarkWidth + doubleLineOffset, remarkHeight + remarkHeaderHeight + doubleLineOffset);
            XRect remarksHeader = new XRect(margin, offsetY, remarkWidth, remarkHeaderHeight);
            XRect remarksRect = new XRect(margin, offsetY + remarkHeaderHeight, remarkWidth, remarkHeight);

            gfx.DrawRectangle(XBrushes.Black, remarksBG);
            gfx.DrawRectangle(rectStyle, remarksHeader);
            gfx.DrawRectangle(rectStyle, remarksRect);

            tf.DrawString("ΠΑΡΑΤΗΡΗΣΕΙΣ ΥΠΟΛΟΓΟΥ", new XFont("Verdana", 12), XBrushes.Black, remarksHeader);
            tf.DrawString(remarks, cellFont, XBrushes.Black, remarksRect);
        }
        else
        {
            //box fits

            XRect remarksBG =
                new XRect(margin - lineOffset, currentYPosition - lineOffset, remarkWidth + doubleLineOffset, remarkHeight + remarkHeaderHeight + doubleLineOffset);
            XRect remarksHeader = new XRect(margin, currentYPosition, remarkWidth, remarkHeaderHeight);
            XRect remarksRect = new XRect(margin, currentYPosition + remarkHeaderHeight, remarkWidth, remarkHeight);

            gfx.DrawRectangle(XBrushes.Black, remarksBG);
            gfx.DrawRectangle(rectStyle, remarksHeader);
            gfx.DrawRectangle(rectStyle, remarksRect);

            tf.DrawString("ΠΑΡΑΤΗΡΗΣΕΙΣ ΥΠΟΛΟΓΟΥ", new XFont("Verdana", 8, XFontStyle.Bold), XBrushes.Black, remarksHeader);
            tf.DrawString(remarks, cellFont, XBrushes.Black, remarksRect);
        }
    }

}
