using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace ElpisRemote.Windows
{
    public class MainFormContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly MainForm _mainForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFormContext"/> class.
        /// </summary>
        public MainFormContext()
        {
            var showHideItem = new MenuItem("Show", ShowHideMenuItemOnClick);
            var exitMenuItem = new MenuItem("Exit", ExitMenuItemOnClick);

            _notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.AppIcon,
                ContextMenu = new ContextMenu(new[] {showHideItem, exitMenuItem}),
                Visible = true
            };
            _notifyIcon.Click += ShowHideMenuItemOnClick;

            _mainForm = new MainForm();
            _mainForm.ShowDialog();
        }

        /// <summary>
        /// Exits the menu item on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ExitMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            Application.Exit();
        }

        /// <summary>
        /// Shows the hide menu item on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ShowHideMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            if (_mainForm.Visible)
                _mainForm.Activate();
            else
                _mainForm.ShowDialog();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.ApplicationContext" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposingManaged">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposingManaged)
        {
            base.Dispose(disposingManaged);

            if (!disposingManaged)
                return;

            _notifyIcon.Dispose();
            _mainForm.Dispose();
        }
    }
}
