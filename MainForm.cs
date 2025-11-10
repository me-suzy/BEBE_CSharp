using System;
using System.IO;
using System.Windows.Forms;
using BebeTaskRecorder.Models;
using System.Drawing;

namespace BebeTaskRecorder
{
    public partial class MainForm : Form
    {
        private TaskRecorder _recorder;
        private TaskPlayer _player;
        private TaskData _currentTask;
        private string _tasksDirectory = "tasks";

        // Controls
        private Button btnRecord;
        private Button btnPlay;
        private Button btnSaveJson;
        private Button btnSaveExe;
        private Button btnLoad;
        private Label lblStatus;
        private Label lblEvents;
        private NumericUpDown numSpeed;
        private CheckBox chkLoop;
        private CheckBox chkRunUntilStop;

        public MainForm()
        {
            InitializeComponent();
            InitializeRecorderPlayer();
        }

        private void InitializeComponent()
        {
            this.Text = "BEBE Task Recorder v3.0 (C#)";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;

            // Status label
            lblStatus = new Label
            {
                Location = new Point(20, y),
                Size = new Size(540, 30),
                Text = "Ready for recording",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblStatus);
            y += 40;

            // Events label
            lblEvents = new Label
            {
                Location = new Point(20, y),
                Size = new Size(540, 20),
                Text = "Events: 0"
            };
            this.Controls.Add(lblEvents);
            y += 30;

            // Record button
            btnRecord = new Button
            {
                Location = new Point(20, y),
                Size = new Size(120, 40),
                Text = "Start Recording\n(F9/ESC to stop)",
                BackColor = Color.LightGreen
            };
            btnRecord.Click += BtnRecord_Click;
            this.Controls.Add(btnRecord);

            // Play button
            btnPlay = new Button
            {
                Location = new Point(150, y),
                Size = new Size(100, 40),
                Text = "Play Task\n(Space)",
                BackColor = Color.LightBlue
            };
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);
            y += 50;

            // Speed control
            var lblSpeed = new Label
            {
                Location = new Point(20, y),
                Size = new Size(80, 20),
                Text = "Speed:"
            };
            this.Controls.Add(lblSpeed);

            numSpeed = new NumericUpDown
            {
                Location = new Point(100, y),
                Size = new Size(60, 20),
                Minimum = 0.1m,
                Maximum = 10m,
                Value = 1m,
                Increment = 0.1m,
                DecimalPlaces = 1
            };
            this.Controls.Add(numSpeed);
            y += 30;

            // Loop checkbox
            chkLoop = new CheckBox
            {
                Location = new Point(20, y),
                Size = new Size(150, 20),
                Text = "Loop"
            };
            this.Controls.Add(chkLoop);
            y += 25;

            // Run until stop checkbox
            chkRunUntilStop = new CheckBox
            {
                Location = new Point(20, y),
                Size = new Size(250, 20),
                Text = "Run until ESC/F9 (continuous)"
            };
            this.Controls.Add(chkRunUntilStop);
            y += 35;

            // Save as JSON button
            btnSaveJson = new Button
            {
                Location = new Point(20, y),
                Size = new Size(120, 35),
                Text = "Save as JSON",
                BackColor = Color.LightYellow
            };
            btnSaveJson.Click += BtnSaveJson_Click;
            this.Controls.Add(btnSaveJson);

            // Save as EXE button
            btnSaveExe = new Button
            {
                Location = new Point(150, y),
                Size = new Size(120, 35),
                Text = "Save as EXE",
                BackColor = Color.LightCoral
            };
            btnSaveExe.Click += BtnSaveExe_Click;
            this.Controls.Add(btnSaveExe);

            // Load button
            btnLoad = new Button
            {
                Location = new Point(280, y),
                Size = new Size(120, 35),
                Text = "Load Task",
                BackColor = Color.LightGray
            };
            btnLoad.Click += BtnLoad_Click;
            this.Controls.Add(btnLoad);
            y += 45;

