using ICalendar.Model;
using System;
using System.Windows.Forms;

namespace ICalendar
{
    public partial class EventList : Form
    {
        public EventDay ReturnedEvent => (eventListBox.SelectedItem as EventDay); 

        public EventList(EventDay[] eventsfromDate)
        {
            InitializeComponent();
            eventListBox.Items.AddRange(eventsfromDate);
            eventListBox.DisplayMember = "Description";

            if (eventsfromDate.Length > 0)
            {
                eventListBox.SelectedIndex = 0;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
