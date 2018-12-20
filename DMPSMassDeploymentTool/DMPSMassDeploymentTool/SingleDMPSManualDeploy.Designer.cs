namespace DMPSMassDeploymentTool
{
    partial class SingleDMPSManualDeploy
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                
                if (vptSessionClass != null)
                    vptSessionClass.CloseSession();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dmpsHostLabel = new System.Windows.Forms.Label();
            this.dmpsIPLabel = new System.Windows.Forms.Label();
            this.nextStepBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logBox = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.step1ConnectButton = new System.Windows.Forms.Button();
            this.step1NextStepButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.step2CollectSignalsButton = new System.Windows.Forms.Button();
            this.step2NextStepButton = new System.Windows.Forms.Button();
            this.numberOfCurrentSignalsCollectedLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.currentSignalSaveLocationLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.step3PushFile = new System.Windows.Forms.Button();
            this.step3NextStepButton = new System.Windows.Forms.Button();
            this.programFileToBePushedLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.step4NextStepButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.step5NextStepButton = new System.Windows.Forms.Button();
            this.setSignalsProgressBar = new System.Windows.Forms.ProgressBar();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.step6DoneButton = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.errorLabel = new System.Windows.Forms.Label();
            this.runningBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // dmpsHostLabel
            // 
            this.dmpsHostLabel.AutoSize = true;
            this.dmpsHostLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dmpsHostLabel.Location = new System.Drawing.Point(3, 9);
            this.dmpsHostLabel.Name = "dmpsHostLabel";
            this.dmpsHostLabel.Size = new System.Drawing.Size(129, 13);
            this.dmpsHostLabel.TabIndex = 0;
            this.dmpsHostLabel.Text = "DMPS: ITB-1101-CP1";
            // 
            // dmpsIPLabel
            // 
            this.dmpsIPLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmpsIPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dmpsIPLabel.Location = new System.Drawing.Point(212, 9);
            this.dmpsIPLabel.Name = "dmpsIPLabel";
            this.dmpsIPLabel.Size = new System.Drawing.Size(158, 13);
            this.dmpsIPLabel.TabIndex = 1;
            this.dmpsIPLabel.Text = "IP:10.66.36.220";
            this.dmpsIPLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // nextStepBox
            // 
            this.nextStepBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nextStepBox.BackColor = System.Drawing.SystemColors.Window;
            this.nextStepBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nextStepBox.FormattingEnabled = true;
            this.nextStepBox.Items.AddRange(new object[] {
            "1 - Connect to DMPS",
            "2 - Save Current Signals",
            "3 - Push new Program / Signal File",
            "4 - Get After-Deployment Signals",
            "5 - Set Signals to Original",
            "6 - Push Display file to Touch Panels"});
            this.nextStepBox.Location = new System.Drawing.Point(63, 25);
            this.nextStepBox.Name = "nextStepBox";
            this.nextStepBox.Size = new System.Drawing.Size(300, 21);
            this.nextStepBox.TabIndex = 4;
            this.nextStepBox.SelectedIndexChanged += new System.EventHandler(this.nextStepBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Next Step";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.logBox);
            this.groupBox1.Location = new System.Drawing.Point(6, 242);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(364, 216);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.FormattingEnabled = true;
            this.logBox.Location = new System.Drawing.Point(3, 16);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(358, 197);
            this.logBox.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.ItemSize = new System.Drawing.Size(81, 15);
            this.tabControl1.Location = new System.Drawing.Point(6, 52);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(361, 156);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.step1ConnectButton);
            this.tabPage1.Controls.Add(this.step1NextStepButton);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Location = new System.Drawing.Point(4, 19);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(353, 133);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1 - Connect";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // step1ConnectButton
            // 
            this.step1ConnectButton.Location = new System.Drawing.Point(9, 23);
            this.step1ConnectButton.Name = "step1ConnectButton";
            this.step1ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.step1ConnectButton.TabIndex = 2;
            this.step1ConnectButton.Text = "Connect";
            this.step1ConnectButton.UseVisualStyleBackColor = true;
            this.step1ConnectButton.Click += new System.EventHandler(this.step1ConnectButton_Click);
            // 
            // step1NextStepButton
            // 
            this.step1NextStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step1NextStepButton.Enabled = false;
            this.step1NextStepButton.Location = new System.Drawing.Point(272, 104);
            this.step1NextStepButton.Name = "step1NextStepButton";
            this.step1NextStepButton.Size = new System.Drawing.Size(75, 23);
            this.step1NextStepButton.TabIndex = 1;
            this.step1NextStepButton.Text = "Next Step";
            this.step1NextStepButton.UseVisualStyleBackColor = true;
            this.step1NextStepButton.Click += new System.EventHandler(this.step1NextStepButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Test connection to DMPS";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.step2CollectSignalsButton);
            this.tabPage2.Controls.Add(this.step2NextStepButton);
            this.tabPage2.Controls.Add(this.numberOfCurrentSignalsCollectedLabel);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.currentSignalSaveLocationLabel);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 19);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(353, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "2 - Save Signals";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // step2CollectSignalsButton
            // 
            this.step2CollectSignalsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.step2CollectSignalsButton.Location = new System.Drawing.Point(6, 107);
            this.step2CollectSignalsButton.Name = "step2CollectSignalsButton";
            this.step2CollectSignalsButton.Size = new System.Drawing.Size(75, 23);
            this.step2CollectSignalsButton.TabIndex = 5;
            this.step2CollectSignalsButton.Text = "Get Signals";
            this.step2CollectSignalsButton.UseVisualStyleBackColor = true;
            this.step2CollectSignalsButton.Click += new System.EventHandler(this.step2CollectSignalsButton_Click);
            // 
            // step2NextStepButton
            // 
            this.step2NextStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step2NextStepButton.Enabled = false;
            this.step2NextStepButton.Location = new System.Drawing.Point(272, 107);
            this.step2NextStepButton.Name = "step2NextStepButton";
            this.step2NextStepButton.Size = new System.Drawing.Size(75, 23);
            this.step2NextStepButton.TabIndex = 4;
            this.step2NextStepButton.Text = "Next Step";
            this.step2NextStepButton.UseVisualStyleBackColor = true;
            this.step2NextStepButton.Click += new System.EventHandler(this.step2NextStepButton_Click);
            // 
            // numberOfCurrentSignalsCollectedLabel
            // 
            this.numberOfCurrentSignalsCollectedLabel.AutoSize = true;
            this.numberOfCurrentSignalsCollectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberOfCurrentSignalsCollectedLabel.Location = new System.Drawing.Point(6, 77);
            this.numberOfCurrentSignalsCollectedLabel.Name = "numberOfCurrentSignalsCollectedLabel";
            this.numberOfCurrentSignalsCollectedLabel.Size = new System.Drawing.Size(14, 13);
            this.numberOfCurrentSignalsCollectedLabel.TabIndex = 3;
            this.numberOfCurrentSignalsCollectedLabel.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Number of Signals Collected:";
            // 
            // currentSignalSaveLocationLabel
            // 
            this.currentSignalSaveLocationLabel.AutoSize = true;
            this.currentSignalSaveLocationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentSignalSaveLocationLabel.Location = new System.Drawing.Point(6, 28);
            this.currentSignalSaveLocationLabel.Name = "currentSignalSaveLocationLabel";
            this.currentSignalSaveLocationLabel.Size = new System.Drawing.Size(292, 13);
            this.currentSignalSaveLocationLabel.TabIndex = 1;
            this.currentSignalSaveLocationLabel.Text = "c:\\Temp\\DMPS\\10.66.36.220\\CurrentSignals.json";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Signals will be saved to:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.step3PushFile);
            this.tabPage3.Controls.Add(this.step3NextStepButton);
            this.tabPage3.Controls.Add(this.programFileToBePushedLabel);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Location = new System.Drawing.Point(4, 19);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(353, 133);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "3 - Push";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // step3PushFile
            // 
            this.step3PushFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step3PushFile.Enabled = false;
            this.step3PushFile.Location = new System.Drawing.Point(6, 104);
            this.step3PushFile.Name = "step3PushFile";
            this.step3PushFile.Size = new System.Drawing.Size(130, 23);
            this.step3PushFile.TabIndex = 9;
            this.step3PushFile.Text = "Push Program";
            this.step3PushFile.UseVisualStyleBackColor = true;
            this.step3PushFile.Click += new System.EventHandler(this.step3PushFile_Click);
            // 
            // step3NextStepButton
            // 
            this.step3NextStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step3NextStepButton.Enabled = false;
            this.step3NextStepButton.Location = new System.Drawing.Point(272, 104);
            this.step3NextStepButton.Name = "step3NextStepButton";
            this.step3NextStepButton.Size = new System.Drawing.Size(75, 23);
            this.step3NextStepButton.TabIndex = 8;
            this.step3NextStepButton.Text = "Next Step";
            this.step3NextStepButton.UseVisualStyleBackColor = true;
            // 
            // programFileToBePushedLabel
            // 
            this.programFileToBePushedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.programFileToBePushedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.programFileToBePushedLabel.Location = new System.Drawing.Point(6, 32);
            this.programFileToBePushedLabel.Name = "programFileToBePushedLabel";
            this.programFileToBePushedLabel.Size = new System.Drawing.Size(341, 38);
            this.programFileToBePushedLabel.TabIndex = 5;
            this.programFileToBePushedLabel.Text = "c:\\Temp\\DMPS\\10.66.36.220\\CurrentSignals.json";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Program file to be pushed:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.step4NextStepButton);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Location = new System.Drawing.Point(4, 19);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(353, 133);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "4 - Get Signals";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // step4NextStepButton
            // 
            this.step4NextStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step4NextStepButton.Enabled = false;
            this.step4NextStepButton.Location = new System.Drawing.Point(272, 104);
            this.step4NextStepButton.Name = "step4NextStepButton";
            this.step4NextStepButton.Size = new System.Drawing.Size(75, 23);
            this.step4NextStepButton.TabIndex = 8;
            this.step4NextStepButton.Text = "Next Step";
            this.step4NextStepButton.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Number of Signals Collected:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(344, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "c:\\Temp\\DMPS\\10.66.36.220\\AfterDeploymentSignals.json";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Signals will be saved to:";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.step5NextStepButton);
            this.tabPage5.Controls.Add(this.setSignalsProgressBar);
            this.tabPage5.Controls.Add(this.label13);
            this.tabPage5.Controls.Add(this.label14);
            this.tabPage5.Location = new System.Drawing.Point(4, 19);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(353, 133);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "5 - Set Signals";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // step5NextStepButton
            // 
            this.step5NextStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step5NextStepButton.Enabled = false;
            this.step5NextStepButton.Location = new System.Drawing.Point(272, 104);
            this.step5NextStepButton.Name = "step5NextStepButton";
            this.step5NextStepButton.Size = new System.Drawing.Size(75, 23);
            this.step5NextStepButton.TabIndex = 11;
            this.step5NextStepButton.Text = "Next Step";
            this.step5NextStepButton.UseVisualStyleBackColor = true;
            // 
            // setSignalsProgressBar
            // 
            this.setSignalsProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.setSignalsProgressBar.Location = new System.Drawing.Point(9, 63);
            this.setSignalsProgressBar.Name = "setSignalsProgressBar";
            this.setSignalsProgressBar.Size = new System.Drawing.Size(338, 23);
            this.setSignalsProgressBar.TabIndex = 10;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(14, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 13);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(131, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "Number of Signals To Set:";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.step6DoneButton);
            this.tabPage6.Controls.Add(this.label15);
            this.tabPage6.Controls.Add(this.label16);
            this.tabPage6.Controls.Add(this.label17);
            this.tabPage6.Controls.Add(this.label18);
            this.tabPage6.Location = new System.Drawing.Point(4, 19);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(353, 133);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "6 - VTZ";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // step6DoneButton
            // 
            this.step6DoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.step6DoneButton.Enabled = false;
            this.step6DoneButton.Location = new System.Drawing.Point(272, 104);
            this.step6DoneButton.Name = "step6DoneButton";
            this.step6DoneButton.Size = new System.Drawing.Size(75, 23);
            this.step6DoneButton.TabIndex = 12;
            this.step6DoneButton.Text = "Done!";
            this.step6DoneButton.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(3, 100);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(184, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "2 - 10.66.36.222, 10.66.36.223";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 80);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(126, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Touch Panels to push to:";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 33);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(341, 38);
            this.label17.TabIndex = 9;
            this.label17.Text = "c:\\Temp\\DMPS\\Something.vtz";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 13);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(125, 13);
            this.label18.TabIndex = 8;
            this.label18.Text = "Display file to be pushed:";
            // 
            // errorLabel
            // 
            this.errorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.errorLabel.AutoSize = true;
            this.errorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.errorLabel.Location = new System.Drawing.Point(7, 461);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(172, 13);
            this.errorLabel.TabIndex = 9;
            this.errorLabel.Text = "If there is an error, we\'ll write it here";
            // 
            // runningBar
            // 
            this.runningBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runningBar.Location = new System.Drawing.Point(6, 214);
            this.runningBar.Name = "runningBar";
            this.runningBar.Size = new System.Drawing.Size(357, 23);
            this.runningBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.runningBar.TabIndex = 10;
            // 
            // SingleDMPSManualDeploy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.runningBar);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nextStepBox);
            this.Controls.Add(this.dmpsIPLabel);
            this.Controls.Add(this.dmpsHostLabel);
            this.Name = "SingleDMPSManualDeploy";
            this.Size = new System.Drawing.Size(375, 485);
            this.Load += new System.EventHandler(this.SingleDMPSManualDeploy_Load);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label dmpsHostLabel;
        private System.Windows.Forms.Label dmpsIPLabel;
        private System.Windows.Forms.ComboBox nextStepBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label numberOfCurrentSignalsCollectedLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label currentSignalSaveLocationLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label programFileToBePushedLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ProgressBar setSignalsProgressBar;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button step1NextStepButton;
        private System.Windows.Forms.Button step2NextStepButton;
        private System.Windows.Forms.Button step3NextStepButton;
        private System.Windows.Forms.Button step4NextStepButton;
        private System.Windows.Forms.Button step5NextStepButton;
        private System.Windows.Forms.Button step6DoneButton;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Button step1ConnectButton;
        private System.Windows.Forms.Button step2CollectSignalsButton;
        private System.Windows.Forms.ListBox logBox;
        private System.Windows.Forms.ProgressBar runningBar;
        private System.Windows.Forms.Button step3PushFile;
    }
}
