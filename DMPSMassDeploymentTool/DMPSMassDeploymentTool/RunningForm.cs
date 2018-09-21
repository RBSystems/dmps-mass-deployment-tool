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
        public void DeployDMPS_OnStepChanged(DeployDMPS.DeploymentStep step)
        {
            step1ButtonConnect.Font = new Font(step1ButtonConnect.Font, FontStyle.Regular);
            step2ButtonRetrieveZIG.Font = new Font(step2ButtonRetrieveZIG.Font, FontStyle.Regular);
            step3ButtonParseSIG.Font = new Font(step3ButtonParseSIG.Font, FontStyle.Regular);
            step4ButtonGetSignals.Font = new Font(step4ButtonGetSignals.Font, FontStyle.Regular);
            step5ButtonPushSPZ.Font = new Font(step5ButtonPushSPZ.Font, FontStyle.Regular);
            step6ButtonPushZig.Font = new Font(step6ButtonPushZig.Font, FontStyle.Regular);
            step65WaitForSystemToLoadButton.Font = new Font(step65WaitForSystemToLoadButton.Font, FontStyle.Regular);
            step7ButtonGetSignals.Font = new Font(step7ButtonGetSignals.Font, FontStyle.Regular);
            step8ButtonSetSignals.Font = new Font(step8ButtonSetSignals.Font, FontStyle.Regular);
            step85SaveAndRebootButton.Font = new Font(step85SaveAndRebootButton.Font, FontStyle.Regular);
            step86WaitForSystemToLoad.Font = new Font(step86WaitForSystemToLoad.Font, FontStyle.Regular);
            step87GetSignalsAfterSet.Font = new Font(step87GetSignalsAfterSet.Font, FontStyle.Regular);
            step9ButtonGetDMPSDevices.Font = new Font(step9ButtonGetDMPSDevices.Font, FontStyle.Regular);
            step10ButtonGetTSPAddresses.Font = new Font(step10ButtonGetTSPAddresses.Font, FontStyle.Regular);
            step11ButtonPushVTZFiles.Font = new Font(step11ButtonPushVTZFiles.Font, FontStyle.Regular);
            
            if (step == DeployDMPS.DeploymentStep.Connect)
                step1ButtonConnect.Font = new Font(step1ButtonConnect.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.RetrieveZIG)
                step2ButtonRetrieveZIG.Font = new Font(step2ButtonRetrieveZIG.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.UnzipAndParseSIG)
                step3ButtonParseSIG.Font = new Font(step3ButtonParseSIG.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.GetCurrentSignals)
                step4ButtonGetSignals.Font = new Font(step4ButtonGetSignals.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.PushNewSPZFile)
                step5ButtonPushSPZ.Font = new Font(step5ButtonPushSPZ.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.PushNewZigFile)
                step6ButtonPushZig.Font = new Font(step6ButtonPushZig.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.WaitForSystemToLoad)
                step65WaitForSystemToLoadButton.Font = new Font(step65WaitForSystemToLoadButton.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.ParseNewSigAndGetCurrentSignals2)
                step7ButtonGetSignals.Font = new Font(step7ButtonGetSignals.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.SetSignals)
                step8ButtonSetSignals.Font = new Font(step8ButtonSetSignals.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.SaveAndReboot)
                step85SaveAndRebootButton.Font = new Font(step85SaveAndRebootButton.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.WaitForSystemToLoad2)
                step86WaitForSystemToLoad.Font = new Font(step86WaitForSystemToLoad.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.GetSignalsAfterSet)
                step87GetSignalsAfterSet.Font = new Font(step87GetSignalsAfterSet.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.GetDMPSDevices)
                step9ButtonGetDMPSDevices.Font = new Font(step9ButtonGetDMPSDevices.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.GetUITouchPanelAddresses)
                step10ButtonGetTSPAddresses.Font = new Font(step10ButtonGetTSPAddresses.Font, FontStyle.Bold);
            else if (step == DeployDMPS.DeploymentStep.PushVtzFiles)
                step11ButtonPushVTZFiles.Font = new Font(step11ButtonPushVTZFiles.Font, FontStyle.Bold);
        }


        string logFileName = "";
        string currentHeader = "";
        List<string> inMemory = new List<string>();
        DateTime lastSave = DateTime.Now;

        public void CloseForm()
        {
            MethodInvoker del = delegate ()
            {
                this.Close();
            };

            if (this.InvokeRequired)
                this.Invoke(del);
            else
                del();
        }

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
                    System.IO.File.WriteAllLines(DeployDMPS.TempDirectory + logFileName, inMemory);
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

        private void nudgeButton_Click(object sender, EventArgs e)
        {
            DeployDMPS.Nudge();
        }

        private void step86WaitForSystemToLoad_Click(object sender, EventArgs e)
        {
            DeployDMPS.WaitForSystemToLoad2();
        }

        private void step87GetSignalsAfterSet_Click(object sender, EventArgs e)
        {
            DeployDMPS.GetSignalsAfterSet();
        }
    }
}
