namespace VPF0toPNG
{
	partial class ExportToVPF
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.gboxInputFile = new System.Windows.Forms.GroupBox();
			this.labelInFile = new System.Windows.Forms.Label();
			this.tboxImportFilePath = new System.Windows.Forms.TextBox();
			this.buttonBrowseImport = new System.Windows.Forms.Button();
			this.gboxOutputOptions = new System.Windows.Forms.GroupBox();
			this.buttonBrowseExport = new System.Windows.Forms.Button();
			this.tboxExportFilePath = new System.Windows.Forms.TextBox();
			this.labelOutFile = new System.Windows.Forms.Label();
			this.gboxBitDepth = new System.Windows.Forms.GroupBox();
			this.rbutton24BPP = new System.Windows.Forms.RadioButton();
			this.rbutton8BPP = new System.Windows.Forms.RadioButton();
			this.rbutton4BPP = new System.Windows.Forms.RadioButton();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonExport = new System.Windows.Forms.Button();
			this.gboxInputFile.SuspendLayout();
			this.gboxOutputOptions.SuspendLayout();
			this.gboxBitDepth.SuspendLayout();
			this.SuspendLayout();
			// 
			// gboxInputFile
			// 
			this.gboxInputFile.Controls.Add(this.labelInFile);
			this.gboxInputFile.Controls.Add(this.tboxImportFilePath);
			this.gboxInputFile.Controls.Add(this.buttonBrowseImport);
			this.gboxInputFile.Location = new System.Drawing.Point(12, 12);
			this.gboxInputFile.Name = "gboxInputFile";
			this.gboxInputFile.Size = new System.Drawing.Size(610, 57);
			this.gboxInputFile.TabIndex = 0;
			this.gboxInputFile.TabStop = false;
			this.gboxInputFile.Text = "Input File";
			// 
			// labelInFile
			// 
			this.labelInFile.AutoSize = true;
			this.labelInFile.Location = new System.Drawing.Point(6, 24);
			this.labelInFile.Name = "labelInFile";
			this.labelInFile.Size = new System.Drawing.Size(76, 13);
			this.labelInFile.TabIndex = 1;
			this.labelInFile.Text = "GIF to Convert";
			// 
			// tboxImportFilePath
			// 
			this.tboxImportFilePath.Location = new System.Drawing.Point(94, 21);
			this.tboxImportFilePath.Name = "tboxImportFilePath";
			this.tboxImportFilePath.Size = new System.Drawing.Size(424, 20);
			this.tboxImportFilePath.TabIndex = 1;
			// 
			// buttonBrowseImport
			// 
			this.buttonBrowseImport.Location = new System.Drawing.Point(524, 18);
			this.buttonBrowseImport.Name = "buttonBrowseImport";
			this.buttonBrowseImport.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowseImport.TabIndex = 0;
			this.buttonBrowseImport.Text = "Browse...";
			this.buttonBrowseImport.UseVisualStyleBackColor = true;
			this.buttonBrowseImport.Click += new System.EventHandler(this.buttonBrowseImport_Click);
			// 
			// gboxOutputOptions
			// 
			this.gboxOutputOptions.Controls.Add(this.buttonBrowseExport);
			this.gboxOutputOptions.Controls.Add(this.tboxExportFilePath);
			this.gboxOutputOptions.Controls.Add(this.labelOutFile);
			this.gboxOutputOptions.Controls.Add(this.gboxBitDepth);
			this.gboxOutputOptions.Location = new System.Drawing.Point(12, 75);
			this.gboxOutputOptions.Name = "gboxOutputOptions";
			this.gboxOutputOptions.Size = new System.Drawing.Size(610, 95);
			this.gboxOutputOptions.TabIndex = 1;
			this.gboxOutputOptions.TabStop = false;
			this.gboxOutputOptions.Text = "Output Options";
			// 
			// buttonBrowseExport
			// 
			this.buttonBrowseExport.Location = new System.Drawing.Point(524, 11);
			this.buttonBrowseExport.Name = "buttonBrowseExport";
			this.buttonBrowseExport.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowseExport.TabIndex = 3;
			this.buttonBrowseExport.Text = "Browse...";
			this.buttonBrowseExport.UseVisualStyleBackColor = true;
			this.buttonBrowseExport.Click += new System.EventHandler(this.buttonBrowseExport_Click);
			// 
			// tboxExportFilePath
			// 
			this.tboxExportFilePath.Location = new System.Drawing.Point(94, 13);
			this.tboxExportFilePath.Name = "tboxExportFilePath";
			this.tboxExportFilePath.Size = new System.Drawing.Size(424, 20);
			this.tboxExportFilePath.TabIndex = 2;
			// 
			// labelOutFile
			// 
			this.labelOutFile.AutoSize = true;
			this.labelOutFile.Location = new System.Drawing.Point(6, 16);
			this.labelOutFile.Name = "labelOutFile";
			this.labelOutFile.Size = new System.Drawing.Size(72, 13);
			this.labelOutFile.TabIndex = 1;
			this.labelOutFile.Text = "VPF to Export";
			// 
			// gboxBitDepth
			// 
			this.gboxBitDepth.Controls.Add(this.rbutton24BPP);
			this.gboxBitDepth.Controls.Add(this.rbutton8BPP);
			this.gboxBitDepth.Controls.Add(this.rbutton4BPP);
			this.gboxBitDepth.Location = new System.Drawing.Point(9, 39);
			this.gboxBitDepth.Name = "gboxBitDepth";
			this.gboxBitDepth.Size = new System.Drawing.Size(590, 48);
			this.gboxBitDepth.TabIndex = 0;
			this.gboxBitDepth.TabStop = false;
			this.gboxBitDepth.Text = "Bit Depth";
			// 
			// rbutton24BPP
			// 
			this.rbutton24BPP.AutoSize = true;
			this.rbutton24BPP.Location = new System.Drawing.Point(405, 19);
			this.rbutton24BPP.Name = "rbutton24BPP";
			this.rbutton24BPP.Size = new System.Drawing.Size(152, 17);
			this.rbutton24BPP.TabIndex = 2;
			this.rbutton24BPP.Text = "24BPP (16,777,216 colors)";
			this.rbutton24BPP.UseVisualStyleBackColor = true;
			// 
			// rbutton8BPP
			// 
			this.rbutton8BPP.AutoSize = true;
			this.rbutton8BPP.Checked = true;
			this.rbutton8BPP.Location = new System.Drawing.Point(216, 19);
			this.rbutton8BPP.Name = "rbutton8BPP";
			this.rbutton8BPP.Size = new System.Drawing.Size(110, 17);
			this.rbutton8BPP.TabIndex = 1;
			this.rbutton8BPP.TabStop = true;
			this.rbutton8BPP.Text = "8BPP (256 colors)";
			this.rbutton8BPP.UseVisualStyleBackColor = true;
			// 
			// rbutton4BPP
			// 
			this.rbutton4BPP.AutoSize = true;
			this.rbutton4BPP.Enabled = false;
			this.rbutton4BPP.Location = new System.Drawing.Point(6, 19);
			this.rbutton4BPP.Name = "rbutton4BPP";
			this.rbutton4BPP.Size = new System.Drawing.Size(104, 17);
			this.rbutton4BPP.TabIndex = 0;
			this.rbutton4BPP.Text = "4BPP (16 colors)";
			this.rbutton4BPP.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(547, 183);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonExport
			// 
			this.buttonExport.Location = new System.Drawing.Point(466, 183);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(75, 23);
			this.buttonExport.TabIndex = 3;
			this.buttonExport.Text = "&Export";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// ExportToVPF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(634, 218);
			this.Controls.Add(this.buttonExport);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.gboxOutputOptions);
			this.Controls.Add(this.gboxInputFile);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ExportToVPF";
			this.Text = "Export to VPF";
			this.gboxInputFile.ResumeLayout(false);
			this.gboxInputFile.PerformLayout();
			this.gboxOutputOptions.ResumeLayout(false);
			this.gboxOutputOptions.PerformLayout();
			this.gboxBitDepth.ResumeLayout(false);
			this.gboxBitDepth.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gboxInputFile;
		private System.Windows.Forms.Label labelInFile;
		private System.Windows.Forms.TextBox tboxImportFilePath;
		private System.Windows.Forms.Button buttonBrowseImport;
		private System.Windows.Forms.GroupBox gboxOutputOptions;
		private System.Windows.Forms.GroupBox gboxBitDepth;
		private System.Windows.Forms.RadioButton rbutton24BPP;
		private System.Windows.Forms.RadioButton rbutton8BPP;
		private System.Windows.Forms.RadioButton rbutton4BPP;
		private System.Windows.Forms.Button buttonBrowseExport;
		private System.Windows.Forms.TextBox tboxExportFilePath;
		private System.Windows.Forms.Label labelOutFile;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonExport;
	}
}