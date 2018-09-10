using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMPSDebugTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            session.OnSessionReady += Session_OnSessionReady;
            session.OnEvent += Session_OnEvent;
            session.OnActivateStrComplete += Session_OnActivateStrComplete;
        }

        private void Session_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log("Activate Str Complete - Success:[" + bSuccess + "], outputs: [" + pszOutputs + "]");
        }

        private void Session_OnEvent(int nTransactionID, [System.Runtime.InteropServices.ComAliasName("VPTCOMSERVERLib.EVptEventType")] VPTCOMSERVERLib.EVptEventType nEventType, int lParam1, int lParam2, string pszwParam)
        {
            Log("EVENT - " + nEventType.ToString() + " - " + lParam1 + ", " + lParam2 + ", [" + pszwParam + "]");
        }

        private void Log(string m)
        {
            MethodInvoker del = delegate ()
            {
                logBox.Text += m + "\r\n";
            };

            if (this.InvokeRequired)
                this.Invoke(del);
            else
                logBox.Text += m + "\r\n";


        }

        private void Session_OnSessionReady()
        {
            Log("Session ready");
        }

        VPTCOMSERVERLib.VptSessionClass session = new VPTCOMSERVERLib.VptSessionClass();

        private void button1_Click(object sender, EventArgs e)
        {
            Log("Connecting to " + connectToSession.Text);
            session.OpenSession("auto " + connectToSession.Text, connectToSession.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string output = "";
            int txnID = 0;
            session.SyncActivateStr(0, "SignalDbgEnterDbgMode", output, 10000, 0);
            session.AsyncActivateStr(0, "SignalDbgStatus " + getSignalNumber.Text, 10000, ref txnID, 0, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string output = "";
            int txnID = 0;
            session.SyncActivateStr(0, "SignalDbgEnterDbgMode", output, 10000, 0);
            session.AsyncActivateStr(0, "SignalDbgSetDigital " + setSignalNumber.Text + ", " + setSignalValue.Text, 10000, ref txnID, 0, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string output = "";
            int txnID = 0;
            session.SyncActivateStr(0, "SignalDbgEnterDbgMode", output, 10000, 0);
            session.AsyncActivateStr(0, "SignalDbgSetAnalog " + setSignalNumber.Text + ", " + setSignalValue.Text, 10000, ref txnID, 0, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string output = "";
            int txnID = 0;
            session.SyncActivateStr(0, "SignalDbgEnterDbgMode", output, 10000, 0);
            session.AsyncActivateStr(0, "SignalDbgSetSerial " + setSignalNumber.Text + ", \"" + setSignalValue.Text + "\"", 10000, ref txnID, 0, 0);
        }
    }
}
