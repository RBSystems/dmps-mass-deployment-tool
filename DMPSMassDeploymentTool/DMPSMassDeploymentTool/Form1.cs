using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace DMPSMassDeploymentTool
{
    public partial class Form1 : Form
    {
        VPTCOMSERVERLib.VptSessionClass vptSessionClass = new VPTCOMSERVERLib.VptSessionClass();
        
        public Form1()
        {
            InitializeComponent();

            vptSessionClass.OnDebugString += VptSessionClass_OnDebugString;            
            vptSessionClass.OnEvent += VptSessionClass_OnEvent;            
            vptSessionClass.OnActivateStrComplete += VptSessionClass_OnActivateStrComplete;
            vptSessionClass.OnSessionReady += VptSessionClass_OnSessionReady;
            vptSessionClass.OnFileXferFileStart += VptSessionClass_OnFileXferFileStart;
            vptSessionClass.OnFileXferFileProgress += VptSessionClass_OnFileXferFileProgress;
            vptSessionClass.OnFileXferFileFinish += VptSessionClass_OnFileXferFileFinish;
            vptSessionClass.OnTaskStart += VptSessionClass_OnTaskStart;
            vptSessionClass.OnTaskProgress += VptSessionClass_OnTaskProgress;
            vptSessionClass.OnTaskComplete += VptSessionClass_OnTaskComplete;
        }
        

        private void VptSessionClass_OnDebugString(int nTransactionID, int nCategory, string pszwData)
        {
            Log("Task Complete - " + nTransactionID + " - " + nCategory + " - " + pszwData);
        }

        private void VptSessionClass_OnTaskComplete(int nTransactionID, byte bSuccess)
        {
            Log("Task Complete - " + nTransactionID + " - " + bSuccess);
        }

        private void VptSessionClass_OnTaskProgress(int nTransactionID, int nPercentageDone, string pszwProgressDescription, int lParam)
        {
            Log("Task Progress - " + nTransactionID + " - " + nPercentageDone + " - " + pszwProgressDescription + " - " + lParam);
        }

        private void VptSessionClass_OnTaskStart(int nTransactionID, string pszwTaskName, string pszwTaskDescription)
        {
            Log("Task Start - " + nTransactionID + " - " + pszwTaskName + " - " + pszwTaskDescription);
        }

        private void browseSPZfileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.FileName = spzFileLocation.Text;
            od.Filter = "Crestron spz file|*.spz";
            if (od.ShowDialog() == DialogResult.OK)
            {
                spzFileLocation.Text = od.FileName;
            }
        }

        private void browseVTZFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.FileName = vtzFileLocation.Text;
            od.Filter = "Crestron vtz file|*.vtz";
            if (od.ShowDialog() == DialogResult.OK)
            {
                vtzFileLocation.Text = od.FileName;
            }
        }

        private void loadListFromELKButton_Click(object sender, EventArgs e)
        {
            string elkURL = ConfigurationManager.AppSettings["ELKURL"];
            string elkSystemsQueryFileName = ConfigurationManager.AppSettings["ELKSystemsQueryFileName"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(elkURL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(System.IO.File.ReadAllText(elkSystemsQueryFileName));
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            var jObject = JObject.Parse(result);
            var deviceList = jObject.SelectTokens("$..device");

            innerDMPSList.Clear();
            foreach (var device in deviceList)
            {
                string h = device["hostname"].Value<string>();
                string ip = device["ipAddress"].Value<string>();

                if (!string.IsNullOrEmpty(h) || !string.IsNullOrEmpty(ip))
                {
                    innerDMPSList.Add(new DMPSAddress()
                    {
                        hostName = h,
                        ipAddress = ip
                    });
                }
            }

            FilterDMPSList();
        }

        private void FilterDMPSList()
        {
            var queryResults =
                innerDMPSList.FindAll(one => 
                    one.hostName.ToLower().Contains(filterText.Text.ToLower()) || 
                    one.ipAddress.ToLower().Contains(filterText.Text.ToLower()));

            dmpsList.Items.Clear();
            queryResults.ForEach(one => dmpsList.Items.Add(one));
        }

        List<DMPSAddress> innerDMPSList = new List<DMPSAddress>();

        private class DMPSAddress
        {
            public string hostName { get; set; }
            public string ipAddress { get; set; }
            public override string ToString()
            {
                return hostName + " (" + ipAddress + ")";
            }
        }

        private void applyFilterButton_Click(object sender, EventArgs e)
        {
            FilterDMPSList();
        }

        private void addDMPSUnit_Click(object sender, EventArgs e)
        {
            var nDMPS = new DMPSAddress() { hostName = addHostNameBox.Text, ipAddress = addIPAddressBox.Text };
            innerDMPSList.Add(nDMPS);
            dmpsList.Items.Insert(0, nDMPS);
            dmpsList.SetItemChecked(0, true);
        }

        List<DMPSAddress> ToUpdateList;
        int CurUpdateIndex = 0;

        private void deployToDMPSButton_Click(object sender, EventArgs e)
        {
            string confirm =
                $"Are you sure you want to deploy?  This will premamently affect the code for {dmpsList.CheckedItems.Count} units.";

            if (MessageBox.Show(confirm, "Confirm?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (MessageBox.Show("Last Chance.....are you really, really sure???", "Really sure?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    ToUpdateList = new List<DMPSAddress>();
                    foreach (var checkedItem in dmpsList.CheckedItems)
                        ToUpdateList.Add(checkedItem as DMPSAddress);

                    CurUpdateIndex = -1;

                    UpdateNextDMPS();
                }
            }
        }

        public void UpdateNextDMPS()
        {
            CurUpdateIndex++;
            if (CurUpdateIndex == ToUpdateList.Count)
            {
                MessageBox.Show("All DMPS units complete!");
            }
            else
            {
                DeployDMPS deploy = new DeployDMPS(ToUpdateList[CurUpdateIndex].hostName, ToUpdateList[CurUpdateIndex].ipAddress, spzFileLocation.Text);
                deploy.OnDMPSDeployedSuccessfully += Deploy_OnDMPSDeployedSuccessfully;
                deploy.OnDMPSDeploymentFatalError += Deploy_OnDMPSDeploymentFatalError;
                deploy.StartDeployment();
            }
        }

        private void Deploy_OnDMPSDeploymentFatalError(string message)
        {
            MessageBox.Show("Error! - " + message);
        }

        private void Deploy_OnDMPSDeployedSuccessfully(string message)
        {
            UpdateNextDMPS();
        }

        RunningForm runnerForm;
        BackgroundWorker worker;

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            runnerForm.Log(e.UserState.ToString());
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //runnerForm.Close();
            MessageBox.Show("Deployment Completed!\n" + e.Result.ToString() + "\nSee output log for details.");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string error = "";

            
            try
            {
                
               

                string outputs = "";

               
                System.Threading.Thread.Sleep(3000);
                //vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref outputs, 5000, 0);                
                //vptSessionClass.SyncActivateStr(0, "SignalDbgStatus", ref outputs, 5000, 0);
                //vptSessionClass.SyncActivateStr(0, "FileSysDirList", ref outputs, 5000, 0);
                

                int a = 1;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                //vptSessionClass.CloseSession();
            }
            
            e.Result = error;
        }

        private void Log(string m)
        {
            if (runnerForm != null)
                runnerForm.Log(m);
        }

        private void VptSessionClass_OnSessionReady()
        {
            Log("Session Ready");
        }

        private void VptSessionClass_OnFileXferFileFinish(int nTransactionID, byte bSuccess)
        {
            Log("File Finish - " + bSuccess.ToString());
        }

        private void VptSessionClass_OnFileXferFileProgress(int nTransactionID, int nFileBytesTransferred, int nTotalBytesTransferred, int nBytesPerSecond, int nEstTotalTimeRemaining)
        {
            Log("File Progress - " + nFileBytesTransferred.ToString() + ", " + nTotalBytesTransferred + ", " + nBytesPerSecond + ", " + nEstTotalTimeRemaining);
        }

        private void VptSessionClass_OnFileXferFileStart(int nTransactionID, string pszwLocalFilename, string pszwRemoteFilename, int nSize, byte bSending, string pszwProtocolName)
        {
            Log("File start - " + pszwLocalFilename + " from " + pszwRemoteFilename + " " + nSize.ToString() + " " + pszwProtocolName);
        }

        private void VptSessionClass_OnEvent(int nTransactionID, [System.Runtime.InteropServices.ComAliasName("VPTCOMSERVERLib.EVptEventType")] VPTCOMSERVERLib.EVptEventType nEventType, int lParam1, int lParam2, string pszwParam)
        {
            if (nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)
            {
                var signal = signalList.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }                
            }
            Log(nEventType.ToString() + " - " + lParam1.ToString() + " = [" + pszwParam + "]");            
        }

        private void VptSessionClass_OnActivateStrComplete(
            int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log(nTransactionID.ToString() + " - " + nAbilityCode.ToString() + " - " + bSuccess + " - " + pszOutputs + " - " + nUserPassBack);
        }

        private void testConnect_Click(object sender, EventArgs e)
        {
            if (runnerForm is null)
                runnerForm = new RunningForm();

            runnerForm.Show();

            vptSessionClass.OpenSession("auto 10.6.36.220", "10.6.36.220");
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            string outputs = "";
            int transactionID = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileXferGet ""\SIMPL\TEC HD.zig"" ""c:\temp\TEC HD.zig""", 5000, ref transactionID, 0, 0);

            Log("txn id: " + transactionID.ToString() + " - " + outputs.ToString());

            int newtxnid = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileSysDirList ""\""", 5000, ref newtxnid, 0, 0);

            Log("txn id: " + newtxnid.ToString() + " - " + outputs.ToString());
        }

        private void testDisconnect_Click(object sender, EventArgs e)
        {
            vptSessionClass.CloseSession();
        }

        List<CrestronSignal> signalList = new List<CrestronSignal>();

        private void testDecodeSig_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(@"C:\temp\TEC HD\"))
                System.IO.Directory.Delete(@"C:\temp\TEC HD\", true);

            System.IO.Compression.ZipFile.ExtractToDirectory(@"C:\temp\TEC HD.zig", @"C:\temp\TEC HD\");

            byte[] sigFile = System.IO.File.ReadAllBytes(@"C:\temp\TEC HD\TEC HD.sig");

            signalList.Clear();

            string signalFileType = "";
            int index = 0;

            while (true)
            {
                signalFileType += (char)(sigFile[index]);
                index++;
                if (signalFileType.EndsWith("]")) break;
            }

            if (signalFileType != "[RLSIG0001]" && signalFileType != "[LOGOSSIG001.000]")
            {
                Log("Unknown sig file type");
            }

            Log("Signal file type: " + signalFileType);
            
            while (index < sigFile.Length)
            {
                var length = BitConverter.ToUInt16(sigFile, index) - 8;
                index = index + 2;
                string signalName = "";
                uint signalIndex = 0;
                byte signalType = 0;
                byte signalFlags = 0;

                if (signalFileType == "[RLSIG0001]")
                {
                    signalName = ASCIIEncoding.ASCII.GetString(sigFile, index, length);
                }
                else if (signalFileType == "[LOGOSSIG001.000]")
                {
                    signalName = Encoding.Unicode.GetString(sigFile, index, length);                        
                }

                index += length;

                if (!string.IsNullOrEmpty(signalName))
                {
                    signalIndex = BitConverter.ToUInt32(sigFile, index);
                    index += 4;
                }

                signalType = sigFile[index];
                index++;
                signalFlags = sigFile[index];
                index++;

                var x = new CrestronSignal() { SignalName = signalName, SignalIndex = signalIndex, SignalFlags = signalFlags, SignalType = signalType };
                Log(x.ToString());
                signalList.Add(x);
            }
        }

        public class CrestronSignal
        {
            public string SignalName { get; set; }
            public uint SignalIndex { get; set; }
            public byte SignalType { get; set; }
            public byte SignalFlags { get; set; }

            public override string ToString()
            {
                return SignalName + " - [Index: " + SignalIndex + "] [Type: " + SignalType.ToString() + "]";
            }

            public bool HasSignalValue { get; set; }
            public string SignalValue { get; set; }
        }

        private void testGetCurrentSignals_Click(object sender, EventArgs e)
        {
            string outputs = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref outputs, 5000, 0);
            vptSessionClass.SyncActivateStr(0, "SignalDbgStatus", ref outputs, 5000, 0);
        }

        private void testSendSPZFile_Click(object sender, EventArgs e)
        {
            string outputs = "";
            vptSessionClass.SyncActivateStr(0, @"ProgramSend ""C:\Users\Matt Blodgett\Box\AV Engineering\Production_Code\TEC\TEC_HD\PROGRAM\Previous_Versions\TEC HD 2016-05-11\TEC hd.spz""", ref outputs, 5000, 0);
        }

        private void testSendVTZFile_Click(object sender, EventArgs e)
        {
            string outputs = "";

            vptSessionClass.CloseSession();

            vptSessionClass.OpenSession("auto 10.6.36.222", "10.6.36.222");

            vptSessionClass.SyncActivateStr(0, @"DisplayListSend ""C:\Users\Matt Blodgett\Box\AV Engineering\Production_Code\TEC\TEC_HD\PROGRAM\Previous_Versions\TEC HD 2016-05-11\TEC hd.vtz""", ref outputs, 5000, 0);
        }

        private void testDisconnectFromVTZ_Click(object sender, EventArgs e)
        {
            vptSessionClass.CloseSession();
        }

        private void testSEndSigFile_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(@"c:\temp\TEC HD.zig"))
                System.IO.File.Delete(@"c:\temp\TEC HD.zig");

            using (var zip = ZipFile.Open(@"c:\temp\TEC HD.zig", ZipArchiveMode.Create))
                zip.CreateEntryFromFile(@"C:\Users\Matt Blodgett\Box\AV Engineering\Production_Code\TEC\TEC_HD\PROGRAM\Previous_Versions\TEC HD 2016-05-11\TEC HD.sig", "TEC HD.sig");

            int transactionID = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileXferPut ""c:\temp\TEC HD.zig"" ""\SIMPL\TEC HD.zig"" ", 5000, ref transactionID, 0, 0);
        }

    }
}
