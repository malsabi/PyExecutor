using PyExecutor.Models;
using System;
using System.Drawing;

namespace PyExecutor.PCPC
{
    public class ChildEventArgs : ParentEventArgs
    {
        public Child CurrentChild { get; set; }
        public Packet Command { get; set; }
        public int Priority { get; private set; }

        public ChildEventArgs(Child CurrentChild, Packet Command, string Message, Color ForeColor) : base("Child", CurrentChild.Id.ToString(), Message, DateTime.Now, ForeColor)
        {
            this.CurrentChild = CurrentChild;
            this.Command = Command;
            Priority = GetPriority();
        }

        public ChildEventArgs(string Message, Color ForeColor) : base("Child", "", Message, DateTime.Now, ForeColor)
        {
            Priority = GetPriority();
        }


        private int GetPriority()
        {
            if (this.Message.Contains("[Information]"))
            {
                return 0;
            }
            else if (this.Message.Contains("[Sent]"))
            {
                return 2;
            }
            else if (this.Message.Contains("[Received]"))
            {
                return 3;
            }
            else if (this.Message.Contains("[Handshake]"))
            {
                return 1;
            }
            else
            {
                return 4;
            }
        }
    }
}