namespace ICalendar
{
  partial class EventList
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
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.eventListBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(51, 39);
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
      this.cancelButton.Location = new System.Drawing.Point(215, 39);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(113, 32);
      this.cancelButton.TabIndex = 9;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // eventListBox
      // 
      this.eventListBox.FormattingEnabled = true;
      this.eventListBox.Location = new System.Drawing.Point(28, 107);
      this.eventListBox.Name = "eventListBox";
      this.eventListBox.Size = new System.Drawing.Size(329, 24);
      this.eventListBox.TabIndex = 10;
      // 
      // EventList
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(376, 164);
      this.ControlBox = false;
      this.Controls.Add(this.eventListBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Name = "EventList";
      this.Text = "Event ";
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.ComboBox eventListBox;
  }
}