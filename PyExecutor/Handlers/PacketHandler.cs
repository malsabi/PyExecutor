using PyExecutor.Models;
using PyExecutor.Queues;
using PyExecutor.Utilities;
using System;
using System.Linq;

namespace PyExecutor.Handlers
{
    public class PacketHandler
    {
        private readonly OutputQueue Output;
        private readonly InputQueue Input;
        private readonly Logger Logger;
        private short FrameCounter = 0;
        public PacketHandler(InputQueue Input, OutputQueue Output, Logger Logger)
        {
            this.Input = Input;
            this.Output = Output;
            this.Logger = Logger;
        }

        public void HandleFromServerManager(IViewNet.Common.Models.Packet Message)
        {
            if (Message.Code == 1111)      // SetDetectionType
            {
                //Send to all children the detection type
                Input.Process(new IViewNet.Common.Models.Packet(5555, "SetDetectionType", Message.Content));
            }
            else if (Message.Code == 1112) // SetOrientation
            {
                //Set the orientation of the image and then send it to the children
            }
            else if (Message.Code == 1113) // GetDetectedFrame
            {
                Input.Enqueue(new Frame(Message.Content, FrameCounter));
                FrameCounter++;
            }
            else
            {
                Logger.Log(new Log("Invalid packet code: " + Message.Code, ConsoleColor.Red));
            }
        }

        public void HandleFromChild(IViewNet.Common.Models.Packet Message)
        {
            if (Message.Code == 7777) // SetDetectedFrame
            {
                short FrameIndex = BitConverter.ToInt16(Message.Content, 0);
                byte[] BitmapBytes = Message.Content.Skip(2).ToArray();
                Output.Enqueue(new Frame(BitmapBytes, FrameIndex));
            }
            else
            {
                Logger.Log(new Log("Invalid packet code: " + Message.Code, ConsoleColor.Red));
            }
        }
    }
}
