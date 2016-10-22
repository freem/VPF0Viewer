﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VPF0toPNG
{
	public partial class Form1 : Form
	{
		public Color[] imagePalette;
		public byte[] imageData;
		public Bitmap imageBitmap;
		public int imageWidth, imageHeight, imageBytes, imageBPP;
		public string originalFilename;

		public Form1() {
			InitializeComponent();
			this.Icon = VPF0toPNG.Properties.Resources.vpf0viewer;
			saveToolStripMenuItem.Enabled = false; // don't let people try and be cute

			/* our current assumptions are the following: */
			imagePalette = new Color[256]; // VPF0 files are 8bpp, and therefore have 256 colors.
			// files can be any size, though, compared to logos (which are fixed at 128x128)

			pboxPreview.BackColor = Color.FromArgb(127, 0, 0, 0);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
			MessageBox.Show("Fire Pro Wrestling Returns VPF0 Viewer and Converter v0.02 (2016/10/22) by freem\nVisit http://firepro.ajworld.net/ for more information.", "About VPF0 Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e) {
			/* open yon file */
			OpenFileDialog ofd;
			ofd = new OpenFileDialog();
			ofd.Title = "Select VPF0 File...";
			ofd.Filter = "VPF0 File|*.vpf|All Files|*.*";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				OpenVPF0File(ofd.FileName);
			}
		}

		/*
		 * saveToolStripMenuItem_Click(object, EventArgs)
		 * Performs the export to PNG.
		 */
		private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
			/* save yon file */
			SaveFileDialog sfd;
			sfd = new SaveFileDialog();
			sfd.Title = "Export PNG...";
			sfd.Filter = "PNG|*.png"; // force PNG only, since VPF0 format can have varying alphas
			sfd.FileName = Path.GetFileNameWithoutExtension(originalFilename); // suggest a filename based on .vpf's filename
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				imageBitmap.Save(sfd.FileName);
			}
		}

		/*
		 * exitToolStripMenuItem_Click(object, EventArgs)
		 * Get the heck out of [this program], you nerd!
		 */
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			this.Close();
		}

		/*
		 * OpenVPF0File(string)
		 * Handles the dirty work of opening a VPF0 file.
		 */
		private void OpenVPF0File(string filePath){
			/* ugh alright, let's do some surgery. */
			FileStream vpf0File = new FileStream(filePath, FileMode.Open);
			BinaryReader br = new BinaryReader(vpf0File);
			if(vpf0File == null){
				MessageBox.Show(String.Format("Unable to open {0}", filePath), "File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			/* find size of data */
			vpf0File.Seek(0x04, SeekOrigin.Begin);
			imageBytes = br.ReadInt32();

			/* get width and height */
			vpf0File.Seek(0x0A, SeekOrigin.Begin);
			imageWidth = br.ReadInt16();
			imageHeight = br.ReadInt16();

			/* get color depth */
			imageBPP = br.ReadInt16();

			if (imageBPP == 8) {
				/* do the palette evolution*/
				ReadPalette(vpf0File, br);
				vpf0File.Seek(0x0420, SeekOrigin.Begin); // xxx: assumption based on 8bpp files having 256 palette entries
			}
			else {
				/* assuming 24bpp... might be a bad idea if other formats exist */
				vpf0File.Seek(0x20, SeekOrigin.Begin);
			}

			/* read data into imageData */
			imageData = new byte[imageBytes];
			vpf0File.Read(imageData, 0, imageBytes);

			/* finished with file handle, release it. dunno if I need this double free, but... */
			br.Close();
			vpf0File.Close();

			/* do conversion of imageData to bitmap data */
			if (imageBPP == 8) {
				ImageToBitmap_8BPP();
			}
			else {
				ImageToBitmap_24BPP();
			}

			/* throw the open filename in titlebar */
			originalFilename = Path.GetFileName(filePath);
			this.Text = String.Format("VPF0 Viewer - {0}", originalFilename);
			statusBarLabel.Text = String.Format("{0} | {1}bpp", originalFilename,imageBPP);

			/* resize window automatically for bigger images */
			if (imageWidth > this.Size.Width-32 || imageHeight > this.Size.Height-96) {
				this.Size = new Size(imageWidth+32,imageHeight+96);
			}

			// re-enable saving, because imageBitmap is guaranteed to not be null anymore... I hope.
			saveToolStripMenuItem.Enabled = true;
		}

		/*
		 * ReadPalette(FileStream, BinaryReader) 
		 * Reads the palette in for an 8bpp image. Assumes a full 256 colors in the palette table.
		 */
		private void ReadPalette(FileStream _fs, BinaryReader _br) {
			_fs.Seek(0x20, SeekOrigin.Begin);
			byte[] tempData = new byte[4];
			for (int i = 0; i < 256; i++) {
				tempData = _br.ReadBytes(4);
				/* multiply alpha by 2; assumption that may not be true... */
				tempData[3] = (byte)Math.Min((int)((tempData[3] + 1) * 2), 255);
				imagePalette[i] = Color.FromArgb(tempData[3],tempData[0],tempData[1],tempData[2]);
			}
		}

		/*
		 * ImageToBitmap_8BPP()
		 * Converts the imageData to the imageBitmap for 8BPP images.
		 */
		private void ImageToBitmap_8BPP() {
			/* prepare bitmap */
			imageBitmap = new Bitmap(imageWidth, imageHeight);
			/* convert data */
			for(int y = 0; y < imageHeight; y++){
				for (int x = 0; x < imageWidth; x++) {
					int pixelLoc = (y * imageWidth) + x;
					imageBitmap.SetPixel(x,y,imagePalette[imageData[pixelLoc]]);
				}
			}
			/* display data */
			this.pboxPreview.Image = imageBitmap;
		}

		/*
		 * ImageToBitmap_24BPP()
		 * Converts the imageData to the imageBitmap for 24BPP images.
		 */
		private void ImageToBitmap_24BPP() {
			/* prepare bitmap */
			imageBitmap = new Bitmap(imageWidth, imageHeight);
			/* convert data... each group of three bytes represents one pixel. */
			int dataCounter = 0;
			for(int y = 0; y < imageHeight; y++){
				for (int x = 0; x < imageWidth; x++) {
					imageBitmap.SetPixel(x, y, Color.FromArgb(255, imageData[dataCounter], imageData[dataCounter + 1], imageData[dataCounter+2]));
					dataCounter += 3;
				}
			}
			/* display data */
			this.pboxPreview.Image = imageBitmap;
		}

		/*
		 * Form1_DragDrop(object, DragEventArgs)
		 * Event run after the item is dropped onto the window.
		 */
		private void Form1_DragDrop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
				// only load the first file
				string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (filePaths.Length == 1) {
					OpenVPF0File(filePaths[0]);
				}
			}
		}

		/*
		 * Form1_DragEnter(object, DragEventArgs)
		 * Event run when an object is dragged into the window, but before being dropped.
		 */
		private void Form1_DragEnter(object sender, DragEventArgs e) {
			// only allow a single file at a time
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && ((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1) {
				e.Effect = DragDropEffects.Link;
			}
			else {
				e.Effect = DragDropEffects.None;
			}
		}
	}
}
