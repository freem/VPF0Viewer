using System;
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

			// don't let people try and be cute
			saveToolStripMenuItem.Enabled = false;
			colorTableToolStripMenuItem.Enabled = false;
			saveImagePaletteToolStripMenuItem.Enabled = false;

			pboxPreview.BackColor = Color.FromArgb(127, 0, 0, 0);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
			MessageBox.Show("Fire Pro Wrestling Returns VPF0 Viewer and Converter v0.0.3 (2016/1x/xx) by freem\nVisit http://firepro.ajworld.net/ for more information.", "About VPF0 Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

			/* todo: apparently VPF0 supports 4bpp, 8bpp, and 24bpp */
			if (imageBPP == 4){
				MessageBox.Show("4BPP reading is slightly broken, sorry.", "Error opening 4BPP format file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
				/*
				imagePalette = new Color[16];
				ReadPalette_4BPP(vpf0File, br);
				colorTableToolStripMenuItem.Enabled = true;
				saveImagePaletteToolStripMenuItem.Enabled = true;
				vpf0File.Seek(0x60, SeekOrigin.Begin);
				*/
			}
			else if (imageBPP == 8) {
				imagePalette = new Color[256];
				ReadPalette_8BPP(vpf0File, br);
				colorTableToolStripMenuItem.Enabled = true;
				saveImagePaletteToolStripMenuItem.Enabled = true;
				// this location is an assumption based on 256 colors:
				vpf0File.Seek(0x0420, SeekOrigin.Begin);
			}
			else {
				/* assuming 24bpp... might be a bad idea if other in-between formats exist */
				colorTableToolStripMenuItem.Enabled = false;
				saveImagePaletteToolStripMenuItem.Enabled = false;
				vpf0File.Seek(0x20, SeekOrigin.Begin);
			}

			/* read data into imageData */
			imageData = new byte[imageBytes];
			vpf0File.Read(imageData, 0, imageBytes);

			/* finished with file handle, release it. dunno if I need this double free, but... */
			br.Close();
			vpf0File.Close();

			/* do conversion of imageData to bitmap data */
			if (imageBPP == 24) {
				ImageToBitmap_24BPP();
			}
			else if(imageBPP == 8) {
				ImageToBitmap_8BPP();
			}
			else if (imageBPP == 4) {
				ImageToBitmap_4BPP();
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
		 * ReadPaletteSection(FileStream, BinaryReader, int)
		 * Handles reading a partial chunk (8 colors) of the palette.
		 */
		private void ReadPaletteSection(FileStream _fs, BinaryReader _br, int _start){
			byte[] tempData = new byte[4];
			for(int i = _start; i < _start+8; i++){
				tempData = _br.ReadBytes(4);
				// multiply alpha by 2; an assumption that may not be true...
				tempData[3] = (byte)Math.Min((int)((tempData[3] + 1) * 2), 255);
				imagePalette[i] = Color.FromArgb(tempData[3],tempData[0],tempData[1],tempData[2]);
			}
		}

		/*
		 * ReadPalette_4BPP(FileStream, BinaryReader)
		 * Reads the palette in for a 4bpp image. Assumes 16 colors in the palette.
		 */
		private void ReadPalette_4BPP(FileStream _fs, BinaryReader _br){
			_fs.Seek(0x20, SeekOrigin.Begin);
			byte[] tempData = new byte[4];
			for (int i = 0; i < 16; i++) {
				tempData = _br.ReadBytes(4);
				// multiply alpha by 2; an assumption that may not be true...
				tempData[3] = (byte)Math.Min((int)((tempData[3] + 1) * 2), 255);
				imagePalette[i] = Color.FromArgb(tempData[3], tempData[0], tempData[1], tempData[2]);
			}
		}

		/*
		 * ReadPalette_8BPP(FileStream, BinaryReader) 
		 * Reads the palette in for an 8bpp image. Assumes a full 256 colors in the palette table.
		 */
		private void ReadPalette_8BPP(FileStream _fs, BinaryReader _br) {
			_fs.Seek(0x20, SeekOrigin.Begin);
			// 256 colors
			ReadPaletteSection(_fs,_br,0x00);
			ReadPaletteSection(_fs,_br,0x10);
			ReadPaletteSection(_fs,_br,0x08);
			ReadPaletteSection(_fs,_br,0x18);
			ReadPaletteSection(_fs,_br,0x20);
			ReadPaletteSection(_fs,_br,0x30);
			ReadPaletteSection(_fs,_br,0x28);
			ReadPaletteSection(_fs,_br,0x38);
			ReadPaletteSection(_fs,_br,0x40);
			ReadPaletteSection(_fs,_br,0x50);
			ReadPaletteSection(_fs,_br,0x48);
			ReadPaletteSection(_fs,_br,0x58);
			ReadPaletteSection(_fs,_br,0x60);
			ReadPaletteSection(_fs,_br,0x70);
			ReadPaletteSection(_fs,_br,0x68);
			ReadPaletteSection(_fs,_br,0x78);
			ReadPaletteSection(_fs,_br,0x80);
			ReadPaletteSection(_fs,_br,0x90);
			ReadPaletteSection(_fs,_br,0x88);
			ReadPaletteSection(_fs,_br,0x98);
			ReadPaletteSection(_fs,_br,0xA0);
			ReadPaletteSection(_fs,_br,0xB0);
			ReadPaletteSection(_fs,_br,0xA8);
			ReadPaletteSection(_fs,_br,0xB8);
			ReadPaletteSection(_fs,_br,0xC0);
			ReadPaletteSection(_fs,_br,0xD0);
			ReadPaletteSection(_fs,_br,0xC8);
			ReadPaletteSection(_fs,_br,0xD8);
			ReadPaletteSection(_fs,_br,0xE0);
			ReadPaletteSection(_fs,_br,0xF0);
			ReadPaletteSection(_fs,_br,0xE8);
			ReadPaletteSection(_fs,_br,0xF8);
		}

		/*
		 * ImageToBitmap_4BPP()
		 * Converts the imageData to the imageBitmap for 4BPP images.
		 */
		private void ImageToBitmap_4BPP() {
			/* prepare bitmap */
			imageBitmap = new Bitmap(imageWidth, imageHeight);
			/* convert data */
			for (int y = 0; y < imageHeight; y++) {
				for (int x = 0; x < imageWidth; x+=2) {
					// this is annoying, because the pixels are packed in nybbles.
					int pixelLoc = (y * (imageWidth/2)) + (x/2);
					byte packedPixel = imageData[pixelLoc];
					imageBitmap.SetPixel(x, y, imagePalette[(packedPixel & 0xF0) >> 8]);
					imageBitmap.SetPixel(x+1, y, imagePalette[(packedPixel & 0x0F)]);
				}
			}
			/* display data */
			this.pboxPreview.Image = imageBitmap;
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

		private void colorTableToolStripMenuItem_Click(object sender, EventArgs e) {
			// load color table item with existing palette
		}

		private void saveImagePaletteToolStripMenuItem_Click(object sender, EventArgs e) {
			// currently unimplemented
		}

		private void exportGIFAsVPFToolStripMenuItem_Click(object sender, EventArgs e) {
			// cool, now we get to play the money making game.
			ExportToVPF exportForm = new ExportToVPF();
			exportForm.Show();
		}
	}
}
