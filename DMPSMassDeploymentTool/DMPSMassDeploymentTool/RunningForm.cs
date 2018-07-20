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
        public void Log(string message)
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
             
            System.IO.File.AppendAllLines(logFileName, new string[] { message });
            logListBox.Items.Add(message);
        }
    }
}
