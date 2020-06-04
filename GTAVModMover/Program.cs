using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GTAVModMover {
	static class Program {

		// Game directory
		public static string basePath = "";
		// Folder that mods get moved into
		public static string backupPath = "";
		public static Form1 f1;

		[STAThread]
		static void Main() {
			if (Environment.GetCommandLineArgs().Length > 1) {
				basePath = Environment.GetCommandLineArgs()[1];
				backupPath = Environment.GetCommandLineArgs()[2];
			}

			if(basePath != "")
				Properties.Settings.Default.basePath = basePath;
			if(backupPath != "")
				Properties.Settings.Default.modsPath = backupPath;

			File.Delete("GTAMoVerUpdater.exe");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			f1 = new Form1();
			f1.Text = "GTAMoVer - v" + Application.ProductVersion;
			Application.Run(f1);
		}

		public static void ConsolePrint(string str) {
			Console.WriteLine(str);
			f1.consoleTextBox.Text += str + "\r\n";
			f1.consoleTextBox.Select(f1.consoleTextBox.Text.Length, 0);
			f1.consoleTextBox.ScrollToCaret();
		}

		public static void changeGameDirectory(string path) {
			Properties.Settings.Default.basePath = path;
			Properties.Settings.Default.Save();
			basePath = path;
			if (Directory.Exists(path)) {
				ConsolePrint("Set game directory: " + basePath);
			} else {
				ConsolePrint("Error: Selected game directory doesn't exist!");
			}
		}

		public static void changeModsDirectory(string path) {
			Properties.Settings.Default.modsPath = path;
			Properties.Settings.Default.Save();
			backupPath = path;
			if (Directory.Exists(path)) {
				ConsolePrint("Set moved mods directory: " + backupPath);
			} else {
				ConsolePrint("Error: Selected directory doesn't exist!");
			}
		}

		public static void disableMods() {
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
					if (!f1.whitelist.Contains(f.Name)) {
						ConsolePrint("Moving file '" + f.Name + "' to moved mods directory.");
						string dest = Path.Combine(backupPath, f.Name);
						File.Move(f.FullName, dest);
					}
				}
				foreach (DirectoryInfo d in dirs) {
					if (!f1.whitelist.Contains(d.Name)) {
						ConsolePrint("Moving directory '" + d.Name + "' to moved mods directory.");
						string dest = Path.Combine(backupPath, d.Name);
						Directory.Move(d.FullName, dest);
					}
				}

				dir = new DirectoryInfo(Path.Combine(basePath, "update/x64/dlcpacks"));
				dirs.Clear();
				foreach (DirectoryInfo d in dir.GetDirectories("*")) {
					dirs.Add(d);
				}
				foreach (DirectoryInfo d in dirs) {
					if (!((d.Name[0] == 'm' && d.Name[1] == 'p') || d.Name.Contains("patchday"))) {
						if (!Directory.Exists(Path.Combine(backupPath, "update/x64/dlcpacks")))
							Directory.CreateDirectory(Path.Combine(backupPath, "update/x64/dlcpacks"));
						ConsolePrint("Moving dlcpacks directory '" + d.Name + "' to moved mods directory.");
						string dest = Path.Combine(backupPath, Path.Combine("update/x64/dlcpacks", d.Name));
						Directory.Move(d.FullName, dest);
					}
				}

				f1.reloadWindow();
				ConsolePrint("--Disabling operation completed--");
			} else {
				ConsolePrint("Error: One of your paths does not exist!");
			}
		}

		public static void enableMods() {
			if (Directory.Exists(basePath) && Directory.Exists(backupPath)) {
				ConsolePrint("--Starting enabling operation--");
				DirectoryInfo dir = new DirectoryInfo(backupPath);
				foreach (FileInfo f in dir.GetFiles("*")) {
					string dest = Path.Combine(basePath, f.Name);
					ConsolePrint("Moving file '" + f.Name + "' to game directory.");
					File.Move(f.FullName, dest);
				}
				foreach (DirectoryInfo d in dir.GetDirectories("*")) {
					if (d.Name != "update") {
						string dest = Path.Combine(basePath, d.Name);
						ConsolePrint("Moving directory '" + d.Name + "' to game directory.");
						Directory.Move(d.FullName, dest);
					} else {
						DirectoryInfo dlcDir = new DirectoryInfo(Path.Combine(backupPath, "update/x64/dlcpacks"));
						foreach (DirectoryInfo c in dlcDir.GetDirectories("*")) {
							string dest = Path.Combine(basePath, Path.Combine("update/x64/dlcpacks", c.Name));
							ConsolePrint("Moving dlcpacks directory '" + c.Name + "' to game directory.");
							Directory.Move(c.FullName, dest);
						}
						Directory.Delete(d.FullName, true);
					}
				}
				f1.reloadWindow();
				ConsolePrint("--Enabling operation completed--");
			} else {
				ConsolePrint("Error: One of your paths does not exist!");
			}
		}
	}
}
