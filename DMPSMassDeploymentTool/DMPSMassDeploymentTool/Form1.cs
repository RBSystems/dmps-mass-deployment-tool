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
                DeployDMPS deploy = new DeployDMPS(ToUpdateList[CurUpdateIndex].hostName, ToUpdateList[CurUpdateIndex].ipAddress, spzFileLocation.Text, vtzFileLocation.Text);
                deploy.OnDMPSDeployedSuccessfully += Deploy_OnDMPSDeployedSuccessfully;
                deploy.OnDMPSDeploymentFatalError += Deploy_OnDMPSDeploymentFatalError;
                deploy.StartDeployment();
            }
        }

        private void Deploy_OnDMPSDeploymentFatalError(string message)
        {
            MessageBox.Show("Error! - " + message);

            if (MessageBox.Show("Do you want to continue", "Continue?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                UpdateNextDMPS();
        }

        private void Deploy_OnDMPSDeployedSuccessfully(string message)
        {
            UpdateNextDMPS();
        }

        RunningForm runnerForm;

        VPTCOMSERVERLib.VptSessionClass cls;
        private void button1_Click(object sender, EventArgs e)
        {
            cls = new VPTCOMSERVERLib.VptSessionClass();
            cls.OnSessionReady += Cls_OnSessionReady;
            cls.OnTaskProgress += Cls_OnTaskProgress;
            cls.OnTaskComplete += Cls_OnTaskComplete;
            cls.OnActivateStrComplete += Cls_OnActivateStrComplete;
            cls.OpenSession("auto 10.66.9.110", "10.66.9.110");
        }

        private void Cls_OnTaskComplete(int nTransactionID, byte bSuccess)
        {
            int x = 1;
        }

        private void Cls_OnActivateStrComplete(int nTransactionID, int nAbilityCode, byte bSuccess, string pszOutputs, int nUserPassBack)
        {
            
            int x = 1;
        }

        private void Cls_OnSessionReady()
        {
            int nTxnID = 0;
            cls.AsyncActivateStr(0, @"SubNetworkReportDevice 2, 13", 10000, ref nTxnID, 0, 0);
        }

        private void Cls_OnTaskProgress(int nTransactionID, int nPercentageDone, string pszwProgressDescription, int lParam)
        {
            int a = 1;
        }
        

    }
}