            // Info label
            var lblInfo = new Label
            {
                Location = new Point(20, y),
                Size = new Size(540, 60),
                Text = "✓ Record: Click 'Start Recording', perform actions, press F9 or ESC to stop\n" +
                       "✓ Play: Click 'Play Task' or press Space\n" +
                       "✓ Save as EXE: Creates standalone executable (requires .NET SDK)",
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8)
            };
            this.Controls.Add(lblInfo);
        }

        private void InitializeRecorderPlayer()
        {
            Directory.CreateDirectory(_tasksDirectory);

            _recorder = new TaskRecorder();
            _recorder.RecordingStopped += (s, e) =>
            {
                this.Invoke((Action)(() =>
                {
                    lblStatus.Text = $"Recording complete: {_recorder.Events.Count} events";
                    lblStatus.ForeColor = Color.Green;
                    lblEvents.Text = $"Events: {_recorder.Events.Count}";
                    btnRecord.Text = "Start Recording\n(F9/ESC to stop)";
                    btnRecord.BackColor = Color.LightGreen;

                    _currentTask = new TaskData
                    {
                        Name = "Task_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                        Created = DateTime.Now,
                        Events = new System.Collections.Generic.List<TaskEvent>(_recorder.Events),
                        Playback = new PlaybackSettings
                        {
                            Speed = (double)numSpeed.Value,
                            Loop = chkLoop.Checked,
                            RunUntilStop = chkRunUntilStop.Checked
                        }
                    };
                }));
            };

            _player = new TaskPlayer();
            _player.PlaybackStarted += (s, e) =>
            {
                this.Invoke((Action)(() =>
                {
                    lblStatus.Text = "Playing...";
                    lblStatus.ForeColor = Color.Orange;
                }));
            };
            _player.PlaybackCompleted += (s, e) =>
            {
                this.Invoke((Action)(() =>
                {
                    lblStatus.Text = "Playback completed";
                    lblStatus.ForeColor = Color.Green;
                }));
            };
        }

        private void BtnRecord_Click(object sender, EventArgs e)
        {
            if (_recorder.IsRecording)
            {
                _recorder.StopRecording();
            }
            else
            {
                lblStatus.Text = "RECORDING... Press F9 or ESC to stop";
                lblStatus.ForeColor = Color.Red;
                btnRecord.Text = "Recording...\n(F9/ESC to stop)";
                btnRecord.BackColor = Color.Red;
                _recorder.StartRecording();
            }
        }

        private async void BtnPlay_Click(object sender, EventArgs e)
        {
            if (_currentTask == null || _currentTask.Events.Count == 0)
            {
                MessageBox.Show("No task to play! Record or load a task first.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentTask.Playback.Speed = (double)numSpeed.Value;
            _currentTask.Playback.Loop = chkLoop.Checked;
            _currentTask.Playback.RunUntilStop = chkRunUntilStop.Checked;

            await _player.PlayAsync(_currentTask.Events, _currentTask.Playback);
        }

        private void BtnSaveJson_Click(object sender, EventArgs e)
        {
            if (_currentTask == null || _currentTask.Events.Count == 0)
            {
                MessageBox.Show("No task to save!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                InitialDirectory = Path.GetFullPath(_tasksDirectory),
                FileName = _currentTask.Name + ".json"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TaskExporter.ExportAsJson(_currentTask, saveDialog.FileName);
                    MessageBox.Show($"Task saved as JSON!\n{Path.GetFileName(saveDialog.FileName)}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSaveExe_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Save as EXE feature is temporarily disabled.\n\n" +
                "Use 'Save as JSON' instead - JSON files can be loaded and played back!\n\n" +
                "The EXE export feature needs additional work to function properly.",
                "Feature Not Available",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                InitialDirectory = Path.GetFullPath(_tasksDirectory)
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentTask = TaskExporter.ImportFromJson(openDialog.FileName);
                    lblEvents.Text = $"Events: {_currentTask.Events.Count}";
                    lblStatus.Text = $"Loaded: {_currentTask.Name}";
                    lblStatus.ForeColor = Color.Blue;
                    numSpeed.Value = (decimal)_currentTask.Playback.Speed;
                    chkLoop.Checked = _currentTask.Playback.Loop;
                    chkRunUntilStop.Checked = _currentTask.Playback.RunUntilStop;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _recorder?.Dispose();
            _player?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

