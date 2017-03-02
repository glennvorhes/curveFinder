namespace FormUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnInput = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbFeet = new System.Windows.Forms.CheckBox();
            this.cbDisslv = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.AngleVariation = new System.Windows.Forms.NumericUpDown();
            this.btIdentifyCurveAreas = new System.Windows.Forms.Button();
            this.rIDField = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleVariation)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(438, 22);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(49, 23);
            this.btnInput.TabIndex = 0;
            this.btnInput.Text = "Input";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.openFileDialogue);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.White;
            this.txtInput.Location = new System.Drawing.Point(24, 22);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(408, 20);
            this.txtInput.TabIndex = 1;
            this.txtInput.WordWrap = false;
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.Location = new System.Drawing.Point(24, 62);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(408, 20);
            this.txtOutput.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbFeet);
            this.groupBox3.Controls.Add(this.cbDisslv);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.AngleVariation);
            this.groupBox3.Location = new System.Drawing.Point(24, 148);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(489, 108);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuration";
            // 
            // cbFeet
            // 
            this.cbFeet.AutoCheck = false;
            this.cbFeet.AutoSize = true;
            this.cbFeet.Location = new System.Drawing.Point(141, 28);
            this.cbFeet.Name = "cbFeet";
            this.cbFeet.Size = new System.Drawing.Size(103, 17);
            this.cbFeet.TabIndex = 22;
            this.cbFeet.TabStop = false;
            this.cbFeet.Text = "Map Unit is Feet";
            this.cbFeet.UseVisualStyleBackColor = true;
            // 
            // cbDisslv
            // 
            this.cbDisslv.AutoSize = true;
            this.cbDisslv.Checked = true;
            this.cbDisslv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisslv.Location = new System.Drawing.Point(20, 28);
            this.cbDisslv.Name = "cbDisslv";
            this.cbDisslv.Size = new System.Drawing.Size(106, 17);
            this.cbDisslv.TabIndex = 21;
            this.cbDisslv.Text = "Dissolved Roads";
            this.cbDisslv.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(17, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(126, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Bearing Angle Threshold:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(250, 63);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "degree";
            // 
            // AngleVariation
            // 
            this.AngleVariation.DecimalPlaces = 1;
            this.AngleVariation.Location = new System.Drawing.Point(150, 61);
            this.AngleVariation.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.AngleVariation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.AngleVariation.Name = "AngleVariation";
            this.AngleVariation.Size = new System.Drawing.Size(94, 20);
            this.AngleVariation.TabIndex = 4;
            this.AngleVariation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AngleVariation.ValueChanged += new System.EventHandler(this.NUDMaxAngleVariationInATangent_ValueChanged);
            // 
            // btIdentifyCurveAreas
            // 
            this.btIdentifyCurveAreas.Enabled = false;
            this.btIdentifyCurveAreas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btIdentifyCurveAreas.Location = new System.Drawing.Point(334, 90);
            this.btIdentifyCurveAreas.Name = "btIdentifyCurveAreas";
            this.btIdentifyCurveAreas.Size = new System.Drawing.Size(84, 42);
            this.btIdentifyCurveAreas.TabIndex = 23;
            this.btIdentifyCurveAreas.Text = "Extract Curves";
            this.btIdentifyCurveAreas.UseVisualStyleBackColor = true;
            this.btIdentifyCurveAreas.Click += new System.EventHandler(this.btIdentifyCurveAreas_Click);
            // 
            // rIDField
            // 
            this.rIDField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rIDField.FormattingEnabled = true;
            this.rIDField.Location = new System.Drawing.Point(138, 103);
            this.rIDField.Name = "rIDField";
            this.rIDField.Size = new System.Drawing.Size(176, 21);
            this.rIDField.TabIndex = 25;
            this.rIDField.SelectedIndexChanged += new System.EventHandler(this.rIDField_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 106);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(111, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Select Road ID Field :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(442, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Output";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 296);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rIDField);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btIdentifyCurveAreas);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Curve Finder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleVariation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown AngleVariation;
        private System.Windows.Forms.Button btIdentifyCurveAreas;
        private System.Windows.Forms.CheckBox cbFeet;
        private System.Windows.Forms.CheckBox cbDisslv;
        private System.Windows.Forms.ComboBox rIDField;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;

    }
}

