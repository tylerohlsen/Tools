using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace ElpisRemote.Windows
{
    public partial class MainForm : Form
    {
        private const int MediaKeyPlay = 179;
        private const int MediaKeyNext = 176;
        private const int MediaKeyPrevious = 177;
        private const int MediaKeyVolumeUp = 175;
        private const int MediaKeyVolumeDown = 174;
        private const int MediaKeyVolumeMute = 173;
        
        private readonly IKeyboardMouseEvents _keyboardMouseEvents;

        public MainForm()
        {
            InitializeComponent();
            textBoxAddress.Text = Properties.Settings.Default.ElpisServerAddress;

            _keyboardMouseEvents = Hook.GlobalEvents();
            _keyboardMouseEvents.KeyUp += GlobalEventsOnKeyUp;
            
            Disposed += OnDisposed;
        }

        private void OnDisposed(object sender, EventArgs eventArgs)
        {
            _keyboardMouseEvents.Dispose();
        }

        private async void GlobalEventsOnKeyUp(object sender, KeyEventArgs keyPressEventArgs)
        {
            switch (keyPressEventArgs.KeyValue)
            {
                case MediaKeyPlay:
                    await TogglePlayPause();
                    break;

                case MediaKeyNext:
                    await Skip();
                    break;
            }
        }

        private Uri ValidateAndGetAddress()
        {
            Uri address;
            if (!Uri.TryCreate(textBoxAddress.Text, UriKind.Absolute, out address))
            {
                MessageBox.Show("Failed to parse address.", "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            return address;
        }

        private async Task<bool> SendRequestAsync(Uri baseAddress, string path)
        {
            var uriBuilder = new UriBuilder(baseAddress) {Path = path};
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.GetAsync(uriBuilder.Uri);
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SendCommandAsync(string command, bool showSuccessAndSave = false)
        {
            var baseAddress = ValidateAndGetAddress();
            if (baseAddress == null)
                return;

            var result = await SendRequestAsync(baseAddress, command);
            if (result)
            {
                if (showSuccessAndSave)
                {
                    MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Properties.Settings.Default.ElpisServerAddress = textBoxAddress.Text;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                MessageBox.Show("Connection Failed!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            await Connect();
        }

        private async Task Connect()
        {
            await SendCommandAsync("connect", true);
        }

        private async void buttonPlay_Click(object sender, EventArgs e)
        {
            await TogglePlayPause();
        }

        private async Task TogglePlayPause()
        {
            await SendCommandAsync("toggleplaypause");
        }

        private async void buttonSkip_Click(object sender, EventArgs e)
        {
            await Skip();
        }

        private async Task Skip()
        {
            await SendCommandAsync("next");
        }
    }
}
