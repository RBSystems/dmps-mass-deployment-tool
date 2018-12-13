using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMPSMassDeploymentTool
{
    public partial class SingleDMPSManualDeploy : UserControl
    {
        public SingleDMPSManualDeploy()
        {
            InitializeComponent();
        }
        
        VPTCOMSERVERLib.VptSessionClass vptSessionClass { get; set; }

        private string dmpsHost = "";
        public string DMPSHost
        {
            get { return dmpsHost; }
            set
            {
                dmpsHost = value;
                dmpsHostLabel.Text = dmpsHost;
            }
        }

        private string dmpsIP = "";
        public string DMPSIPAddress
        {
            get { return dmpsIP; }
            set
            {
                dmpsIP = value;
                dmpsIPLabel.Text = dmpsIP;
            }
        }

        public string SPZFileLocation { get; set; }
        public string VTZFileLocation { get; set; }

        private void SingleDMPSManualDeploy_Load(object sender, EventArgs e)
        {
            //hide headers
            tabControl1.ItemSize = new Size(1, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            nextStepBox.SelectedIndex = 0;

            //create the COM API object
            vptSessionClass = new VPTCOMSERVERLib.VptSessionClass();

            //listen to events
            vptSessionClass.OnActivateComplete += VptSessionClass_OnActivateComplete;
            vptSessionClass.OnActivateStrComplete += VptSessionClass_OnActivateStrComplete; 
            vptSessionClass.OnAsyncActivateStart += VptSessionClass_OnAsyncActivateStart;
            vptSessionClass.OnDebugString += VptSessionClass_OnDebugString;
            vptSessionClass.OnEvent += VptSessionClass_OnEvent;
            vptSessionClass.OnFileXferBatchFinish += VptSessionClass_OnFileXferBatchFinish;
            vptSessionClass.OnFileXferBatchStart += VptSessionClass_OnFileXferBatchStart;
            vptSessionClass.OnFileXferFileStart += VptSessionClass_OnFileXferFileStart;
            vptSessionClass.OnFileXferFileProgress += VptSessionClass_OnFileXferFileProgress;
            vptSessionClass.OnFileXferFileFinish += VptSessionClass_OnFileXferFileFinish;
            vptSessionClass.OnSessionReady += VptSessionClass_OnSessionReady;
            vptSessionClass.OnTaskStart += VptSessionClass_OnTaskStart;
            vptSessionClass.OnTaskProgress += VptSessionClass_OnTaskProgress;
            vptSessionClass.OnTaskComplete += VptSessionClass_OnTaskComplete;
        }

        private void VptSessionClass_OnFileXferBatchStart(int nTransactionID, string pszwDescription, int nTotalFiles, int nTotalSize)
        {
            Log("API On Async Activate STart - nTransactionID:[" + nTransactionID + "], " +
               "pszwDescription:[" + pszwDescription + "], " +
               "nTotalFiles:[" + nTotalFiles + "], " +
               "nTotalSize:[" + nTotalSize + "]");
        }

        private void VptSessionClass_OnFileXferBatchFinish(int nTransactionID, byte bSuccess)
        {
            Log("API On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
               "bSuccess:[" + bSuccess + "]");
        }

        private void VptSessionClass_OnAsyncActivateStart(int nTransactionID, int nAbilityCode, int nUserPassBack)
        {
            Log("API On Async Activate STart - nTransactionID:[" + nTransactionID + "], " +
               "nAbilityCode:[" + nAbilityCode + "], " +
               "nUserPassBack:[" + nUserPassBack + "]");
        }

        private void VptSessionClass_OnActivateComplete(int nTransactionID, int nAbilityCode, byte bSuccess, ref Array psaOutputs, int nUserPassBack)
        {
            Log("API On Activate Complete - nTransactionID:[" + nTransactionID + "], " +
                "nAbilityCode:[" + nAbilityCode + "], " +
                "bSuccess:[" + bSuccess + "], psaOutputs:[" + string.Join("|", psaOutputs) + "], " +
                "nUserPassBack:[" + nUserPassBack + "]");
        }

        private void VptSessionClass_OnTaskComplete(int nTransactionID, byte bSuccess)
        {
            Log("API On Task Complete - nTransactionID:[" + nTransactionID + "], " +
                "bSuccess:[" + bSuccess + "]");
        }

        private void VptSessionClass_OnTaskProgress(int nTransactionID, int nPercentageDone, string pszwProgressDescription, int lParam)
        {
            Log("API On Task Progress - nTransactionID:[" + nTransactionID + "], " +
               "nPercentageDone:[" + nPercentageDone + "], " +
               "pszwProgressDescription:[" + pszwProgressDescription + "], " +
               "lParam:[" + lParam + "]");
        }

        private void VptSessionClass_OnTaskStart(int nTransactionID, string pszwTaskName, string pszwTaskDescription)
        {
            Log("API On Task Start - nTransactionID:[" + nTransactionID + "], " +
                "pszwTaskName:[" + pszwTaskName + "], " +
                "pszwTaskDescription:[" + pszwTaskDescription + "]");
        }

        private void VptSessionClass_OnFileXferFileFinish(int nTransactionID, byte bSuccess)
        {
            Log("API On File Xfer File Finish - nTransactionID:[" + nTransactionID + "], " +
                "bSuccess:[" + bSuccess + "]");
        }

        private void VptSessionClass_OnFileXferFileProgress(int nTransactionID, int nFileBytesTransferred, int nTotalBytesTransferred, int nBytesPerSecond, int nEstTotalTimeRemaining)
        {
            Log("API On File Xfer File Progress - nTransactionID:[" + nTransactionID + "], " +
                "nFileBytesTransferred:[" + nFileBytesTransferred + "], " +
                "nTotalBytesTransferred:[" + nTotalBytesTransferred + "], nBytesPerSecond:[" + nBytesPerSecond + "], " +
                "nEstTotalTimeRemaining:[" + nEstTotalTimeRemaining + "]");
        }

        private void VptSessionClass_OnFileXferFileStart(int nTransactionID, string pszwLocalFilename, string pszwRemoteFilename, int nSize, byte bSending, string pszwProtocolName)
        {
            Log("API On File Xfer File Start - nTransactionID:[" + nTransactionID + "], " +
                "pszwLocalFilename:[" + pszwLocalFilename + "], " +
                "pszwRemoteFilename:[" + pszwRemoteFilename + "], nSize:[" + nSize + "], " +
                "bSending:[" + bSending + "], pszwProtocolName :[" + pszwProtocolName + "]");
        }

        
        
        private void VptSessionClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            
        }

        private void VptSessionClass_OnEvent(int nTransactionID,
            [System.Runtime.InteropServices.ComAliasName("VPTCOMSERVERLib.EVptEventType")] VPTCOMSERVERLib.EVptEventType nEventType,
            int lParam1, int lParam2, string pszwParam)
        {
            
        }
        
        private void VptSessionClass_OnDebugString(int nTransactionID, int nCategory, string pszwData)
        {
            Log("API On Debug String - nTransactionID:[" + nTransactionID + "], nCategory:[" + nCategory + "], data:[" + pszwData + "]");
        }

        private void nextStepBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = nextStepBox.SelectedIndex;
            this.BackColor = SystemColors.Control;

            step1NextStepButton.Enabled = false;
            step2NextStepButton.Enabled = false;
            step3NextStepButton.Enabled = false;
            step4NextStepButton.Enabled = false;
            step5NextStepButton.Enabled = false;
            step6DoneButton.Enabled = false;

            errorLabel.Text = "";

            if (nextStepBox.SelectedIndex == 0)
            {
                //nothing to validate here
            }
            else if (nextStepBox.SelectedIndex == 1)
            {
                //set up the one to save it to
                currentSignalSaveLocationLabel.Text =
                    System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["DMPSDeploymentLocation"],
                    this.dmpsIP + "-" + this.dmpsHost,
                    "CurrentSignals.json");

                //reset counter to zero
                numberOfCurrentSignalsCollectedLabel.Text = "0";
            }
        }

        void Log(string m)
        {
            if (this.logBox.InvokeRequired)
            {
                this.logBox.Invoke((Action)(() => logBox.Items.Insert(0, (DateTime.Now.ToString("hh:mm:ss.fff") + " - " + m))));
            }
            else
            {
                logBox.Items.Insert(0, (DateTime.Now.ToString("hh:mm:ss.fff") + " - " + m));
            }

            string logPath = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["DMPSDeploymentLocation"],
                    this.dmpsIP + "-" + this.dmpsHost,
                    System.Configuration.ConfigurationManager.AppSettings["LogFileName"]);

            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logPath)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
            }

            System.IO.File.AppendAllLines(logPath, m.Split(new char[] { '\n' }));
        }

        void Go(Action a)
        {
            if (this.InvokeRequired)
                this.Invoke(a);
            else
                a();
        }


        #region Step 1

        bool connected = false;
        public void EnsureConnection()
        {
            if (vptSessionClass == null || vptSessionClass.IsSessionReady(1000) != 1)
            {
                if (vptSessionClass != null)
                    vptSessionClass.CloseSession();

                connected = false;

                int connect = vptSessionClass.OpenSession("auto " + DMPSIPAddress, DMPSIPAddress);
                string output = "";
                vptSessionClass.SyncActivateStr(0, "ProductGetInfo", output, 1000, 1);

                DateTime timeout = DateTime.Now.AddSeconds(10);
                while (!connected && timeout > DateTime.Now)
                {
                    System.Threading.Thread.Sleep(100);
                }
                
                if (!connected)
                {
                    errorLabel.Text = "Error connecting - try again";
                    this.BackColor = Color.MistyRose;
                }
            }
        }

        private void step1ConnectButton_Click(object sender, EventArgs e)
        {
            EnsureConnection();
        }

        private void VptSessionClass_OnSessionReady()
        {
            connected = true;
            Log("API On Session Ready");

            Go(() => {
                errorLabel.Text = "";
                step1NextStepButton.Enabled = true;
                this.BackColor = Color.PaleGreen;
            });
        }
        private void step1NextStepButton_Click(object sender, EventArgs e)
        {
            nextStepBox.SelectedIndex = 1;
        }


        #endregion

        #region Step 2
        private void step2CollectSignalsButton_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(currentSignalSaveLocationLabel.Text))
            {
                if (MessageBox.Show("There is already a current signal file.  This will overwrite that.  Continue?", "Overwite?", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }

            //first, we have to go get the ZIG file and parse it




            //then, we can go and get the signals from the DMPS
            TelnetGetDMPSSignals sigFinder = new TelnetGetDMPSSignals();
            sigFinder.OnLog += SigFinder_OnLog; ;
            sigFinder.OnComplete += SigFinder_OnComplete; ;
            sigFinder.StartGetDMPSSignals(this.DMPSIPAddress);
        }

        private void SigFinder_OnComplete(object sender, List<DeployDMPS.CrestronSignal> e)
        {
            if (e == null)
            {
                errorLabel.Text = "No signals returned";
                this.BackColor = Color.MistyRose;
            }
            else
            {
                e.Sort((a, b) => (a.SignalName ?? "").CompareTo(b.SignalName ?? ""));
                
                if (System.IO.File.Exists(currentSignalSaveLocationLabel.Text))
                    System.IO.File.Delete(currentSignalSaveLocationLabel.Text);

                var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(e.ToArray());

                System.IO.File.WriteAllText(currentSignalSaveLocationLabel.Text, signalListJson);

                Go(() =>
                {
                    if (e.Count < 2000)
                    {
                        errorLabel.Text = "Less than 2000 signals returned";
                        this.BackColor = Color.MistyRose;
                    }
                    else
                    {
                        errorLabel.Text = "";
                        this.BackColor = Color.PaleGreen;
                        step2NextStepButton.Enabled = true;
                    }
                });
            }
        }

        private void SigFinder_OnLog(object sender, string e)
        {
            Log(e);
        }
        
        private void step2NextStepButton_Click(object sender, EventArgs e)
        {
            nextStepBox.SelectedIndex = 2;
        }

        #endregion

    }
}
