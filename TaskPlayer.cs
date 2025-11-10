using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BebeTaskRecorder.Models;

namespace BebeTaskRecorder
{
    public class TaskPlayer
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private bool _isPlaying = false;
        private bool _stopRequested = false;
        private GlobalHooks _stopHooks;

        public bool IsPlaying => _isPlaying;

        public event EventHandler PlaybackStarted;
        public event EventHandler PlaybackCompleted;
        public event EventHandler PlaybackStopped;

        public TaskPlayer()
        {
            _stopHooks = new GlobalHooks();
            _stopHooks.KeyDown += (s, e) =>
            {
                if (e == Keys.F9 || e == Keys.Escape)
                {
                    StopPlayback();
                }
            };
        }

        public async Task PlayAsync(List<TaskEvent> events, PlaybackSettings settings)
        {
            if (_isPlaying || events == null || events.Count == 0) return;

            _isPlaying = true;
            _stopRequested = false;
            PlaybackStarted?.Invoke(this, EventArgs.Empty);

            _stopHooks.StartHooks();

            try
            {
                int loopCount = settings.RunUntilStop ? int.MaxValue : (settings.Loop ? int.MaxValue : 1);
                
                for (int loop = 0; loop < loopCount && !_stopRequested; loop++)
                {
                    for (int i = 0; i < events.Count && !_stopRequested; i++)
                    {
                        var currentEvent = events[i];

                        // Calculate delay
                        if (i > 0)
                        {
                            double delay = (currentEvent.Timestamp - events[i - 1].Timestamp) / settings.Speed;
                            if (delay > 0)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(delay));
                            }
                        }

                        if (_stopRequested) break;

                        ExecuteEvent(currentEvent);
                    }

                    if (!settings.Loop && !settings.RunUntilStop) break;
                }
            }
            finally
            {
                _stopHooks.StopHooks();
                _isPlaying = false;

                if (_stopRequested)
                    PlaybackStopped?.Invoke(this, EventArgs.Empty);
                else
                    PlaybackCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StopPlayback()
        {
            _stopRequested = true;
        }

        private void ExecuteEvent(TaskEvent evt)
        {
            switch (evt.Type)
            {
                case "mouse_move":
                    SetCursorPos(evt.X, evt.Y);
                    break;

                case "mouse_click":
                    SetCursorPos(evt.X, evt.Y);
                    string button = evt.Button?.ToLower() ?? "left";
                    
                    if (evt.Pressed)
                    {
                        if (button.Contains("left"))
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                        else if (button.Contains("right"))
                            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                        else if (button.Contains("middle"))
                            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
                    }
                    else
                    {
                        if (button.Contains("left"))
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                        else if (button.Contains("right"))
                            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                        else if (button.Contains("middle"))
                            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
                    }
                    break;

                case "mouse_scroll":
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)(evt.Dy * 100), UIntPtr.Zero);
                    break;

                case "key_press":
                    SendKey(evt.Key, false);
                    break;

                case "key_release":
                    SendKey(evt.Key, true);
                    break;
            }
        }

        private void SendKey(string keyName, bool release)
        {
            if (string.IsNullOrEmpty(keyName)) return;

            if (Enum.TryParse<Keys>(keyName, true, out Keys key))
            {
                byte vk = (byte)key;
                keybd_event(vk, 0, release ? KEYEVENTF_KEYUP : 0, UIntPtr.Zero);
            }
        }

        public void Dispose()
        {
            _stopHooks?.Dispose();
        }
    }
}

