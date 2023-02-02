using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Pabo.Calendar;


namespace ICalendar
{
    public partial class YearCalendar : Form
    {
        ToolTip tp;
        ContextMenuStrip cMenu;
        private GoogleCalendarImporter eventImporter;
        const int MonthsInAYear = 12;
        int year = DateTime.Now.Year;
        Pabo.Calendar.MonthCalendar[] months;

        const string CredentialsPath = "./credentials.json";
        const string TokenPath = "./token.json";
        const string ApplicationName = "Google Calendar API .NET Quickstart";

        private void Init()
        {
            months = new[]
            {
                monthCalendar1,
                monthCalendar2,
                monthCalendar3,
                monthCalendar4,
                monthCalendar5,
                monthCalendar6,
                monthCalendar7,
                monthCalendar8,
                monthCalendar9,
                monthCalendar10,
                monthCalendar11,
                monthCalendar12
            };


            tp = new ToolTip();

            cMenu = new ContextMenuStrip();
            cMenu.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);

            for (int monthIdx = 1; monthIdx <= months.Length; monthIdx++)
            {
                var noOfDaysInMonth = DateTime.DaysInMonth(year, monthIdx);
                months[monthIdx - 1].MinDate = new DateTime(year, monthIdx, 1);
                months[monthIdx - 1].MaxDate = new DateTime(year, monthIdx, noOfDaysInMonth);
                months[monthIdx - 1].ActiveMonth.Year = year;
                months[monthIdx - 1].ActiveMonth.Month = monthIdx;
                months[monthIdx - 1].ContextMenuStrip = cMenu;
                months[monthIdx - 1].ShowToday = true;
                months[monthIdx - 1].FirstDayOfWeek = 2;
                months[monthIdx - 1].DayGotFocus += DayGotFocus;
                months[monthIdx - 1].DayLostFocus += DayLostFocus;
                months[monthIdx - 1].Header.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx - 1].Header.TextColor = Color.Black;
                months[monthIdx - 1].KeyboardEnabled = false;
                months[monthIdx - 1].Weekdays.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx - 1].Weekdays.TextColor = Color.Black;

                months[monthIdx - 1].Month.Colors.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx - 1].Month.Colors.Days.Text = Color.Black;
                months[monthIdx - 1].Month.Colors.Days.BackColor1 = SystemColors.ActiveCaption;
            }
        }

        public YearCalendar()
        {
            var year = DateTime.Today.Year;
            var googleOptions = new GoogleOptions()
            {
                ApplicationName = ApplicationName,
                CredentialsPath = CredentialsPath,
                TokenPath = TokenPath
            };

            eventImporter = new GoogleCalendarImporter(googleOptions);
            InitializeComponent();
            Init();

            RefreshEvents();
        }

        // This event handler is invoked when the ContextMenuStrip
        // control's Opening event is raised. It demonstrates
        // dynamic item addition and dynamic SourceControl 
        // determination with reuse.
        void cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Acquire references to the owning control and item.
            Control c = cMenu.SourceControl as Control;
            ToolStripDropDownItem tsi = cMenu.OwnerItem as ToolStripDropDownItem;

            cMenu.Items.Clear();
            cMenu.Items.Add("Add event");
            cMenu.Items.Add("Remove event");
            cMenu.ItemClicked += new ToolStripItemClickedEventHandler(cMenu_EventClicked);
            e.Cancel = false;
        }

        void ClearDates()
        {
            for (int monthIdx = 1; monthIdx <= months.Length; monthIdx++)
            {
                months[monthIdx - 1].ResetDateInfo();
            }
        }

        internal void AddEventsToMonths()
        {
            eventImporter.RefreshEvents(year);

            var dateItems = new Dictionary<DateItem, int>();

            foreach (var ev in eventImporter.Events)
            {
                var startTime = ev.StartDate;
                var endTime = ev.EndDate;

                if (startTime != null && endTime != null)
                {
                    var currentDate = startTime;
                    var eventColor = ConvertToColor(ev.CalendarBkColor);

                    do
                    {
                        var monthIdx = currentDate.Month-1;
                        var dtItem = new DateItem();
                        dtItem.Date = currentDate;
                        dtItem.BackColor1 = eventColor;
                        dtItem.Enabled = true;
                        currentDate = currentDate.AddDays(1);
                        dateItems.Add(dtItem, monthIdx);
                    } while (currentDate.AddMinutes(1) <= endTime);
                }
            }

            for(int i=0 ; i<months.Length; i++) 
            {
                months[i].AddDateInfo(dateItems.Where(kvp=> kvp.Value==i).Select(kvp=> kvp.Key).ToArray());
            }            
        }

        private Color ConvertToColor(string colorString)
        {
            Color rgbColor = Color.White;
            colorString = colorString.Trim('#');

            var splitString = Enumerable.Range(0, colorString.Length / 2)
                    .Select(i => colorString.Substring(i * 2, 2));

            var splitInts = splitString.Select(item => int.Parse(item, System.Globalization.NumberStyles.HexNumber)).ToArray();
            rgbColor = Color.FromArgb(splitInts[0], splitInts[1], splitInts[2]);

            return rgbColor;
        }

        private void RefreshEvents()
        {
            ClearDates();
            AddEventsToMonths();
        }

        private void DayGotFocus(object sender, DayEventArgs e)
        {
            var selectedDate = Convert.ToDateTime(e.Date);
            var eventTitles = eventImporter.GetEventNamesForDay(selectedDate).ToList();
            var tpText = String.Join(Environment.NewLine, eventTitles);

            tp.BackColor = Color.LightYellow;
            tp.Show(tpText, (sender as Pabo.Calendar.MonthCalendar), 3000);

            (sender as Pabo.Calendar.MonthCalendar).SelectDate(selectedDate);
        }

        private void DayLostFocus(object sender, DayEventArgs e)
        {
            if (tp != null)
            {
                tp.Hide((sender as Pabo.Calendar.MonthCalendar));
            }
        }

        void cMenu_EventClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem menuItem = e.ClickedItem;
            ContextMenuStrip menuStrip = menuItem.Owner as ContextMenuStrip;
            Pabo.Calendar.MonthCalendar month = menuStrip.SourceControl as Pabo.Calendar.MonthCalendar;
            SelectedDatesCollection selected = month.SelectedDates;

            if (selected.Count == 0)
                return;

            switch (e.ClickedItem.Text)
            {
                case "Add event":
                    AddEvent(selected);
                    break;
                case "Remove event":
                    RemoveEvent(selected);
                    break;
                default:
                    break;
            }
        }

        private void RemoveEvent(SelectedDatesCollection selected)
        {
            var eventsOfTheDay = eventImporter.GetEventsForDay(selected[0]).ToArray();
            EventList evList = new EventList(eventsOfTheDay);

            if (evList.ShowDialog() == DialogResult.OK)
            {
                eventImporter.DeleteEvent(evList.ReturnedEvent.Id, evList.ReturnedEvent.CalendarId);
                RefreshEvents();
            }
        }

        private void AddEvent(SelectedDatesCollection selected)
        {
            var evDescForm = new EventDescription(
                selected[0],
                selected[selected.Count-1], eventImporter.Calendars.ToArray());

            if (evDescForm.ShowDialog() == DialogResult.OK)
            {
                eventImporter.AddEvent(evDescForm.ReturnedEvent);
                RefreshEvents();
            }
        }
    }
}
