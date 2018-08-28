using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref Sm.Windows.Controls.StartupBase.SysObj);
            System.Threading.Thread.CurrentThread.CurrentCulture = Sm.Windows.Controls.StartupBase.SysObj.SysCultureInfo;
            if (Sm.Windows.Controls.StartupBase.SysObj == null)
            {
                //System.Windows.MessageBox.Show("Can not connect to server. Please login again", "Connection Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
       Hashtable h=     Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>("{\"Name1\":{\"key1\":\"value1\",\"key2\":\"Value2\"}}");
       Hashtable h2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(h["Name1"].ToString());
            HddtLib.SoftDreams.PublishHandler publisher = new HddtLib.SoftDreams.PublishHandler();
            publisher.ShowConfig();
            while(true)
            {
                publisher.UpdateDmKh("KH0078          ");
                if (Console.ReadLine() == "Q")
                    break;
            }
           
          
        }
    }
}
