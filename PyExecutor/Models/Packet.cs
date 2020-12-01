using PyExecutor.PCPC;

namespace PyExecutor.Models
{
    /// <summary>
    /// Our Packet Structure consists of CommandName, CommandDelimiter, CommandContent
    /// [Length, ["GetHandShake", "|", "Content"], ]
    /// </summary>
    public class Packet
    {
        public int CommandLength { get; set; }
        public string CommandName { get; set; }
        public string CommandContent { get; set; }
        public string ExtraContent { get; set; }

        public Packet(string CommandName, string CommandContent, string ExtraContent = "")
        {
            this.CommandName = CommandName;
            this.CommandContent = CommandContent;
            this.ExtraContent = ExtraContent;
        }

        public Packet()
        {
            CommandName = string.Empty;
            CommandContent = string.Empty;
            ExtraContent = string.Empty;
        }

        public override string ToString()
        {
            string Command;
            Command = string.Format("{0}{1}{2}{3}{4}", CommandName, ParentConfig.COMMAND_DELIMITER, CommandContent, ParentConfig.COMMAND_DELIMITER, ExtraContent);
            CommandLength = Command.Length;
            return string.Format("{0}{1}", CommandLength, Command);
        }

        public static Packet GetCommand(string LineToProcess)
        {
            Packet Command = new Packet();
            string[] Contents = LineToProcess.Split(ParentConfig.COMMAND_DELIMITER);

            Command.CommandName = Contents[0];
            Command.CommandContent = Contents[1];
            Command.ExtraContent = Contents[2];
            Command.CommandLength = Command.CommandContent.Length;

            return Command;
        }
    }
}