using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMPSMassDeploymentTool
{
    public class DeployDMPS : IDisposable
    {
        public delegate void MyEventHandler (string message);
        public event MyEventHandler OnDMPSDeployedSuccessfully;
        public event MyEventHandler OnDMPSDeploymentFatalError;

        public string DMPSHostName;
        public string DMPSIPAddress;
        public string NewSPZFileName;


        public DeployDMPS(string hostName, string ipAddress, string spzFilename)
        {
            this.DMPSHostName = hostName;
            this.DMPSIPAddress = ipAddress;
            this.NewSPZFileName = spzFilename;
        }

        enum DeploymentStep
        {
            Connect,
            RetrieveZIG,
            UnzipAndParseSIG,
            GetCurrentSignals,
            PushNewSPZFile,
            PushNewZigFile,            
            SetSignals,
            GetUITouchPanelAddresses,
            PushVtzFiles
        }

        DeploymentStep curStep;
        RunningForm logForm;
        int curAPITransactionID;
        int retryNumber = 0;

        VPTCOMSERVERLib.VptSessionClass vptSessionClass { get; set; }

        public void Dispose()
        {
            if (vptSessionClass != null)
                vptSessionClass.CloseSession();
        }

        void Log(string m)
        {
            logForm.Log(DateTime.Now.ToString("hh:mm:ss.fff") + " - " + m);
        }

        public void StartDeployment()
        {
            //create the running form and show it
            logForm = new RunningForm();
            logForm.Show();
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

            //connect
            int connect = vptSessionClass.OpenSession("auto " + DMPSIPAddress, DMPSIPAddress);
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

            if (curStep == DeploymentStep.Connect)
            {
                //kick off the next step
                Log("Connect complete.");
                RetrieveZigFile();
            }
        }

        private void VptSessionClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log("\tAPI On Activate Str Complete - nTransactionID:[" + nTransactionID + "], nAbilityCode:[" + nAbilityCode + "], bSuccess:[" + bSuccess + "], pszOutputs:[" + pszOutputs + "], nUserPassBack:[" + nUserPassBack + "]");

            if (bSuccess == 0)
            {
                if (curStep == DeploymentStep.RetrieveZIG && retryNumber < 5)
                {
                    RetrieveZigFile();
                    retryNumber++;
                }
            }
            else
            {
                if (curStep == DeploymentStep.RetrieveZIG)
                {
                    if (nTransactionID == curAPITransactionID)
                    {
                        
                        //start the next step
                        Log("ZIG file Xfer complete");
                        UnzipAndParseSigFile();                        
                    }
                }
                else if (curStep == DeploymentStep.GetCurrentSignals)
                {
                    int count = signalList.Count(one => one.HasSignalValue);
                    Log("Current Signals complete - " + count + " out of " + signalList.Count + " retrieved current values");
                    SendSPZFile();
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
            Log(nEventType.ToString() + " - " + lParam1.ToString() + " = [" + pszwParam + "]");
        }

        private void VptSessionClass_OnDebugString(int nTransactionID, int nCategory, string pszwData)
        {
            Log("\tAPI On Debug String - nTransactionID:[" + nTransactionID + "], nCategory:[" + nCategory + "], data:[" + pszwData + "]");
        }

        private void RetrieveZigFile()
        {
            curStep = DeploymentStep.RetrieveZIG;
            Log("---------------------");
            Log("Step 2 - Retrieving ZIG file");
            if (!System.IO.Directory.Exists(@"C:\temp\"))
                System.IO.Directory.CreateDirectory(@"C:\temp\");

            if (System.IO.File.Exists(@"C:\temp\TEC HD.zig"))
                System.IO.File.Delete(@"C:\temp\TEC HD.zig");

            string outputs = "";
            vptSessionClass.AsyncActivateStr(0, @"FileXferGet ""\SIMPL\TEC HD.zig"" ""c:\temp\TEC HD.zig""", 10000, ref curAPITransactionID, 0, 0);

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

        private void UnzipAndParseSigFile()
        {
            curStep = DeploymentStep.UnzipAndParseSIG;
            Log("---------------");
            Log("Unzipping and parsing SIG file");

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
                //Log("\t" + x.ToString());
                signalList.Add(x);

                if (signalList.Count % 25 == 0)
                    Log("\t" + signalList.Count.ToString() + " signals parsed");
            }


            //start next step
            GetCurrentSignals();
        }

        private void GetCurrentSignals()
        {
            curStep = DeploymentStep.GetCurrentSignals;
            Log("---------------");
            Log("Getting current signals from DMPS....");                        
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus", 10000, ref curAPITransactionID, 0, 0);            
        }

        private void SendSPZFile()
        {
            curStep = DeploymentStep.PushNewSPZFile;
            Log("---------------");
            Log("Sending new SPZ file....");
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus", 10000, ref curAPITransactionID, 0, 0);
        }
    }
}
