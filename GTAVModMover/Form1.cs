using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GTAVModMover {
	public partial class Form1 : Form {

		public List<string> whitelist = new List<string>();

		public Form1() {
			InitializeComponent();
		}

		public void reloadWindow() {
			int detected = modsDetected();
			if (detected > -1) {
				label1.Text = detected + " mod files/folders detected.";
				label1.ForeColor = Color.DarkGreen;
			} else {
				label1.Text = "Unable detect mods.";
				label1.ForeColor = Color.DarkRed;
			}
			textBox1.Text = Program.basePath;
			textBox2.Text = Program.backupPath;
		}

		private void chooseGameDirectory() {
			folderBrowserDialog1.Description = "Choose your GTA root folder...";
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
				Program.changeGameDirectory(folderBrowserDialog1.SelectedPath);
				textBox1.Text = Program.basePath;
			}
		}

		private void chooseModsDirectory() {
			folderBrowserDialog1.Description = "Choose folder to move your mods into...";
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
				Program.changeModsDirectory(folderBrowserDialog1.SelectedPath);
				textBox2.Text = Program.backupPath;
			}
		}

		private void Form1_Load(object sender, EventArgs e) {

			if (Properties.Settings.Default.basePath == "") {
				chooseGameDirectory();
			} else {
				Program.changeGameDirectory(Properties.Settings.Default.basePath);
			}
			textBox1.Text = Program.basePath;
			if (Properties.Settings.Default.modsPath == "") {
				chooseModsDirectory();
			} else {
				Program.changeModsDirectory(Properties.Settings.Default.modsPath);
			}
			textBox2.Text = Program.backupPath;

			whitelist.Add("_CommonRedist");
			whitelist.Add("Installers");
			whitelist.Add("update");
			whitelist.Add("x64");
			whitelist.Add("Redistributables");
			whitelist.Add(".egstore");
			whitelist.Add("ReadMe");
			whitelist.Add("EOSSDK-Win64-Shipping.dll");
			whitelist.Add("GPUPerfAPIDX11-x64.dll");
			whitelist.Add("NvPmApi.Core.win64.dll");
			whitelist.Add("version.txt");
			whitelist.Add("bink2w64.dll");
			whitelist.Add("commandline.txt");
			whitelist.Add("common.rpf");
			whitelist.Add("d3dcompiler_46.dll");
			whitelist.Add("d3dcsx_46.dll");
			whitelist.Add("GFSDK_ShadowLib.win64.dll");
			whitelist.Add("GFSDK_TXAA.win64.dll");
			whitelist.Add("GFSDK_TXAA_AlphaResolve.win64.dll");
			whitelist.Add("GTA5.exe");
			whitelist.Add("GTAVLanguageSelect.exe");
			whitelist.Add("GTAVLauncher.exe");
			whitelist.Add("installscript.vdf");
			whitelist.Add("PlayGTAV.exe");
			whitelist.Add("steam_api64.dll");
			string letters = "abcdefghijklmnopqrstuvwxyz";
			for (int i = 0; i < 26; i++) {
				whitelist.Add("x64" + letters[i] + ".rpf");
			}

			if (!Directory.Exists(Program.basePath)) {
				Program.ConsolePrint("Error: Selected game directory doesn't exist!");
			} else {
				reloadWindow();
			}

			checkForUpdates();
		}

		private int modsDetected() {
			if (Directory.Exists(Program.basePath)) {
				int detected = 0;
				DirectoryInfo dir = new DirectoryInfo(Program.basePath);
				List<FileInfo> files = new List<FileInfo>();
				List<DirectoryInfo> dirs = new List<DirectoryInfo>();
				foreach (FileInfo f in dir.GetFiles("*")) {
					files.Add(f);
				}
				foreach (DirectoryInfo d in dir.GetDirectories("*")) {
					dirs.Add(d);
				}
				foreach (FileInfo f in files) {
					if (!whitelist.Contains(f.Name)) {
						detected++;
					}
				}
				foreach (DirectoryInfo d in dirs) {
					if (!whitelist.Contains(d.Name)) {
						detected++;
					}
				}

				dir = new DirectoryInfo(Path.Combine(Program.basePath, "update/x64/dlcpacks"));
				dirs.Clear();
				foreach (DirectoryInfo d in dir.GetDirectories("*")) {
					dirs.Add(d);
				}
				foreach (DirectoryInfo d in dirs) {
					if (!((d.Name[0] == 'm' && d.Name[1] == 'p') || d.Name.Contains("patchday"))) {
						detected++;
					}
				}

				return detected;
			}
			return -1;
		}

		// Disable mods
		private void button1_Click(object sender, EventArgs e) {
			Program.disableMods();
		}

		// Enable mods
		private void button2_Click(object sender, EventArgs e) {
			Program.enableMods();
		}

		private void button3_Click(object sender, EventArgs e) {
			reloadWindow();
			Program.ConsolePrint("--Refreshed game directory--");
		}

		private void button4_Click(object sender, EventArgs e) {
			chooseGameDirectory();
			reloadWindow();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == (char)Keys.Return) {
				Program.changeGameDirectory(textBox1.Text);
				reloadWindow();
			}
		}

		private void button5_Click(object sender, EventArgs e) {
			chooseModsDirectory();
			reloadWindow();
		}

		private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == (char)Keys.Return) {
				Program.changeModsDirectory(textBox2.Text);
				reloadWindow();
			}
		}

		private void checkForUpdates() {
			using (WebClient client = new WebClient()) {
				string src = client.DownloadString("https://raw.githubusercontent.com/ckosmic/GTAMoVer/master/GTAVModMover/Properties/AssemblyInfo.cs");
				string begin = "[assembly: AssemblyVersion(\"";
				int index = src.IndexOf(begin) + begin.Length;
				int stop = src.IndexOf("\")]\n[assembly: AssemblyFileVersion(");
				Version onlineVersion = new Version(src.Substring(index, stop - index));
				Version version = new Version(Application.ProductVersion);
				if (onlineVersion > version) {
					Program.ConsolePrint("A new version (" + onlineVersion + ") is available.");
					Form2 f2 = new Form2();
					f2.ShowDialog();
				} else if (onlineVersion == version) {
					Program.ConsolePrint("GTAMoVer is up to date.");
				} else {
					Program.ConsolePrint("You have a version ahead?!");
				}
			}
		}
	}
}
