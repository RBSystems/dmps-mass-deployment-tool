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
using System.Threading;

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
            //get the old ones
            string elkURL = ConfigurationManager.AppSettings["ELKURL"];
            string elkSystemsQueryFileName = ConfigurationManager.AppSettings["ELKSystemsQueryFileName"];

            string result = PostJsonGetData(elkURL, System.IO.File.ReadAllText(elkSystemsQueryFileName));
            
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
                        ipAddress = ip,
                        source = "O"
                    });
                }
            }


            //get the new ones
            string elkURL2 = ConfigurationManager.AppSettings["ELKURL2"];
            string elkSystemsQueryFileName2 = ConfigurationManager.AppSettings["ELKSystemsQueryFileName2"];
            string elkSystemsQueryFileName3 = ConfigurationManager.AppSettings["ELKSystemsQueryFileName3"];

            string result2 = PostJsonGetData(elkURL2, System.IO.File.ReadAllText(elkSystemsQueryFileName2));

            var jObject2 = JObject.Parse(result2);

            var deviceList2 = jObject2.SelectTokens("$.aggregations.devices.buckets..key").ToList().Select(one => one.Value<string>()).ToList();
            var deviceData2 = jObject2.SelectTokens("$.hits.hits.._source.data").ToList().Select(one => one.Value<string>()).ToList();

            foreach (var device in deviceList2)
            {
                var data = deviceData2.Find(one => one.StartsWith("EVENT~" + device));
                if (data == null)
                {                    
                    string result3 = PostJsonGetData(elkURL2, System.IO.File.ReadAllText(elkSystemsQueryFileName3).Replace("$HOSTNAME$", device));
                    var jObject3 = JObject.Parse(result3);
                    data = jObject3.SelectToken("$.hits.hits.._source.data").Value<string>();
                }

                innerDMPSList.Add(new DMPSAddress()
                {
                    hostName = device,
                    ipAddress = data.Split(new char[] { '~' })[2],
                    source = "N"
                });
            }

            var stringList = innerDMPSList.Select(one => one.hostName + "," + one.ipAddress + "," + one.source);
            System.IO.File.WriteAllLines(@"C:\temp\DMPS\DMPSList.txt", stringList);
            
            FilterDMPSList();
        }

        private string PostJsonGetData(string url, string json)
        {
            var httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest2.ContentType = "application/json";
            httpWebRequest2.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest2.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
            string result2 = "";
            using (var streamReader = new StreamReader(httpResponse2.GetResponseStream()))
            {
                result2 = streamReader.ReadToEnd();
            }

            return result2;
        }

        private void FilterDMPSList()
        {
            var queryResults =
                innerDMPSList.FindAll(one => 
                    one.hostName.ToLower().Contains(filterText.Text.ToLower()) || 
                    one.ipAddress.ToLower().Contains(filterText.Text.ToLower()));

            queryResults.Sort((a, b) => a.hostName.CompareTo(b.hostName));

            dmpsList.Items.Clear();
            queryResults.ForEach(one => dmpsList.Items.Add(one));
        }

        List<DMPSAddress> innerDMPSList = new List<DMPSAddress>();

        private class DMPSAddress
        {
            public string hostName { get; set; }
            public string ipAddress { get; set; }
            public string source { get; set; }
            public override string ToString()
            {
                return hostName + " (" + ipAddress + ") [" + source + "]";
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
                
        int CurUpdateIndex = 0;

        private void deployToDMPSButton_Click(object sender, EventArgs e)
        {
            string confirm =
                $"Are you sure you want to deploy?  This will premamently affect the code for {dmpsList.CheckedItems.Count} units.";

            if (MessageBox.Show(confirm, "Confirm?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (MessageBox.Show("Last Chance.....are you really, really sure???", "Really sure?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    var count = dmpsList.CheckedItems.Count;
                    var countPerRow = (int)Math.Sqrt(count);
                    var workingArea = Screen.GetWorkingArea(this);
                    var width = workingArea.Width / countPerRow;
                    
                    var countPerColumn = (double)(count / countPerRow) == ((double)count / (double)countPerRow) ? (count / countPerRow) : (count / countPerRow) + 1;
                    var height = workingArea.Height / countPerColumn;

                    var CurPoint = new Point(workingArea.Left, workingArea.Top);
                    int curRowCount = 0;

                    foreach (var checkedItem in dmpsList.CheckedItems)
                    {
                        var thisDMPS = checkedItem as DMPSAddress;

                        //SingleDMPSManualDeploy dep = new SingleDMPSManualDeploy();
                        //dep.DMPSHost = thisDMPS.hostName;
                        //dep.DMPSIPAddress = thisDMPS.ipAddress;
                        //dep.SPZFileLocation = spzFileLocation.Text;
                        //dep.VTZFileLocation = vtzFileLocation.Text;
                        //ResizeableControl rc = new ResizeableControl(dep);
                        //flowLayoutPanel1.Controls.Add(dep);                        

                        DeployDMPS deploy = new DeployDMPS(thisDMPS.hostName, thisDMPS.ipAddress, spzFileLocation.Text, vtzFileLocation.Text);
                        deploy.OnDMPSDeployedSuccessfully += Deploy_OnDMPSDeployedSuccessfully;
                        deploy.OnDMPSDeploymentFatalError += Deploy_OnDMPSDeploymentFatalError;
                        deploy.StartDeployment(this, new Size(width, height), CurPoint);

                        curRowCount++;
                        if (curRowCount == countPerRow)
                        {
                            curRowCount = 0;
                            CurPoint.X = workingArea.Left;
                            CurPoint.Y = CurPoint.Y + height;
                        }
                        else
                        {
                            CurPoint.X = CurPoint.X + width;
                        }
                    }

                    //tabControl1.SelectedIndex = 1;
                }
            }
        }

        private void Deploy_OnDMPSDeploymentFatalError(string message)
        {
            MessageBox.Show("Error! - " + message);

            //if (MessageBox.Show("Do you want to continue", "Continue?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                //UpdateNextDMPS();
        }

        private void Deploy_OnDMPSDeployedSuccessfully(string message)
        {
            //UpdateNextDMPS();
            MessageBox.Show("Done! - " + message);
        }

        private void repushSignalsButton_Click(object sender, EventArgs e)
        {
            string confirm =
                $"Are you sure you want to deploy?  This will premamently affect the code for {dmpsList.CheckedItems.Count} units.";

            if (MessageBox.Show(confirm, "Confirm?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (MessageBox.Show("Last Chance.....are you really, really sure???", "Really sure?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    var count = dmpsList.CheckedItems.Count;
                    var countPerRow = (int)Math.Sqrt(count);
                    var workingArea = Screen.GetWorkingArea(this);
                    var width = workingArea.Width / countPerRow;
                    var countPerColumn = (count / countPerRow) + 1;
                    var height = workingArea.Height / countPerColumn;

                    var CurPoint = new Point(workingArea.Left, workingArea.Top);
                    int curRowCount = 0;

                    foreach (var checkedItem in dmpsList.CheckedItems)
                    {
                        var thisDMPS = checkedItem as DMPSAddress;     

                        DeployDMPS deploy = new DeployDMPS(thisDMPS.hostName, thisDMPS.ipAddress, spzFileLocation.Text, vtzFileLocation.Text);
                        deploy.OnDMPSDeployedSuccessfully += Deploy_OnDMPSDeployedSuccessfully;
                        deploy.OnDMPSDeploymentFatalError += Deploy_OnDMPSDeploymentFatalError;
                        deploy.RepushSignals(this, new Size(width, height), CurPoint);

                        curRowCount++;
                        if (curRowCount == countPerRow)
                        {
                            curRowCount = 0;
                            CurPoint.X = workingArea.Left;
                            CurPoint.Y = CurPoint.Y + height;
                        }
                        else
                        {
                            CurPoint.X = CurPoint.X + width;
                        }
                    }
                }
            }
        }
    }
}
