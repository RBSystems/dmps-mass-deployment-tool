using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace DMPSMassDeploymentTool
{
    public partial class RunningForm : Form
    {
        public RunningForm()
        {
            InitializeComponent();
            logFileName = ConfigurationManager.AppSettings["LogFileName"];
            string dirName = System.IO.Path.GetDirectoryName(logFileName);

            if (!System.IO.Directory.Exists(dirName) && !string.IsNullOrEmpty(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }
        }

        string logFileName = "";
        string currentHeader = "";
        List<string> inMemory = new List<string>();
        DateTime lastSave = DateTime.Now;

        public void Log(string message)
        {
            MethodInvoker del = delegate ()
            {                
                if (message.StartsWith("---"))
                {
                    currentStatus.Text = message;
                    currentHeader = message;
                }
                else
                {
                    currentStatus.Text = currentHeader + "\n" + message;
                }

                inMemory.Add(message);
                logListBox.Items.Insert(0, message);
                logListBox.Update();

                if (DateTime.Now.Subtract(lastSave).TotalSeconds >= 5)
                    System.IO.File.WriteAllLines(@"C:\temp\DMPSDeployment.log", inMemory);
            };

            if (this.InvokeRequired)
                this.Invoke(del);
            else
                del();            
        }

        public DeployDMPS DeployDMPS { get; internal set; }

        private void step1ButtonConnect_Click(object sender, EventArgs e)
        {
            DeployDMPS.Connect();
        }

        private void step2ButtonRetrieveZIG_Click(object sender, EventArgs e)
        {
            DeployDMPS.RetrieveZigFile();
        }

        private void step3ButtonParseSIG_Click(object sender, EventArgs e)
        {
            DeployDMPS.UnzipAndParseSigFile();
        }

        private void step4ButtonGetSignals_Click(object sender, EventArgs e)
        {
            DeployDMPS.GetCurrentSignals();
        }

        private void step5ButtonPushSPZ_Click(object sender, EventArgs e)
        {
            DeployDMPS.SendSPZFile();
        }

        private void step6ButtonPushZig_Click(object sender, EventArgs e)
        {
            DeployDMPS.SendNewZIGFile();
        }

        private void step7ButtonGetSignals_Click(object sender, EventArgs e)
        {
            DeployDMPS.GetCurrentSignals2();
        }

        private void step8ButtonSetSignals_Click(object sender, EventArgs e)
        {
            DeployDMPS.SetSignals();
        }

        private void step9ButtonGetDMPSDevices_Click(object sender, EventArgs e)
        {
            DeployDMPS.GetDevicesFromDMPS();
        }

        private void step10ButtonGetTSPAddresses_Click(object sender, EventArgs e)
        {
            DeployDMPS.GetTSPIPAddresses();
        }

        private void step11ButtonPushVTZFiles_Click(object sender, EventArgs e)
        {
            DeployDMPS.SendNewVTZFiles();
        }

        private void stopProcessingButton_Click(object sender, EventArgs e)
        {
            DeployDMPS.StopProcessing = true;
        }

        private void step65WaitForSystemToLoadButton_Click(object sender, EventArgs e)
        {
            DeployDMPS.WaitForSystemToLoad();
        }

        private void step85SaveAndRebootButton_Click(object sender, EventArgs e)
        {
            DeployDMPS.SaveAndReboot();
        }
    }
}
