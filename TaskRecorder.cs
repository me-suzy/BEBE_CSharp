using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using BebeTaskRecorder.Models;

namespace BebeTaskRecorder
{
    public class TaskRecorder
    {
        private GlobalHooks _hooks;
        private List<TaskEvent> _events = new List<TaskEvent>();
        private Stopwatch _stopwatch = new Stopwatch();
        private bool _isRecording = false;
        private System.Drawing.Point _lastMousePos;

        public event EventHandler RecordingStopped;

        public bool IsRecording => _isRecording;
        public List<TaskEvent> Events => _events;

        public TaskRecorder()
        {
            _hooks = new GlobalHooks();
            _hooks.MouseMove += OnMouseMove;
            _hooks.MouseClick += OnMouseClick;
            _hooks.KeyDown += OnKeyDown;
            _hooks.KeyUp += OnKeyUp;
        }

        public void StartRecording()
        {
            if (_isRecording) return;

            _events.Clear();
            _stopwatch.Restart();
            _isRecording = true;
            _hooks.StartHooks();
        }

        public void StopRecording()
        {
            if (!_isRecording) return;

            _isRecording = false;
            _stopwatch.Stop();
            _hooks.StopHooks();
            RecordingStopped?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isRecording) return;

            // Only record if mouse moved significantly (reduce event count)
            if (Math.Abs(e.X - _lastMousePos.X) > 5 || Math.Abs(e.Y - _lastMousePos.Y) > 5)
            {
                _events.Add(new TaskEvent
                {
                    Type = "mouse_move",
                    Timestamp = _stopwatch.Elapsed.TotalSeconds,
                    X = e.X,
                    Y = e.Y
                });
                _lastMousePos = new System.Drawing.Point(e.X, e.Y);
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (!_isRecording) return;

            string button = e.Button == MouseButtons.Left ? "left" : 
                           e.Button == MouseButtons.Right ? "right" : "middle";

            _events.Add(new TaskEvent
            {
                Type = "mouse_click",
                Timestamp = _stopwatch.Elapsed.TotalSeconds,
                X = e.X,
                Y = e.Y,
                Button = button,
                Pressed = true // Simplified - record as single click
            });
        }

        private void OnKeyDown(object sender, Keys key)
        {
            if (!_isRecording) return;

            // Stop recording on F9 or Escape
            if (key == Keys.F9 || key == Keys.Escape)
            {
                StopRecording();
                return;
            }

            _events.Add(new TaskEvent
            {
                Type = "key_press",
                Timestamp = _stopwatch.Elapsed.TotalSeconds,
                Key = key.ToString()
            });
        }

        private void OnKeyUp(object sender, Keys key)
        {
            if (!_isRecording) return;

            _events.Add(new TaskEvent
            {
                Type = "key_release",
                Timestamp = _stopwatch.Elapsed.TotalSeconds,
                Key = key.ToString()
            });
        }

        public void Dispose()
        {
            _hooks?.Dispose();
        }
    }
}
