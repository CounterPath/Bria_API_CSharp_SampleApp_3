// (C) Copyright 2014 - CounterPath Corporation. All rights reserved.
//
// THIS SOURCE CODE IS PROVIDED AS A SAMPLE WITH THE SOLE PURPOSE OF DEMONSTRATING A POSSIBLE
// USE OF A COUNTERPATH API. IT IS NOT INTENDED AS A USABLE PRODUCT OR APPLICATION FOR ANY 
// PARTICULAR PURPOSE OR TASK, WHETHER IT BE FOR COMMERCIAL OR PERSONAL USE.
//
// COUNTERPATH DOES NOT REPRESENT OR WARRANT THAT ANY COUNTERPATH APIs OR SAMPLE CODE ARE FREE
// OF INACCURACIES, ERRORS, BUGS, OR INTERRUPTIONS, OR ARE RELIABLE, ACCURATE, COMPLETE, OR 
// OTHERWISE VALID.
//
// THE COUNTERPATH APIs AND ASSOCIATED SAMPLE APPLICATIONS ARE PROVIDED "AS IS" WITH NO WARRANTY, 
// EXPRESS OR IMPLIED, OF ANY KIND AND COUNTERPATH EXPRESSLY DISCLAIMS ANY AND ALL WARRANTIES AND 
// CONDITIONS, INCLUDING, BUT NOT LIMITED TO, ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR 
// A PARTICULAR PURPOSE, AVAILABLILTIY, SECURITY, TITLE AND/OR NON-INFRINGEMENT.  
//
// YOUR USE OF COUNTERPATH APIS AND SAMPLE CODE IS AT YOUR OWN DISCRETION AND RISK, AND YOU WILL 
// BE SOLELY RESPONSIBLE FOR ANY DAMAGE THAT RESULTS FROM THE USE OF ANY COUNTERPATH APIs OR
// SAMPLE CODE INCLUDING, BUT NOT LIMITED TO, ANY DAMAGE TO YOUR COMPUTER SYSTEM OR LOSS OF DATA. 
//
// COUNTERPATH DOES NOT PROVIDE ANY SUPPORT FOR THE SAMPLE APPLICATIONS.
//
// TO VIEW THE OFFICIAL VERSION OF THE TERMS OF USE FOR COUNTERPATH APIs, PLEASE 
// READ IT ON THE WEB_SITE AT: http://www.counterpath.com/bria-desktop-api/

using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Bria_API_CSharp_SampleApp
{
   public partial class CallHistoryView : Form
   {
      public CallHistoryView(List<BriaAPI.CallHistoryItem> callHistoryList)
      {
         InitializeComponent();

         foreach (BriaAPI.CallHistoryItem historyItem in callHistoryList)
         {
            TimeSpan duration = TimeSpan.FromSeconds(historyItem.Duration);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(FromUnixTime(historyItem.TimeInitiated), TimeZoneInfo.Local);

            ListViewItem item = new ListViewItem(new[] { 
               historyItem.Type.ToString(), 
               historyItem.Number, 
               historyItem.DisplayName, 
               duration.ToString(@"h\:mm\:ss"), 
               localTime.ToString() });
            this.CallHistory_ListView.Items.Add(item);
         }
      }

      private DateTime FromUnixTime(long unixTime)
      {
         var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         return epoch.AddSeconds(unixTime);
      }

      private void Close_Button_Click(object sender, EventArgs e)
      {
         this.Close();
      }
   }
}
