using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

namespace Phabricator
{
    public partial class InstallArcanistForm : Form
    {
        private Thread _installThread;

        public InstallArcanistForm()
        {
            InitializeComponent();
        }

        private void InstallArcanistForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_installThread != null)
                _installThread.Abort();
            _installThread = null;
        }

        private void SetProgress(string message)
        {
            if (_installButton.InvokeRequired)
            {
                _installButton.Invoke(new Action(() =>
                    {
                    _installButton.Enabled = false;
                    _installLabel.Text = message;
                    }));
            }
            else
            {
                _installButton.Enabled = false;
                _installLabel.Text = message;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout ( IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult );
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int WM_SETTINGCHANGE = 0x1a;
        private const int SMTO_ABORTIFHUNG = 0x0002;

        private void NotifyProgramsOfUpdatedPath()
        {
            // FIXME: This doesn't seem to work properly?
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, null, SMTO_ABORTIFHUNG, 10000, IntPtr.Zero);
        }

        private void SetComplete()
        {
            if (_installButton.InvokeRequired)
            {
                _installButton.Invoke(new Action(() =>
                {
                    _installThread = null;
                    _installButton.Enabled = true;
                    _installButton.Text = "Close";
                    _installLabel.Text = "Installation complete.";
                }));
            }
            else
            {
                _installThread = null;
                _installButton.Enabled = true;
                _installButton.Text = "Close";
                _installLabel.Text = "Installation complete.";
            }
        }

        private void _installButton_Click(object sender, EventArgs e)
        {
            if (_installButton.Text == "Close")
            {
                Close();
                return;
            }
            if (_installThread == null)
            {
                _installThread = new Thread(PerformInstall);
                _installThread.IsBackground = true;
                _installThread.Start();
            }
        }

        private void PerformInstall()
        {
            // NOTE: This will only work under Windows, and access to this form
            // is entirely disabled by PhabricatorMenus.

            SetProgress("Checking for administrator permissions...");

            try
            {
                var regtest = Registry.LocalMachine
                    .OpenSubKey("System", true)
                    .OpenSubKey("CurrentControlSet", true)
                    .OpenSubKey("Control", true)
                    .OpenSubKey("Session Manager", true)
                    .OpenSubKey("Environment", true);
                regtest.SetValue("Path", regtest.GetValue("Path"), RegistryValueKind.ExpandString);
            }
            catch (SecurityException)
            {
                SetProgress("ERROR: Run Git Extensions as administrator and try again.");
                return;
            }
            
            // Download and extract PHP and then proceed.
            PerformExtractPHP(() =>
            {
                // Now make sure PHP is in the PATH.
                SetProgress("Adding PHP to your PATH...");
                var regkey = Registry.LocalMachine
                    .OpenSubKey("System", true)
                    .OpenSubKey("CurrentControlSet", true)
                    .OpenSubKey("Control", true)
                    .OpenSubKey("Session Manager", true)
                    .OpenSubKey("Environment", true);
                var path = regkey.GetValue("Path") as string;
                if (!path.ToLowerInvariant().Contains(";c:\\php\\"))
                {
                    path += ";C:\\PHP\\";
                    regkey.SetValue("Path", path, RegistryValueKind.ExpandString);
                }

                // TODO: Git clone arcanist and libphutil, then add them to the
                // environment PATH as well.

                // Mark the installation as complete.
                NotifyProgramsOfUpdatedPath();
                SetComplete();
            });
        }

        private void PerformExtractPHP(Action onComplete)
        {
            SetProgress("Detecting PHP...");

            if (Directory.Exists("C:\\PHP"))
            {
                SetProgress("C:\\PHP already exists.  Assuming PHP is installed (continuing in 2 seconds).");
                Thread.Sleep(2000);
                onComplete();
                return;
            }
            else
            {
                SetProgress("Downloading PHP package...");
                var client = new WebClient();
                client.DownloadProgressChanged += (sender, e) =>
                {
                    if (_installThread == null)
                        client.CancelAsync();
                    SetProgress("Downloading PHP package (" + e.ProgressPercentage + "% complete)...");
                };
                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (_installThread == null)
                        return;
                    SetProgress("Extracting PHP package...");
                    var zip = new ZipFile(Path.Combine(Path.GetTempPath(), "PHP.zip"));
                    Directory.CreateDirectory("C:\\PHP");
                    foreach (ZipEntry entry in zip)
                    {
                        if (!entry.IsFile)
                            continue;

                        var entryFileName = entry.Name;
                        var buffer = new byte[4096];
                        var zipStream = zip.GetInputStream(entry);
                        String fullZipToPath = Path.Combine("C:\\PHP", entryFileName);
                        string directoryName = Path.GetDirectoryName(fullZipToPath);
                        if (directoryName.Length > 0)
                            Directory.CreateDirectory(directoryName);
                        using (var streamWriter = File.Create(fullZipToPath))
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }

                    if (_installThread != null)
                        onComplete();
                };
                client.DownloadFileAsync(
                    new Uri("http://windows.php.net/downloads/releases/php-5.4.14-nts-Win32-VC9-x86.zip"),
                    Path.Combine(Path.GetTempPath(), "PHP.zip"));
            }
        }
    }
}
