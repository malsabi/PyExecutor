using System;

namespace PyExecutor.Models
{
    public class Log
    {
        public string Message { get; set; }
        public ConsoleColor Color { get; set; }
        public Log(string Message, ConsoleColor Color)
        {
            this.Message = Message;
            this.Color = Color;
        }
    }
}