using Google.Apis.Calendar.v3.Data;
using ICalendar.Model;
using System;
using System.Windows.Forms;

namespace ICalendar
{
    public partial class EventDescription : Form
    {
        private EventDay tempEvent;
        public EventDay ReturnedEvent => tempEvent;

        public EventDescription(DateTime startDate, DateTime endDate, CalendarListEntry[] calendars )
        {
            InitializeComponent();

            comboCalendars.Items.AddRange(calendars);
            comboCalendars.DisplayMember = "Summary";
            comboCalendars.ValueMember = "Id";
            if (calendars.Length > 0)
                comboCalendars.SelectedIndex = 0;

            tempEvent = new EventDay()
            {
                Description = string.Empty,
                StartDate = startDate,
                EndDate = endDate,
            };

            objToGui(tempEvent);
        }

        public EventDescription(EventDay @event, CalendarListEntry[] calendars)
        {
            InitializeComponent();

            comboCalendars.Items.AddRange(calendars);
            comboCalendars.DisplayMember = "Summary";
            comboCalendars.ValueMember = "Id";
            if (calendars.Length > 0)
                comboCalendars.SelectedIndex = 0;

            objToGui(@event);
        }

        private void guiToObj()
        {
            tempEvent = new EventDay();
            tempEvent.Description = textTitle.Text;
            tempEvent.StartDate = dateTimeStart.Value;
            tempEvent.EndDate = dateTimeEnd.Value.AddMinutes(1);
            tempEvent.CalendarId = (comboCalendars.SelectedItem as CalendarListEntry).Id;
        }
        private void objToGui(EventDay @event)
        {
            textTitle.Text = @event.Description;
            dateTimeStart.Value = @event.StartDate;
            dateTimeEnd.Value = @event.EndDate;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            guiToObj();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
