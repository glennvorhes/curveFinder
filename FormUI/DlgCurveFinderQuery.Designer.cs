namespace FormUI
{
    partial class DlgCurveFinderQuery
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
            this.cbLayers = new System.Windows.Forms.ComboBox();
            this.btIdentify = new System.Windows.Forms.Button();
            this.lvCurves = new System.Windows.Forms.ListView();
            this.ColCurveID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColLayerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColPolyLineID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColStreetName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColDirIndc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColCurveIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColLengh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColRadius = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColAngle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbFeet = new System.Windows.Forms.CheckBox();
            this.cbDisslv = new System.Windows.Forms.CheckBox();
            this.rIDField = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btIdentifyCurveAreas = new System.Windows.Forms.Button();
            this.cbInputLayerIsCurve = new System.Windows.Forms.CheckBox();
            this.cbExportSeg = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NUDMinAngleToStartACurve = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve = new System.Windows.Forms.NumericUpDown();
            this.NUDMinNumofSegmentsInACurve = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.NUDMaxAngleVariationInATangent = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinAngleBetweenConsecutiveSegmentsInACurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinAngleToStartACurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinNumofSegmentsInACurve)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAngleVariationInATangent)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a Layer : ";
            // 
            // cbLayers
            // 
            this.cbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayers.FormattingEnabled = true;
            this.cbLayers.Location = new System.Drawing.Point(102, 20);
            this.cbLayers.Name = "cbLayers";
            this.cbLayers.Size = new System.Drawing.Size(617, 21);
            this.cbLayers.TabIndex = 1;
            // 
            // btIdentify
            // 
            this.btIdentify.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btIdentify.Location = new System.Drawing.Point(581, 81);
            this.btIdentify.Name = "btIdentify";
            this.btIdentify.Size = new System.Drawing.Size(107, 49);
            this.btIdentify.TabIndex = 2;
            this.btIdentify.Text = "Identify Curves";
            this.btIdentify.UseVisualStyleBackColor = true;
            // 
            // lvCurves
            // 
            this.lvCurves.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColCurveID,
            this.ColLayerName,
            this.ColPolyLineID,
            this.ColStreetName,
            this.ColDirIndc,
            this.ColCurveIndex,
            this.ColLengh,
            this.ColRadius,
            this.ColAngle});
            this.lvCurves.FullRowSelect = true;
            this.lvCurves.GridLines = true;
            this.lvCurves.HideSelection = false;
            this.lvCurves.Location = new System.Drawing.Point(15, 19);
            this.lvCurves.Name = "lvCurves";
            this.lvCurves.Size = new System.Drawing.Size(100, 12);
            this.lvCurves.TabIndex = 3;
            this.lvCurves.UseCompatibleStateImageBehavior = false;
            this.lvCurves.View = System.Windows.Forms.View.Details;
            this.lvCurves.Visible = false;
            // 
            // ColCurveID
            // 
            this.ColCurveID.Text = "ID";
            this.ColCurveID.Width = 48;
            // 
            // ColLayerName
            // 
            this.ColLayerName.Text = "Layer";
            this.ColLayerName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColLayerName.Width = 104;
            // 
            // ColPolyLineID
            // 
            this.ColPolyLineID.Text = "Polyline ID";
            this.ColPolyLineID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColPolyLineID.Width = 65;
            // 
            // ColStreetName
            // 
            this.ColStreetName.Text = "StreetName";
            this.ColStreetName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColStreetName.Width = 87;
            // 
            // ColDirIndc
            // 
            this.ColDirIndc.Text = "RoadDirection";
            this.ColDirIndc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColDirIndc.Width = 82;
            // 
            // ColCurveIndex
            // 
            this.ColCurveIndex.Text = "Curve Index";
            this.ColCurveIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColCurveIndex.Width = 79;
            // 
            // ColLengh
            // 
            this.ColLengh.Text = "Curve Length  ( m )";
            this.ColLengh.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColLengh.Width = 110;
            // 
            // ColRadius
            // 
            this.ColRadius.Text = "Radius  ( m )";
            this.ColRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColRadius.Width = 97;
            // 
            // ColAngle
            // 
            this.ColAngle.Text = "Central Angle  ( degree )";
            this.ColAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColAngle.Width = 138;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvCurves);
            this.groupBox1.Location = new System.Drawing.Point(9, 786);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(163, 40);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Identified Curves";
            this.groupBox1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbFeet);
            this.groupBox2.Controls.Add(this.cbDisslv);
            this.groupBox2.Controls.Add(this.rIDField);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btIdentifyCurveAreas);
            this.groupBox2.Controls.Add(this.cbInputLayerIsCurve);
            this.groupBox2.Controls.Add(this.cbExportSeg);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.cbLayers);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(8, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(738, 769);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Query";
            // 
            // cbFeet
            // 
            this.cbFeet.AutoSize = true;
            this.cbFeet.Location = new System.Drawing.Point(136, 100);
            this.cbFeet.Name = "cbFeet";
            this.cbFeet.Size = new System.Drawing.Size(103, 17);
            this.cbFeet.TabIndex = 18;
            this.cbFeet.Text = "Map Unit is Feet";
            this.cbFeet.UseVisualStyleBackColor = true;
            // 
            // cbDisslv
            // 
            this.cbDisslv.AutoSize = true;
            this.cbDisslv.Location = new System.Drawing.Point(15, 100);
            this.cbDisslv.Name = "cbDisslv";
            this.cbDisslv.Size = new System.Drawing.Size(112, 17);
            this.cbDisslv.TabIndex = 7;
            this.cbDisslv.Text = "Dissolved   Roads";
            this.cbDisslv.UseVisualStyleBackColor = true;
            // 
            // rIDField
            // 
            this.rIDField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rIDField.FormattingEnabled = true;
            this.rIDField.Location = new System.Drawing.Point(158, 57);
            this.rIDField.Name = "rIDField";
            this.rIDField.Size = new System.Drawing.Size(254, 21);
            this.rIDField.TabIndex = 17;
            this.rIDField.Click += new System.EventHandler(this.rIDField_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(111, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Select Road ID Field :";
            // 
            // btIdentifyCurveAreas
            // 
            this.btIdentifyCurveAreas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btIdentifyCurveAreas.Location = new System.Drawing.Point(635, 75);
            this.btIdentifyCurveAreas.Name = "btIdentifyCurveAreas";
            this.btIdentifyCurveAreas.Size = new System.Drawing.Size(84, 42);
            this.btIdentifyCurveAreas.TabIndex = 2;
            this.btIdentifyCurveAreas.Text = "Extract Curves";
            this.btIdentifyCurveAreas.UseVisualStyleBackColor = true;
            this.btIdentifyCurveAreas.Click += new System.EventHandler(this.btIdentifyCurveAreas_Click);
            // 
            // cbInputLayerIsCurve
            // 
            this.cbInputLayerIsCurve.AutoSize = true;
            this.cbInputLayerIsCurve.Location = new System.Drawing.Point(294, 511);
            this.cbInputLayerIsCurve.Name = "cbInputLayerIsCurve";
            this.cbInputLayerIsCurve.Size = new System.Drawing.Size(181, 17);
            this.cbInputLayerIsCurve.TabIndex = 7;
            this.cbInputLayerIsCurve.Text = "Input Layer is already curve layer";
            this.cbInputLayerIsCurve.UseVisualStyleBackColor = true;
            this.cbInputLayerIsCurve.Visible = false;
            // 
            // cbExportSeg
            // 
            this.cbExportSeg.AutoSize = true;
            this.cbExportSeg.Location = new System.Drawing.Point(32, 511);
            this.cbExportSeg.Name = "cbExportSeg";
            this.cbExportSeg.Size = new System.Drawing.Size(161, 17);
            this.cbExportSeg.TabIndex = 5;
            this.cbExportSeg.Text = "Export Segments Distribution";
            this.cbExportSeg.UseVisualStyleBackColor = true;
            this.cbExportSeg.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.btIdentify);
            this.groupBox4.Controls.Add(this.NUDMinAngleBetweenConsecutiveSegmentsInACurve);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.NUDMinAngleToStartACurve);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve);
            this.groupBox4.Controls.Add(this.NUDMinNumofSegmentsInACurve);
            this.groupBox4.Location = new System.Drawing.Point(15, 548);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(704, 200);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Curve";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minimum Number of Segments to Form a Curve:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(339, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Maximum Allowable Angle between Consecutive Segments in a Curve:";
            // 
            // NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve
            // 
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.DecimalPlaces = 1;
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Location = new System.Drawing.Point(432, 140);
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Name = "NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve";
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.TabIndex = 4;
            this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Minimum Requried Angle to Start a Curve: ";
            // 
            // NUDMinAngleBetweenConsecutiveSegmentsInACurve
            // 
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.DecimalPlaces = 1;
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.Location = new System.Drawing.Point(432, 110);
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.Name = "NUDMinAngleBetweenConsecutiveSegmentsInACurve";
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.TabIndex = 4;
            this.NUDMinAngleBetweenConsecutiveSegmentsInACurve.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(380, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Maximum Allowable CounterAngle Between Consecutive Segments in a Curve: ";
            // 
            // NUDMinAngleToStartACurve
            // 
            this.NUDMinAngleToStartACurve.DecimalPlaces = 1;
            this.NUDMinAngleToStartACurve.Location = new System.Drawing.Point(432, 81);
            this.NUDMinAngleToStartACurve.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.NUDMinAngleToStartACurve.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUDMinAngleToStartACurve.Name = "NUDMinAngleToStartACurve";
            this.NUDMinAngleToStartACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMinAngleToStartACurve.TabIndex = 4;
            this.NUDMinAngleToStartACurve.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(387, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Maximum Allowable Number of Consecutive CounterAngle Segments in a Curve: ";
            // 
            // NUDMaxAngleBetweenConsecutiveSegmentsInACurve
            // 
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.DecimalPlaces = 1;
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Location = new System.Drawing.Point(432, 52);
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Maximum = new decimal(new int[] {
            170,
            0,
            0,
            0});
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Name = "NUDMaxAngleBetweenConsecutiveSegmentsInACurve";
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.TabIndex = 4;
            this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(337, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Minimum Required Angle between Consecutive Segments in a Curve: ";
            // 
            // NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve
            // 
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Location = new System.Drawing.Point(432, 170);
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Name = "NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve";
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.TabIndex = 4;
            this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NUDMinNumofSegmentsInACurve
            // 
            this.NUDMinNumofSegmentsInACurve.Location = new System.Drawing.Point(432, 23);
            this.NUDMinNumofSegmentsInACurve.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUDMinNumofSegmentsInACurve.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUDMinNumofSegmentsInACurve.Name = "NUDMinNumofSegmentsInACurve";
            this.NUDMinNumofSegmentsInACurve.Size = new System.Drawing.Size(120, 20);
            this.NUDMinNumofSegmentsInACurve.TabIndex = 4;
            this.NUDMinNumofSegmentsInACurve.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.NUDMaxAngleVariationInATangent);
            this.groupBox3.Location = new System.Drawing.Point(15, 135);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(704, 67);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuration";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(126, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Bearing Angle Threshold:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(539, 34);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "degree";
            // 
            // NUDMaxAngleVariationInATangent
            // 
            this.NUDMaxAngleVariationInATangent.DecimalPlaces = 2;
            this.NUDMaxAngleVariationInATangent.Location = new System.Drawing.Point(413, 32);
            this.NUDMaxAngleVariationInATangent.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUDMaxAngleVariationInATangent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUDMaxAngleVariationInATangent.Name = "NUDMaxAngleVariationInATangent";
            this.NUDMaxAngleVariationInATangent.Size = new System.Drawing.Size(120, 20);
            this.NUDMaxAngleVariationInATangent.TabIndex = 4;
            this.NUDMaxAngleVariationInATangent.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // DlgCurveFinderQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 240);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgCurveFinderQuery";
            this.Text = "Find Horizontal Curves";
            this.Load += new System.EventHandler(this.DlgCurveFinderQuery_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinAngleBetweenConsecutiveSegmentsInACurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinAngleToStartACurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAngleBetweenConsecutiveSegmentsInACurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMinNumofSegmentsInACurve)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMaxAngleVariationInATangent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLayers;
        private System.Windows.Forms.Button btIdentify;
        private System.Windows.Forms.ListView lvCurves;
        private System.Windows.Forms.ColumnHeader ColLayerName;
        private System.Windows.Forms.ColumnHeader ColCurveIndex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ColumnHeader ColPolyLineID;
        private System.Windows.Forms.ColumnHeader ColRadius;
        private System.Windows.Forms.ColumnHeader ColAngle;
        private System.Windows.Forms.ColumnHeader ColLengh;
        private System.Windows.Forms.ColumnHeader ColStreetName;
        private System.Windows.Forms.ColumnHeader ColDirIndc;
        private System.Windows.Forms.ColumnHeader ColCurveID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NUDMinNumofSegmentsInACurve;
        private System.Windows.Forms.NumericUpDown NUDMaxAngleBetweenConsecutiveSegmentsInACurve;
        private System.Windows.Forms.NumericUpDown NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve;
        private System.Windows.Forms.NumericUpDown NUDMinAngleBetweenConsecutiveSegmentsInACurve;
        private System.Windows.Forms.NumericUpDown NUDMinAngleToStartACurve;
        private System.Windows.Forms.NumericUpDown NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbInputLayerIsCurve;
        private System.Windows.Forms.CheckBox cbExportSeg;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btIdentifyCurveAreas;
        private System.Windows.Forms.NumericUpDown NUDMaxAngleVariationInATangent;
        private System.Windows.Forms.ComboBox rIDField;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbDisslv;
        private System.Windows.Forms.CheckBox cbFeet;
    }
}