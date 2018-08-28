using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            System.Threading.Thread.CurrentThread.CurrentCulture = StartUp.SysObj.SysCultureInfo;
            if (StartUp.SysObj == null)
            {
                System.Windows.MessageBox.Show("Can not connect to server. Please login again", "Connection Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }

            StartUp oStartup = new StartUp();
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "04.01.05";
            StartUp.Editing_Stt_Rec = e.Args.Count() > 1 ? e.Args[1].ToString() : string.Empty;
            oStartup.Run();
        }
    }
}
