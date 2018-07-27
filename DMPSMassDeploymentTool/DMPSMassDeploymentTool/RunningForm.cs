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
    }
}
