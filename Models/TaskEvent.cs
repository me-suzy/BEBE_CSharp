using System;

namespace BebeTaskRecorder.Models
{
    public class TaskEvent
    {
        public string Type { get; set; }  // mouse_move, mouse_click, key_press, etc.
        public double Timestamp { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Button { get; set; }  // left, right, middle
        public bool Pressed { get; set; }
        public string Key { get; set; }
        public int Dx { get; set; }  // scroll delta X
        public int Dy { get; set; }  // scroll delta Y
    }
}

