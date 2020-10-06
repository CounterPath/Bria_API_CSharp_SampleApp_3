using System;
using System.Windows.Forms;

namespace Bria_API_CSharp_SampleApp
{
    static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new BriaPhoneRemoteControl());
      }
   }
}
