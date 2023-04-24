using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnitySQLite.Utilities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing.Layout;
using System.Drawing.Printing;
using System.IO;
using System.Threading;

namespace SPS
{
    /// <summary>
    /// Each service will contain some assignments. These will be decribed here. WIP
    /// </summary>
    [Serializable]
    public class ServiceAssignment
    {
        //public string date;
        public string description;
        public bool isCompleted;

        public ServiceAssignment(/*string _date,*/ string _description, bool _isCompleted)
        {
            //date = _date;
            description = _description;
            isCompleted = _isCompleted;
        }

        public void Complete()
        {
            isCompleted = true;
        }
    }

    [Serializable]
    public enum ServiceAssignmentType
    {
        None = 0,
        Spot_Check,
        other,
        Overhaul,
    }
    [Serializable]
    public enum ServiceStatus
    {
        nothing,
        pending,
        completed,
        postponed
    }

    /// <summary>
    /// Service class holds all required information for each service.
    /// </summary>
    [Serializable]
    public class Service
    {
        public string name, id, descr;
        private int _currentHours;
        public DateTime lastServiceDate;
        public int lastServiceHours;
        public int nextServiceHours;
        //public ServiceStatus status;
        public string postponedServiceDescr;
        public string completedServiceDescr;
        public string serviceHistoryPostponed;
        public string serviceHistoryCompleted;
        public int CurrentHours
        {
            get { return _currentHours; }
            set { _currentHours = value; }
        }
        public int CurrentDays
        {
            get
            {
                return Mathf.Abs((lastServiceDate - DateTime.Now).Days);
            }
        }
        public int lastServiceDays;
        public int nextServiceDays;
        public int systemName;
        public List<string> descriptions;
        public List<int> serviceHours;
        public List<int> serviceDays;
        public List<ServiceAssignmentType> serviceTypesHours;
        public List<ServiceAssignmentType> serviceTypesDays;
        public List<ServiceStatus> serviceStatuses;
        public List<List<ServiceAssignment>> serviceAssignments; //WIP
        public List<List<ServiceStatus>> serviceAssignmentsStatuses;

        public Service() { }

        public Service(string json)
        {
            json = json.Replace(".", ",");
            Service s = JsonConvert.DeserializeObject<Service>(json);
            name = s.name;
            id = s.id;
            descr = s.descr;
            systemName = s.systemName;
            CurrentHours = s.CurrentHours;
            lastServiceDate = s.lastServiceDate;
            postponedServiceDescr = s.postponedServiceDescr;
            completedServiceDescr = s.completedServiceDescr;
            serviceHistoryPostponed = s.serviceHistoryPostponed;
            serviceHistoryCompleted = s.serviceHistoryCompleted;
            lastServiceHours = s.lastServiceHours;
            nextServiceHours = s.nextServiceHours;
            lastServiceDays = s.lastServiceDays;
            nextServiceDays = s.nextServiceDays;
            descriptions = s.descriptions;
            serviceHours = s.serviceHours;
            serviceDays = s.serviceDays;
            serviceTypesHours = s.serviceTypesHours;
            serviceTypesDays = s.serviceTypesDays;
            serviceStatuses = s.serviceStatuses;
            serviceAssignments = s.serviceAssignments;
            serviceAssignmentsStatuses = s.serviceAssignmentsStatuses;
        }

        public Service(string name, string descr, string id, int hours, int lastHours, int nextHours, string lastDate, int system, List<ServiceEntry> entries)
        {
            this.name = name;
            this.id = id;
            this.descr = descr;
            CurrentHours = hours;
            //date SHOULD HAVE BEEN saved like this
            lastServiceDate = DateTime.ParseExact(lastDate, "dd-MM-yy", null);
            lastServiceHours = lastHours;
            nextServiceHours = nextHours;
            this.systemName = system;

            serviceHours = new List<int>();
            serviceDays = new List<int>();
            descriptions = new List<string>();
            serviceTypesHours = new List<ServiceAssignmentType>();
            serviceTypesDays = new List<ServiceAssignmentType>();
            serviceStatuses = new List<ServiceStatus>();
            serviceAssignments = new List<List<ServiceAssignment>>();
            serviceAssignmentsStatuses = new List<List<ServiceStatus>>();


            for (int i = 0; i < entries.Count; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                //serviceTypesHours.Add(entries[i].TypesHours);
                //serviceTypesDays.Add(entries[i].TypesDays);
                serviceStatuses.Add(entries[i].Status);
                serviceAssignments.Add(entries[i].assignments);
                serviceAssignmentsStatuses.Add(entries[i].assignmentsStatuses);
            }
        }

