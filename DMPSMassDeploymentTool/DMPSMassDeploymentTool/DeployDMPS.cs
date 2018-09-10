﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMPSMassDeploymentTool
{
    public class DeployDMPS : IDisposable
    {
        public delegate void MyEventHandler (string message);
        public delegate void StepChangedEventHandler(DeploymentStep step);
        public event MyEventHandler OnDMPSDeployedSuccessfully;
        public event MyEventHandler OnDMPSDeploymentFatalError;
        public event StepChangedEventHandler OnStepChanged;

        public string DMPSHostName;
        public string DMPSIPAddress;
        public string NewSPZFileName;
        public string NewVTZFileName;


        public DeployDMPS(string hostName, string ipAddress, string spzFilename, string vtzFilename)
        {
            this.DMPSHostName = hostName;
            this.DMPSIPAddress = ipAddress;
            this.NewSPZFileName = spzFilename;
            this.NewVTZFileName = vtzFilename;
        }

        public enum DeploymentStep
        {
            Connect,
            RetrieveZIG,
            UnzipAndParseSIG,
            GetCurrentSignals,
            PushNewSPZFile,
            WaitForSystemToLoad,
            PushNewZigFile,
            ParseNewSigAndGetCurrentSignals2,
            SetSignals,
            SaveAndReboot,
            GetDMPSDevices,
            GetUITouchPanelAddresses,
            PushVtzFiles
        }

        DeploymentStep curStep;
        RunningForm logForm;
        int curAPITransactionID;
        int retryNumber = 0;

        VPTCOMSERVERLib.VptSessionClass vptSessionClass { get; set; }

        public bool StopProcessing { get; set; }

        public void Dispose()
        {
            if (vptSessionClass != null)
                vptSessionClass.CloseSession();
        }

        void Log(string m)
        {            
            logForm.Log(DateTime.Now.ToString("hh:mm:ss.fff") + " - " + m);
        }

        public string TempDirectory
        {
            get
            {
                var x = @"C:\Temp\DMPSDeploy\" + DMPSIPAddress + @"\";
                if (!System.IO.Directory.Exists(x))
                    System.IO.Directory.CreateDirectory(x);

                return x;
            }
        }

        public void StartDeployment(Form1 masterForm)
        {
            //create the running form and show it
            logForm = new RunningForm();
            logForm.DeployDMPS = this;
            logForm.Show();
            this.OnStepChanged += logForm.DeployDMPS_OnStepChanged;
            
            Log("Starting Deployment for " + DMPSHostName + " [" + DMPSIPAddress + "]");

            curStep = DeploymentStep.Connect;

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

            logForm.targetDMPS.Text = "Target DMPS Unit:" + DMPSIPAddress + " (" + DMPSHostName + ")";

            Connect();
        }

        private void VptSessionClass_OnFileXferBatchStart(int nTransactionID, string pszwDescription, int nTotalFiles, int nTotalSize)
        {
            Log("\tAPI On Async Activate STart - nTransactionID:[" + nTransactionID + "], " +
               "pszwDescription:[" + pszwDescription + "], " +
               "nTotalFiles:[" + nTotalFiles + "], " +
               "nTotalSize:[" + nTotalSize + "]");
        }

        private void VptSessionClass_OnFileXferBatchFinish(int nTransactionID, byte bSuccess)
        {
            Log("\tAPI On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
               "bSuccess:[" + bSuccess + "]");
        }

        private void VptSessionClass_OnAsyncActivateStart(int nTransactionID, int nAbilityCode, int nUserPassBack)
        {
            Log("\tAPI On Async Activate STart - nTransactionID:[" + nTransactionID + "], " +
               "nAbilityCode:[" + nAbilityCode + "], " +
               "nUserPassBack:[" + nUserPassBack + "]");
        }

        private void VptSessionClass_OnActivateComplete(int nTransactionID, int nAbilityCode, byte bSuccess, ref Array psaOutputs, int nUserPassBack)
        {
            Log("\tAPI On Activate Complete - nTransactionID:[" + nTransactionID + "], " +
                "nAbilityCode:[" + nAbilityCode + "], " +
                "bSuccess:[" + bSuccess + "], psaOutputs:[" + string.Join("|", psaOutputs) + "], " +
                "nUserPassBack:[" + nUserPassBack + "]");
        }

        private void VptSessionClass_OnTaskComplete(int nTransactionID, byte bSuccess)
        {
            Log("\tAPI On Task Complete - nTransactionID:[" + nTransactionID + "], " +
                "bSuccess:[" + bSuccess + "]");
        }

        private void VptSessionClass_OnTaskProgress(int nTransactionID, int nPercentageDone, string pszwProgressDescription, int lParam)
        {
            Log("\tAPI On Task Progress - nTransactionID:[" + nTransactionID + "], " +
               "nPercentageDone:[" + nPercentageDone + "], " +
               "pszwProgressDescription:[" + pszwProgressDescription + "], " + 
               "lParam:[" + lParam + "]");
        }

        private void VptSessionClass_OnTaskStart(int nTransactionID, string pszwTaskName, string pszwTaskDescription)
        {
            Log("\tAPI On Task Start - nTransactionID:[" + nTransactionID + "], " +
                "pszwTaskName:[" + pszwTaskName + "], " +
                "pszwTaskDescription:[" + pszwTaskDescription + "]");
        }

        private void VptSessionClass_OnFileXferFileFinish(int nTransactionID, byte bSuccess)
        {
            Log("\tAPI On File Xfer File Finish - nTransactionID:[" + nTransactionID + "], " +
                "bSuccess:[" + bSuccess + "]");
        }
        
        private void VptSessionClass_OnFileXferFileProgress(int nTransactionID, int nFileBytesTransferred, int nTotalBytesTransferred, int nBytesPerSecond, int nEstTotalTimeRemaining)
        {
            Log("\tAPI On File Xfer File Progress - nTransactionID:[" + nTransactionID + "], " +
                "nFileBytesTransferred:[" + nFileBytesTransferred + "], " +
                "nTotalBytesTransferred:[" + nTotalBytesTransferred + "], nBytesPerSecond:[" + nBytesPerSecond + "], " +
                "nEstTotalTimeRemaining:[" + nEstTotalTimeRemaining + "]");
        }

        private void VptSessionClass_OnFileXferFileStart(int nTransactionID, string pszwLocalFilename, string pszwRemoteFilename, int nSize, byte bSending, string pszwProtocolName)
        {
            Log("\tAPI On File Xfer File Start - nTransactionID:[" + nTransactionID + "], " + 
                "pszwLocalFilename:[" + pszwLocalFilename + "], " + 
                "pszwRemoteFilename:[" + pszwRemoteFilename + "], nSize:[" + nSize + "], " + 
                "bSending:[" + bSending + "], pszwProtocolName :[" + pszwProtocolName + "]");
        }

        private void VptSessionClass_OnSessionReady()
        {
            Log("\tAPI On Session Ready");            

            FigureOutNextStepAfterConnect();
        }

        private void VptSessionClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log("\tAPI On Activate Str Complete - nTransactionID:[" + nTransactionID + "], nAbilityCode:[" + nAbilityCode + "], bSuccess:[" + bSuccess + "], " + 
                "pszOutputs:[" + pszOutputs + "], nUserPassBack:[" + nUserPassBack + "]");
            
            if (bSuccess == 0)
            {
                if (curStep == DeploymentStep.RetrieveZIG && retryNumber < 5)
                {
                    if (StopProcessing) return;
                    RetrieveZigFile();
                    retryNumber++;
                }
                else if (curStep == DeploymentStep.SetSignals)
                {
                    SetNextSignal();
                }
            }
            else
            {
                if (curStep == DeploymentStep.RetrieveZIG)
                {
                    if (nTransactionID == curAPITransactionID)
                    {
                        if (StopProcessing) return;
                        //start the next step
                        Log("ZIG file Xfer complete");
                        UnzipAndParseSigFile();                        
                    }
                }
                else if (curStep == DeploymentStep.GetCurrentSignals)
                {
                    int count = signalList.Count(one => one.HasSignalValue);
                    Log("Current Signals complete - " + count + " out of " + signalList.Count + " retrieved current values");

                    var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalList.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "OldSignalsComplete.json"))
                        System.IO.File.Delete(TempDirectory + "OldSignalsComplete.json");

                    System.IO.File.WriteAllText(TempDirectory + "OldSignalsComplete.json", signalListJson);

                    if (StopProcessing) return;
                    
                    SendSPZFile();
                }
                else if (curStep == DeploymentStep.PushNewSPZFile)
                {                    
                    Log("New SPZ file pushed");

                    if (StopProcessing) return;
                    WaitForSystemToLoad();
                }
                else if (curStep == DeploymentStep.PushNewZigFile)
                {                    
                    Log("New ZIG file pushed");

                    if (System.IO.File.Exists(TempDirectory + "PushDMPSFilesComplete.txt"))
                        System.IO.File.Delete(TempDirectory + "PushDMPSFilesComplete.txt");

                    System.IO.File.WriteAllText(TempDirectory + "PushDMPSFilesComplete.txt", "DMPS Files pushed complete on " + DateTime.Now.ToString());
                    
                    if (StopProcessing) return;
                    GetCurrentSignals2();
                }
                else if (curStep == DeploymentStep.ParseNewSigAndGetCurrentSignals2)
                {                    
                    int count = signalListAfterDeployment.Count(one => one.HasSignalValue);
                    Log("New Signals complete - " + count + " out of " + signalListAfterDeployment.Count + " retrieved current values");

                    var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterDeployment.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "NewSignalsComplete.json"))
                        System.IO.File.Delete(TempDirectory + "NewSignalsComplete.json");

                    System.IO.File.WriteAllText(TempDirectory + "NewSignalsComplete.json", signalListJson);

                    if (StopProcessing) return;
                    SetSignals();
                }
                else if (curStep == DeploymentStep.SetSignals)
                {
                    SetNextSignal();
                }
                else if (curStep == DeploymentStep.SaveAndReboot)
                {
                    Log("Signals saved and device reboot initiated");

                    if (System.IO.File.Exists(TempDirectory + "NewSignalsSet.txt"))
                        System.IO.File.Delete(TempDirectory + "NewSignalsSet.txt");

                    System.IO.File.WriteAllText(TempDirectory + "NewSignalsSet.txt", "New Signals set on " + DateTime.Now.ToString());

                    if (StopProcessing) return;
                    //trigger next step
                    GetDevicesFromDMPS();
                }
                else if (curStep == DeploymentStep.GetDMPSDevices)
                {
                    //parse outputs
                    DeviceResultsFromDMPS = pszOutputs.Split(new char[] { '\"', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (StopProcessing) return;
                    GetTSPIPAddresses();
                }
                else if (curStep == DeploymentStep.GetUITouchPanelAddresses)
                {
                    //parse output
                    string[] x = pszOutputs.Split(new char[] { '\"', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (x[0].StartsWith("TSW"))
                    {
                        vtzIPAddresses.Add(x[x.Length - 2]);
                    }
                    GetNextTSPIPAddress();
                }
            }
        }

        private void VptSessionClass_OnEvent(int nTransactionID, 
            [System.Runtime.InteropServices.ComAliasName("VPTCOMSERVERLib.EVptEventType")] VPTCOMSERVERLib.EVptEventType nEventType, 
            int lParam1, int lParam2, string pszwParam)
        {
            Log("\tAPI On Event - nTransactionID:[" + nTransactionID + "], nEventType:[" + nEventType + "], lParam1:[" + lParam1 + "], lParam2:[" + lParam2 + "], pszwParam:[" + pszwParam + "]");

            if (curStep == DeploymentStep.GetCurrentSignals && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)
            {
                var signal = signalList.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }
            }
            else if (curStep == DeploymentStep.ParseNewSigAndGetCurrentSignals2 && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)
            {
                var signal = signalListAfterDeployment.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }
            }
            else if (curStep == DeploymentStep.WaitForSystemToLoad && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)
            {
                if (pszwParam == "0" && lParam1 == startupBusySignal.SignalIndex)
                {
                    Log("Startup busy has reset - going to next step");
                    if (StopProcessing) return;
                    SendNewZIGFile();
                    return;
                }

                System.Threading.Thread.Sleep(5000);
                WaitForSystemToLoad();
            }

            Log(nEventType.ToString() + " - " + lParam1.ToString() + " = [" + pszwParam + "]");
        }

        private void VptSessionClass_OnDebugString(int nTransactionID, int nCategory, string pszwData)
        {
            Log("\tAPI On Debug String - nTransactionID:[" + nTransactionID + "], nCategory:[" + nCategory + "], data:[" + pszwData + "]");
        }
                
        public void FigureOutNextStepAfterConnect()
        {
            if (StopProcessing) return;

            //determine if we need to resume
            if (System.IO.File.Exists(TempDirectory + "DeploymentComplete.txt"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have finished a deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    curStep = DeploymentStep.RetrieveZIG;
                    System.IO.Directory.Delete(TempDirectory, true);
                    var x = TempDirectory; //this will force it to recreate because I put it in the get { }
                }
                else
                {
                    return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignalsSet.txt"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - start vtz files?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsCompleteFromFile();
                    curStep = DeploymentStep.GetDMPSDevices;
                }
                else
                {
                    if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " was mid-deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        curStep = DeploymentStep.RetrieveZIG;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignalsComplete.json"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress + 
                        " appears to have stopped in the middle of a deployment.  Resume deployment - set signals?", "", 
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsCompleteFromFile();
                    curStep = DeploymentStep.SetSignals;
                }
                else
                {
                    if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " was mid-deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        curStep = DeploymentStep.RetrieveZIG;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "PushDMPSFilesComplete.txt"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - get new signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsFromFile();
                    curStep = DeploymentStep.ParseNewSigAndGetCurrentSignals2;
                }
                else
                {
                    if(MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " was mid-deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        curStep = DeploymentStep.RetrieveZIG;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "OldSignalsComplete.json"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - push files to DMPS?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsFromFile();
                    curStep = DeploymentStep.PushNewSPZFile;
                }
                else
                {
                    if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " was mid-deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        curStep = DeploymentStep.RetrieveZIG;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignals.json"))
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - get current signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsFromFile();
                    LoadNewSignalsFromFile();
                    curStep = DeploymentStep.GetCurrentSignals;
                }
                else
                {
                    if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " was mid-deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        curStep = DeploymentStep.RetrieveZIG;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            var step = curStep;

            if (step == DeployDMPS.DeploymentStep.Connect)
                RetrieveZigFile(); //this one is the only one that is differnet - the rest are more like a "resume"
            else if (step == DeployDMPS.DeploymentStep.RetrieveZIG)
                RetrieveZigFile();
            else if (step == DeployDMPS.DeploymentStep.UnzipAndParseSIG)
                UnzipAndParseSigFile();
            else if (step == DeployDMPS.DeploymentStep.GetCurrentSignals)
                GetCurrentSignals();
            else if (step == DeployDMPS.DeploymentStep.PushNewSPZFile)
                SendSPZFile();
            else if (step == DeployDMPS.DeploymentStep.PushNewZigFile)
                SendNewZIGFile();
            else if (step == DeployDMPS.DeploymentStep.WaitForSystemToLoad)
                WaitForSystemToLoad();
            else if (step == DeployDMPS.DeploymentStep.ParseNewSigAndGetCurrentSignals2)
                GetCurrentSignals2();
            else if (step == DeployDMPS.DeploymentStep.SetSignals)
                SetSignals();
            else if (step == DeployDMPS.DeploymentStep.SaveAndReboot)
                SaveAndReboot();
            else if (step == DeployDMPS.DeploymentStep.GetDMPSDevices)
                GetDevicesFromDMPS();
            else if (step == DeployDMPS.DeploymentStep.GetUITouchPanelAddresses)
                GetTSPIPAddresses();
            else if (step == DeployDMPS.DeploymentStep.PushVtzFiles)
                SendNewVTZFiles();
        }
        public void Connect()
        {
            //connect
            int connect = vptSessionClass.OpenSession("auto " + DMPSIPAddress, DMPSIPAddress);
            string output = "";
            vptSessionClass.SyncActivateStr(0, "ProductGetInfo", output, 10000, 1);
            string x = output;
        }

        public void RetrieveZigFile()
        {
            curStep = DeploymentStep.RetrieveZIG;
            Log("---------------------");
            Log("Step 2 - Retrieving ZIG file");            

            if (System.IO.File.Exists(TempDirectory + "TEC HD.zig"))
                System.IO.File.Delete(TempDirectory + "TEC HD.zig");

            string outputs = "";
            vptSessionClass.AsyncActivateStr(0, @"FileXferGet ""\SIMPL\TEC HD.zig"" """ + TempDirectory + "TEC HD.zig\"", 10000, ref curAPITransactionID, 0, 0);

            Log("\tTransaction ID:[" + curAPITransactionID + "]");
            Log("\tFileXferGet Output:[" + outputs + "]");
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


        List<CrestronSignal> signalList = new List<CrestronSignal>();

        List<CrestronSignal> signalListAfterDeployment = new List<CrestronSignal>();

        public void UnzipAndParseSigFile()
        {
            curStep = DeploymentStep.UnzipAndParseSIG;
            Log("---------------");
            Log("Unzipping and parsing SIG file");

            if (System.IO.Directory.Exists(TempDirectory + @"TEC HD\"))
                System.IO.Directory.Delete(TempDirectory + @"TEC HD\", true);

            System.IO.Compression.ZipFile.ExtractToDirectory(TempDirectory + @"TEC HD.zig", TempDirectory +  @"TEC HD\");

            byte[] sigFile = System.IO.File.ReadAllBytes(TempDirectory + @"TEC HD\TEC HD.sig");

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
                //Log("\t" + x.ToString());
                signalList.Add(x);

                if (signalList.Count % 25 == 0)
                    Log("\t" + signalList.Count.ToString() + " signals parsed");
            }

            //save it
            var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalList.ToArray());
            
            if (System.IO.File.Exists(TempDirectory + "OldSignals.json"))
                System.IO.File.Delete(TempDirectory + "OldSignals.json");

            System.IO.File.WriteAllText(TempDirectory + "OldSignals.json", signalListJson);

            //parse new one since we have it, even though we don't use it yet
            ParseNewSigFile();

            if (StopProcessing) return;

            //start next step
            GetCurrentSignals();
        }

        public void LoadOldSignalsFromFile()
        {
            signalList = new List<CrestronSignal>(
                Newtonsoft.Json.JsonConvert.DeserializeObject<CrestronSignal[]>(System.IO.File.ReadAllText(TempDirectory + "OldSignals.json")));
        }

        public void LoadOldSignalsCompleteFromFile()
        {
            signalList = new List<CrestronSignal>(
                Newtonsoft.Json.JsonConvert.DeserializeObject<CrestronSignal[]>(System.IO.File.ReadAllText(TempDirectory + "OldSignalsComplete.json")));
        }

        public void LoadNewSignalsFromFile()
        {
            signalListAfterDeployment = new List<CrestronSignal>(
                Newtonsoft.Json.JsonConvert.DeserializeObject<CrestronSignal[]>(System.IO.File.ReadAllText(TempDirectory + "NewSignals.json")));
        }

        public void LoadNewSignalsCompleteFromFile()
        {
            signalListAfterDeployment = new List<CrestronSignal>(
                Newtonsoft.Json.JsonConvert.DeserializeObject<CrestronSignal[]>(System.IO.File.ReadAllText(TempDirectory + "NewSignalsComplete.json")));
        }

        public void GetCurrentSignals()
        {
            curStep = DeploymentStep.GetCurrentSignals;
            Log("---------------");
            Log("Getting current signals from DMPS....");
            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus", 10000, ref curAPITransactionID, 0, 0);            
        }

        public void SendSPZFile()
        {
            curStep = DeploymentStep.PushNewSPZFile;
            Log("---------------");
            Log("Sending new SPZ file....");
            vptSessionClass.AsyncActivateStr(0, "ProgramSend \"" + NewSPZFileName + "\"", 10000, curAPITransactionID, 0, 0);
        }

        public void SendNewZIGFile()
        {
            curStep = DeploymentStep.PushNewZigFile;

            Log("---------------");
            Log("Sending new ZIG file....");

            string sigFileName = System.IO.Path.ChangeExtension(NewSPZFileName, ".sig");
            string zigFileName = System.IO.Path.ChangeExtension(NewSPZFileName, ".zig");

            if (System.IO.File.Exists(TempDirectory + System.IO.Path.GetFileName(zigFileName)))
                System.IO.File.Delete(TempDirectory + System.IO.Path.GetFileName(zigFileName));

            using (var zip = ZipFile.Open(TempDirectory + System.IO.Path.GetFileName(zigFileName), ZipArchiveMode.Create))
                zip.CreateEntryFromFile(sigFileName, System.IO.Path.GetFileName(sigFileName));

            int transactionID = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileXferPut """ + TempDirectory + System.IO.Path.GetFileName(zigFileName) + @""" ""\SIMPL\" + System.IO.Path.GetFileName(zigFileName) + @""" ", 
                5000, ref transactionID, 0, 0);            
        }

        public void ParseNewSigFile()
        {
            Log("---------------");
            Log("Parsing new SIG file");

            string sigFileName = System.IO.Path.ChangeExtension(NewSPZFileName, ".sig");
            byte[] sigFile = System.IO.File.ReadAllBytes(sigFileName);

            signalListAfterDeployment.Clear();

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
                //Log("\t" + x.ToString());
                signalListAfterDeployment.Add(x);

                if (signalListAfterDeployment.Count % 25 == 0)
                    Log("\t" + signalListAfterDeployment.Count.ToString() + " signals parsed");
            }

            //save it
            var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterDeployment.ToArray());

            if (System.IO.File.Exists(TempDirectory + "NewSignals.json"))
                System.IO.File.Delete(TempDirectory + "NewSignals.json");

            System.IO.File.WriteAllText(TempDirectory + "NewSignals.json", signalListJson);
        }

        public void GetCurrentSignals2()
        {
            curStep = DeploymentStep.ParseNewSigAndGetCurrentSignals2;
            

            Log("---------------");
            Log("Getting current signals from DMPS (again)....");
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus", 10000, ref curAPITransactionID, 0, 0);
        }

        List<Tuple<string, string>> signalsToSet = new List<Tuple<string, string>>();
        int signalIndexToSet = 0;

        public void SetSignals()
        {
            //see which ones are different from what they used to be and set them           
            signalsToSet = new List<Tuple<string, string>>();
            signalIndexToSet = 0;
            foreach (var oldSignal in signalList)
            {
                if (oldSignal.SignalName.StartsWith("::") ||
                    oldSignal.SignalName.StartsWith("//"))
                {
                    continue;
                }

                var newSignal = signalListAfterDeployment.Find(one => one.SignalName == oldSignal.SignalName);

                if (newSignal != null)
                {
                    if (
                        (oldSignal.HasSignalValue && newSignal.HasSignalValue && oldSignal.SignalValue != newSignal.SignalValue) ||
                        (oldSignal.HasSignalValue && !newSignal.HasSignalValue)
                       )
                    {
                        //reset this one
                        if (newSignal.SignalType == 0)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetDigital " + newSignal.SignalIndex + ", " + oldSignal.SignalValue, newSignal.SignalName));
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetAnalog " + newSignal.SignalIndex + ", " + oldSignal.SignalValue, newSignal.SignalName));
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetSerial " + newSignal.SignalIndex + ", \"" + oldSignal.SignalValue + "\"", newSignal.SignalName));
                    }                    
                    else if (!oldSignal.HasSignalValue && newSignal.HasSignalValue)
                    {
                        //reset this one to default
                        if (newSignal.SignalType == 0)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetDigital " + newSignal.SignalIndex + ", 0", newSignal.SignalName));
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetAnalog " + newSignal.SignalIndex + ", 0", newSignal.SignalName));
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetSerial " + newSignal.SignalIndex + ", \"\"", newSignal.SignalName));
                    }
                }
            }

            foreach (var newSignal in signalListAfterDeployment)
            {
                if (newSignal.SignalName.StartsWith("::") ||
                    newSignal.SignalName.StartsWith("//"))
                {
                    continue;
                }

                var oldSignal = signalList.Find(one => one.SignalName == newSignal.SignalName);

                if (oldSignal == null)
                {
                    //new signal - make sure it didn't get a value
                    if (newSignal.HasSignalValue && newSignal.SignalValue != "0" && newSignal.SignalValue != "")
                    {
                        //reset it
                        if (newSignal.SignalType == 0)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetDigital " + newSignal.SignalIndex + ", 0", newSignal.SignalName));
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetAnalog " + newSignal.SignalIndex + ", 0", newSignal.SignalName));
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new Tuple<string, string>("SignalDbgSetSerial " + newSignal.SignalIndex + ", \"\"", newSignal.SignalName));
                    }
                }
            }

            System.IO.File.WriteAllLines(TempDirectory + @"NeedToOutput.txt", signalsToSet.Select(one => one.Item1 + "|" + one.Item2));

            SetNextSignal();
        }

        public void SetNextSignal()
        {
            curStep = DeploymentStep.SetSignals;

            if (signalIndexToSet >= signalsToSet.Count)
            {
                //next step
                if (StopProcessing) return;
                SaveAndReboot();
            }
            else
            {
                var command = signalsToSet[signalIndexToSet];

                signalIndexToSet++;

                Log("Setting " + command.Item2 + " [" + command.Item1 + "]");
                vptSessionClass.AsyncActivateStr(0, command.Item1, 10000, ref curAPITransactionID, 0, 0);
            }
        }

        public void SaveAndReboot()
        {
            curStep = DeploymentStep.SaveAndReboot;
            Log("---------------");
            Log("Saving and rebooting....");

            var signal = signalListAfterDeployment.Find(one => one.SignalName == "Store_Settings");

            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgPulseDigital " + signal.SignalIndex + ", 50", ref output, 10000, 0);

            vptSessionClass.AsyncActivateStr(0, "ProgramRestart", 10000, ref curAPITransactionID, 0, 0);
        }

        string[] DeviceResultsFromDMPS;
        int deviceToQueryIndex = 0;

        public void GetDevicesFromDMPS()
        {
            curStep = DeploymentStep.GetDMPSDevices;
            Log("---------------");
            Log("Getting current devices from DMPS....");            
            vptSessionClass.AsyncActivateStr(0, "SubNetworkReportDevices 2", 10000, ref curAPITransactionID, 0, 0);
        }

        public void GetTSPIPAddresses()
        {
            curStep = DeploymentStep.GetUITouchPanelAddresses;
            Log("---------------");
            Log("Getting device information....");
            deviceToQueryIndex = 1;

            GetNextTSPIPAddress();
        }

        public void GetNextTSPIPAddress()
        {
            if (deviceToQueryIndex + 2 >= DeviceResultsFromDMPS.Length)
            {
                //next step
                Log("Complete query of Devices");
                if (StopProcessing) return;
                SendNewVTZFiles();
            }
            else
            {
                

                Log("Getting device info for index" + DeviceResultsFromDMPS[deviceToQueryIndex] + "....");

                int value = Convert.ToInt32(DeviceResultsFromDMPS[deviceToQueryIndex], 16);

                vptSessionClass.AsyncActivateStr(0, "SubNetworkReportDevice 2, " + value, 10000, ref curAPITransactionID, 0, 0);

                deviceToQueryIndex = deviceToQueryIndex + 2;
            }
        }
                
        CrestronSignal startupBusySignal;

        public void WaitForSystemToLoad()
        {
            curStep = DeploymentStep.WaitForSystemToLoad;            
            Log("---Waiting for system to load---");
            System.Threading.Thread.Sleep(30000);
            startupBusySignal = signalListAfterDeployment.Find(one => one.SignalName == "System_busy");            
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + startupBusySignal.SignalIndex, 10000, ref curAPITransactionID, 0, 0);
        }

        List<string> vtzIPAddresses = new List<string>();
        int vtzIndex = 0;
        public void SendNewVTZFiles()
        {
            curStep = DeploymentStep.PushVtzFiles;
            Log("---------------");
            Log("Pushing VTZ files to " + vtzIPAddresses.Count + " devices....");

            //return;

            vtzIndex = 0;

            vptSessionClass.CloseSession();

            vtzClass.OnSessionReady += VtzClass_OnSessionReady;
            vtzClass.OnAsyncActivateStart += VtzClass_OnAsyncActivateStart;
            vtzClass.OnFileXferBatchStart += VtzClass_OnFileXferBatchStart;
            vtzClass.OnFileXferFileStart += VtzClass_OnFileXferFileStart;
            vtzClass.OnFileXferFileProgress += VtzClass_OnFileXferFileProgress;
            vtzClass.OnFileXferFileFinish += VtzClass_OnFileXferFileFinish;
            vtzClass.OnFileXferBatchFinish += VtzClass_OnFileXferBatchFinish;
            vtzClass.OnActivateStrComplete += VtzClass_OnActivateStrComplete;

            SendNextVTZFile();
        }

        private void VtzClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log("\tVTZ On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
               "bSuccess:[" + bSuccess + "]");

            SendNextVTZFile();
        }

        private void VtzClass_OnFileXferBatchFinish(int nTransactionID, byte bSuccess)
        {
            Log("\tVTZ On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
               "bSuccess:[" + bSuccess + "]");                        
        }

        private void VtzClass_OnFileXferFileFinish(int nTransactionID, byte bSuccess)
        {
            Log("\tVTZ On File Xfer File Finish - nTransactionID:[" + nTransactionID + "], " +
               "bSuccess:[" + bSuccess + "]");
        }

        private void VtzClass_OnFileXferFileProgress(int nTransactionID, int nFileBytesTransferred, int nTotalBytesTransferred, int nBytesPerSecond, int nEstTotalTimeRemaining)
        {
            Log("\tVTZ On File Xfer File Progress - nTransactionID:[" + nTransactionID + "], " +
                 "nFileBytesTransferred:[" + nFileBytesTransferred + "], " +
                 "nTotalBytesTransferred:[" + nTotalBytesTransferred + "], nBytesPerSecond:[" + nBytesPerSecond + "], " +
                 "nEstTotalTimeRemaining:[" + nEstTotalTimeRemaining + "]");
        }

        private void VtzClass_OnFileXferBatchStart(int nTransactionID, string pszwDescription, int nTotalFiles, int nTotalSize)
        {
            Log("\tVTZ On File Xfer Batch Start - nTransactionID:[" + nTransactionID + "], " +
               "pszwDescription:[" + pszwDescription + "], " +
               "nTotalFiles:[" + nTotalFiles + "], nTotalSize:[" + nTotalSize + "]");
        }

        private void VtzClass_OnFileXferFileStart(int nTransactionID, string pszwLocalFilename, string pszwRemoteFilename, int nSize, byte bSending, string pszwProtocolName)
        {
            Log("\tVTZ On File Xfer File Start - nTransactionID:[" + nTransactionID + "], " +
               "pszwLocalFilename:[" + pszwLocalFilename + "], " +
               "pszwRemoteFilename:[" + pszwRemoteFilename + "], nSize:[" + nSize + "], " +
               "bSending:[" + bSending + "], pszwProtocolName :[" + pszwProtocolName + "]");
        }

        private void VtzClass_OnAsyncActivateStart(int nTransactionID, int nAbilityCode, int nUserPassBack)
        {
            Log("VTZ Session starting nTransactionID:[" + nTransactionID + "], nAbilityCode:[" + nAbilityCode + "], nUserPassBack:[" + nUserPassBack + "]");
        }

        private void VtzClass_OnSessionReady()
        {
            vtzClass.AsyncActivateStr(0, "DisplayListSend \"" + NewVTZFileName + "\"", 10000, ref curAPITransactionID, 0, 0);
        }

        VPTCOMSERVERLib.VptSessionClass vtzClass = new VPTCOMSERVERLib.VptSessionClass();        

        public void SendNextVTZFile()
        {
            if (vtzIndex == vtzIPAddresses.Count)
            {
                //all done
                if (System.IO.File.Exists(TempDirectory + "DeploymentComplete.txt"))
                    System.IO.File.Delete(TempDirectory + "DeploymentComplete.txt");

                System.IO.File.WriteAllText(TempDirectory + "DeploymentComplete.txt", "Deployment Complete on " + DateTime.Now.ToString());

                if (OnDMPSDeployedSuccessfully != null)
                {
                    OnDMPSDeployedSuccessfully("DMPS " + DMPSHostName + " / " + DMPSIPAddress + " Deployment Complete!");
                }

                logForm.CloseForm();

            }
            else
            {
                string ip = vtzIPAddresses[vtzIndex];
                vtzIndex++;
                vtzClass.CloseSession();
                vtzClass.OpenSession("auto " + ip, ip);
                Log("Connection to session on TouchPanel " + ip);
                string output = "";
                vtzClass.SyncActivateStr(0, "ProductGetInfo", output, 10000, 1);
            }
        }
    }
}