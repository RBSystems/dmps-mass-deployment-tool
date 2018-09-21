namespace DMPSMassDeploymentTool
{
    partial class RunningForm
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
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.currentStatus = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.step1ButtonConnect = new System.Windows.Forms.Button();
            this.step2ButtonRetrieveZIG = new System.Windows.Forms.Button();
            this.step3ButtonParseSIG = new System.Windows.Forms.Button();
            this.step4ButtonGetSignals = new System.Windows.Forms.Button();
            this.step5ButtonPushSPZ = new System.Windows.Forms.Button();
            this.step6ButtonPushZig = new System.Windows.Forms.Button();
            this.step7ButtonGetSignals = new System.Windows.Forms.Button();
            this.step8ButtonSetSignals = new System.Windows.Forms.Button();
            this.step9ButtonGetDMPSDevices = new System.Windows.Forms.Button();
            this.step10ButtonGetTSPAddresses = new System.Windows.Forms.Button();
            this.step11ButtonPushVTZFiles = new System.Windows.Forms.Button();
            this.targetDMPS = new System.Windows.Forms.Label();
            this.stopProcessingButton = new System.Windows.Forms.Button();
            this.step85SaveAndRebootButton = new System.Windows.Forms.Button();
            this.step65WaitForSystemToLoadButton = new System.Windows.Forms.Button();
            this.nudgeButton = new System.Windows.Forms.Button();
            this.step86WaitForSystemToLoad = new System.Windows.Forms.Button();
            this.step87GetSignalsAfterSet = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Status";
            // 
            // currentStatus
            // 
            this.currentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentStatus.Location = new System.Drawing.Point(131, 12);
            this.currentStatus.Multiline = true;
            this.currentStatus.Name = "currentStatus";
            this.currentStatus.ReadOnly = true;
            this.currentStatus.Size = new System.Drawing.Size(1263, 38);
            this.currentStatus.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.logListBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1152, 849);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // logListBox
            // 
            this.logListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 16;
            this.logListBox.Location = new System.Drawing.Point(3, 18);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(1146, 828);
            this.logListBox.TabIndex = 0;
            // 
            // step1ButtonConnect
            // 
            this.step1ButtonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step1ButtonConnect.Location = new System.Drawing.Point(1167, 105);
            this.step1ButtonConnect.Name = "step1ButtonConnect";
            this.step1ButtonConnect.Size = new System.Drawing.Size(230, 31);
            this.step1ButtonConnect.TabIndex = 3;
            this.step1ButtonConnect.Text = "Step 1 - Connect";
            this.step1ButtonConnect.UseVisualStyleBackColor = true;
            this.step1ButtonConnect.Click += new System.EventHandler(this.step1ButtonConnect_Click);
            // 
            // step2ButtonRetrieveZIG
            // 
            this.step2ButtonRetrieveZIG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step2ButtonRetrieveZIG.Location = new System.Drawing.Point(1167, 142);
            this.step2ButtonRetrieveZIG.Name = "step2ButtonRetrieveZIG";
            this.step2ButtonRetrieveZIG.Size = new System.Drawing.Size(230, 31);
            this.step2ButtonRetrieveZIG.TabIndex = 4;
            this.step2ButtonRetrieveZIG.Text = "Step 2 - Retrieve ZIG";
            this.step2ButtonRetrieveZIG.UseVisualStyleBackColor = true;
            this.step2ButtonRetrieveZIG.Click += new System.EventHandler(this.step2ButtonRetrieveZIG_Click);
            // 
            // step3ButtonParseSIG
            // 
            this.step3ButtonParseSIG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step3ButtonParseSIG.Location = new System.Drawing.Point(1167, 179);
            this.step3ButtonParseSIG.Name = "step3ButtonParseSIG";
            this.step3ButtonParseSIG.Size = new System.Drawing.Size(230, 31);
            this.step3ButtonParseSIG.TabIndex = 5;
            this.step3ButtonParseSIG.Text = "Step 3 - Parse SIG";
            this.step3ButtonParseSIG.UseVisualStyleBackColor = true;
            this.step3ButtonParseSIG.Click += new System.EventHandler(this.step3ButtonParseSIG_Click);
            // 
            // step4ButtonGetSignals
            // 
            this.step4ButtonGetSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step4ButtonGetSignals.Location = new System.Drawing.Point(1167, 216);
            this.step4ButtonGetSignals.Name = "step4ButtonGetSignals";
            this.step4ButtonGetSignals.Size = new System.Drawing.Size(230, 31);
            this.step4ButtonGetSignals.TabIndex = 6;
            this.step4ButtonGetSignals.Text = "Step 4 - Get Current Signals";
            this.step4ButtonGetSignals.UseVisualStyleBackColor = true;
            this.step4ButtonGetSignals.Click += new System.EventHandler(this.step4ButtonGetSignals_Click);
            // 
            // step5ButtonPushSPZ
            // 
            this.step5ButtonPushSPZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step5ButtonPushSPZ.Location = new System.Drawing.Point(1167, 288);
            this.step5ButtonPushSPZ.Name = "step5ButtonPushSPZ";
            this.step5ButtonPushSPZ.Size = new System.Drawing.Size(230, 31);
            this.step5ButtonPushSPZ.TabIndex = 7;
            this.step5ButtonPushSPZ.Text = "Step 5 - Push SPZ File";
            this.step5ButtonPushSPZ.UseVisualStyleBackColor = true;
            this.step5ButtonPushSPZ.Click += new System.EventHandler(this.step5ButtonPushSPZ_Click);
            // 
            // step6ButtonPushZig
            // 
            this.step6ButtonPushZig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step6ButtonPushZig.Location = new System.Drawing.Point(1167, 362);
            this.step6ButtonPushZig.Name = "step6ButtonPushZig";
            this.step6ButtonPushZig.Size = new System.Drawing.Size(230, 31);
            this.step6ButtonPushZig.TabIndex = 8;
            this.step6ButtonPushZig.Text = "Step 6 - Push Zig File";
            this.step6ButtonPushZig.UseVisualStyleBackColor = true;
            this.step6ButtonPushZig.Click += new System.EventHandler(this.step6ButtonPushZig_Click);
            // 
            // step7ButtonGetSignals
            // 
            this.step7ButtonGetSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step7ButtonGetSignals.Location = new System.Drawing.Point(1167, 450);
            this.step7ButtonGetSignals.Name = "step7ButtonGetSignals";
            this.step7ButtonGetSignals.Size = new System.Drawing.Size(230, 31);
            this.step7ButtonGetSignals.TabIndex = 9;
            this.step7ButtonGetSignals.Text = "Step 7 - Get new Signals";
            this.step7ButtonGetSignals.UseVisualStyleBackColor = true;
            this.step7ButtonGetSignals.Click += new System.EventHandler(this.step7ButtonGetSignals_Click);
            // 
            // step8ButtonSetSignals
            // 
            this.step8ButtonSetSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step8ButtonSetSignals.Location = new System.Drawing.Point(1167, 527);
            this.step8ButtonSetSignals.Name = "step8ButtonSetSignals";
            this.step8ButtonSetSignals.Size = new System.Drawing.Size(230, 31);
            this.step8ButtonSetSignals.TabIndex = 10;
            this.step8ButtonSetSignals.Text = "Step 8 - Set Signals";
            this.step8ButtonSetSignals.UseVisualStyleBackColor = true;
            this.step8ButtonSetSignals.Click += new System.EventHandler(this.step8ButtonSetSignals_Click);
            // 
            // step9ButtonGetDMPSDevices
            // 
            this.step9ButtonGetDMPSDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step9ButtonGetDMPSDevices.Location = new System.Drawing.Point(1170, 710);
            this.step9ButtonGetDMPSDevices.Name = "step9ButtonGetDMPSDevices";
            this.step9ButtonGetDMPSDevices.Size = new System.Drawing.Size(230, 31);
            this.step9ButtonGetDMPSDevices.TabIndex = 11;
            this.step9ButtonGetDMPSDevices.Text = "Step 9 - Get DMPS Devices";
            this.step9ButtonGetDMPSDevices.UseVisualStyleBackColor = true;
            this.step9ButtonGetDMPSDevices.Click += new System.EventHandler(this.step9ButtonGetDMPSDevices_Click);
            // 
            // step10ButtonGetTSPAddresses
            // 
            this.step10ButtonGetTSPAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step10ButtonGetTSPAddresses.Location = new System.Drawing.Point(1170, 747);
            this.step10ButtonGetTSPAddresses.Name = "step10ButtonGetTSPAddresses";
            this.step10ButtonGetTSPAddresses.Size = new System.Drawing.Size(230, 31);
            this.step10ButtonGetTSPAddresses.TabIndex = 12;
            this.step10ButtonGetTSPAddresses.Text = "Step 10 - Get TSP Addresses";
            this.step10ButtonGetTSPAddresses.UseVisualStyleBackColor = true;
            this.step10ButtonGetTSPAddresses.Click += new System.EventHandler(this.step10ButtonGetTSPAddresses_Click);
            // 
            // step11ButtonPushVTZFiles
            // 
            this.step11ButtonPushVTZFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step11ButtonPushVTZFiles.Location = new System.Drawing.Point(1170, 784);
            this.step11ButtonPushVTZFiles.Name = "step11ButtonPushVTZFiles";
            this.step11ButtonPushVTZFiles.Size = new System.Drawing.Size(230, 31);
            this.step11ButtonPushVTZFiles.TabIndex = 13;
            this.step11ButtonPushVTZFiles.Text = "Step 11 - Push VTZ Files";
            this.step11ButtonPushVTZFiles.UseVisualStyleBackColor = true;
            this.step11ButtonPushVTZFiles.Click += new System.EventHandler(this.step11ButtonPushVTZFiles_Click);
            // 
            // targetDMPS
            // 
            this.targetDMPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetDMPS.Location = new System.Drawing.Point(1167, 56);
            this.targetDMPS.Name = "targetDMPS";
            this.targetDMPS.Size = new System.Drawing.Size(230, 46);
            this.targetDMPS.TabIndex = 14;
            this.targetDMPS.Text = "Target DMPS: 10.6.36.220";
            // 
            // stopProcessingButton
            // 
            this.stopProcessingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stopProcessingButton.Location = new System.Drawing.Point(1167, 871);
            this.stopProcessingButton.Name = "stopProcessingButton";
            this.stopProcessingButton.Size = new System.Drawing.Size(230, 31);
            this.stopProcessingButton.TabIndex = 15;
            this.stopProcessingButton.Text = "Stop Auto Advance";
            this.stopProcessingButton.UseVisualStyleBackColor = true;
            this.stopProcessingButton.Click += new System.EventHandler(this.stopProcessingButton_Click);
            // 
            // step85SaveAndRebootButton
            // 
            this.step85SaveAndRebootButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step85SaveAndRebootButton.Location = new System.Drawing.Point(1167, 564);
            this.step85SaveAndRebootButton.Name = "step85SaveAndRebootButton";
            this.step85SaveAndRebootButton.Size = new System.Drawing.Size(230, 31);
            this.step85SaveAndRebootButton.TabIndex = 16;
            this.step85SaveAndRebootButton.Text = "Step 8.5 - Save and Reboot";
            this.step85SaveAndRebootButton.UseVisualStyleBackColor = true;
            this.step85SaveAndRebootButton.Click += new System.EventHandler(this.step85SaveAndRebootButton_Click);
            // 
            // step65WaitForSystemToLoadButton
            // 
            this.step65WaitForSystemToLoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step65WaitForSystemToLoadButton.Location = new System.Drawing.Point(1167, 325);
            this.step65WaitForSystemToLoadButton.Name = "step65WaitForSystemToLoadButton";
            this.step65WaitForSystemToLoadButton.Size = new System.Drawing.Size(230, 31);
            this.step65WaitForSystemToLoadButton.TabIndex = 17;
            this.step65WaitForSystemToLoadButton.Text = "Step 5.5 - Wait for system to load";
            this.step65WaitForSystemToLoadButton.UseVisualStyleBackColor = true;
            this.step65WaitForSystemToLoadButton.Click += new System.EventHandler(this.step65WaitForSystemToLoadButton_Click);
            // 
            // nudgeButton
            // 
            this.nudgeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nudgeButton.Location = new System.Drawing.Point(1167, 834);
            this.nudgeButton.Name = "nudgeButton";
            this.nudgeButton.Size = new System.Drawing.Size(230, 31);
            this.nudgeButton.TabIndex = 18;
            this.nudgeButton.Text = "Nudge";
            this.nudgeButton.UseVisualStyleBackColor = true;
            this.nudgeButton.Click += new System.EventHandler(this.nudgeButton_Click);
            // 
            // step86WaitForSystemToLoad
            // 
            this.step86WaitForSystemToLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step86WaitForSystemToLoad.Location = new System.Drawing.Point(1167, 601);
            this.step86WaitForSystemToLoad.Name = "step86WaitForSystemToLoad";
            this.step86WaitForSystemToLoad.Size = new System.Drawing.Size(230, 31);
            this.step86WaitForSystemToLoad.TabIndex = 19;
            this.step86WaitForSystemToLoad.Text = "Step 8.6 - Wait for System to load";
            this.step86WaitForSystemToLoad.UseVisualStyleBackColor = true;
            this.step86WaitForSystemToLoad.Click += new System.EventHandler(this.step86WaitForSystemToLoad_Click);
            // 
            // step87GetSignalsAfterSet
            // 
            this.step87GetSignalsAfterSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.step87GetSignalsAfterSet.Location = new System.Drawing.Point(1167, 638);
            this.step87GetSignalsAfterSet.Name = "step87GetSignalsAfterSet";
            this.step87GetSignalsAfterSet.Size = new System.Drawing.Size(230, 31);
            this.step87GetSignalsAfterSet.TabIndex = 20;
            this.step87GetSignalsAfterSet.Text = "Step 8.7 - Get Signals after Set";
            this.step87GetSignalsAfterSet.UseVisualStyleBackColor = true;
            this.step87GetSignalsAfterSet.Click += new System.EventHandler(this.step87GetSignalsAfterSet_Click);
            // 
            // RunningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1406, 917);
            this.Controls.Add(this.step87GetSignalsAfterSet);
            this.Controls.Add(this.step86WaitForSystemToLoad);
            this.Controls.Add(this.nudgeButton);
            this.Controls.Add(this.step65WaitForSystemToLoadButton);
            this.Controls.Add(this.step85SaveAndRebootButton);
            this.Controls.Add(this.stopProcessingButton);
            this.Controls.Add(this.targetDMPS);
            this.Controls.Add(this.step11ButtonPushVTZFiles);
            this.Controls.Add(this.step10ButtonGetTSPAddresses);
            this.Controls.Add(this.step9ButtonGetDMPSDevices);
            this.Controls.Add(this.step8ButtonSetSignals);
            this.Controls.Add(this.step7ButtonGetSignals);
            this.Controls.Add(this.step6ButtonPushZig);
            this.Controls.Add(this.step5ButtonPushSPZ);
            this.Controls.Add(this.step4ButtonGetSignals);
            this.Controls.Add(this.step3ButtonParseSIG);
            this.Controls.Add(this.step2ButtonRetrieveZIG);
            this.Controls.Add(this.step1ButtonConnect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.currentStatus);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "RunningForm";
            this.Text = "Deploying code to DMPS....";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox currentStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.Button step1ButtonConnect;
        private System.Windows.Forms.Button step2ButtonRetrieveZIG;
        private System.Windows.Forms.Button step3ButtonParseSIG;
        private System.Windows.Forms.Button step4ButtonGetSignals;
        private System.Windows.Forms.Button step5ButtonPushSPZ;
        private System.Windows.Forms.Button step6ButtonPushZig;
        private System.Windows.Forms.Button step7ButtonGetSignals;
        private System.Windows.Forms.Button step8ButtonSetSignals;
        private System.Windows.Forms.Button step9ButtonGetDMPSDevices;
        private System.Windows.Forms.Button step10ButtonGetTSPAddresses;
        private System.Windows.Forms.Button step11ButtonPushVTZFiles;
        private System.Windows.Forms.Button stopProcessingButton;
        public System.Windows.Forms.Label targetDMPS;
        private System.Windows.Forms.Button step85SaveAndRebootButton;
        private System.Windows.Forms.Button step65WaitForSystemToLoadButton;
        private System.Windows.Forms.Button nudgeButton;
        private System.Windows.Forms.Button step86WaitForSystemToLoad;
        private System.Windows.Forms.Button step87GetSignalsAfterSet;
    }
}