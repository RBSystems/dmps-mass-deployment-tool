using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DMPSMassDeploymentTool
{
    public class TelnetGetDMPSSignals
    {
        TentacleSoftware.Telnet.TelnetClient client;
        bool done = false;
        bool failed = false;
        DateTime completeTimeout = DateTime.Now.AddSeconds(10);
        public event EventHandler<string> OnLog;

        public event EventHandler<List<CrestronSignal>> OnComplete;
        List<CrestronSignal> list = new List<CrestronSignal>();

        System.Timers.Timer t = new System.Timers.Timer(1000);
        
        public bool StartGetDMPSSignals(string ipAddress)
        {
            if (OnLog != null) OnLog(this, "Telnet connecting to : <" + ipAddress + ">");
            client = new TentacleSoftware.Telnet.TelnetClient(ipAddress, 23, TimeSpan.FromSeconds(1), System.Threading.CancellationToken.None);
            client.MessageReceived += HandleMessageReceived;
            client.ConnectionClosed += HandleConnectionClosed;
            client.Connect();

            System.Threading.Thread.Sleep(500);
            client.Send("\n");

            completeTimeout = DateTime.Now.AddSeconds(10);
            t.Elapsed += T_Elapsed;
            t.Start();
            
            return true;               
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {           
            if (completeTimeout < DateTime.Now)
            {
                t.Stop();
                done = true;
                client.Disconnect();
                OnComplete?.Invoke(this, list);                
            }
        }
        int rcvCount = 0;
        private void HandleMessageReceived(object sender, string message)
        {
            //OnLog?.Invoke(this, "Telnet received: <" + message + ">");

            rcvCount++;

            if (rcvCount % 50 == 0)
                OnLog?.Invoke(this, "Received " + rcvCount + " messages");

            if (message.Contains("Console"))
            {
                client.Send("\n");
            }
            else if (message.StartsWith("DMPS") && message.EndsWith(">") && list.Count == 0)
            {
                client.Send("DBGSIGNAL ALL SYNC");
            }    
            else if (message.Contains("=") && message.IndexOf("=") == 8)
            {
                completeTimeout = DateTime.Now.AddSeconds(10);

                string signalNumber = message.Substring(0, 8);
                uint signalInt = uint.Parse(signalNumber, System.Globalization.NumberStyles.HexNumber);

                string signalValue = message.Substring(9);

                if (signalValue.StartsWith("["))
                {
                    //parse ascii                    
                    string[] split = signalValue.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    signalValue = "";
                    foreach (var x in split)
                        signalValue += ((char)(int.Parse(x, System.Globalization.NumberStyles.HexNumber)));
                }

                list.Add(new CrestronSignal() { HasSignalValue = true, SignalIndex = signalInt, SignalValue = signalValue });
            }
        }

        private void HandleConnectionClosed(object sender, EventArgs e)
        {
            if (OnLog != null) OnLog(this, "Telnet connection closed");

            if (!failed && !done)
            {
                failed = true;
                OnComplete?.Invoke(this, null);
            }
        }
    }
}