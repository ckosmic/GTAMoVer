using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Security.Authentication;
using System.Diagnostics;

namespace GTAVModMover {
	public partial class Form2 : Form {
		public Form2() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			Close();
		}

		private void button2_Click(object sender, EventArgs e) {
			using (WebClient client = new WebClient()) {
				ServicePointManager.SecurityProtocol = (SecurityProtocolType)((SslProtocols)0x00000C00);
				client.DownloadFile("https://github.com/ckosmic/GTAMoVer/raw/master/GTAVModMover/bin/Debug/GTAVModMover.exe", "GTAVModMoverUpdated.exe");
				client.DownloadFile("https://github.com/ckosmic/GTAMoVer/raw/master/GTAVModMover/GTAMoVerUpdater.exe", "GTAMoVerUpdater.exe");
				Program.enableMods();
				Process proc = new Process();
				proc.StartInfo.FileName = "GTAMoVerUpdater.exe";
				proc.StartInfo.Arguments = "\"" + Program.basePath + "\" \"" + Program.backupPath + "\"";
				proc.Start();
			}
		}

		private string getLatestUpdateString() {
			using (WebClient client = new WebClient()) {
				string src = client.DownloadString("https://raw.githubusercontent.com/ckosmic/GTAMoVer/master/GTAVModMover/Properties/AssemblyInfo.cs");
				string begin = "[assembly: AssemblyVersion(\"";
				int index = src.IndexOf(begin) + begin.Length;
				int stop = src.IndexOf("\")]\n[assembly: AssemblyFileVersion(");
				string onlineVersion = src.Substring(index, stop - index);
				return onlineVersion;
			}
		}

		private void Form2_Load(object sender, EventArgs e) {
			label2.Text = "New version: " + getLatestUpdateString();
		}

		private void label3_Click(object sender, EventArgs e) {

		}

		private void label2_Click(object sender, EventArgs e) {

		}
	}
}
