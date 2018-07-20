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

namespace DMPSMassDeploymentTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void deployToDMPSButton_Click(object sender, EventArgs e)
        {
            string confirm =
                $"Are you sure you want to deploy?  This will premamently affect the code for {dmpsList.CheckedItems.Count} units.";

            if (MessageBox.Show(confirm, "Confirm?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (MessageBox.Show("Last Chance.....are you really, really sure???", "Really sure?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    List<DMPSAddress> ToUpdateList = new List<DMPSAddress>();
                    foreach (var checkedItem in dmpsList.CheckedItems)
                        ToUpdateList.Add(checkedItem as DMPSAddress);

                    worker = new BackgroundWorker();
                    worker.DoWork += Worker_DoWork;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.WorkerReportsProgress = true;
                    worker.ProgressChanged += Worker_ProgressChanged;

                    worker.RunWorkerAsync(ToUpdateList);

                    runnerForm = new RunningForm();
                    runnerForm.ShowDialog();
                }
            }
        }

        RunningForm runnerForm;
        BackgroundWorker worker;

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            runnerForm.Log(e.UserState.ToString());
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runnerForm.Close();
            MessageBox.Show("Deployment Completed!\n" + e.Result.ToString() + "\nSee output log for details.");
        }
        
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string error = "";

            VPTCOMSERVERLib.VptSessionClass vptSessionClass = new VPTCOMSERVERLib.VptSessionClass();
            try
            {
                vptSessionClass.OpenSession("auto 10.6.36.220", "10.6.36.220");
                vptSessionClass.OnActivateStrComplete += VptSessionClass_OnActivateStrComplete;
                string outputs = "";
                vptSessionClass.SyncActivateStr(0, "ReportHardware", ref outputs, 5000, 0);

                int a = 1;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                vptSessionClass.CloseSession();
            }
            
            e.Result = error;
        }

        private void VptSessionClass_OnActivateStrComplete(
            int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            int a = 1;
        }
    }
}
