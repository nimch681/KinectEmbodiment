namespace KinectServer
{
    partial class MainWindowForm
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
            this.btStart = new System.Windows.Forms.Button();
            this.btCalibrate = new System.Windows.Forms.Button();
            this.btRecord = new System.Windows.Forms.Button();
            this.lClientListBox = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.recordingWorker = new System.ComponentModel.BackgroundWorker();
            this.txtSeqName = new System.Windows.Forms.TextBox();
            this.btRefineCalib = new System.Windows.Forms.Button();
            this.OpenGLWorker = new System.ComponentModel.BackgroundWorker();
            this.savingWorker = new System.ComponentModel.BackgroundWorker();
            this.updateWorker = new System.ComponentModel.BackgroundWorker();
            this.btShowLive = new System.Windows.Forms.Button();
            this.btSettings = new System.Windows.Forms.Button();
            this.refineWorker = new System.ComponentModel.BackgroundWorker();
            this.lbSeqName = new System.Windows.Forms.Label();
            this.BodyView = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CounterIdeal = new System.Windows.Forms.Label();
            this.IdealNext = new System.Windows.Forms.Button();
            this.IdealNo = new System.Windows.Forms.Button();
            this.IdeaYes = new System.Windows.Forms.Button();
            this.idealText = new System.Windows.Forms.TextBox();
            this.IdealPlus = new System.Windows.Forms.Button();
            this.IdeaMinus = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.AddFile = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.ScollDis = new System.Windows.Forms.Button();
            this.SimilarBody = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CounterSimilar = new System.Windows.Forms.Label();
            this.similarNext = new System.Windows.Forms.Button();
            this.SimilarNo = new System.Windows.Forms.Button();
            this.SimilarYes = new System.Windows.Forms.Button();
            this.sizeText = new System.Windows.Forms.TextBox();
            this.plusSimilar = new System.Windows.Forms.Button();
            this.minusSimilar = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Counter45 = new System.Windows.Forms.Label();
            this.RandomBodyText = new System.Windows.Forms.TextBox();
            this.NextButton = new System.Windows.Forms.Button();
            this.BodySizeNo = new System.Windows.Forms.Button();
            this.BodySizeYes = new System.Windows.Forms.Button();
            this.BodySize = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.RandomBodiesArray = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.New = new System.Windows.Forms.Button();
            this.ParticipantsID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(12, 12);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(95, 23);
            this.btStart.TabIndex = 0;
            this.btStart.Text = "Start server";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btCalibrate
            // 
            this.btCalibrate.Location = new System.Drawing.Point(12, 70);
            this.btCalibrate.Name = "btCalibrate";
            this.btCalibrate.Size = new System.Drawing.Size(95, 23);
            this.btCalibrate.TabIndex = 2;
            this.btCalibrate.Text = "Calibrate";
            this.btCalibrate.UseVisualStyleBackColor = true;
            this.btCalibrate.Click += new System.EventHandler(this.btCalibrate_Click);
            // 
            // btRecord
            // 
            this.btRecord.Location = new System.Drawing.Point(12, 126);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(95, 23);
            this.btRecord.TabIndex = 4;
            this.btRecord.Text = "Start recording";
            this.btRecord.UseVisualStyleBackColor = true;
            this.btRecord.Click += new System.EventHandler(this.btRecord_Click);
            // 
            // lClientListBox
            // 
            this.lClientListBox.FormattingEnabled = true;
            this.lClientListBox.HorizontalScrollbar = true;
            this.lClientListBox.Location = new System.Drawing.Point(113, 12);
            this.lClientListBox.Name = "lClientListBox";
            this.lClientListBox.Size = new System.Drawing.Size(219, 108);
            this.lClientListBox.TabIndex = 5;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 651);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(373, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // recordingWorker
            // 
            this.recordingWorker.WorkerSupportsCancellation = true;
            this.recordingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.recordingWorker_DoWork);
            this.recordingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.recordingWorker_RunWorkerCompleted);
            // 
            // txtSeqName
            // 
            this.txtSeqName.Location = new System.Drawing.Point(110, 145);
            this.txtSeqName.MaxLength = 40;
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(88, 20);
            this.txtSeqName.TabIndex = 7;
            this.txtSeqName.Text = "noname";
            // 
            // btRefineCalib
            // 
            this.btRefineCalib.Location = new System.Drawing.Point(12, 97);
            this.btRefineCalib.Name = "btRefineCalib";
            this.btRefineCalib.Size = new System.Drawing.Size(95, 23);
            this.btRefineCalib.TabIndex = 11;
            this.btRefineCalib.Text = "Refine calib";
            this.btRefineCalib.UseVisualStyleBackColor = true;
            this.btRefineCalib.Click += new System.EventHandler(this.btRefineCalib_Click);
            // 
            // OpenGLWorker
            // 
            this.OpenGLWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OpenGLWorker_DoWork);
            this.OpenGLWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OpenGLWorker_RunWorkerCompleted);
            // 
            // savingWorker
            // 
            this.savingWorker.WorkerSupportsCancellation = true;
            this.savingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.savingWorker_DoWork);
            this.savingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.savingWorker_RunWorkerCompleted);
            // 
            // updateWorker
            // 
            this.updateWorker.WorkerSupportsCancellation = true;
            this.updateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker_DoWork);
            // 
            // btShowLive
            // 
            this.btShowLive.Location = new System.Drawing.Point(12, 154);
            this.btShowLive.Name = "btShowLive";
            this.btShowLive.Size = new System.Drawing.Size(95, 23);
            this.btShowLive.TabIndex = 12;
            this.btShowLive.Text = "Show live";
            this.btShowLive.UseVisualStyleBackColor = true;
            this.btShowLive.Click += new System.EventHandler(this.btShowLive_Click);
            // 
            // btSettings
            // 
            this.btSettings.Location = new System.Drawing.Point(12, 41);
            this.btSettings.Name = "btSettings";
            this.btSettings.Size = new System.Drawing.Size(95, 23);
            this.btSettings.TabIndex = 13;
            this.btSettings.Text = "Settings";
            this.btSettings.UseVisualStyleBackColor = true;
            this.btSettings.Click += new System.EventHandler(this.btSettings_Click);
            // 
            // refineWorker
            // 
            this.refineWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refineWorker_DoWork);
            this.refineWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refineWorker_RunWorkerCompleted);
            // 
            // lbSeqName
            // 
            this.lbSeqName.AutoSize = true;
            this.lbSeqName.Location = new System.Drawing.Point(110, 126);
            this.lbSeqName.Name = "lbSeqName";
            this.lbSeqName.Size = new System.Drawing.Size(88, 13);
            this.lbSeqName.TabIndex = 14;
            this.lbSeqName.Text = "Sequence name:";
            // 
            // BodyView
            // 
            this.BodyView.Location = new System.Drawing.Point(12, 183);
            this.BodyView.Name = "BodyView";
            this.BodyView.Size = new System.Drawing.Size(95, 23);
            this.BodyView.TabIndex = 28;
            this.BodyView.Text = "Resizing off";
            this.BodyView.UseVisualStyleBackColor = true;
            this.BodyView.Click += new System.EventHandler(this.ResetHoloLens_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CounterIdeal);
            this.groupBox1.Controls.Add(this.IdealNext);
            this.groupBox1.Controls.Add(this.IdealNo);
            this.groupBox1.Controls.Add(this.IdeaYes);
            this.groupBox1.Controls.Add(this.idealText);
            this.groupBox1.Controls.Add(this.IdealPlus);
            this.groupBox1.Controls.Add(this.IdeaMinus);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(8, 505);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 113);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resizing Ideal Body";
            // 
            // CounterIdeal
            // 
            this.CounterIdeal.AutoSize = true;
            this.CounterIdeal.Location = new System.Drawing.Point(22, 28);
            this.CounterIdeal.Name = "CounterIdeal";
            this.CounterIdeal.Size = new System.Drawing.Size(27, 13);
            this.CounterIdeal.TabIndex = 48;
            this.CounterIdeal.Text = "No: ";
            this.CounterIdeal.Click += new System.EventHandler(this.label2_Click);
            // 
            // IdealNext
            // 
            this.IdealNext.Location = new System.Drawing.Point(274, 67);
            this.IdealNext.Name = "IdealNext";
            this.IdealNext.Size = new System.Drawing.Size(78, 23);
            this.IdealNext.TabIndex = 47;
            this.IdealNext.Text = "Next";
            this.IdealNext.UseVisualStyleBackColor = true;
            this.IdealNext.Click += new System.EventHandler(this.IdealNext_Click);
            // 
            // IdealNo
            // 
            this.IdealNo.Location = new System.Drawing.Point(196, 67);
            this.IdealNo.Name = "IdealNo";
            this.IdealNo.Size = new System.Drawing.Size(78, 23);
            this.IdealNo.TabIndex = 46;
            this.IdealNo.Text = "No";
            this.IdealNo.UseVisualStyleBackColor = true;
            this.IdealNo.Click += new System.EventHandler(this.IdealNo_Click);
            // 
            // IdeaYes
            // 
            this.IdeaYes.Location = new System.Drawing.Point(120, 67);
            this.IdeaYes.Name = "IdeaYes";
            this.IdeaYes.Size = new System.Drawing.Size(79, 23);
            this.IdeaYes.TabIndex = 45;
            this.IdeaYes.Text = "Yes";
            this.IdeaYes.UseVisualStyleBackColor = true;
            this.IdeaYes.Click += new System.EventHandler(this.IdeaYes_Click);
            // 
            // idealText
            // 
            this.idealText.Location = new System.Drawing.Point(102, 28);
            this.idealText.MaxLength = 40;
            this.idealText.Name = "idealText";
            this.idealText.Size = new System.Drawing.Size(39, 20);
            this.idealText.TabIndex = 44;
            // 
            // IdealPlus
            // 
            this.IdealPlus.Location = new System.Drawing.Point(148, 26);
            this.IdealPlus.Name = "IdealPlus";
            this.IdealPlus.Size = new System.Drawing.Size(35, 23);
            this.IdealPlus.TabIndex = 43;
            this.IdealPlus.Text = "+";
            this.IdealPlus.UseVisualStyleBackColor = true;
            this.IdealPlus.Click += new System.EventHandler(this.IdealPlus_Click);
            // 
            // IdeaMinus
            // 
            this.IdeaMinus.Location = new System.Drawing.Point(58, 26);
            this.IdeaMinus.Name = "IdeaMinus";
            this.IdeaMinus.Size = new System.Drawing.Size(35, 23);
            this.IdeaMinus.TabIndex = 42;
            this.IdeaMinus.Text = "-";
            this.IdeaMinus.UseVisualStyleBackColor = true;
            this.IdeaMinus.Click += new System.EventHandler(this.IdeaMinus_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.AddFile);
            this.groupBox2.Location = new System.Drawing.Point(110, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 61);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Add File";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(178, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 23);
            this.button1.TabIndex = 37;
            this.button1.Text = "Dispose";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddFile
            // 
            this.AddFile.Location = new System.Drawing.Point(12, 20);
            this.AddFile.Name = "AddFile";
            this.AddFile.Size = new System.Drawing.Size(160, 23);
            this.AddFile.TabIndex = 36;
            this.AddFile.Text = "Load";
            this.AddFile.UseVisualStyleBackColor = true;
            this.AddFile.Click += new System.EventHandler(this.AddFile_Click);
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(13, 213);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(94, 23);
            this.Stop.TabIndex = 31;
            this.Stop.Text = "Stop sending";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // ScollDis
            // 
            this.ScollDis.Location = new System.Drawing.Point(12, 464);
            this.ScollDis.Name = "ScollDis";
            this.ScollDis.Size = new System.Drawing.Size(94, 23);
            this.ScollDis.TabIndex = 28;
            this.ScollDis.Text = "Ideal body off";
            this.ScollDis.UseVisualStyleBackColor = true;
            this.ScollDis.Click += new System.EventHandler(this.ScollDis_Click);
            // 
            // SimilarBody
            // 
            this.SimilarBody.Location = new System.Drawing.Point(13, 320);
            this.SimilarBody.Name = "SimilarBody";
            this.SimilarBody.Size = new System.Drawing.Size(94, 23);
            this.SimilarBody.TabIndex = 32;
            this.SimilarBody.Text = "Similar body off";
            this.SimilarBody.UseVisualStyleBackColor = true;
            this.SimilarBody.Click += new System.EventHandler(this.SimilarBody_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.CounterSimilar);
            this.groupBox3.Controls.Add(this.similarNext);
            this.groupBox3.Controls.Add(this.SimilarNo);
            this.groupBox3.Controls.Add(this.SimilarYes);
            this.groupBox3.Controls.Add(this.sizeText);
            this.groupBox3.Controls.Add(this.plusSimilar);
            this.groupBox3.Controls.Add(this.minusSimilar);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(13, 345);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(353, 113);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Resizing Simlar Body";
            // 
            // CounterSimilar
            // 
            this.CounterSimilar.AutoSize = true;
            this.CounterSimilar.Location = new System.Drawing.Point(17, 31);
            this.CounterSimilar.Name = "CounterSimilar";
            this.CounterSimilar.Size = new System.Drawing.Size(27, 13);
            this.CounterSimilar.TabIndex = 43;
            this.CounterSimilar.Text = "No: ";
            // 
            // similarNext
            // 
            this.similarNext.Location = new System.Drawing.Point(272, 84);
            this.similarNext.Name = "similarNext";
            this.similarNext.Size = new System.Drawing.Size(78, 23);
            this.similarNext.TabIndex = 41;
            this.similarNext.Text = "Next";
            this.similarNext.UseVisualStyleBackColor = true;
            this.similarNext.Click += new System.EventHandler(this.similarNext_Click);
            // 
            // SimilarNo
            // 
            this.SimilarNo.Location = new System.Drawing.Point(191, 84);
            this.SimilarNo.Name = "SimilarNo";
            this.SimilarNo.Size = new System.Drawing.Size(78, 23);
            this.SimilarNo.TabIndex = 40;
            this.SimilarNo.Text = "No";
            this.SimilarNo.UseVisualStyleBackColor = true;
            this.SimilarNo.Click += new System.EventHandler(this.SimilarNo_Click);
            // 
            // SimilarYes
            // 
            this.SimilarYes.Location = new System.Drawing.Point(109, 84);
            this.SimilarYes.Name = "SimilarYes";
            this.SimilarYes.Size = new System.Drawing.Size(79, 23);
            this.SimilarYes.TabIndex = 39;
            this.SimilarYes.Text = "Yes";
            this.SimilarYes.UseVisualStyleBackColor = true;
            this.SimilarYes.Click += new System.EventHandler(this.SimilarYes_Click);
            // 
            // sizeText
            // 
            this.sizeText.Location = new System.Drawing.Point(97, 31);
            this.sizeText.MaxLength = 40;
            this.sizeText.Name = "sizeText";
            this.sizeText.Size = new System.Drawing.Size(39, 20);
            this.sizeText.TabIndex = 38;
            // 
            // plusSimilar
            // 
            this.plusSimilar.Location = new System.Drawing.Point(143, 29);
            this.plusSimilar.Name = "plusSimilar";
            this.plusSimilar.Size = new System.Drawing.Size(35, 23);
            this.plusSimilar.TabIndex = 37;
            this.plusSimilar.Text = "+";
            this.plusSimilar.UseVisualStyleBackColor = true;
            this.plusSimilar.Click += new System.EventHandler(this.plusSimilar_Click);
            // 
            // minusSimilar
            // 
            this.minusSimilar.Location = new System.Drawing.Point(53, 29);
            this.minusSimilar.Name = "minusSimilar";
            this.minusSimilar.Size = new System.Drawing.Size(35, 23);
            this.minusSimilar.TabIndex = 36;
            this.minusSimilar.Text = "-";
            this.minusSimilar.UseVisualStyleBackColor = true;
            this.minusSimilar.Click += new System.EventHandler(this.minusSimilar_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Counter45);
            this.groupBox4.Controls.Add(this.RandomBodyText);
            this.groupBox4.Controls.Add(this.NextButton);
            this.groupBox4.Controls.Add(this.BodySizeNo);
            this.groupBox4.Controls.Add(this.BodySizeYes);
            this.groupBox4.Controls.Add(this.BodySize);
            this.groupBox4.Controls.Add(this.label23);
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.label30);
            this.groupBox4.Controls.Add(this.label31);
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Controls.Add(this.label33);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(15, 265);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 49);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Random 45 Question";
            // 
            // Counter45
            // 
            this.Counter45.AutoSize = true;
            this.Counter45.Location = new System.Drawing.Point(15, 23);
            this.Counter45.Name = "Counter45";
            this.Counter45.Size = new System.Drawing.Size(27, 13);
            this.Counter45.TabIndex = 42;
            this.Counter45.Text = "No: ";
            // 
            // RandomBodyText
            // 
            this.RandomBodyText.Location = new System.Drawing.Point(62, 20);
            this.RandomBodyText.MaxLength = 40;
            this.RandomBodyText.Name = "RandomBodyText";
            this.RandomBodyText.Size = new System.Drawing.Size(39, 20);
            this.RandomBodyText.TabIndex = 39;
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(270, 20);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(78, 23);
            this.NextButton.TabIndex = 37;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // BodySizeNo
            // 
            this.BodySizeNo.Location = new System.Drawing.Point(189, 20);
            this.BodySizeNo.Name = "BodySizeNo";
            this.BodySizeNo.Size = new System.Drawing.Size(78, 23);
            this.BodySizeNo.TabIndex = 36;
            this.BodySizeNo.Text = "No";
            this.BodySizeNo.UseVisualStyleBackColor = true;
            this.BodySizeNo.Click += new System.EventHandler(this.BodySizeNo_Click);
            // 
            // BodySizeYes
            // 
            this.BodySizeYes.Location = new System.Drawing.Point(107, 20);
            this.BodySizeYes.Name = "BodySizeYes";
            this.BodySizeYes.Size = new System.Drawing.Size(79, 23);
            this.BodySizeYes.TabIndex = 35;
            this.BodySizeYes.Text = "Yes";
            this.BodySizeYes.UseVisualStyleBackColor = true;
            this.BodySizeYes.Click += new System.EventHandler(this.BodySizeYes_Click);
            // 
            // BodySize
            // 
            this.BodySize.AutoSize = true;
            this.BodySize.Location = new System.Drawing.Point(76, 16);
            this.BodySize.Name = "BodySize";
            this.BodySize.Size = new System.Drawing.Size(0, 13);
            this.BodySize.TabIndex = 29;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(111, 75);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(13, 13);
            this.label23.TabIndex = 27;
            this.label23.Text = "1";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(20, 75);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(22, 13);
            this.label24.TabIndex = 17;
            this.label24.Text = "0.7";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(138, 75);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(22, 13);
            this.label25.TabIndex = 26;
            this.label25.Text = "1.1";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(48, 75);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(22, 13);
            this.label26.TabIndex = 18;
            this.label26.Text = "0.8";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(170, 75);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(22, 13);
            this.label27.TabIndex = 25;
            this.label27.Text = "1.2";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(76, 75);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(22, 13);
            this.label28.TabIndex = 19;
            this.label28.Text = "0.9";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(198, 75);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(22, 13);
            this.label29.TabIndex = 24;
            this.label29.Text = "1.3";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(311, 75);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(22, 13);
            this.label30.TabIndex = 20;
            this.label30.Text = "1.7";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(226, 75);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(22, 13);
            this.label31.TabIndex = 23;
            this.label31.Text = "1.4";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(282, 75);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(22, 13);
            this.label32.TabIndex = 21;
            this.label32.Text = "1.6";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(254, 75);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(22, 13);
            this.label33.TabIndex = 22;
            this.label33.Text = "1.5";
            // 
            // RandomBodiesArray
            // 
            this.RandomBodiesArray.Location = new System.Drawing.Point(13, 242);
            this.RandomBodiesArray.Name = "RandomBodiesArray";
            this.RandomBodiesArray.Size = new System.Drawing.Size(123, 23);
            this.RandomBodiesArray.TabIndex = 36;
            this.RandomBodiesArray.Text = "Fill Random Bodies";
            this.RandomBodiesArray.UseVisualStyleBackColor = true;
            this.RandomBodiesArray.Click += new System.EventHandler(this.RandomBodiesArray_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(204, 624);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(78, 23);
            this.Save.TabIndex = 37;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(285, 624);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 38;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // New
            // 
            this.New.Location = new System.Drawing.Point(288, 143);
            this.New.Name = "New";
            this.New.Size = new System.Drawing.Size(72, 23);
            this.New.TabIndex = 39;
            this.New.Text = "New";
            this.New.UseVisualStyleBackColor = true;
            this.New.Click += new System.EventHandler(this.New_Click);
            // 
            // ParticipantsID
            // 
            this.ParticipantsID.Location = new System.Drawing.Point(204, 145);
            this.ParticipantsID.Name = "ParticipantsID";
            this.ParticipantsID.Size = new System.Drawing.Size(78, 20);
            this.ParticipantsID.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Participant ID";
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 673);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ParticipantsID);
            this.Controls.Add(this.New);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.RandomBodiesArray);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.SimilarBody);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ScollDis);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BodyView);
            this.Controls.Add(this.lbSeqName);
            this.Controls.Add(this.btSettings);
            this.Controls.Add(this.btShowLive);
            this.Controls.Add(this.btRefineCalib);
            this.Controls.Add(this.txtSeqName);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lClientListBox);
            this.Controls.Add(this.btRecord);
            this.Controls.Add(this.btCalibrate);
            this.Controls.Add(this.btStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindowForm";
            this.Text = "LiveScanServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btCalibrate;
        private System.Windows.Forms.Button btRecord;
        private System.Windows.Forms.ListBox lClientListBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.ComponentModel.BackgroundWorker recordingWorker;
        private System.Windows.Forms.TextBox txtSeqName;
        private System.Windows.Forms.Button btRefineCalib;
        private System.ComponentModel.BackgroundWorker OpenGLWorker;
        private System.ComponentModel.BackgroundWorker savingWorker;
        private System.ComponentModel.BackgroundWorker updateWorker;
        private System.Windows.Forms.Button btShowLive;
        private System.Windows.Forms.Button btSettings;
        private System.ComponentModel.BackgroundWorker refineWorker;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label lbSeqName;
        private System.Windows.Forms.Button BodyView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Button ScollDis;
        private System.Windows.Forms.Button SimilarBody;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button BodySizeNo;
        private System.Windows.Forms.Button BodySizeYes;
        private System.Windows.Forms.Label BodySize;
        private System.Windows.Forms.Button RandomBodiesArray;
        private System.Windows.Forms.Button similarNext;
        private System.Windows.Forms.Button SimilarNo;
        private System.Windows.Forms.Button SimilarYes;
        private System.Windows.Forms.TextBox sizeText;
        private System.Windows.Forms.Button plusSimilar;
        private System.Windows.Forms.Button minusSimilar;
        private System.Windows.Forms.Button IdealNext;
        private System.Windows.Forms.Button IdealNo;
        private System.Windows.Forms.Button IdeaYes;
        private System.Windows.Forms.TextBox idealText;
        private System.Windows.Forms.Button IdealPlus;
        private System.Windows.Forms.Button IdeaMinus;
        private System.Windows.Forms.TextBox RandomBodyText;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button New;
        private System.Windows.Forms.Button AddFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ParticipantsID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CounterIdeal;
        private System.Windows.Forms.Label CounterSimilar;
        private System.Windows.Forms.Label Counter45;
    }
}

