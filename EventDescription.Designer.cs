namespace ICalendar
{
  partial class EventDescription
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.textTitle = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.comboCalendars = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title";
            // 
            // dateTimeStart
            // 
            this.dateTimeStart.Location = new System.Drawing.Point(15, 104);
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.Size = new System.Drawing.Size(333, 22);
            this.dateTimeStart.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Start of event";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "End of event";
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Location = new System.Drawing.Point(16, 182);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(332, 22);
            this.dateTimeEnd.TabIndex = 5;
            // 
            // textTitle
            // 
            this.textTitle.Location = new System.Drawing.Point(16, 42);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(332, 22);
            this.textTitle.TabIndex = 7;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(27, 291);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(113, 32);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(191, 291);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(113, 32);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // comboCalendars
            // 
            this.comboCalendars.FormattingEnabled = true;
            this.comboCalendars.Location = new System.Drawing.Point(15, 240);
            this.comboCalendars.Name = "comboCalendars";
            this.comboCalendars.Size = new System.Drawing.Size(332, 24);
            this.comboCalendars.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 221);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Calendar";
            // 
            // EventDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(376, 354);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboCalendars);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textTitle);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimeStart);
            this.Controls.Add(this.label1);
            this.Name = "EventDescription";
            this.Text = "Event ";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.DateTimePicker dateTimeStart;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.DateTimePicker dateTimeEnd;
    private System.Windows.Forms.TextBox textTitle;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox comboCalendars;
        private System.Windows.Forms.Label label2;
    }
}