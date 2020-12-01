using IViewNet.Common;
using IViewNet.Pipes;
using PyExecutor.Handlers;
using PyExecutor.Models;
using PyExecutor.PCPC;
using PyExecutor.Queues;
using PyExecutor.Utilities;
using System;
using System.Windows.Forms;

namespace PyExecutor
{
    public partial class Main : Form
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
        private ParentConfig ParentServerConfig;
        private NetConfig ServerConfig;
        private Parent ParentServer;
        private InputQueue Input;
        private OutputQueue Output;
        private Logger MainLogger;
        private PacketHandler PacketHandler;
        private PipeConfig Config;
        private IViewPipeClient Pipeline;
        #endregion
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            MainLogger = new Logger(50);
            InitializePipe();
            Initialize();
        }
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            ParentServer.CloseParent();
        }
        private void Initialize()
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
        }
        #region "Pipeline"
        #region "Private Methods"
        private void InitializePipe()
        {
            Pipeline = new IViewPipeClient(CreatePipeConfig())
            {
                PacketManager = CreatePacketManager()
            };
            OnLogBox("ClientPipelining Successfully Initialized ");
        }
        private PipeConfig CreatePipeConfig()
        {
            Config = new PipeConfig(BUFFER_SIZE, MAX_PIPES);
            return Config;
        }
        private void AddPipeHandlers()
        {
            Pipeline.PipeConnectedEvent += SetOnPipeConnected;
            Pipeline.PipeReceivedEvent += SetOnPipeReceived;
            Pipeline.PipeSentEvent += SetOnPipeSent;
            Pipeline.PipeClosedEvent += SetOnPipeClosed;
            Pipeline.PipeExceptionEvent += SetOnPipeException;
        }
        private void RemovePipeHandlers()
        {
            Pipeline.PipeConnectedEvent -= SetOnPipeConnected;
            Pipeline.PipeReceivedEvent -= SetOnPipeReceived;
            Pipeline.PipeSentEvent -= SetOnPipeSent;
            Pipeline.PipeClosedEvent -= SetOnPipeClosed;
            Pipeline.PipeExceptionEvent -= SetOnPipeException;
        }
        private PacketManager CreatePacketManager()
        {
            PacketManager PacketManager = new PacketManager();
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1111, "SetDetectionType", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1112, "SetOrientation", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1113, "GetDetectedFrame", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1114, "SetDetectedFrame", null));
            PacketManager.AddPacket(new IViewNet.Common.Models.Packet(1115, "EndOfFrame", null));
            return PacketManager;
        }
        private void SetOnPipeConnected()
        {
            OnLogBox("End Pipe Connected Successfully");
        }
        private void SetOnPipeReceived(IViewNet.Common.Models.Packet Message)
        {
            OnLogBox(string.Format("Pipe Received From ServerManager: {0}", Message.Name));
            PacketHandler.HandleFromServerManager(Message);
        }
        private void SetOnPipeSent(IViewNet.Common.Models.Packet Message)
        {
            OnLogBox(string.Format("Pipe Sent To ServerManager: {0}", Message.Name));
        }
        private void SetOnPipeClosed()
        {
            OnLogBox("End Pipe Closed Successfully");
        }
        private void SetOnPipeException(Exception Error)
        {
            OnLogBox(string.Format("Pipe Exception: {0}", Error.Message));
        }
        #endregion
        #region "Public Methods"
        public void ConnectPipeline()
        {
            if (Pipeline.IsPipeConnected == false)
            {
                AddPipeHandlers();
                Pipeline.AttemptToConnect();
            }
        }
        public void ClosePipeline()
        {
            if (Pipeline.IsPipeConnected == true)
            {
                RemovePipeHandlers();
                Pipeline.ShutdownClient();
            }
        }
        public void SendMessage(IViewNet.Common.Models.Packet Message)
        {
            if (Pipeline.IsPipeConnected == true)
            {
                Pipeline.SendMessage(Message);
            }
        }
        #endregion
        #endregion
        #region "Event Handler For Parent"
        private void ParentServer_OnParentCreatedEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnParentClosedEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnExceptionEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void Output_OnFrameEvent(Frame Item)
        {
            OnLogBox(string.Format("Frame[{0}] Received From Child Bytes: {1}", Item.Index, Item.BitmapBytes.Length));
            try
            {
                IViewNet.Common.Models.Packet P = new IViewNet.Common.Models.Packet(1114, "SetDetectedFrame", Item.BitmapBytes);
                Pipeline.SendMessage(P);
            }
            catch (Exception ex)
            {
                OnLogBox("EXCEPTION: " + ex.Message);
            }
        }
        #endregion
        #region "Event Handler For Child"
        private void ParentServer_OnChildCreateEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnChildConnectEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnChildAuthenticateEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnChildSendEvent(IViewNet.Common.Models.Packet Message)
        {
            OnLogBox(string.Format("Sent To Child: {0}", Message.Name));
        }
        private void ParentServer_OnChildReceiveEvent(IViewNet.Common.Models.Packet Message)
        {
            OnLogBox(string.Format("Received From Child: {0}", Message.Name));
            PacketHandler.HandleFromChild(Message);
        }
        private void ParentServer_OnChildDisconnectEvent(string Message)
        {
            OnLogBox(Message);
        }
        private void ParentServer_OnChildDestroyEvent(string Message)
        {
            OnLogBox(Message);
        }
        #endregion

        private delegate void OnLogBoxDelegate(string Message);
        private void OnLogBox(string Message)
        {
            if (LogBox.InvokeRequired)
            {
                LogBox.Invoke(new OnLogBoxDelegate(OnLogBox), new object[] { Message });
            }
            else
            {
                if (LogBox.Lines.Length == 150)
                {
                    LogBox.Clear();
                    LogBox.AppendText(Message + Environment.NewLine);
                }
                else
                {
                    LogBox.AppendText(Message + Environment.NewLine);
                }
            }
        }

        private void QueueCounter_Tick(object sender, EventArgs e)
        {
            InputQueueCountLabel.Text = "Input Queue Count: " + Input.GetCount;
            OutputQueueCountLabel.Text = "Output Queue Count: " + Output.GetCount;
        }
    }
}
