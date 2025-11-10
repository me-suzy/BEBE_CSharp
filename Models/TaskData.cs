using System;
using System.Collections.Generic;

namespace BebeTaskRecorder.Models
{
    public class TaskData
    {
        public string Name { get; set; }
        public string Version { get; set; } = "3.0";
        public DateTime Created { get; set; }
        public List<TaskEvent> Events { get; set; } = new List<TaskEvent>();
        public PlaybackSettings Playback { get; set; } = new PlaybackSettings();
        public ScheduleSettings Schedule { get; set; } = new ScheduleSettings();
    }

    public class PlaybackSettings
    {
        public double Speed { get; set; } = 1.0;
        public bool Loop { get; set; } = false;
        public bool RunUntilStop { get; set; } = false;
    }

    public class ScheduleSettings
    {
        public bool Enabled { get; set; } = false;
        public List<string> Days { get; set; } = new List<string>();
        public string TimeFrom { get; set; } = "00:00";
        public string TimeTo { get; set; } = "23:59";
    }
}

