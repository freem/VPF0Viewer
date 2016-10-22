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
		public int imageWidth, imageHeight, imageBytes;

		public Form1() {
			InitializeComponent();
			this.Icon = VPF0toPNG.Properties.Resources.vpf0viewer;

			/* our current assumptions are the following: */
			imagePalette = new Color[256]; // VPF0 files are 8bpp, and therefore have 256 colors.
			// files can be any size, though, compared to logos (which are fixed at 128x128)

			pboxPreview.BackColor = Color.FromArgb(127, 0, 0, 0);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
			MessageBox.Show("Fire Pro Wrestling Returns VPF0 Viewer and Converter v0.01 (2016/10/22) by freem\nVisit http://firepro.ajworld.net/ for more information.", "About VPF0 Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

		private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
			/* save yon file */
			SaveFileDialog sfd;
			sfd = new SaveFileDialog();
			sfd.Title = "Export PNG...";
			sfd.Filter = "PNG|*.png";
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				imageBitmap.Save(sfd.FileName);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			this.Close();
		}

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

			/* color depth??? is assumed to be 8, but I don't know what the value right after width and height is for.
			 * it's possible that the VPF0 format can support bit depths greater than 8, but I don't know how they're
			 * handled, since I haven't found one yet. */

			/* do the palette evolution */
			ReadPalette(vpf0File, br);

			/* read data into imageData */
			imageData = new byte[imageBytes];
			vpf0File.Seek(0x0420, SeekOrigin.Begin); // xxx: assumption based on 8bpp/256-color palette images
			vpf0File.Read(imageData, 0, imageBytes);

			/* finished with file handle, release it. dunno if I need this double free, but... */
			br.Close();
			vpf0File.Close();

			/* do conversion of imageData to bitmap data */
			ImageToBitmap();

			/* resize window automatically for bigger images */
			if (imageWidth > this.Size.Width && imageHeight > this.Size.Height) {
				this.Size = new Size(imageWidth+32,imageHeight+96);
			}
		}

		private void ReadPalette(FileStream _fs, BinaryReader _br) {
			_fs.Seek(0x20, SeekOrigin.Begin);
			byte[] tempData = new byte[4];
			for (int i = 0; i < 256; i++) {
				tempData = _br.ReadBytes(4);
				/* multiply alpha by 2; assumption that may not be true... */
				tempData[3] = (byte)Math.Min((int)tempData[3] * 2, 255);
				imagePalette[i] = Color.FromArgb(tempData[3],tempData[0],tempData[1],tempData[2]);
			}
		}

		private void ImageToBitmap() {
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
	}
}
