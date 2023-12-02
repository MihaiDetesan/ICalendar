using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Calendar.v3.Data;
using Pabo.Calendar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace ICalendar
{
    public partial class YearCalendar : Form
    {
        ToolTip tp;
        ContextMenuStrip cMenu;
        private GoogleCalendarImporter eventImporter;
        const int MonthsInAYear = 12;
        Pabo.Calendar.MonthCalendar[] months;
        List<System.Windows.Forms.CheckBox> selectedCalendars;

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

            for (int monthIdx = 0; monthIdx < months.Length; monthIdx++)
            {
                var year = (int)((DateTime.Now.Month + monthIdx) / 13) + DateTime.Now.Year;
                var month = (int)((DateTime.Now.Month + monthIdx) / 13) + ((DateTime.Now.Month + monthIdx) % 13);
                var noOfDaysInMonth = DateTime.DaysInMonth(year, month);
                months[monthIdx].MinDate = new DateTime(year, month, 1);
                months[monthIdx].MaxDate = new DateTime(year, month, noOfDaysInMonth);
                months[monthIdx].ActiveMonth.Year = year;
                months[monthIdx].ActiveMonth.Month = month;
                months[monthIdx].ContextMenuStrip = cMenu;
                months[monthIdx].ShowToday = true;
                months[monthIdx].FirstDayOfWeek = 2;
                months[monthIdx].DayGotFocus += DayGotFocus;
                months[monthIdx].DayLostFocus += DayLostFocus;
                months[monthIdx].Header.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx].Header.TextColor = Color.Black;
                months[monthIdx].KeyboardEnabled = false;
                months[monthIdx].Weekdays.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx].Weekdays.TextColor = Color.Black;
                months[monthIdx].Month.Colors.BackColor1 = SystemColors.ActiveCaption;
                months[monthIdx].Month.Colors.Days.Text = Color.Black;
                months[monthIdx].Month.Colors.Days.BackColor1 = SystemColors.ActiveCaption;
            }

            //calendarsPanel.Dock = DockStyle.Fill;
            selectedCalendars = new List<System.Windows.Forms.CheckBox>();
        }

        public YearCalendar()
        {
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
            cMenu.Items.Add("Edit event");
            cMenu.ItemClicked += new ToolStripItemClickedEventHandler(cMenu_EventClicked);
            e.Cancel = false;
        }

        internal void AddEventsToMonths()
        {
            var dateItems = new List<DateItem>();

            //var holidayText = string.Empty;

            //foreach(var kvp in eventImporter.CommittedHolidayDaysPerYear)
            //{
            //    holidayText += $" Taken {kvp.Value} in year {kvp.Key}";
            //}

            //holidayDays.Text = holidayText;

            foreach (var ev in eventImporter.Events)
            {
                var startTime = ev.StartDate;
                var endTime = ev.EndDate.AddMinutes(-1);

                if (endTime.Subtract(startTime).Days > 100)
                {
                    throw new NullReferenceException($"Dates: {startTime} and {endTime}");
                }

                if (startTime != null && endTime != null)
                {
                    var currentDate = startTime;
                    var eventColor = ConvertToColor(ev.CalendarBkColor);

                    do
                    {
                        var dtItem = new DateItem();
                        dtItem.Date = currentDate;
                        dtItem.BackColor1 = eventColor;
                        dtItem.Enabled = true;
                        currentDate = currentDate.AddDays(1);
                        dateItems.Add(dtItem);
                    } while (currentDate.Date <= endTime.Date);
                }
            }

            Parallel.ForEach(months, month =>
            {
                month.ResetDateInfo();
                month.AddDateInfo(dateItems.Where(item => item.Date.Month == month.MinDate.Month).ToArray());
            });

            Invalidate(true);
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
            eventImporter.RefreshEvents(months[0].MinDate, months[11].MaxDate);

            AddSelectedCalendars(eventImporter.Calendars);
            AddEventsToMonths();
        }

        private void AddSelectedCalendars(IList<CalendarListEntry> calendars)
        {
            selectedCalendars.Clear();
            int checkboxHeight = calendarsPanel.Height / calendars.Count;

            foreach (var calendar in calendars) 
            {
                var calendarCheckBox = new System.Windows.Forms.CheckBox()
                {
                    Text = calendar.Summary,
                    Checked = calendar.Selected ?? false,
                    Dock = DockStyle.Top,
                    Height = checkboxHeight,
                };

                calendarCheckBox.CheckedChanged += (sender, eventArgs) =>
                {
                    eventImporter.ChageSelectStatus(
                        (sender as System.Windows.Forms.CheckBox).Text, 
                        (sender as System.Windows.Forms.CheckBox).Checked);

                    RefreshEvents();
                };

                selectedCalendars.Add(calendarCheckBox);
                calendarsPanel.Controls.Add(calendarCheckBox);
            }
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
                case "Edit event":
                    EditEvent(selected);
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

        private void EditEvent(SelectedDatesCollection selected)
        {
            var eventsOfTheDay = eventImporter.GetEventsForDay(selected[0]).ToArray();
            EventList evList = new EventList(eventsOfTheDay);

            if (evList.ShowDialog() == DialogResult.OK)
            {
                var evDescForm = new EventDescription(evList.ReturnedEvent, eventImporter.Calendars.Where(x => x.Summary == evList.ReturnedEvent.Calendar).ToArray());

                if (evDescForm.ShowDialog() == DialogResult.OK)
                {
                    eventImporter.EditEvent(evDescForm.ReturnedEvent, evList.ReturnedEvent.Id, evList.ReturnedEvent.CalendarId);
                    RefreshEvents();
                }
            }
        }


        private void AddEvent(SelectedDatesCollection selected)
        {
            var evDescForm = new EventDescription(
                selected[0],
                selected[selected.Count - 1], eventImporter.Calendars.ToArray());

            if (evDescForm.ShowDialog() == DialogResult.OK)
            {
                eventImporter.AddEvent(evDescForm.ReturnedEvent);
                RefreshEvents();
            }
        }

    }
}
