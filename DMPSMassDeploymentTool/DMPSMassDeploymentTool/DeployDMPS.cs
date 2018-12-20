using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Drawing;

namespace DMPSMassDeploymentTool
{
    public class DeployDMPS : IDisposable
    {
        public delegate void MyEventHandler(string message);
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
            WaitForSystemToLoad2,
            GetSignalsAfterSet,
            GetDMPSDevices,
            GetUITouchPanelAddresses,
            PushVtzFiles
        }

        DeploymentStep curStep;
        DeploymentStep CurStep
        {
            get { return curStep; }
            set
            {
                curStep = value;

                OnStepChanged?.Invoke(curStep);
            }
        }

        public Dictionary<DeploymentStep, int> TxnIDs = new Dictionary<DeploymentStep, int>();


        RunningForm logForm;
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

        public void StartDeployment(Form1 masterForm, Size formSize, Point formTopLeft)
        {
            //create the running form and show it
            logForm = new RunningForm();
            logForm.DeployDMPS = this;            
            logForm.Show();
            logForm.Top = formTopLeft.Y;
            logForm.Left = formTopLeft.X;
            logForm.Size = formSize;
            this.OnStepChanged += logForm.DeployDMPS_OnStepChanged;

            Log("Starting Deployment for " + DMPSHostName + " [" + DMPSIPAddress + "]");

            CurStep = DeploymentStep.Connect;

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

        bool isRepush = false;
        public void RepushSignals(Form1 masterForm, Size formSize, Point formTopLeft)
        {
            isRepush = true;

            //create the running form and show it
            logForm = new RunningForm();
            logForm.DeployDMPS = this;
            logForm.Show();
            logForm.Top = formTopLeft.Y;
            logForm.Left = formTopLeft.X;
            logForm.Size = formSize;
            this.OnStepChanged += logForm.DeployDMPS_OnStepChanged;

            Log("Starting Re-push signals for " + DMPSHostName + " [" + DMPSIPAddress + "]");

            if (!File.Exists(Path.Combine(TempDirectory, "UnableToSetSignals.txt")) ||
                !File.Exists(Path.Combine(TempDirectory, "OldSignalsComplete.json")) ||
                !File.Exists(Path.Combine(TempDirectory, "PushDMPSFilesComplete.txt"))
                )
            {
                Log("Unable to re-push signals, one or more files missing");
            }

            string[] UnableToSetSignals = File.ReadAllLines(Path.Combine(TempDirectory, "UnableToSetSignals.txt"));

            if (MessageBox.Show("Last time, " + this.DMPSHostName + " was unable to push " + UnableToSetSignals.Length + " signals.  Continue with re-push?", "", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
            {
                logForm.Close();
                return;
            }

            CurStep = DeploymentStep.Connect;
            
            LoadOldSignalsCompleteFromFile();

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

            logForm.targetDMPSGroupBox.Text = "Target DMPS Unit:" + DMPSIPAddress + " (" + DMPSHostName + ")";

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

            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);

            FigureOutNextStepAfterConnect();
        }



        private void VptSessionClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            Log("\tAPI On Activate Str Complete - nTransactionID:[" + nTransactionID + "], nAbilityCode:[" + nAbilityCode + "], bSuccess:[" + bSuccess + "], " +
                "pszOutputs:[" + pszOutputs + "], nUserPassBack:[" + nUserPassBack + "]");

            if (bSuccess == 0)
            {
                if (CurStep == DeploymentStep.RetrieveZIG && retryNumber < 5)
                {
                    if (StopProcessing) return;
                    RetrieveZigFile();
                    retryNumber++;
                }
                else if (CurStep == DeploymentStep.SetSignals)
                {
                    Log("No Signal set success recieved...Retrying...");
                    signalIndexToSet--;
                    SetNextSignal();
                }
                else if (CurStep == DeploymentStep.SaveAndReboot)
                {
                    Log("Retrying...");
                    SaveAndReboot();
                }
            }
            else
            {
                if (CurStep == DeploymentStep.RetrieveZIG && TxnIDs[CurStep] == nTransactionID)
                {
                    if (StopProcessing) return;
                    //start the next step
                    Log("ZIG file Xfer complete");
                    UnzipAndParseSigFile();
                }
                /*else if (CurStep == DeploymentStep.GetCurrentSignals && TxnIDs[CurStep] == nTransactionID)
                {
                    if (signalList[NextOldSignalIndex].HasSignalValue)
                    {
                        Log("Signal received");
                        GetNextOldSignal();
                    }
                    else
                    {
                        Log("Signal not received...");
                        NextOldSignalIndex--;
                        GetNextOldSignal();
                    }
                }*/
                else if (CurStep == DeploymentStep.PushNewSPZFile && (TxnIDs[CurStep] == nTransactionID || TxnIDs[CurStep] == 0))
                {
                    Log("New SPZ file pushed");

                    if (StopProcessing) return;
                    WaitForSystemToLoad();
                }
                else if (CurStep == DeploymentStep.PushNewZigFile && TxnIDs[CurStep] == nTransactionID)
                {
                    Log("New ZIG file pushed");

                    if (System.IO.File.Exists(TempDirectory + "PushDMPSFilesComplete.txt"))
                        System.IO.File.Delete(TempDirectory + "PushDMPSFilesComplete.txt");

                    System.IO.File.WriteAllText(TempDirectory + "PushDMPSFilesComplete.txt", "DMPS Files pushed complete on " + DateTime.Now.ToString());

                    if (StopProcessing) return;
                    GetCurrentSignals2();
                }
                /*else if (CurStep == DeploymentStep.ParseNewSigAndGetCurrentSignals2 && TxnIDs[CurStep] == nTransactionID)
                {
                    if (signalListAfterDeployment[NextNewSignalIndex].HasSignalValue)
                    {
                        Log("Signal received");
                        GetNextNewSignal();
                    }
                    else
                    {
                        Log("Signal not received...");
                        NextNewSignalIndex--;
                        GetNextNewSignal();
                    }
                }*/
                else if (CurStep == DeploymentStep.SetSignals && TxnIDs[CurStep] == nTransactionID)
                {
                    if (CheckSetSignal)
                    {
                        Log("Checking signal just set...");
                        CheckSignalJustSet();
                    }
                    else
                    {
                        //here
                        if (signalIndexToSet >= 0 && signalIndexToSet < signalsToSet.Count)
                        {
                            var command = signalsToSet[signalIndexToSet];
                            if (command.IsConfirmed)
                            {
                                Log("Signal Confirmed...Setting next signal...");
                                SetNextSignal();
                            }
                            else
                            {
                                Log("Signal Not Confirmed...retrying");
                                CheckSignalJustSet();
                            }
                        }
                        else
                        {
                            SetNextSignal();
                        }
                    }
                }
                /*else if (CurStep == DeploymentStep.GetSignalsAfterSet && TxnIDs[CurStep] == nTransactionID)
                {
                    if (signalListAfterSet[NextNewSignalAfterSetIndex].HasSignalValue)
                    {
                        Log("Signal received");
                        GetNextNewSignalAfterSet();
                    }
                    else
                    {
                        Log("Signal not received...");
                        NextNewSignalAfterSetIndex--;
                        GetNextNewSignalAfterSet();
                    }
                }*/
                else if (CurStep == DeploymentStep.SaveAndReboot && TxnIDs[CurStep] == nTransactionID)
                {
                    Log("Signals saved and device reboot initiated");

                    if (System.IO.File.Exists(TempDirectory + "NewSignalsSet.txt"))
                        System.IO.File.Delete(TempDirectory + "NewSignalsSet.txt");

                    System.IO.File.WriteAllText(TempDirectory + "NewSignalsSet.txt", "New Signals set on " + DateTime.Now.ToString());

                    if (StopProcessing) return;
                    //trigger next step
                    WaitForSystemToLoad2();
                }
                else if (CurStep == DeploymentStep.GetDMPSDevices && TxnIDs[CurStep] == nTransactionID)
                {
                    //parse outputs
                    DeviceResultsFromDMPS = pszOutputs.Split(new char[] { '\"', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (DeviceResultsFromDMPS.Length == 0)
                    {
                        //wait ten seconds and redo
                        Log("Waiting 10 seconds because no dmps devices returned...");
                        System.Threading.Thread.Sleep(10000);
                        GetDevicesFromDMPS();
                    }
                    else
                    {
                        if (StopProcessing) return;
                        GetTSPIPAddresses();
                    }
                }
                else if (CurStep == DeploymentStep.GetUITouchPanelAddresses && TxnIDs[CurStep] == nTransactionID)
                {
                    //parse output
                    string[] x = pszOutputs.Split(new char[] { '\"', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (x[0].StartsWith("TSW"))
                    {
                        vtzIPAddresses.Add(x[x.Length - 2]);
                    }
                    GetNextTSPIPAddress();
                }
                else if (CurStep == DeploymentStep.WaitForSystemToLoad)
                {
                    WaitForSystemToLoad();
                }
                else if (CurStep == DeploymentStep.WaitForSystemToLoad2)
                {
                    WaitForSystemToLoad2();
                }
            }
        }

        private void VptSessionClass_OnEvent(int nTransactionID,
            [System.Runtime.InteropServices.ComAliasName("VPTCOMSERVERLib.EVptEventType")] VPTCOMSERVERLib.EVptEventType nEventType,
            int lParam1, int lParam2, string pszwParam)
        {
            Log("\tAPI On Event - nTransactionID:[" + nTransactionID + "], nEventType:[" + nEventType + "], lParam1:[" + lParam1 + "], lParam2:[" + lParam2 + "], pszwParam:[" + pszwParam + "]");

            if (CurStep == DeploymentStep.GetCurrentSignals && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState && TxnIDs[CurStep] == nTransactionID)
            {
                var signal = signalList.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }
            }
            else if (CurStep == DeploymentStep.ParseNewSigAndGetCurrentSignals2 && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState && TxnIDs[CurStep] == nTransactionID)
            {
                var signal = signalListAfterDeployment.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }
            }
            else if (CurStep == DeploymentStep.GetSignalsAfterSet && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState && TxnIDs.ContainsKey(CurStep) && TxnIDs[CurStep] == nTransactionID)
            {
                var signal = signalListAfterSet.Find(one => one.SignalIndex == lParam1);
                if (signal != null)
                {
                    Log("Signal found: " + signal.SignalName);
                    signal.HasSignalValue = true;
                    signal.SignalValue = pszwParam;
                }
            }
            else if (CurStep == DeploymentStep.WaitForSystemToLoad && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)// && TxnIDs[CurStep] == nTransactionID)
            {
                if (pszwParam == "0" && lParam1 == startupBusySignal.SignalIndex)
                {
                    Log("Startup busy has reset - going to next step");
                    if (StopProcessing) return;
                    SendNewZIGFile();
                    return;
                }

                Log("Startup busy is still on");
                WaitForSystemToLoad();
            }
            else if (CurStep == DeploymentStep.WaitForSystemToLoad2 && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState)// && TxnIDs[CurStep] == nTransactionID)
            {
                if (pszwParam == "0" && lParam1 == startupBusySignal.SignalIndex)
                {
                    Log("Startup busy has reset - going to next step");
                    if (StopProcessing) return;
                    //DO SOMETHING HERE
                    GetSignalsAfterSet();
                    return;
                }

                Log("Startup busy is still on");
                WaitForSystemToLoad2();
            }
            else if (CurStep == DeploymentStep.SetSignals && nEventType == VPTCOMSERVERLib.EVptEventType.EVptEventType_SignalState && TxnIDs[CurStep] == nTransactionID)
            {
                //check to make sure it came back appropriately
                var command = signalsToSet[signalIndexToSet];

                if (command.Signal.SignalIndex == lParam1)
                {
                    Log("Expected signal returned value of [" + pszwParam + "], expected [" + command.ExpectedValue + "]");

                    if (pszwParam.Equals(command.ExpectedValue))
                    {
                        command.IsConfirmed = true;
                        Log("Signal confirmed set.");
                    }
                    else
                    {
                        //retry
                        Log("Signal not set correctly - resetting to set again.");
                        if (!retryCount.ContainsKey(command.Signal.SignalIndex))
                            retryCount.Add(command.Signal.SignalIndex, 0);

                        retryCount[command.Signal.SignalIndex]++;

                        if (retryCount[command.Signal.SignalIndex] < 3)
                            signalIndexToSet--;
                        else
                        {
                            Log("Retry count exceeded 3...continuing on...");
                            SignalsUnableToSet.Add(command.Signal.SignalName + ", index [" + command.Signal.SignalIndex + "], expected value [" + command.ExpectedValue + "]");
                            System.IO.File.WriteAllLines(this.TempDirectory + "UnableToSetSignals.txt", SignalsUnableToSet);
                            command.IsConfirmed = true;
                        }
                    }
                }
                else
                {
                    Log("Unexpected signal [" + lParam1 + "] returned value of [" + pszwParam + "], " +
                        "expected signal,value [" + command.Signal.SignalIndex + ", " + command.ExpectedValue + "]");
                }
            }

            //Log(nEventType.ToString() + " - " + lParam1.ToString() + " = [" + pszwParam + "]");
        }

        Dictionary<uint, int> retryCount = new Dictionary<uint, int>();
        List<string> SignalsUnableToSet = new List<string>();

        private void VptSessionClass_OnDebugString(int nTransactionID, int nCategory, string pszwData)
        {
            Log("\tAPI On Debug String - nTransactionID:[" + nTransactionID + "], nCategory:[" + nCategory + "], data:[" + pszwData + "]");
        }

        public bool StartOver()
        {
            if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have finished a deployment.  Do you want to start over?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                CurStep = DeploymentStep.RetrieveZIG;
                System.IO.Directory.Delete(TempDirectory, true);
                var x = TempDirectory; //this will force it to recreate because I put it in the get { }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FigureOutNextStepAfterConnect()
        {
            if (StopProcessing) return;

            //determine if we need to resume
            if (System.IO.File.Exists(TempDirectory + "DeploymentComplete.txt") && !isRepush)
            {
                if (!StartOver()) return;
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignalsAfterSetComplete.json") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - start vtz files?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsCompleteFromFile();
                    CurStep = DeploymentStep.GetDMPSDevices;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignalsSet.txt") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment -  confirm signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsCompleteFromFile();
                    CurStep = DeploymentStep.GetSignalsAfterSet;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignalsComplete.json") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - set signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsCompleteFromFile();
                    CurStep = DeploymentStep.SetSignals;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "PushDMPSFilesComplete.txt") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - get new signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsFromFile();
                    CurStep = DeploymentStep.ParseNewSigAndGetCurrentSignals2;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "OldSignalsComplete.json") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - push files to DMPS?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsCompleteFromFile();
                    LoadNewSignalsFromFile();
                    CurStep = DeploymentStep.PushNewSPZFile;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (System.IO.File.Exists(TempDirectory + "NewSignals.json") && !isRepush)
            {
                if (MessageBox.Show("DMPS " + DMPSHostName + " / " + DMPSIPAddress +
                        " appears to have stopped in the middle of a deployment.  Resume deployment - get current signals?", "",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    LoadOldSignalsFromFile();
                    LoadNewSignalsFromFile();
                    CurStep = DeploymentStep.GetCurrentSignals;
                }
                else
                {
                    if (!StartOver()) return;
                }
            }
            else if (isRepush)
            {
                
                LoadOldSignalsCompleteFromFile();
                LoadNewSignalsFromFile();
                CurStep = DeploymentStep.ParseNewSigAndGetCurrentSignals2;                
            }

            var step = CurStep;

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
            else if (step == DeployDMPS.DeploymentStep.WaitForSystemToLoad2)
                WaitForSystemToLoad2();
            else if (step == DeployDMPS.DeploymentStep.GetSignalsAfterSet)
                GetSignalsAfterSet();
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
            vptSessionClass.SyncActivateStr(0, "ProductGetInfo", output, 1000, 1);
            string x = output;
        }

        public void Nudge()
        {
            if (vptSessionClass.IsSessionReady(1000) == 0)
            {
                Log("Session not ready...");
            }
            else
            {
                Log("Nudging...");
                string output = "";
                vptSessionClass.SyncActivateStr(0, "ProductGetInfo", output, 1000, 1);
                string x = output;
            }
        }

        #region Step 1 - Retrieve ZIG

        public void RetrieveZigFile()
        {
            CurStep = DeploymentStep.RetrieveZIG;
            Log("---------------------");
            Log("Step 2 - Retrieving ZIG file");

            if (System.IO.File.Exists(TempDirectory + "TEC HD.zig"))
                System.IO.File.Delete(TempDirectory + "TEC HD.zig");

            string outputs = "";
            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileXferGet ""\SIMPL\TEC HD.zig"" """ + TempDirectory + "TEC HD.zig\"", 10000, ref curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;

            Log("\tTransaction ID:[" + curAPITransactionID + "]");
            Log("\tFileXferGet Output:[" + outputs + "]");
        }

        #endregion

        #region Step 2 - Unzip and Parse SIG

        public void UnzipAndParseSigFile()
        {
            CurStep = DeploymentStep.UnzipAndParseSIG;
            Log("---------------");
            Log("Unzipping and parsing SIG file");

            if (System.IO.Directory.Exists(TempDirectory + @"TEC HD\"))
                System.IO.Directory.Delete(TempDirectory + @"TEC HD\", true);

            System.IO.Compression.ZipFile.ExtractToDirectory(TempDirectory + @"TEC HD.zig", TempDirectory + @"TEC HD\");

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


        #endregion

        #region Step 3 - Get Current Signals

      

        List<CrestronSignal> signalList = new List<CrestronSignal>();

        List<CrestronSignal> signalListAfterDeployment = new List<CrestronSignal>();

        List<CrestronSignal> signalListAfterSet = new List<CrestronSignal>();

        public bool IgnoreSignalName(string SignalName)
        {
            return SignalName.StartsWith("::") ||
                    SignalName.StartsWith("//") ||
                    (SignalName.StartsWith("DMPS-300-C._") && !SignalName.Contains("Mic") && !SignalName.Contains("Mute") && !SignalName.Contains("Vol"));
        }

        public void GetCurrentSignals()
        {
            CurStep = DeploymentStep.GetCurrentSignals;
            Log("---------------");
            Log("Getting current signals from DMPS....");
            /*string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);

            NextOldSignalIndex = -1;
            GetNextOldSignal();     */

            TelnetGetDMPSSignals sigFinder = new TelnetGetDMPSSignals();
            sigFinder.OnLog += SigFinder_OnLog;
            sigFinder.OnComplete += SigFinder_OnComplete;
            sigFinder.StartGetDMPSSignals(DMPSIPAddress);
        }

        private void SigFinder_OnComplete(object sender, List<CrestronSignal> e)
        {
            if (e == null)
            {
                //no signals returned
                Log("No signals returned, trying again");
                GetCurrentSignals();
            }
            else
            {
                if (MessageBox.Show($"{e.Count} signals returned.  Continue?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    GetCurrentSignals();
                    return;
                }

                foreach (var signal in e)
                {
                    var sig = signalList.Find(one => one.SignalIndex == signal.SignalIndex);
                    if (sig != null)
                    {
                        sig.SignalValue = signal.SignalValue;
                        sig.HasSignalValue = true;
                    }
                }

                //move on to next step
                int count = signalList.Count(one => one.HasSignalValue);

                var possibleCount = signalList.Count(one => !IgnoreSignalName(one.SignalName));

                Log("Current Signals complete - " + count + " out of " + possibleCount + " retrieved current values");

                var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalList.ToArray());

                if (System.IO.File.Exists(TempDirectory + "OldSignalsComplete.json"))
                    System.IO.File.Delete(TempDirectory + "OldSignalsComplete.json");

                System.IO.File.WriteAllText(TempDirectory + "OldSignalsComplete.json", signalListJson);

                if (StopProcessing) return;

                SendSPZFile();
            }
        }

        private void SigFinder_OnLog(object sender, string e)
        {
            Log(e);
        }

        /*
        int NextOldSignalIndex = -1;
        public void GetNextOldSignal()
        {
            while (true)
            {
                NextOldSignalIndex++;

                if (NextOldSignalIndex >= signalList.Count)
                {
                    //move on to next step
                    int count = signalList.Count(one => one.HasSignalValue);

                    var possibleCount = signalList.Count(one => !IgnoreSignalName(one.SignalName));

                    Log("Current Signals complete - " + count + " out of " + possibleCount + " retrieved current values");

                    var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalList.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "OldSignalsComplete.json"))
                        System.IO.File.Delete(TempDirectory + "OldSignalsComplete.json");

                    System.IO.File.WriteAllText(TempDirectory + "OldSignalsComplete.json", signalListJson);

                    if (StopProcessing) return;

                    SendSPZFile();

                    return;
                }

                if (IgnoreSignalName(signalList[NextOldSignalIndex].SignalName))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            
            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + signalList[NextOldSignalIndex].SignalIndex, 10000, ref curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;
        }*/

        #endregion

        #region Step 4 - Push new SPZ File
        public void SendSPZFile()
        {
            CurStep = DeploymentStep.PushNewSPZFile;
            Log("---------------");
            Log("Sending new SPZ file....");

            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "ProgramSend \"" + NewSPZFileName + "\"", 10000, curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 5 - Push new ZIG File
        public void SendNewZIGFile()
        {
            CurStep = DeploymentStep.PushNewZigFile;

            Log("---------------");
            Log("Sending new ZIG file....");

            string sigFileName = System.IO.Path.ChangeExtension(NewSPZFileName, ".sig");
            string zigFileName = System.IO.Path.ChangeExtension(NewSPZFileName, ".zig");

            if (System.IO.File.Exists(TempDirectory + System.IO.Path.GetFileName(zigFileName)))
                System.IO.File.Delete(TempDirectory + System.IO.Path.GetFileName(zigFileName));

            using (var zip = ZipFile.Open(TempDirectory + System.IO.Path.GetFileName(zigFileName), ZipArchiveMode.Create))
                zip.CreateEntryFromFile(sigFileName, System.IO.Path.GetFileName(sigFileName));

            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, @"FileXferPut """ + TempDirectory + System.IO.Path.GetFileName(zigFileName) + @""" ""\SIMPL\" + System.IO.Path.GetFileName(zigFileName) + @""" ",
                5000, ref curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 6 - Wait for System to Load

        CrestronSignal startupBusySignal;
        bool firstReboot = true;

        public void WaitForSystemToLoad()
        {
            if (firstReboot)
            {
                CurStep = DeploymentStep.WaitForSystemToLoad;
                Log("---Waiting for system to load - 2 minutes---");
                System.Threading.Thread.Sleep(120000);
                firstReboot = false;
            }
            else
            {
                Log("---Waiting for system to load - 30 seconds---");
                System.Threading.Thread.Sleep(30000);
            }
            Log("---Finding System_busy signal---");
            startupBusySignal = signalListAfterDeployment.Find(one => one.SignalName == "System_busy");
            int curAPITransactionID = 0;
            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + startupBusySignal.SignalIndex, 10000, ref curAPITransactionID, 0, 0);
            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 7 - Get Current Signals after Load

        public void GetCurrentSignals2()
        {
            CurStep = DeploymentStep.ParseNewSigAndGetCurrentSignals2;

            Log("---------------");
            Log("Getting current signals from DMPS (again)....");

            TelnetGetDMPSSignals sigFinder = new TelnetGetDMPSSignals();
            sigFinder.OnLog += SigFinder_OnLog1;
            sigFinder.OnComplete += SigFinder_OnComplete1;

            sigFinder.StartGetDMPSSignals(DMPSIPAddress);
            //NextNewSignalIndex = -1;
            //GetNextNewSignal();                        
        }

        private void SigFinder_OnComplete1(object sender, List<CrestronSignal> e)
        {
            if (e == null)
            {
                //nothing returned
                Log("No signals returned");
                GetCurrentSignals2();
            }
            else
            {
                foreach (var signal in e)
                {
                    var sig = signalListAfterDeployment.Find(one => one.SignalIndex == signal.SignalIndex);
                    if (sig != null)
                    {
                        sig.SignalValue = signal.SignalValue;
                        sig.HasSignalValue = true;
                    }
                }

                //move on to next step
                int count = signalListAfterDeployment.Count(one => one.HasSignalValue);
                int possibleCount = signalListAfterDeployment.Count(one => !IgnoreSignalName(one.SignalName));
                Log("New Signals complete - " + count + " out of " + possibleCount + " retrieved current values");

                var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterDeployment.ToArray());

                if (System.IO.File.Exists(TempDirectory + "NewSignalsComplete.json"))
                    System.IO.File.Delete(TempDirectory + "NewSignalsComplete.json");

                System.IO.File.WriteAllText(TempDirectory + "NewSignalsComplete.json", signalListJson);

                if (StopProcessing) return;

                SetSignals();
            }
        }

        private void SigFinder_OnLog1(object sender, string e)
        {
            Log(e);
        }

        /*int NextNewSignalIndex = -1;

        public bool IgnoreSignalName(string SignalName)
        {
            return SignalName.StartsWith("::") ||
                    SignalName.StartsWith("//") ||
                    (SignalName.StartsWith("DMPS-300-C._") && !SignalName.Contains("Mic") && !SignalName.Contains("Mute") && !SignalName.Contains("Vol"));
        }

        public void GetNextNewSignal()
        {
            while (true)
            {
                NextNewSignalIndex++;

                if (NextNewSignalIndex >= signalListAfterDeployment.Count)
                {
                    //move on to next step
                    int count = signalListAfterDeployment.Count(one => one.HasSignalValue);
                    int possibleCount = signalListAfterDeployment.Count(one => !IgnoreSignalName(one.SignalName));
                    Log("New Signals complete - " + count + " out of " + possibleCount + " retrieved current values");

                    var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterDeployment.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "NewSignalsComplete.json"))
                        System.IO.File.Delete(TempDirectory + "NewSignalsComplete.json");

                    System.IO.File.WriteAllText(TempDirectory + "NewSignalsComplete.json", signalListJson);

                    if (StopProcessing) return;
                    SetSignals();

                    return;
                }

                if (IgnoreSignalName(signalListAfterDeployment[NextNewSignalIndex].SignalName))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }


            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + signalListAfterDeployment[NextNewSignalIndex].SignalIndex, 10000, ref curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;
        }*/

        #endregion

        #region Step 8 - Set Signals

        public class SignalToSet
        {
            public string CommandToSet { get; set; }
            public string CommandToGet { get; set; }
            public CrestronSignal Signal { get; set; }
            public bool IsConfirmed { get; set; }
            public string ExpectedValue { get; set; }
        }
        List<SignalToSet> signalsToSet = new List<SignalToSet>();
        int signalIndexToSet = -1;

        public void SetSignals()
        {
            //see which ones are different from what they used to be and set them           
            signalsToSet = new List<SignalToSet>();
            signalIndexToSet = -1;
            foreach (var oldSignal in signalList)
            {
                if (IgnoreSignalName(oldSignal.SignalName))
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
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetDigital " + newSignal.SignalIndex + ", " + oldSignal.SignalValue,
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = oldSignal.SignalValue
                            });
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetAnalog " + newSignal.SignalIndex + ", " + oldSignal.SignalValue,
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = oldSignal.SignalValue
                            });
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetSerial " + newSignal.SignalIndex + ", \"" + oldSignal.SignalValue + "\"",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = oldSignal.SignalValue
                            });
                    }
                    else if (!oldSignal.HasSignalValue && newSignal.HasSignalValue)
                    {
                        //reset this one to default
                        if (newSignal.SignalType == 0)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetDigital " + newSignal.SignalIndex + ", 0",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = "0"
                            });
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetAnalog " + newSignal.SignalIndex + ", 0",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = "0"
                            });
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetSerial " + newSignal.SignalIndex + ", \"\"",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = ""
                            });
                    }
                }
            }

            foreach (var newSignal in signalListAfterDeployment)
            {
                if (IgnoreSignalName(newSignal.SignalName))
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
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetDigital " + newSignal.SignalIndex + ", 0",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = "0"
                            });
                        else if (newSignal.SignalType == 1)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetAnalog " + newSignal.SignalIndex + ", 0",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = "0"
                            });
                        else if (newSignal.SignalType == 2)
                            signalsToSet.Add(new SignalToSet()
                            {
                                CommandToSet = "SignalDbgSetSerial " + newSignal.SignalIndex + ", \"\"",
                                Signal = newSignal,
                                IsConfirmed = false,
                                CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                                ExpectedValue = ""
                            });
                    }
                }

                if (newSignal.SignalName == "ReportToHost")
                {
                    var x = signalsToSet.Find(one => one.Signal.SignalIndex == newSignal.SignalIndex);
                    if (x == null)
                    {
                        x = new SignalToSet()
                        {
                            CommandToSet = "SignalDbgSetSerial " + newSignal.SignalIndex + ", \"sandbag.byu.edu\"",
                            Signal = newSignal,
                            IsConfirmed = false,
                            CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                            ExpectedValue = "sandbag.byu.edu"
                        };
                        signalsToSet.Add(x);
                    }

                    if (x.ExpectedValue != "sandbag.byu.edu")
                    {
                        x.CommandToSet = "SignalDbgSetSerial " + newSignal.SignalIndex + ", \"sandbag.byu.edu\"";
                        x.ExpectedValue = "sandbag.byu.edu";
                    }
                }

                if (newSignal.SignalName == "ReportToHostPort")
                {
                    var x = signalsToSet.Find(one => one.Signal.SignalIndex == newSignal.SignalIndex);
                    if (x == null)
                    {
                        x = new SignalToSet()
                        {
                            CommandToSet = "SignalDbgSetAnalog " + newSignal.SignalIndex + ", 10010",
                            Signal = newSignal,
                            IsConfirmed = false,
                            CommandToGet = "SignalDbgStatus " + newSignal.SignalIndex,
                            ExpectedValue = "10010"
                        };
                        signalsToSet.Add(x);
                    }

                    if (x.ExpectedValue != "sandbag.byu.edu")
                    {
                        x.CommandToSet = "SignalDbgSetAnalog " + newSignal.SignalIndex + ", 10010";
                        x.ExpectedValue = "10010";
                    }
                }
            }

            System.IO.File.WriteAllLines(TempDirectory + @"NeedToOutput.txt",
                signalsToSet.Select(one => one.CommandToSet + "|" + one.Signal.SignalName + " (index " + one.Signal.SignalIndex + ")"));

            string output = "";
            Log("Entering Debug mode...");
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);

            SetNextSignal();
        }

        bool CheckSetSignal = true;

        public void SetNextSignal()
        {
            CurStep = DeploymentStep.SetSignals;

            if (signalIndexToSet >= signalsToSet.Count - 1)
            {
                //next step
                if (StopProcessing) return;
                SaveAndReboot();
            }
            else
            {
                signalIndexToSet++;                
                var command = signalsToSet[signalIndexToSet];

                //set it up so we know to check it
                CheckSetSignal = true;

                Log("Setting " + command.Signal.SignalName + " [" + command.CommandToSet + "] - index [" + signalIndexToSet + "] out of [" + signalsToSet.Count + "]");
                int curAPITransactionID = 0;
                vptSessionClass.AsyncActivateStr(0, command.CommandToSet, 10000, ref curAPITransactionID, 0, 0);
                TxnIDs[CurStep] = curAPITransactionID;
            }
        }

        public void CheckSignalJustSet()
        {
            var command = signalsToSet[signalIndexToSet];

            CheckSetSignal = false;

            Log("Checking " + command.Signal.SignalName + " [" + command.CommandToGet + "]");
            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, command.CommandToGet, 10000, ref curAPITransactionID, 0, 0);
            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 9 - Save and Reboot

        public void SaveAndReboot()
        {
            CurStep = DeploymentStep.SaveAndReboot;
            Log("---------------");
            Log("Saving and rebooting....");

            var signal = signalListAfterDeployment.Find(one => one.SignalName == "Store_Settings");

            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgPulseDigital " + signal.SignalIndex + ", 50", ref output, 10000, 0);

            if (isRepush)
            {
                Log("Signals have been re-pushed!");

                if (OnDMPSDeployedSuccessfully != null)
                {
                    OnDMPSDeployedSuccessfully("DMPS " + DMPSHostName + " / " + DMPSIPAddress + " Signals Re-pushed!");
                }

                logForm.CloseForm();

                return;
            }

            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "ProgramRestart", 10000, ref curAPITransactionID, 0, 0);
            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 10 - Wait for System to Load again

        bool secondTimeFirstReboot = true;

        public void WaitForSystemToLoad2()
        {            
            if (secondTimeFirstReboot)
            {
                CurStep = DeploymentStep.WaitForSystemToLoad2;
                Log("---Waiting for system to load - 2 minutes---");
                System.Threading.Thread.Sleep(120000);
                secondTimeFirstReboot = false;
            }
            else
            {
                Log("---Waiting for system to load - 30 seconds---");
                System.Threading.Thread.Sleep(30000);
            }
            
            Log("---Finding System_busy signal---");
            startupBusySignal = signalListAfterDeployment.Find(one => one.SignalName == "System_busy");
            int curAPITransactionID = 0;
            string output = "";
            vptSessionClass.SyncActivateStr(0, "SignalDbgEnterDbgMode", ref output, 10000, 0);
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + startupBusySignal.SignalIndex, 10000, ref curAPITransactionID, 0, 0);
            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 11 - Get Signals After Set

        public void GetSignalsAfterSet()
        {           
            //copy the list for the redo
            signalListAfterSet = new List<CrestronSignal>();
            signalListAfterDeployment.ForEach(one => signalListAfterSet.Add(
                new CrestronSignal()
                {
                    HasSignalValue = false,
                    SignalFlags = one.SignalFlags,
                    SignalIndex = one.SignalIndex,
                    SignalName = one.SignalName,
                    SignalType = one.SignalType
                }));

            CurStep = DeploymentStep.GetSignalsAfterSet;

            Log("---------------");
            Log("Getting current signals from DMPS (for the third time, just to double check)....");

            /*NextNewSignalAfterSetIndex = -1;
            GetNextNewSignalAfterSet();*/

            TelnetGetDMPSSignals sigFinder = new TelnetGetDMPSSignals();
            sigFinder.OnLog += SigFinder_OnLog2;
            sigFinder.OnComplete += SigFinder_OnComplete2;

            sigFinder.StartGetDMPSSignals(DMPSIPAddress);
        }

        private void SigFinder_OnComplete2(object sender, List<CrestronSignal> e)
        {
            if (e == null)
            {
                Log("No signals returned");
                if (StopProcessing) return;
                GetSignalsAfterSet();
            }
            else
            {
                //fill it up
                foreach (var signal in e)
                {
                    var sig = signalListAfterSet.Find(one => one.SignalIndex == signal.SignalIndex);
                    if (sig != null)
                    {
                        sig.HasSignalValue = true;
                        sig.SignalValue = signal.SignalValue;
                    }
                }

                //check
                List<CrestronSignal> signalsNotSetCorrectly = new List<CrestronSignal>();
                foreach (var oldSignal in signalList)
                {
                    if (!IgnoreSignalName(oldSignal.SignalName))
                    {
                        var newSignalAfterSet = signalListAfterSet.Find(one => one.SignalName == oldSignal.SignalName);
                        if (newSignalAfterSet != null && (!newSignalAfterSet.HasSignalValue || newSignalAfterSet.SignalValue != oldSignal.SignalValue))
                        {
                            //PROBLEM THIS IS A PROBLEM
                            signalsNotSetCorrectly.Add(newSignalAfterSet);
                        }
                    }
                }

                if (signalsNotSetCorrectly.Count > 0)
                {
                    var signalListNotCorrectJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalsNotSetCorrectly.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json"))
                        System.IO.File.Delete(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json");

                    System.IO.File.WriteAllText(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json", signalListNotCorrectJson);
                }

                //move on to next step
                int count = signalListAfterSet.Count(one => one.HasSignalValue);
                int possibleCount = signalListAfterSet.Count(one => !IgnoreSignalName(one.SignalName));
                Log("New Signals after set complete - " + count + " out of " + possibleCount + " retrieved current values");

                var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterSet.ToArray());

                if (System.IO.File.Exists(TempDirectory + "NewSignalsAfterSetComplete.json"))
                    System.IO.File.Delete(TempDirectory + "NewSignalsAfterSetComplete.json");

                System.IO.File.WriteAllText(TempDirectory + "NewSignalsAfterSetComplete.json", signalListJson);

                if (StopProcessing) return;
                GetDevicesFromDMPS();
            }
        }

        private void SigFinder_OnLog2(object sender, string e)
        {
            Log(e);
        }

        //int NextNewSignalAfterSetIndex = -1;
        /*
        public void GetNextNewSignalAfterSet()
        {
            while (true)
            {
                NextNewSignalAfterSetIndex++;

                if (NextNewSignalAfterSetIndex >= signalListAfterSet.Count)
                {
                    //do a check and see if we have signals that didn't set right
                    List<CrestronSignal> signalsNotSetCorrectly = new List<CrestronSignal>();
                    foreach (var oldSignal in signalList)
                    {
                        if (!IgnoreSignalName(oldSignal.SignalName))
                        {
                            var newSignalAfterSet = signalListAfterSet.Find(one => one.SignalName == oldSignal.SignalName);
                            if (newSignalAfterSet != null && (!newSignalAfterSet.HasSignalValue || newSignalAfterSet.SignalValue != oldSignal.SignalValue))
                            {
                                //PROBLEM THIS IS A PROBLEM
                                signalsNotSetCorrectly.Add(newSignalAfterSet);
                            }
                        }
                    }

                    if (signalsNotSetCorrectly.Count > 0)
                    {
                        var signalListNotCorrectJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalsNotSetCorrectly.ToArray());

                        if (System.IO.File.Exists(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json"))
                            System.IO.File.Delete(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json");

                        System.IO.File.WriteAllText(TempDirectory + "ProblemSignalsNotCorrectAfterSet.json", signalListNotCorrectJson);
                    }

                    //move on to next step
                    int count = signalListAfterSet.Count(one => one.HasSignalValue);
                    int possibleCount = signalListAfterSet.Count(one => !IgnoreSignalName(one.SignalName));
                    Log("New Signals after set complete - " + count + " out of " + possibleCount + " retrieved current values");

                    var signalListJson = Newtonsoft.Json.JsonConvert.SerializeObject(signalListAfterSet.ToArray());

                    if (System.IO.File.Exists(TempDirectory + "NewSignalsAfterSetComplete.json"))
                        System.IO.File.Delete(TempDirectory + "NewSignalsAfterSetComplete.json");

                    System.IO.File.WriteAllText(TempDirectory + "NewSignalsAfterSetComplete.json", signalListJson);

                    if (StopProcessing) return;
                    GetDevicesFromDMPS();

                    return;
                }

                if (IgnoreSignalName(signalListAfterSet[NextNewSignalAfterSetIndex].SignalName))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "SignalDbgStatus " + signalListAfterSet[NextNewSignalAfterSetIndex].SignalIndex, 10000, ref curAPITransactionID, 0, 0);

            TxnIDs[CurStep] = curAPITransactionID;
        }*/

        #endregion

        #region Step 12 - Get DMPS Devices

        string[] DeviceResultsFromDMPS;
        int deviceToQueryIndex = 0;

        public void GetDevicesFromDMPS()
        {
            CurStep = DeploymentStep.GetDMPSDevices;
            Log("---------------");
            Log("Getting current devices from DMPS....");
            int curAPITransactionID = 0;
            vptSessionClass.AsyncActivateStr(0, "SubNetworkReportDevices 2", 10000, ref curAPITransactionID, 0, 0);
            TxnIDs[CurStep] = curAPITransactionID;
        }

        #endregion

        #region Step 13 - Get UI TSP Addresses
        
        public void GetTSPIPAddresses()
        {
            CurStep = DeploymentStep.GetUITouchPanelAddresses;
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

                int curAPITransactionID = 0;
                vptSessionClass.AsyncActivateStr(0, "SubNetworkReportDevice 2, " + value, 10000, ref curAPITransactionID, 0, 0);
                TxnIDs[CurStep] = curAPITransactionID;

                deviceToQueryIndex = deviceToQueryIndex + 2;
            }
        }

        #endregion

        #region Step 14 - Push VTZ Files

        List<string> vtzIPAddresses = new List<string>();
        int vtzIndex = 0;
        public void SendNewVTZFiles()
        {
            vtzIPAddresses = vtzIPAddresses.Distinct().ToList();

            CurStep = DeploymentStep.PushVtzFiles;
            Log("---------------");
            Log("Pushing VTZ files to " + vtzIPAddresses.Count + " devices....");

            foreach (string vtzAddress in vtzIPAddresses)
            {
                //ftp and push the vtz file
                if (!SendVTZOverFTP(vtzAddress))
                {
                    Log("ERROR!!!!!!!!!  Didn't ftp file");
                    return;
                }

                //telnet and tell it to ProjectLoad
                //wait for it to get done
                TelnetVTZ vtzSender = new TelnetVTZ();
                vtzSender.OnLog += VtzSender_OnLog;
                if (!vtzSender.SendVTZFile(vtzAddress))
                {
                    Log("ERROR!!!!!!!!!  Didn't telnet command");
                    return;
                }
            }

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

        private void VtzSender_OnLog(object sender, string e)
        {
            Log(e);
        }

        public byte[] GetFileData(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                return ASCIIEncoding.ASCII.GetBytes(sr.ReadToEnd());
            }
        }

        public bool SendVTZOverFTP(string ipAddress)
        {
            try
            {
                Log("Sending file via ftp to [" + ipAddress + "]...");

                string url = "ftp://" + ipAddress + "/display/" + Path.GetFileName(NewVTZFileName);
                var ftpWebRequest = WebRequest.Create(url) as FtpWebRequest;
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                //ftpWebRequest.Credentials = new NetworkCredential("something", "something");
                byte[] fileData = System.IO.File.ReadAllBytes(NewVTZFileName);
                System.IO.File.WriteAllBytes(@"C:\temp\test.zip", fileData);
                //byte[] fileData = System.IO.File.ReadAllBytes(@"c:\temp\test.json");
                //byte[] fileData = GetFileData(NewVTZFileName);            
                using (var requestStream = ftpWebRequest.GetRequestStream())
                {
                    requestStream.Write(fileData, 0, fileData.Length);
                }
                var response = ftpWebRequest.GetResponse() as FtpWebResponse;

                Log("Status Description: " + response.StatusDescription);

                return true;
            }
            catch (Exception ex)
            {
                Log("Exception while uploading - " + ex.Message);
                return false;
            }

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ipAddress + "/Display/" + Path.GetFileName(NewVTZFileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Copy the contents of the file to the request stream.
            var ftpStream = request.GetRequestStream();
            //FileStream file = File.OpenRead(NewVTZFileName);
            byte[] test = System.IO.File.ReadAllBytes(NewVTZFileName);

            //int length = 65536;
            //byte[] buffer = new byte[length];
            //int bytesRead = 0;
            //int total = 0;
            //do
            //{
            //    total += length;
            //    Log("Writing " + total + " to ftp stream");
            //    bytesRead = file.Read(buffer, 0, length);
            //    ftpStream.Write(buffer, 0, bytesRead);
            //}
            //while (bytesRead != 0);

            //file.Close();
            ftpStream.Write(test, 0, test.Length);
            ftpStream.Close();

            return true;
        }

        //public void SendNewVTZFiles()
        //{
        //    CurStep = DeploymentStep.PushVtzFiles;
        //    Log("---------------");
        //    Log("Pushing VTZ files to " + vtzIPAddresses.Count + " devices....");

        //    //return;

        //    vtzIndex = 0;

        //    vptSessionClass.CloseSession();

        //    vtzClass.OnSessionReady += VtzClass_OnSessionReady;
        //    vtzClass.OnAsyncActivateStart += VtzClass_OnAsyncActivateStart;
        //    vtzClass.OnFileXferBatchStart += VtzClass_OnFileXferBatchStart;
        //    vtzClass.OnFileXferFileStart += VtzClass_OnFileXferFileStart;
        //    vtzClass.OnFileXferFileProgress += VtzClass_OnFileXferFileProgress;
        //    vtzClass.OnFileXferFileFinish += VtzClass_OnFileXferFileFinish;
        //    vtzClass.OnFileXferBatchFinish += VtzClass_OnFileXferBatchFinish;
        //    vtzClass.OnActivateStrComplete += VtzClass_OnActivateStrComplete;

        //    SendNextVTZFile();
        //}

        //private void VtzClass_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        //{
        //    Log("\tVTZ On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
        //       "bSuccess:[" + bSuccess + "]");

        //    SendNextVTZFile();
        //}

        //private void VtzClass_OnFileXferBatchFinish(int nTransactionID, byte bSuccess)
        //{
        //    Log("\tVTZ On File Xfer Batch Finish - nTransactionID:[" + nTransactionID + "], " +
        //       "bSuccess:[" + bSuccess + "]");                        
        //}

        //private void VtzClass_OnFileXferFileFinish(int nTransactionID, byte bSuccess)
        //{
        //    Log("\tVTZ On File Xfer File Finish - nTransactionID:[" + nTransactionID + "], " +
        //       "bSuccess:[" + bSuccess + "]");
        //}

        //private void VtzClass_OnFileXferFileProgress(int nTransactionID, int nFileBytesTransferred, int nTotalBytesTransferred, int nBytesPerSecond, int nEstTotalTimeRemaining)
        //{
        //    Log("\tVTZ On File Xfer File Progress - nTransactionID:[" + nTransactionID + "], " +
        //         "nFileBytesTransferred:[" + nFileBytesTransferred + "], " +
        //         "nTotalBytesTransferred:[" + nTotalBytesTransferred + "], nBytesPerSecond:[" + nBytesPerSecond + "], " +
        //         "nEstTotalTimeRemaining:[" + nEstTotalTimeRemaining + "]");
        //}

        //private void VtzClass_OnFileXferBatchStart(int nTransactionID, string pszwDescription, int nTotalFiles, int nTotalSize)
        //{
        //    Log("\tVTZ On File Xfer Batch Start - nTransactionID:[" + nTransactionID + "], " +
        //       "pszwDescription:[" + pszwDescription + "], " +
        //       "nTotalFiles:[" + nTotalFiles + "], nTotalSize:[" + nTotalSize + "]");
        //}

        //private void VtzClass_OnFileXferFileStart(int nTransactionID, string pszwLocalFilename, string pszwRemoteFilename, int nSize, byte bSending, string pszwProtocolName)
        //{
        //    Log("\tVTZ On File Xfer File Start - nTransactionID:[" + nTransactionID + "], " +
        //       "pszwLocalFilename:[" + pszwLocalFilename + "], " +
        //       "pszwRemoteFilename:[" + pszwRemoteFilename + "], nSize:[" + nSize + "], " +
        //       "bSending:[" + bSending + "], pszwProtocolName :[" + pszwProtocolName + "]");
        //}

        //private void VtzClass_OnAsyncActivateStart(int nTransactionID, int nAbilityCode, int nUserPassBack)
        //{
        //    Log("VTZ Session starting nTransactionID:[" + nTransactionID + "], nAbilityCode:[" + nAbilityCode + "], nUserPassBack:[" + nUserPassBack + "]");
        //}

        //private void VtzClass_OnSessionReady()
        //{
        //    int curAPITransactionID = 0;
        //    vtzClass.AsyncActivateStr(0, "DisplayListSend \"" + NewVTZFileName + "\"", 10000, ref curAPITransactionID, 0, 0);
        //}

        //VPTCOMSERVERLib.VptSessionClass vtzClass = new VPTCOMSERVERLib.VptSessionClass();        

        //public void SendNextVTZFile()
        //{
        //    if (vtzIndex == vtzIPAddresses.Count)
        //    {
        //        //all done
        //        if (System.IO.File.Exists(TempDirectory + "DeploymentComplete.txt"))
        //            System.IO.File.Delete(TempDirectory + "DeploymentComplete.txt");

        //        System.IO.File.WriteAllText(TempDirectory + "DeploymentComplete.txt", "Deployment Complete on " + DateTime.Now.ToString());

        //        if (OnDMPSDeployedSuccessfully != null)
        //        {
        //            OnDMPSDeployedSuccessfully("DMPS " + DMPSHostName + " / " + DMPSIPAddress + " Deployment Complete!");
        //        }

        //        logForm.CloseForm();

        //    }
        //    else
        //    {
        //        string ip = vtzIPAddresses[vtzIndex];
        //        vtzIndex++;
        //        vtzClass.CloseSession();
        //        vtzClass.OpenSession("auto " + ip, ip);
        //        Log("Connection to session on TouchPanel " + ip);
        //        string output = "";
        //        vtzClass.SyncActivateStr(0, "ProductGetInfo", output, 10000, 1);
        //    }
        //}

        #endregion
    }
}