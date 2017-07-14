using System;
using NAudio.CoreAudioApi;

namespace VolumeSync
{
    public class VolumeManager
    {
        public VolumeModel GetVolume()
        {
            return AudioDeviceAction(device => new VolumeModel
            {
                Level = device.AudioEndpointVolume.MasterVolumeLevelScalar,
                IsMuted = device.AudioEndpointVolume.Mute
            });
        }

        public void ChangeVolume(VolumeModel volume)
        {
            ChangeVolume(absoluteValue: volume.Level);
            ChangeMute(volume.IsMuted);
        }
        
        public void ChangeVolume(float? modifier = null, float? absoluteValue = null)
        {
            AudioDeviceAction(device =>
            {
                var currentVolumeLevel = device.AudioEndpointVolume.MasterVolumeLevelScalar;

                var newVolumeLevel = currentVolumeLevel;
                if (modifier.HasValue)
                    newVolumeLevel = currentVolumeLevel + modifier.Value;
                if (absoluteValue.HasValue)
                    newVolumeLevel = absoluteValue.Value;

                newVolumeLevel = newVolumeLevel > 1 ? 1 : newVolumeLevel;
                newVolumeLevel = newVolumeLevel < 0 ? 0 : newVolumeLevel;

                device.AudioEndpointVolume.MasterVolumeLevelScalar = newVolumeLevel;
            });
        }

        public void ChangeMute(bool? muted = null)
        {
            AudioDeviceAction(device =>
            {
                if (muted.HasValue)
                    device.AudioEndpointVolume.Mute = muted.Value;
                else
                    device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            });
        }

        private void AudioDeviceAction(Action<MMDevice> action)
        {
            using (var devicesEnumerator = new MMDeviceEnumerator())
            {
                var device = devicesEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                action(device);
            }
        }

        private T AudioDeviceAction<T>(Func<MMDevice, T> action)
        {
            using (var devicesEnumerator = new MMDeviceEnumerator())
            {
                var device = devicesEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                return action(device);
            }
        }
    }
}