        public bool ChangeHours(int hours)
        {
            if (lastServiceHours > hours)
            {
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
                {
                    showLoadingIndicator = false,
                    mainText = "Οι ώρες λειτουργίας δεν γίνεται να είναι λιγότερες από τις ώρες προηγούμενης επισκευής.",
                    useRightButton = true,
                    rightButtonLabel = "OK",
                    onRightButtonClick = () => { MessageBox.Instance.HideMessageBox(); },
                    useLeftButton = false,
                    showLabel = false,
                }, -1);
                return false;
            }
            CurrentHours = hours;
            return true;
        }

        public void ChangeEntries(ServiceEntry[] entries)
        {

            serviceHours = new List<int>();
            serviceDays = new List<int>();
            descriptions = new List<string>();
            serviceTypesHours = new List<ServiceAssignmentType>();
            serviceTypesDays = new List<ServiceAssignmentType>();
            serviceStatuses = new List<ServiceStatus>();
            serviceAssignments = new List<List<ServiceAssignment>>();
            serviceAssignmentsStatuses = new List<List<ServiceStatus>>();

            for (int i = 0; i < entries.Length; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                //serviceTypesHours.Add(entries[i].TypesHours);
                //serviceTypesDays.Add(entries[i].TypesDays);
                serviceStatuses.Add(entries[i].Status);
                serviceAssignments.Add(entries[i].assignments);
                serviceAssignmentsStatuses.Add(entries[i].assignmentsStatuses);
            }
        }

        public void ChangeDescription(int index, string description)
        {
            descriptions[index] = description;
        }

