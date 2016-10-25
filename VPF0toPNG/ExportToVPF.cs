using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;

namespace VPF0toPNG
{
	public partial class ExportToVPF : Form
	{
		public string importPath, exportPath;
		public int targetWidth, targetHeight, targetBitDepth;

		public ExportToVPF() {
			InitializeComponent();

			// do some prep
		}

		private void buttonBrowseImport_Click(object sender, EventArgs e) {
			//OpenFileDialog, but don't actually open the file yet
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select GIF to Convert";
			//ofd.Filter = "PNG files|*.png";
			ofd.Filter = "GIF files|*.gif";
			ofd.CheckFileExists = true;
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				tboxImportFilePath.Text = ofd.FileName;
			}
		}

		private void buttonBrowseExport_Click(object sender, EventArgs e) {
			//SaveFileDialog, but don't actually save the file yet
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Select VPF to Export";
			sfd.Filter = "VPF files|*.vpf";
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				tboxExportFilePath.Text = sfd.FileName;
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void buttonExport_Click(object sender, EventArgs e) {
			// guest starring "we do dee" and "dir t. work"

			#region Sanity Checks
			// blank import file sanity check
			if(tboxImportFilePath.Text == ""){
				MessageBox.Show("Must choose GIF file to convert!", "Invalid Input File", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// blank export file sanity check
			if (tboxExportFilePath.Text == "") {
				MessageBox.Show("Must provide output VPF filename!", "Invalid Output File", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// xxx: currently disable 4bpp because I don't have proper reading code, much less writing!
			if (rbutton4BPP.Checked) {
				MessageBox.Show("Dunno how you managed this, but 4BPP export is not yet supported.", "4BPP Output not yet supported", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			#endregion

			/* ok then, I can bring along my friends to the work party. */
			#region Read input file
			// this assumes the checks were passed above.
			importPath = tboxImportFilePath.Text;
			exportPath = tboxExportFilePath.Text;

			FileStream gifStream = new FileStream(importPath,FileMode.Open,FileAccess.Read,FileShare.Read);
			GifBitmapDecoder gifDecoder = new GifBitmapDecoder(gifStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			BitmapSource inputSource = gifDecoder.Frames[0];
			Bitmap gifBitmap = new Bitmap(gifStream);

			targetWidth = inputSource.PixelWidth;
			targetHeight = inputSource.PixelHeight;

			// bit depth depends on one of the rbutton*BPP.Checked values
			if (rbutton24BPP.Checked) {
				// 24bpp is straightforward, just throw the color values and go
				targetBitDepth = 24;
			}
			else if(rbutton8BPP.Checked) {
				// 8bpp needs palette entries in an awkward order
				// then it needs palette entries representing the pixels
				targetBitDepth = 8;
			}
			else if (rbutton4BPP.Checked) {
				// do nothing yet; should be similar to 4bpp but no?
				targetBitDepth = 4;
			}
			#endregion

			#region Write VPF file
			// let's deal with the output file
			FileStream vpfOut = new FileStream(exportPath,FileMode.Create,FileAccess.Write,FileShare.None);
			BinaryWriter bw = new BinaryWriter(vpfOut);

			// header magic
			bw.Write('V');
			bw.Write('P');
			bw.Write('F');
			bw.Write('0');

			// output file size (dependent on bpp and image size)
			if (targetBitDepth == 24) {
				// width*height*3
				bw.Write((UInt32)(targetWidth * targetHeight * 3));
			}
			else if (targetBitDepth == 8) {
				//width*height
				bw.Write((UInt32)(targetWidth * targetHeight));
			}
			else if (targetBitDepth == 4) {
				//width*height
				bw.Write((UInt32)0);
			}

			// unknown crap
			bw.Write((UInt16)0x0013);

			// image width
			bw.Write((UInt16)targetWidth);

			// image height
			bw.Write((UInt16)targetHeight);

			// color depth
			bw.Write((UInt16)targetBitDepth);

			// xxx: these junk things are for 8bpp
			// junk1 (1024 in BCD)
			bw.Write((byte)0x20);
			bw.Write((byte)0x04);
			bw.Write((byte)0x00);
			bw.Write((byte)0x01);

			// junk2
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);
			bw.Write((byte)0x04);

			// junk3
			bw.Write((byte)0x20);
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);

			// junk4
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);
			bw.Write((byte)0x00);

			if (targetBitDepth == 24) {
				// if this is 24bpp, just output the raw color data.
				for (int y = 0; y < targetHeight; y++) {
					for (int x = 0; x < targetWidth; x++) {
						Color tempColor = gifBitmap.GetPixel(x, y);
						bw.Write((byte)tempColor.R);
						bw.Write((byte)tempColor.G);
						bw.Write((byte)tempColor.B);
					}
				}
			}
			else if (targetBitDepth == 8) {
				// color table (256 colors, 4 channels per color)
				WritePaletteSection(inputSource, bw, 0x00);
				WritePaletteSection(inputSource, bw, 0x10);
				WritePaletteSection(inputSource, bw, 0x08);
				WritePaletteSection(inputSource, bw, 0x18);
				WritePaletteSection(inputSource, bw, 0x20);
				WritePaletteSection(inputSource, bw, 0x30);
				WritePaletteSection(inputSource, bw, 0x28);
				WritePaletteSection(inputSource, bw, 0x38);
				WritePaletteSection(inputSource, bw, 0x40);
				WritePaletteSection(inputSource, bw, 0x50);
				WritePaletteSection(inputSource, bw, 0x48);
				WritePaletteSection(inputSource, bw, 0x58);
				WritePaletteSection(inputSource, bw, 0x60);
				WritePaletteSection(inputSource, bw, 0x70);
				WritePaletteSection(inputSource, bw, 0x68);
				WritePaletteSection(inputSource, bw, 0x78);
				WritePaletteSection(inputSource, bw, 0x80);
				WritePaletteSection(inputSource, bw, 0x90);
				WritePaletteSection(inputSource, bw, 0x88);
				WritePaletteSection(inputSource, bw, 0x98);
				WritePaletteSection(inputSource, bw, 0xA0);
				WritePaletteSection(inputSource, bw, 0xB0);
				WritePaletteSection(inputSource, bw, 0xA8);
				WritePaletteSection(inputSource, bw, 0xB8);
				WritePaletteSection(inputSource, bw, 0xC0);
				WritePaletteSection(inputSource, bw, 0xD0);
				WritePaletteSection(inputSource, bw, 0xC8);
				WritePaletteSection(inputSource, bw, 0xD8);
				WritePaletteSection(inputSource, bw, 0xE0);
				WritePaletteSection(inputSource, bw, 0xF0);
				WritePaletteSection(inputSource, bw, 0xE8);
				WritePaletteSection(inputSource, bw, 0xF8);

				// image data
				// xxx: this routine is SLOWWWWWWWWWWW
				for (int y = 0; y < targetHeight; y++) {
					for (int x = 0; x < targetWidth; x++) {
						Color tempColor = gifBitmap.GetPixel(x,y);
						int colorIndex = 0;
						foreach (System.Windows.Media.Color c in inputSource.Palette.Colors) {
							if (tempColor.R == c.R && tempColor.G == c.G && tempColor.B == c.B) {
								break;
							}
							colorIndex++;
						}
						bw.Write((byte)colorIndex);
					}
				}
			}
			else if (targetBitDepth == 4) {
				// color table (16 colors, 4 channels per color)
				// ja ja ja
			}

			// done with files
			gifStream.Close();
			bw.Close();
			vpfOut.Close();
			#endregion

			// if this worked, close it
			MessageBox.Show(String.Format("Successfully exported {0} to VPF format.",Path.GetFileName(importPath)),"Export Successful!");
			this.Close();
		}

		private void WritePaletteSection(BitmapSource _source, BinaryWriter _bw, int _start) {
			for (int i = _start; i < _start + 8; i++) {
				System.Windows.Media.Color temp = _source.Palette.Colors[i];
				_bw.Write((byte)temp.R); // red
				_bw.Write((byte)temp.G); // green
				_bw.Write((byte)temp.B); // blue
				_bw.Write((byte)0x7F); // normally alpha
			}
		}
	}
}
