using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DMPSMassDeploymentTool
{
    public class TelnetVTZ
    {
        TentacleSoftware.Telnet.TelnetClient client;
        bool done = false;
        bool failed = false;
        DateTime minuteTimeout = DateTime.Now.AddMinutes(1);
        public event EventHandler<string> OnLog;

        public bool SendVTZFile(string ipAddress)
        {
            if (OnLog != null) OnLog(this, "Telnet connecting to : <" + ipAddress + ">");
            client = new TentacleSoftware.Telnet.TelnetClient(ipAddress, 23, TimeSpan.FromSeconds(1), System.Threading.CancellationToken.None);
            client.MessageReceived += HandleMessageReceived;
            client.ConnectionClosed += HandleConnectionClosed;
            client.Connect();

           
            DateTime Timeout = DateTime.Now.AddMinutes(10);

            while (!done && !failed && Timeout > DateTime.Now)
            {
                if (minuteTimeout < DateTime.Now)
                {
                    client.Send("\n");
                    minuteTimeout = DateTime.Now.AddMinutes(1);
                }
                System.Threading.Thread.Sleep(500);
            }

            if (Timeout < DateTime.Now)
            {
                if (OnLog != null) OnLog(this, "Telnet Timeout.  Failing");
            }

            if (done) return true;

            return false;
        }


        private void HandleMessageReceived(object sender, string message)
        {
            minuteTimeout = DateTime.Now.AddMinutes(1);

            if (OnLog != null) OnLog(this, "Telnet received: <" + message + ">");

            if (message.Contains("Console"))
            {
                client.Send("\n");
            }
            else if (message.StartsWith("TSW") && message.EndsWith(">"))
            {
                client.Send("ProjectLoad\n");
            }
            else if (message == "Installing Project, Please wait...")
            {
                //started = true;
            }
            else if (message == "Success. Restarting UI...")
            {
                done = true;
                client.Disconnect();                
            }
            else if (message == "ERROR: Error installing project, Attempting to restart previous project...")
            {
                failed = true;
                client.Disconnect();                
            }
        }

        private void HandleConnectionClosed(object sender, EventArgs e)
        {
            if (OnLog != null) OnLog(this, "Telnet connection closed");

            if (!failed && !done)
                failed = true;
        }
    }
}