namespace Bria_API_CSharp_SampleApp
{
   partial class CallHistoryView
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
         this.Close_Button = new System.Windows.Forms.Button();
         this.CallHistory_ListView = new System.Windows.Forms.ListView();
         this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.Number = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.DisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.Duration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.SuspendLayout();
         // 
         // Close_Button
         // 
         this.Close_Button.Location = new System.Drawing.Point(515, 579);
         this.Close_Button.Name = "Close_Button";
         this.Close_Button.Size = new System.Drawing.Size(75, 23);
         this.Close_Button.TabIndex = 1;
         this.Close_Button.Text = "Close";
         this.Close_Button.UseVisualStyleBackColor = true;
         this.Close_Button.Click += new System.EventHandler(this.Close_Button_Click);
         // 
         // CallHistory_ListView
         // 
         this.CallHistory_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Type,
            this.Number,
            this.DisplayName,
            this.Duration,
            this.Time});
         this.CallHistory_ListView.GridLines = true;
         this.CallHistory_ListView.Location = new System.Drawing.Point(25, 27);
         this.CallHistory_ListView.MultiSelect = false;
         this.CallHistory_ListView.Name = "CallHistory_ListView";
         this.CallHistory_ListView.Size = new System.Drawing.Size(565, 546);
         this.CallHistory_ListView.TabIndex = 2;
         this.CallHistory_ListView.UseCompatibleStateImageBehavior = false;
         this.CallHistory_ListView.View = System.Windows.Forms.View.Details;
         // 
         // Type
         // 
         this.Type.Text = "Type";
         this.Type.Width = 83;
         // 
         // Number
         // 
         this.Number.Text = "Number";
         this.Number.Width = 105;
         // 
         // DisplayName
         // 
         this.DisplayName.Text = "Display Name";
         this.DisplayName.Width = 168;
         // 
         // Duration
         // 
         this.Duration.Text = "Duration";
         // 
         // Time
         // 
         this.Time.Text = "Time";
         this.Time.Width = 145;
         // 
         // CallHistoryView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(618, 614);
         this.Controls.Add(this.CallHistory_ListView);
         this.Controls.Add(this.Close_Button);
         this.Name = "CallHistoryView";
         this.Text = "CallHistoryView";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button Close_Button;
      private System.Windows.Forms.ListView CallHistory_ListView;
      private System.Windows.Forms.ColumnHeader Type;
      private System.Windows.Forms.ColumnHeader Number;
      private System.Windows.Forms.ColumnHeader DisplayName;
      private System.Windows.Forms.ColumnHeader Duration;
      private System.Windows.Forms.ColumnHeader Time;
   }
}