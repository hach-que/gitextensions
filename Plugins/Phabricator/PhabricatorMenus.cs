using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GitCommands.Utils;
using GitUIPluginInterfaces;

namespace Phabricator
{
    public class PhabricatorMenus
    {
        private Form _mainForm;
        private InstallArcanistForm _installArcanistForm;

        public bool Initialize(Form owner)
        {
            _mainForm = owner;

            var menuStrip = FindControl<MenuStrip>(owner, p => true);
            var toolStrip = FindControl<ToolStrip>(owner, p => p.Name == "ToolStrip");

            if (menuStrip == null)
                throw new Exception("Cannot find main menu");
            if (toolStrip == null)
                throw new Exception("Cannot find main tool strip");
            
            // Just put "Install Arcanist" under the tools menu for now, until we have
            // a full suite of Arcanist tools.
            var arcanistMenu = new ToolStripMenuItem("Arcanist");
            var toolsMenu = menuStrip.Items.Cast<ToolStripMenuItem>().First(x => x.Text == "Tools");
            toolsMenu.DropDownItems.Insert(
                toolsMenu.DropDownItems.IndexOf(
                    toolsMenu.DropDownItems.Cast<ToolStripItem>().First(x => x.Text == "PuTTY")) + 1,
                arcanistMenu);

            // Add the installation menu item if appropriate.
            if (!IsArcanistInstalled())
            {
                var installMenuItem = new ToolStripMenuItem("Install", null, (sender, e) =>
                    {
                        if (_installArcanistForm == null)
                        {
                            var form = new InstallArcanistForm();
                            form.FormClosed += (_1, _2) => { _installArcanistForm = null; };
                            form.ShowDialog();
                        }
                    });
                installMenuItem.Enabled = !EnvUtils.RunningOnUnix();
                if (EnvUtils.RunningOnUnix())
                    installMenuItem.Text += " (only on Windows)";
                arcanistMenu.DropDownItems.Add(installMenuItem);
            }

            // If there are no menu items, hide the Arcanist menu item.
            //if (arcanistMenu.DropDownItems.Cast<ToolStripMenuItem>().Count(x => x.Visible && x.Enabled) == 0)
            //    arcanistMenu.Visible = false;

            return true;
        }

        public static bool IsArcanistInstalled()
        {
            return ExistsOnPath("arc") && ExistsOnPath("php");
        }

        public static bool ExistsOnPath(string fileName)
        {
            if (GetFullPath(fileName) != null)
                return true;
            return false;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        public void Update(GitUIBaseEventArgs e)
        {
            // Disable our menu items if the user doesn't have a repository open.
            /*



            // Correct enabled/visibility of our menu/tool strip items.

            bool validWorkingDir = e.GitModule.IsValidGitWorkingDir();

            _gitReviewMenuItem.Enabled = validWorkingDir;

            bool showGerritItems = validWorkingDir && File.Exists(e.GitModule.GitWorkingDir + ".gitreview");

            foreach (var item in _gerritMenuItems)
            {
                item.Visible = showGerritItems;
            }

            _installCommitMsgMenuItem.Visible =
                showGerritItems &&
                !HaveValidCommitMsgHook(e.GitModule.GetGitDirectory());*/
        }

        private T FindControl<T>(Control form, Func<T, bool> predicate)
            where T : Control
        {
            return FindControl(form.Controls, predicate);
        }

        private T FindControl<T>(IEnumerable controls, Func<T, bool> predicate)
            where T : Control
        {
            foreach (Control control in controls)
            {
                var result = control as T;

                if (result != null && predicate(result))
                    return result;

                result = FindControl(control.Controls, predicate);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
