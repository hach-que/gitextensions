using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitUIPluginInterfaces;

namespace Phabricator
{
    public class PhabricatorPlugin : GitPluginBase, IGitPluginForRepository
    {
        private IGitUICommands _gitUiCommands;
        private PhabricatorMenus _phabricatorMenus = new PhabricatorMenus();
        private bool _initialized;

        public override string Description
        {
            get { return "Provides Phabricator integration."; }
        }

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
        {
            return false;
        }

        public override void Register(IGitUICommands gitUiCommands)
        {
            _gitUiCommands = gitUiCommands;
            gitUiCommands.PostBrowseInitialize += gitUiCommands_PostBrowseInitialize;
            gitUiCommands.PostRegisterPlugin += gitUiCommands_PostRegisterPlugin;
        }

        public override void Unregister(IGitUICommands gitUiCommands)
        {
            gitUiCommands.PostBrowseInitialize -= gitUiCommands_PostBrowseInitialize;
            gitUiCommands.PostRegisterPlugin -= gitUiCommands_PostRegisterPlugin;
            _gitUiCommands = null;
        }

        void gitUiCommands_PostRegisterPlugin(object sender, GitUIBaseEventArgs e)
        {
            if (!_initialized)
                _initialized = _phabricatorMenus.Initialize((Form)e.OwnerForm);

            _phabricatorMenus.Update(e);
        }

        void gitUiCommands_PostBrowseInitialize(object sender, GitUIBaseEventArgs e)
        {
            if (!_initialized)
                _initialized = _phabricatorMenus.Initialize((Form)e.OwnerForm);

            _phabricatorMenus.Update(e);
        }
    }
}
