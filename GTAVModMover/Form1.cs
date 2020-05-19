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
		public string basePath;
		public string backupPath;

		public Form1() {
			InitializeComponent();
		}

		private void reloadWindow() {
			int detected = modsDetected();
			if (detected > -1) {
				label1.Text = detected + " mod files/folders detected.";
				label1.ForeColor = Color.DarkGreen;
			} else {
				label1.Text = "Unable detect mods.";
				label1.ForeColor = Color.DarkRed;
			}
		}

		private void changeGameDirectory(string path) {
			Properties.Settings.Default.basePath = path;
			Properties.Settings.Default.Save();
			basePath = path;
			if (Directory.Exists(path)) {
				ConsolePrint("Set game directory: " + basePath);
				textBox1.Text = basePath;
			} else {
				ConsolePrint("Error: Selected game directory doesn't exist!");
				textBox1.Text = basePath;
			}
		}

		private void chooseGameDirectory() {
			folderBrowserDialog1.Description = "Choose your GTA root folder...";
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
				changeGameDirectory(folderBrowserDialog1.SelectedPath);
			}
		}

		private void changeModsDirectory(string path) {
			Properties.Settings.Default.modsPath = path;
			Properties.Settings.Default.Save();
			backupPath = path;
			if (Directory.Exists(path)) {
				ConsolePrint("Set moved mods directory: " + backupPath);
				textBox2.Text = backupPath;
			} else {
				ConsolePrint("Error: Selected directory doesn't exist!");
				textBox2.Text = backupPath;
			}
		}

		private void chooseModsDirectory() {
			folderBrowserDialog1.Description = "Choose folder to move your mods into...";
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
				changeModsDirectory(folderBrowserDialog1.SelectedPath);
			}
		}

		private void Form1_Load(object sender, EventArgs e) {

			if (Properties.Settings.Default.basePath == "") {
				chooseGameDirectory();
			} else {
				changeGameDirectory(Properties.Settings.Default.basePath);
			}
			if (Properties.Settings.Default.modsPath == "") {
				chooseModsDirectory();
			} else {
				changeModsDirectory(Properties.Settings.Default.modsPath);
			}

			whitelist.Add("_CommonRedist");
			whitelist.Add("Installers");
			whitelist.Add("update");
			whitelist.Add("x64");
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

			if (!Directory.Exists(basePath)) {
				ConsolePrint("Error: Selected game directory doesn't exist!");
			} else {
				reloadWindow();
			}

			checkForUpdates();
		}

		private int modsDetected() {
			if (Directory.Exists(basePath)) {
				int detected = 0;
				DirectoryInfo dir = new DirectoryInfo(basePath);
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
				return detected;
			}
			return -1;
		}

		// Disable mods
		private void button1_Click(object sender, EventArgs e) {
			if (Directory.Exists(basePath) && Directory.Exists(backupPath)) {
				ConsolePrint("--Starting disabling operation--");
				DirectoryInfo dir = new DirectoryInfo(basePath);
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
						ConsolePrint("Moving file '" + f.Name + "' to moved mods directory.");
						string dest = Path.Combine(backupPath, f.Name);
						File.Move(f.FullName, dest);
					}
				}
				foreach (DirectoryInfo d in dirs) {
					if (!whitelist.Contains(d.Name)) {
						ConsolePrint("Moving directory '" + d.Name + "' to moved mods directory.");
						string dest = Path.Combine(backupPath, d.Name);
						Directory.Move(d.FullName, dest);
					}
				}
				reloadWindow();
				ConsolePrint("--Disabling operation completed--");
			} else {
				ConsolePrint("Error: One of your paths does not exist!");
			}
		}

		// Enable mods
		private void button2_Click(object sender, EventArgs e) {
			if (Directory.Exists(basePath) && Directory.Exists(backupPath)) {
				ConsolePrint("--Starting enabling operation--");
				DirectoryInfo dir = new DirectoryInfo(backupPath);
				foreach (FileInfo f in dir.GetFiles("*")) {
					string dest = Path.Combine(basePath, f.Name);
					ConsolePrint("Moving file '" + f.Name + "' to game directory.");
					File.Move(f.FullName, dest);
				}
				foreach (DirectoryInfo d in dir.GetDirectories("*")) {
					string dest = Path.Combine(basePath, d.Name);
					ConsolePrint("Moving directory '" + d.Name + "' to game directory.");
					Directory.Move(d.FullName, dest);
				}
				reloadWindow();
				ConsolePrint("--Enabling operation completed--");
			} else {
				ConsolePrint("Error: One of your paths does not exist!");
			}
		}

		private void ConsolePrint(string str) {
			Console.WriteLine(str);
			consoleTextBox.Text += str + "\r\n";
			consoleTextBox.Select(consoleTextBox.Text.Length, 0);
			consoleTextBox.ScrollToCaret();
		}

		private void button3_Click(object sender, EventArgs e) {
			reloadWindow();
			ConsolePrint("--Refreshed game directory--");
		}

		private void button4_Click(object sender, EventArgs e) {
			chooseGameDirectory();
			reloadWindow();
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == (char)Keys.Return) {
				changeGameDirectory(textBox1.Text);
				reloadWindow();
			}
		}

		private void button5_Click(object sender, EventArgs e) {
			chooseModsDirectory();
			reloadWindow();
		}

		private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == (char)Keys.Return) {
				changeModsDirectory(textBox2.Text);
				reloadWindow();
			}
		}

		private void checkForUpdates() {
			//https://raw.githubusercontent.com/ckosmic/GTAMoVer/master/GTAVModMover/Form1.cs
			//https://github.com/ckosmic/GTAMoVer/raw/master/GTAVModMover/bin/Debug/GTAVModMover.exe
			using (WebClient client = new WebClient()) {
				string src = client.DownloadString("https://raw.githubusercontent.com/ckosmic/GTAMoVer/master/GTAVModMover/Properties/AssemblyInfo.cs");
				int index = src.IndexOf("[assembly: AssemblyVersion(");
				int stop = src.IndexOf("[assembly: AssemblyFileVersion(");
				ConsolePrint(src.Substring(index, stop - index));
			}
		}
	}
}
