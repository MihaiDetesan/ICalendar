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

            objToGui();
        }

        private void guiToObj()
        {
            tempEvent.Description = textTitle.Text;
            tempEvent.StartDate = dateTimeStart.Value;
            tempEvent.EndDate = dateTimeEnd.Value;
            tempEvent.CalendarId = (comboCalendars.SelectedItem as CalendarListEntry).Id;
        }
        private void objToGui()
        {
            textTitle.Text = tempEvent.Description;
            dateTimeStart.Value = tempEvent.StartDate;
            dateTimeEnd.Value = tempEvent.EndDate;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            guiToObj();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
