using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using VolumeSync.Properties;

namespace VolumeSync
{
    public partial class MainForm : Form
    {
        private const float VolumeChangeModifier = 0.05f;

        private const int MediaKeyPlay = 179;
        private const int MediaKeyNext = 176;
        private const int MediaKeyPrevious = 177;
        private const int MediaKeyVolumeUp = 175;
        private const int MediaKeyVolumeDown = 174;
        private const int MediaKeyVolumeMute = 173;

        private readonly IKeyboardMouseEvents _keyboardMouseEvents;
        private readonly VolumeManager _volumeManager = new VolumeManager();
        
        public MainForm()
        {
            InitializeComponent();
            textBoxAddress.Text = Settings.Default.OtherComputerAddress;

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
                case MediaKeyVolumeUp:
                case MediaKeyVolumeDown:
                case MediaKeyVolumeMute:
                    await UpdateOtherComputerVolume();
                    break;
            }
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            await Connect();
        }

        private void buttonVolumeDown_Click(object sender, EventArgs e)
        {
            _volumeManager.ChangeVolume(-VolumeChangeModifier);
            _volumeManager.ChangeMute(false);
        }

        private void buttonVolumeUp_Click(object sender, EventArgs e)
        {
            _volumeManager.ChangeVolume(VolumeChangeModifier);
            _volumeManager.ChangeMute(false);
        }

        private void buttonMute_Click(object sender, EventArgs e)
        {
            _volumeManager.ChangeMute();
        }

        private async Task Connect()
        {
            await SendCommandAsync<object>(HttpMethod.Get, "ping", showSuccessAndSave: true);
        }

        private async Task UpdateOtherComputerVolume()
        {
            var volume = _volumeManager.GetVolume();
            await SendCommandAsync(HttpMethod.Put, "volume", volume);
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
        
        private async Task SendCommandAsync<T>(HttpMethod httpMethod, string command, T bodyObject = default(T), bool showSuccessAndSave = false)
        {
            var baseAddress = ValidateAndGetAddress();
            if (baseAddress == null)
                return;

            var result = await SendRequestAsync(httpMethod, baseAddress, "api/" + command, bodyObject);
            if (result)
            {
                if (showSuccessAndSave)
                {
                    MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Settings.Default.OtherComputerAddress = baseAddress.ToString();
                    Settings.Default.Save();
                }
            }
            else
            {
                MessageBox.Show("Connection Failed!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task<bool> SendRequestAsync<T>(HttpMethod httpMethod, Uri baseAddress, string path, T bodyObject = default(T))
        {
            var uriBuilder = new UriBuilder(baseAddress) { Path = path };
            var request = new HttpRequestMessage(httpMethod, uriBuilder.Uri);

            if (bodyObject != null)
                request.Content = new ObjectContent<T>(bodyObject, new JsonMediaTypeFormatter());

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.SendAsync(request);
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
