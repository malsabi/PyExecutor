using IViewNet.Common;
using IViewNet.Pipes;
using PyExecutor.Models;
using PyExecutor.PCPC;
using PyExecutor.Queues;
using PyExecutor.Utilities;
using PyExecutor.Handlers;
using System;
using System.Windows.Forms;

namespace PyExecutor
{
    class Program
    {
        #region "Constants"
        private const int MAXIMUM_BACKLOG = 1000;
        private const int MAXIMUM_CONNECTIONS = 500;
        private const int MESSAGE_SIZE = 1024 * 1024 * 10;
        private const int BUFFER_SIZE = 1024 * 100;
        private const int HEADER_SIZE = 4;
        private const int MAX_PIPES = 1;
        #endregion

        #region "Private Fields"
        private static ParentConfig ParentServerConfig;
        private static NetConfig ServerConfig;
        private static Parent ParentServer;
        private static InputQueue Input;
        private static OutputQueue Output;
        private static Logger MainLogger;
        private static PacketHandler PacketHandler;
        private static PipeConfig Config;
        private static IViewPipeClient Pipeline;
        #endregion

        static void Main()
        {
            Application.Run(new Main());
            //MainLogger = new Logger(50);
            //InitializePipe();
            //Initialize();
        }
        private static void Initialize()
        {
            ParentServerConfig = new ParentConfig(2);
            ServerConfig = new NetConfig();
            ServerConfig.SetMaxBackLogConnections(MAXIMUM_BACKLOG);
            ServerConfig.SetMaxConnections(MAXIMUM_CONNECTIONS);
            ServerConfig.SetMaxMessageSize(MESSAGE_SIZE);
            ServerConfig.SetBufferSize(BUFFER_SIZE);
            ServerConfig.SetHeaderSize(HEADER_SIZE);
            ServerConfig.SetEnableKeepAlive(false);
            ServerConfig.SetPort(1660);
            ParentServer = new Parent(ParentServerConfig, ServerConfig);
            ParentServer.OnParentCreatedEvent += ParentServer_OnParentCreatedEvent;
            ParentServer.OnParentClosedEvent += ParentServer_OnParentClosedEvent;
            ParentServer.OnExceptionEvent += ParentServer_OnExceptionEvent;
            ParentServer.OnChildCreateEvent += ParentServer_OnChildCreateEvent;
            ParentServer.OnChildConnectEvent += ParentServer_OnChildConnectEvent;
            ParentServer.OnChildAuthenticateEvent += ParentServer_OnChildAuthenticateEvent;
            ParentServer.OnChildSendEvent += ParentServer_OnChildSendEvent;
            ParentServer.OnChildReceiveEvent += ParentServer_OnChildReceiveEvent;
            ParentServer.OnChildDisconnectEvent += ParentServer_OnChildDisconnectEvent;
            ParentServer.OnChildDestroyEvent += ParentServer_OnChildDestroyEvent;
            Input = new InputQueue(ParentServer);
            Output = new OutputQueue();
            Output.OnFrameEvent += Output_OnFrameEvent;
            ParentServer.StartParent();
            PacketHandler = new PacketHandler(Input, Output, MainLogger);
            ConnectPipeline();
            Pipeline.SendMessage(new IViewNet.Common.Models.Packet(1111, "SetDetectionType", new byte[1024 * 19]));
            Console.ReadKey();
        }
        #region "Pipeline"
        #region "Private Methods"
        private static void InitializePipe()
        {
            Pipeline = new IViewPipeClient(CreatePipeConfig())
            {
                PacketManager = CreatePacketManager()
            };
            MainLogger.Log(new Log("ClientPipelining Successfully Initialized ", ConsoleColor.Green));
        }
        private static PipeConfig CreatePipeConfig()
        {
            Config = new PipeConfig(BUFFER_SIZE, MAX_PIPES);
            return Config;
        }
        private static void AddPipeHandlers()
        {
            Pipeline.PipeConnectedEvent += SetOnPipeConnected;
            Pipeline.PipeReceivedEvent += SetOnPipeReceived;
            Pipeline.PipeSentEvent += SetOnPipeSent;
            Pipeline.PipeClosedEvent += SetOnPipeClosed;
            Pipeline.PipeExceptionEvent += SetOnPipeException;
        }
        private static void RemovePipeHandlers()
        {
            Pipeline.PipeConnectedEvent -= SetOnPipeConnected;
            Pipeline.PipeReceivedEvent -= SetOnPipeReceived;
            Pipeline.PipeSentEvent -= SetOnPipeSent;
            Pipeline.PipeClosedEvent -= SetOnPipeClosed;
            Pipeline.PipeExceptionEvent -= SetOnPipeException;
        }
        private static PacketManager CreatePacketManager()
        {
            PacketManager PacketManager = new PacketManager();
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1111, "SetDetectionType", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1112, "SetOrientation", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1113, "GetDetectedFrame", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1114, "SetDetectedFrame", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1115, "EndOfFrame", null));
            return PacketManager;
        }
        private static void SetOnPipeConnected()
        {
            MainLogger.Log(new Log("End Pipe Connected Successfully", ConsoleColor.Green));
        }
        private static void SetOnPipeReceived(IViewNet.Common.Models.Packet Message)
        {
            MainLogger.Log(new Log(string.Format("Pipe Received From ServerManager: {0}", Message.Name), ConsoleColor.Yellow));
            PacketHandler.HandleFromServerManager(Message);
        }
        private static void SetOnPipeSent(IViewNet.Common.Models.Packet Message)
        {
            MainLogger.Log(new Log(string.Format("Pipe Sent To ServerManager: {0}", Message.Name), ConsoleColor.Blue));
        }
        private static void SetOnPipeClosed()
        {
            MainLogger.Log(new Log("End Pipe Closed Successfully", ConsoleColor.Cyan));
        }
        private static void SetOnPipeException(Exception Error)
        {
            MainLogger.Log(new Log(string.Format("Pipe Exception: {0}", Error.Message), ConsoleColor.Red));
        }
        #endregion
        #region "Public Methods"
        public static void ConnectPipeline()
        {
            if (Pipeline.IsPipeConnected == false)
            {
                AddPipeHandlers();
                Pipeline.AttemptToConnect();
            }
        }
        public static void ClosePipeline()
        {
            if (Pipeline.IsPipeConnected == true)
            {
                RemovePipeHandlers();
                Pipeline.ShutdownClient();
            }
        }
        public static void SendMessage(IViewNet.Common.Models.Packet Message)
        {
            if (Pipeline.IsPipeConnected == true)
            {
                Pipeline.SendMessage(Message);
            }
        }
        #endregion
        #endregion
        #region "Event Handler For Parent"
        private static void ParentServer_OnParentCreatedEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnParentClosedEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnExceptionEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void Output_OnFrameEvent(Frame Item)
        {
            Console.WriteLine("Frame[{0}] Received From Child Bytes: {1}", Item.Index, Item.BitmapBytes.Length);
            try
            {
                Console.WriteLine("BEFORE SENT TO SERVER MANAGER");
                Pipeline.SendMessage(new IViewNet.Common.Models.Packet(1114, "SetDetectedFrame", Item.BitmapBytes));
                Console.WriteLine("AFTER SENT TO SERVER MANAGER");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
        #endregion
        #region "Event Handler For Child"
        private static void ParentServer_OnChildCreateEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnChildConnectEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnChildAuthenticateEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnChildSendEvent(IViewNet.Common.Models.Packet Message)
        {
            Console.WriteLine("Sent To Child: {0}", Message.Name);
        }
        private static void ParentServer_OnChildReceiveEvent(IViewNet.Common.Models.Packet Message)
        {
            Console.WriteLine("Received From Child: {0}", Message.Name);
            PacketHandler.HandleFromChild(Message);
        }
        private static void ParentServer_OnChildDisconnectEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        private static void ParentServer_OnChildDestroyEvent(string Message)
        {
            Console.WriteLine(Message);
        }
        #endregion
    }
}