using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMPSMassDeploymentTool
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TelnetGetDMPSSignals sig = new TelnetGetDMPSSignals();
            sig.OnLog += Sig_OnLog;
            sig.OnComplete += Sig_OnComplete;
            
            sig.StartGetDMPSSignals("10.6.36.220");
        }

        private void Sig_OnComplete(object sender, List<DeployDMPS.CrestronSignal> e)
        {
            string s = "";
            foreach (var x in e)
                s += x.SignalIndex + " = " + x.SignalValue + "\r\n";

            textBox1.Invoke((Action)(() => textBox1.Text += s));

            MessageBox.Show("Done!");
        }

        private void Sig_OnLog(object sender, string e)
        {
            textBox1.Invoke((Action)(() => textBox1.Text += e + "\r\n"));
        }
    }
}