        public void SetServiceStatusPostponed(ServiceEntry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].IsSelected)
                {
                    serviceHistoryPostponed += string.Format("Την {0} αναβλήθησαν οι κάτωθι επισκευές:\n", DateTime.Now.ToString("dd-MM-yy"));
                    postponedServiceDescr = string.Format("Την {0} αναβλήθησαν οι εξής επισκευές:", DateTime.Now.ToString("dd-MM-yy"));

                    entries[i].Status = ServiceStatus.postponed;
                    postponedServiceDescr = string.Format("{0}\n{1}", postponedServiceDescr, entries[i].Descr);
                    serviceHistoryPostponed += string.Format("{0}\n", entries[i].Descr);
                    entries[i].IsSelected = false;

                }
            }
        }

        public void SetServiceStatusCompleted(ServiceEntry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].IsSelected)
                {
                    serviceHistoryCompleted += string.Format("Την {0} ολοκληρώθηκαν οι κάτωθι επισκευές:\n", DateTime.Now.ToString("dd-MM-yy"));
                    completedServiceDescr = string.Format("Την {0} ολοκληρώθηκαν οι εξής επισκευές:", DateTime.Now.ToString("dd-MM-yy"));

                    entries[i].Status = ServiceStatus.completed;
                    completedServiceDescr = string.Format("{0}\n{1}", completedServiceDescr, entries[i].Descr);
                    serviceHistoryCompleted += string.Format("{0}\n", entries[i].Descr);
                    entries[i].IsSelected = false;
                }
            }

            if (entries.Length > 0)
            {
                lastServiceHours = CurrentHours / serviceHours.Min() * serviceHours.Min();
                nextServiceHours = (CurrentHours / serviceHours.Min() + 1) * serviceHours.Min();
                lastServiceDays = CurrentDays / serviceDays.Min() * serviceDays.Min();
                nextServiceDays = (CurrentDays / serviceDays.Min() + 1) * serviceDays.Min();

                UpdateServiceStatuses(entries.Select(entry => entry.Status).ToArray());
            }
        }

        public void UpdateServiceStatuses(ServiceStatus[] statuses)
        {
            serviceStatuses = statuses.ToList();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class ReportEntry 
    {
        private string _machineryName;
        public string MachineryName { get { return _machineryName; } }
        private string _machineryId;
        public string MachineryID { get { return _machineryId; } }
        private string _system;
        public string System { get { return _system; } }
        private string _dept;
        public string Department { get { return _dept; } }
        private string _serviceDescription;
        public string ServiceDescription { get { return _serviceDescription; } }
        private string _status;
        public string ServiceStatus { get { return _status; } }

        public ReportEntry(string name, string id, string sys, string dep, string servDesc, string stat)
        {
            _machineryName = name;
            _machineryId = id;
            _system = sys;
            _dept = dep;
            _serviceDescription = servDesc;
            _status = stat;
        }

        public ReportEntry() 
        {
            _machineryName = "";
            _machineryId = "";
            _system = "";
            _dept = "";
            _serviceDescription = "";
            _status = "";
        }
    }

    /// <summary>
    /// The Report class holds all info on the current report, and saves it into a pdf file.
    /// </summary>
    public class Report
    {
        private static List<ReportEntry> _reportEntries;
        private static bool _isReportPending;
        public static bool IsReportPending { get { return _isReportPending; } }

        public static int margin = 30;
        private static PdfPage currentPage;
        private static XGraphics gfx;
        private static PdfDocument pdfDocument;
        public static void AddReportEntry(ReportEntry entry)
        {
            if (_reportEntries == null)
                _reportEntries = new List<ReportEntry>
                {
                    new ReportEntry()
                };

            _isReportPending = true;
            _reportEntries.Add(entry);
        }

        public static void AddReportEntries(List<ReportEntry> reportEntries)
        {
            if (_reportEntries == null)
            {
                _reportEntries = new List<ReportEntry>
                {
                    new ReportEntry()
                };
            }

            _isReportPending = true;
            _reportEntries.AddRange(reportEntries);
        }

        public static List<ReportEntry> ConsumeData()
        {
            if (_reportEntries == null) 
            {
                Debug.LogWarning("Report Entry list is empty, no report can be printed.");
                return new List<ReportEntry>();
            }
            _isReportPending = false;
            return _reportEntries;
        }

        public static void ClearEntries() 
        {
            _reportEntries.Clear();
            _reportEntries = null;
        }

        public static void ThreadedCreatePDF(double biggestEdge, string badgePath, string hnBadgePath, string shipName, string remarks)
        {
            if (!IsReportPending) { return; } //report already done and no new info has been added
            // Create a new PDF document
            pdfDocument = new PdfDocument();

            // Create an empty page
            currentPage = pdfDocument.AddPage();

            int width = (int)currentPage.Width;
            int height = (int)currentPage.Height;

            // Get an XGraphics object for drawing
            gfx = XGraphics.FromPdfPage(currentPage);
            XTextFormatter textFormatter = new XTextFormatter(gfx);

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

        static (double, double) CreateHeaderTemplate(XGraphics gfx, double biggestEdge, string badgePath, string hnBadgePath, string shipName)
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

        static string SavePdfDocument(PdfDocument document)
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
      
        static void CreateReport(double offsetX, double offsetY, string remarks)
        {
            //first check how many entries the current report has
            List<ReportEntry> reportEntries = ConsumeData();
            if (reportEntries.Count == 0)
            {
                //there is no data, so we should just return
                //there will be a warning shown from the ConsumeData function
                //return here to avoid unecessary memory allocation
                Debug.Log("There are no report entries in the list. Aborting pdf creation.");
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
            XRect pageNoRect = new XRect(currentPage.Width - margin * 0.75f, currentPage.Height - margin * 0.5f, 10, 10);

            int currentPageNo = 0;
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

                //draw page number
                gfx.DrawRectangle(rectStyle, pageNoRect);
                tf.DrawString("1/1", cellFont, XBrushes.Black, pageNoRect, format);

            }
            else
            {
                //more than 1 page
                //draw the background for the first one
                gfx.DrawRectangle(XBrushes.Black, offsetX - lineOffset, offsetY,
                   doubleElementWidth + doubleLineOffset + lineOffset - margin,
                   18 * (elementHeight + lineOffset) + lineOffset);


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
                        //draw page number
                        //XRect pageNoRect = new XRect(currentPage.Width - margin * 0.6f, currentPage.Height - margin * 0.5f, 10, 10);
                        gfx.DrawRectangle(rectStyle, pageNoRect);
                        tf.DrawString(string.Format("{0}/{1}", currentPageNo + 1, amountElements / 18 + 1), cellFont, XBrushes.Black, pageNoRect, format);

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

            //draw page number
            //XRect pageNoRect = new XRect(currentPage.Width - margin * 0.6f, currentPage.Height - margin * 0.5f, 10, 10);
            gfx.DrawRectangle(rectStyle, pageNoRect);
            tf.DrawString(string.Format("{0}/{1}", currentPageNo + 1, amountElements / 18 + 1), cellFont, XBrushes.Black, pageNoRect, format);
            ClearEntries();
        }
    }
}
