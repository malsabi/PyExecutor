namespace PyExecutor.PCPC
{
    public class ParentConfig
    {
        /// <summary>
        /// Static string that reperents packet delimiter for strucuting
        /// </summary>
        public static char COMMAND_DELIMITER
        {
            get
            {
                return '|';
            }
        }

        /// <summary>
        /// Static string that represents command name for getting handshake
        /// </summary>
        public static string COMMAND_NAME_GETHANDSHAKE
        {
            get
            {
                return "GetHandShake";
            }
        }

        /// <summary>
        /// Static string that represents command name for setting handshake
        /// </summary>
        public static string COMMAND_NAME_SETHANDSHAKE
        {
            get
            {
                return "SetHandShake";
            }
        }

        /// <summary>
        /// Static string that represents command name for getting detected frame
        /// </summary>
        public static string COMMAND_NAME_GETDETECTEDFRAME
        {
            get
            {
                return "GetDetectedFrame";
            }
        }

        /// <summary>
        /// Static string that represents command name for setting detected frame
        /// </summary>
        public static string COMMAND_NAME_SETDETECTEDFRAME
        {
            get
            {
                return "SetDetectedFrame";
            }
        }

        /// <summary>
        /// Static string that represents command name for terminating the current child
        /// </summary>
        public static string COMMAND_NAME_TERMINATE
        {
            get
            {
                return "Terminate";
            }
        }
        
        /// <summary>
        /// Static string that represents command name if the child has any errors
        /// </summary>
        public static string COMMAND_NAME_ERROR
        {
            get
            {
                return "Error";
            }
        }

        /// <summary>
        /// Static string that represents command name if the child has any information
        /// </summary>
        public static string COMMAND_NAME_INFO
        {
            get
            {
                return "Information";
            }
        }

        /// <summary>
        /// Static string that represents command name to get child status
        /// </summary>
        public static string COMMAND_NAME_GET_STATUS
        {
            get
            {
                return "GetChildStatus";
            }
        }

        /// <summary>
        /// Static string that represents command name to set child status
        /// </summary>
        public static string COMMAND_NAME_SET_STATUS
        {
            get
            {
                return "SetChildStatus";
            }
        }


        /// <summary>
        /// The python shell path for executing the python script
        /// </summary>
        public static string PythonShellPath
        {
            get
            {
                //return "C:\\Users\\malsa\\anaconda3\\python.exe";
                return "C:\\Users\\malsa\\anaconda3\\envs\\yolov5-3.0\\python.exe";
            }
        }

        /// <summary>
        /// The python script path to be executed ie: detect.py
        /// </summary>
        public static string PythonScriptPath
        {
            get
            {
                return "C:\\Users\\malsa\\Documents\\PyCharm\\Projects\\yolov5-3.0\\Main.py";
                //return "C:\\Users\\malsa\\Documents\\PyCharm\\Projects\\PythonChild\\main.py";
            }
        }
  
        /// <summary>
        /// The maximum number of child process can be created
        /// </summary>
        public int MaximumChildrenInstances { get; set; }


        public ParentConfig()
        {
            this.MaximumChildrenInstances = 5;
        }
        public ParentConfig(int MaximumChildrenInstances)
        {
            this.MaximumChildrenInstances = MaximumChildrenInstances;
        }
    }
}