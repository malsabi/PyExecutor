using System;
using System.Drawing;

namespace PyExecutor.PCPC
{
    public class ParentEventArgs
    {
        public string From { get; set; }
        public string ID { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public Color ForeColor { get; set; }

        public ParentEventArgs(string From, string ID, string Message, DateTime TimeStamp, Color ForeColor)
        {
            this.From = From;
            this.ID = ID;
            this.Message = Message;
            this.TimeStamp = TimeStamp;
            this.ForeColor = ForeColor;
        }
    }
}
