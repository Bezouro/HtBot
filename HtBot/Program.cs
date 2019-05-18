using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinecraftClient.Protocol;
using System.Reflection;
using System.Threading;
using MinecraftClient.Protocol.Handlers.Forge;
using MinecraftClient.Protocol.Session;
using MinecraftClient.WinAPI;
using MinecraftClient.HtBot;

namespace MinecraftClient
{
    /// <summary>
    /// Minecraft Console Client by ORelio and Contributors (c) 2012-2018.
    /// Allows to connect to any Minecraft server, send and receive text, automated scripts.
    /// This source code is released under the CDDL 1.0 License.
    /// </summary>
    static class Program
    {
        public static McTcpClient Client;
        public static string[] startupargs;

        public const string Version = MCHighestVersion;
        public const string MCLowestVersion = "1.4.6";
        public const string MCHighestVersion = "1.13.2";
        public static readonly string BuildInfo = null;
        public static bool doReconnect = true;
        public static string connectedServer;

        private static Thread offlinePrompt = null;
        private static bool useMcVersionOnce = false;

        /// <summary>
        /// The main entry point of Minecraft Console Client
        /// </summary>
        static void Main(string[] args)
        {
            ConsoleIO.WriteLineFormatted("§2[HtBot] §9Inicializando o bot xD");
            //Console.WriteLine("Console Client for MC {0} to {1} - v{2} - By ORelio & Contributors", MCLowestVersion, MCHighestVersion, Version);
            HtBot.Telegram.Start();

            //Build information to facilitate processing of bug reports
            if (BuildInfo != null)
            {
                ConsoleIO.WriteLineFormatted("§8" + BuildInfo);
            }

            //Debug input ?
            if (args.Length == 1 && args[0] == "--keyboard-debug")
            {
                Console.WriteLine("Keyboard debug mode: Press any key to display info");
                ConsoleIO.DebugReadInput();
            }

            //Setup ConsoleIO
            ConsoleIO.LogPrefix = "§8[MCC] ";
            if (args.Length >= 1 && args[args.Length - 1] == "BasicIO")
            {
                ConsoleIO.BasicIO = true;
                args = args.Where(o => !Object.ReferenceEquals(o, args[args.Length - 1])).ToArray();
            }

            //Take advantage of Windows 10 / Mac / Linux UTF-8 console
            if (isUsingMono || WindowsVersion.WinMajorVersion >= 10)
            {
                Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;
            }

            //Process ini configuration file
            if (args.Length >= 1 && System.IO.File.Exists(args[0]) && System.IO.Path.GetExtension(args[0]).ToLower() == ".ini")
            {
                Settings.LoadSettings(args[0]);

                //remove ini configuration file from arguments array
                List<string> args_tmp = args.ToList<string>();
                args_tmp.RemoveAt(0);
                args = args_tmp.ToArray();
            }
            else if (System.IO.File.Exists("MinecraftClient.ini"))
            {
                Settings.LoadSettings("MinecraftClient.ini");
            }
            else Settings.WriteDefaultSettings("MinecraftClient.ini");

            Settings.Login = "HtBot";
            Settings.Password = "-";
            

            //Other command-line arguments
            if (args.Length >= 1)
            {
                string server = args[0].ToLower();
                switch (server)
                {

                    case "asgard": Settings.SetServerIP("Asgard.craftlandia.com.br"); connectedServer = "Asgard"; break;
                    case "destiny": Settings.SetServerIP("destiny.craftlandia.com.br"); connectedServer = "Destiny"; break;
                    case "propano": Settings.SetServerIP("propano.craftlandia.com.br"); connectedServer = "Propano"; break;
                    default: Settings.SetServerIP("legacy.craftlandia.com.br"); connectedServer = "Asgard"; break;

                }

                //Single command?
                //if (args.Length >= 3)
                //{
                //    Settings.SingleCommand = args[2];
                //}

            }
            else
            {
                Settings.SetServerIP("legacy.craftlandia.com.br");
                connectedServer = "Legacy";
            }

            if (Settings.ConsoleTitle != "")
            {
                Settings.Username = "New Window";
                Console.Title = Settings.ExpandVars(Settings.ConsoleTitle);
            }

            //Load cached sessions from disk if necessary
            if (Settings.SessionCaching == CacheType.Disk)
            {
                bool cacheLoaded = SessionCache.InitializeDiskCache();
                if (Settings.DebugMessages)
                    ConsoleIO.WriteLineFormatted(cacheLoaded ? "§8Os dados da sessão foram carregados do disco." : "§8Não foram encontradas sessões armazenadas no disco.");
            }

            //Asking the user to type in missing data such as Username and Password

            if (Settings.Login == "")
            {
                Console.Write(ConsoleIO.BasicIO ? "Insira seu nick/email do minecraft \n" : "Login : ");
                Settings.Login = Console.ReadLine();
            }
            if (Settings.Password == "" && (Settings.SessionCaching == CacheType.None || !SessionCache.Contains(Settings.Login.ToLower())))
            {
                //RequestPassword();
            }

            startupargs = args;
            InitializeClient();
        }

        /// <summary>
        /// Reduest user to submit password.
        /// </summary>
        private static void RequestPassword()
        {
            Console.Write(ConsoleIO.BasicIO ? "Por favor, insira a senha para o usuario " + Settings.Login + ".\n" : "Senha : ");
            Settings.Password = ConsoleIO.BasicIO ? Console.ReadLine() : ConsoleIO.ReadPassword();
            if (Settings.Password == "") { Settings.Password = "-"; }
            if (!ConsoleIO.BasicIO)
            {
                //Hide password length
                Console.CursorTop--; Console.Write("Senha : <******>");
                for (int i = 19; i < Console.BufferWidth; i++) { Console.Write(' '); }
            }
        }

        /// <summary>
        /// Start a new Client
        /// </summary>
        private static void InitializeClient()
        {

            vars.logInTimestamp = Telegram.data.getTimestamp() + 60;

            SessionToken session = new SessionToken();

            ProtocolHandler.LoginResult result = ProtocolHandler.LoginResult.LoginRequired;

            if (Settings.Password == "-")
            {
                //ConsoleIO.WriteLineFormatted("§8Iniciando no modo OffLine.");
                result = ProtocolHandler.LoginResult.Success;
                session.PlayerID = "0";
                session.PlayerName = Settings.Login;
            }
            else
            {
                // Validate cached session or login new session.
                if (Settings.SessionCaching != CacheType.None && SessionCache.Contains(Settings.Login.ToLower()))
                {
                    session = SessionCache.Get(Settings.Login.ToLower());
                    result = ProtocolHandler.GetTokenValidation(session);
                    if (result != ProtocolHandler.LoginResult.Success)
                    {
                        ConsoleIO.WriteLineFormatted("§8Cached session is invalid or expired.");
                        if (Settings.Password == "")
                            RequestPassword();
                    }
                    else ConsoleIO.WriteLineFormatted("§8Cached session is still valid for " + session.PlayerName + '.');
                }

                if (result != ProtocolHandler.LoginResult.Success)
                {
                    Console.WriteLine("Connecting to Minecraft.net...");
                    result = ProtocolHandler.GetLogin(Settings.Login, Settings.Password, out session);

                    if (result == ProtocolHandler.LoginResult.Success && Settings.SessionCaching != CacheType.None)
                    {
                        SessionCache.Store(Settings.Login.ToLower(), session);
                    }
                }

            }

            if (result == ProtocolHandler.LoginResult.Success)
            {
                Settings.Username = "HtBot";

                if (Settings.ConsoleTitle != "")
                    Console.Title = Settings.ExpandVars(Settings.ConsoleTitle);

                if (Settings.playerHeadAsIcon)
                    ConsoleIcon.setPlayerIconAsync(Settings.Username);

                if (Settings.DebugMessages)
                    Console.WriteLine("Success. (session ID: " + session.ID + ')');

                //ProtocolHandler.RealmsListWorlds(Settings.Username, PlayerID, sessionID); //TODO REMOVE

                if (Settings.ServerIP == "")
                {
                    Console.Write("Server IP : ");
                    Settings.SetServerIP(Console.ReadLine());
                }

                //Get server version
                int protocolversion = 0; 
                ForgeInfo forgeInfo = null;

                //if (Settings.ServerVersion != "" && Settings.ServerVersion.ToLower() != "auto")
                if (true)
                {
                        //protocolversion = Protocol.ProtocolHandler.MCVer2ProtocolVersion(Settings.ServerVersion);
                        protocolversion = Protocol.ProtocolHandler.MCVer2ProtocolVersion("1.8");

                    if (protocolversion != 0)
                    {
                        ConsoleIO.WriteLineFormatted("§8Usando a versão " + Settings.ServerVersion + " (protocolo v" + protocolversion + ')');
                    }
                    else ConsoleIO.WriteLineFormatted("§8A versão '" + Settings.ServerVersion + "' é desconhecida ou não suportada :/\nDetectando versão compativel automaticamente!");

                    if (useMcVersionOnce)
                    {
                        useMcVersionOnce = false;
                        Settings.ServerVersion = "";
                    }
                }

                if (protocolversion == 0)
                {
                    //Console.WriteLine("Obtendo informações do servidor...");
                    if (!ProtocolHandler.GetServerInfo(Settings.ServerIP, Settings.ServerPort, ref protocolversion, ref forgeInfo))
                    {
                        //HandleFailure("Falha ao conectar :/", true, ChatBots.AutoRelog.DisconnectReason.ConnectionLost);
                        return;
                    }
                }

                if (protocolversion != 0)
                {
                    try
                    {
                        //Start the main TCP client
                        if (Settings.SingleCommand != "")
                        {
                            Client = new McTcpClient(session.PlayerName, session.PlayerID, session.ID, Settings.ServerIP, Settings.ServerPort, protocolversion, forgeInfo, Settings.SingleCommand);
                        }
                        else Client = new McTcpClient(session.PlayerName, session.PlayerID, session.ID, protocolversion, forgeInfo, Settings.ServerIP, Settings.ServerPort);

                        //Update console title
                        if (Settings.ConsoleTitle != "")
                            Console.Title = Settings.ExpandVars(Settings.ConsoleTitle);
                    }
                    catch (NotSupportedException) { HandleFailure("Não foi possivel conectar-se ao servidor : Versão nao suportada !", true); }
                }
                else HandleFailure("Falha ao determinar a versão do servidor.", true);
            }
            else
            {
                string failureMessage = "Minecraft Login failed : ";
                switch (result)
                {
                    case ProtocolHandler.LoginResult.AccountMigrated: failureMessage += "Account migrated, use e-mail as username."; break;
                    case ProtocolHandler.LoginResult.ServiceUnavailable: failureMessage += "Login servers are unavailable. Please try again later."; break;
                    case ProtocolHandler.LoginResult.WrongPassword: failureMessage += "Incorrect password, blacklisted IP or too many logins."; break;
                    case ProtocolHandler.LoginResult.InvalidResponse: failureMessage += "Invalid server response."; break;
                    case ProtocolHandler.LoginResult.NotPremium: failureMessage += "User not premium."; break;
                    case ProtocolHandler.LoginResult.OtherError: failureMessage += "Network error."; break;
                    case ProtocolHandler.LoginResult.SSLError: failureMessage += "SSL Error."; break;
                    default: failureMessage += "Unknown Error."; break;
                }
                if (result == ProtocolHandler.LoginResult.SSLError && isUsingMono)
                {
                    ConsoleIO.WriteLineFormatted("§8It appears that you are using Mono to run this program."
                        + '\n' + "The first time, you have to import HTTPS certificates using:"
                        + '\n' + "mozroots --import --ask-remove");
                    return;
                }
                HandleFailure(failureMessage, false, ChatBot.DisconnectReason.LoginRejected);
            }
        }

        /// <summary>
        /// Disconnect the current client from the server and restart it
        /// </summary>
        /// <param name="delaySeconds">Optional delay, in seconds, before restarting</param>
        public static void Restart(int delaySeconds = 0)
        {
            vars.loggedIn = false;
            new Thread(new ThreadStart(delegate
            {
                if (Client != null) { Client.Disconnect(); ConsoleIO.Reset(); }
                if (offlinePrompt != null) { offlinePrompt.Abort(); offlinePrompt = null; ConsoleIO.Reset(); }
                if (delaySeconds > 0)
                {
                    ConsoleIO.WriteLineFormatted("§3[Conexão] §cReconectando o HtBot em  " + delaySeconds + " segundos");
                    System.Threading.Thread.Sleep(delaySeconds * 1000);
                }
                //Console.WriteLine("Reconectando o HtBot...");
                InitializeClient();
            })).Start();
        }

        /// <summary>
        /// Disconnect the current client from the server and exit the app
        /// </summary>
        public static void Exit()
        {
            new Thread(new ThreadStart(delegate
            {
                if (Client != null) { Client.Disconnect(); ConsoleIO.Reset(); }
                if (offlinePrompt != null) { offlinePrompt.Abort(); offlinePrompt = null; ConsoleIO.Reset(); }
                if (Settings.playerHeadAsIcon) { ConsoleIcon.revertToCMDIcon(); }
                Environment.Exit(0);
            })).Start();
        }

        /// <summary>
        /// Handle fatal errors such as ping failure, login failure, server disconnection, and so on.
        /// Allows AutoRelog to perform on fatal errors, prompt for server version, and offline commands.
        /// </summary>
        /// <param name="errorMessage">Error message to display and optionally pass to AutoRelog bot</param>
        /// <param name="versionError">Specify if the error is related to an incompatible or unkown server version</param>
        /// <param name="disconnectReason">If set, the error message will be processed by the AutoRelog bot</param>
        public static void HandleFailure(string errorMessage = null, bool versionError = false, ChatBots.AutoRelog.DisconnectReason? disconnectReason = null)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                ConsoleIO.Reset();
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
                Console.WriteLine(errorMessage);

                if (disconnectReason.HasValue)
                {
                    if (ChatBots.AutoRelog.OnDisconnectStatic(disconnectReason.Value, errorMessage))
                        return; //AutoRelog is triggering a restart of the client
                }
            }

            if (Settings.interactiveMode)
            {
                if (versionError)
                {
                    Console.Write("Versão do servidor : ");
                    Settings.ServerVersion = Console.ReadLine();
                    if (Settings.ServerVersion != "")
                    {
                        useMcVersionOnce = true;
                        Restart();
                        return;
                    }
                }

                if (doReconnect)
                {
                    McTcpClient.ReconnectionAttemptsLeft = 999;
                    Program.Restart(30);
                    vars.loggedIn = false;
                }
                else
                {
                    if (offlinePrompt == null)
                    {
                        offlinePrompt = new Thread(new ThreadStart(delegate
                        {

                            string command = " ";
                            ConsoleIO.WriteLineFormatted("O HtBot está desconectado. Use '" + (Settings.internalCmdChar == ' ' ? "" : "" + Settings.internalCmdChar) + "help' para ajuda.");
                            ConsoleIO.WriteLineFormatted("Ou aperte Enter para fechar.");
                            while (command.Length > 0)
                            {
                                if (!ConsoleIO.BasicIO)
                                {
                                    ConsoleIO.Write('>');
                                }
                                command = Console.ReadLine().Trim();
                                if (command.Length > 0)
                                {
                                    string message = "";

                                    if (Settings.internalCmdChar != ' '
                                        && command[0] == Settings.internalCmdChar)
                                        command = command.Substring(1);

                                    if (command.StartsWith("reco"))
                                    {
                                        message = new Commands.Reco().Run(null, Settings.ExpandVars(command));
                                    }
                                    else if (command.StartsWith("connect"))
                                    {
                                        message = new Commands.Connect().Run(null, Settings.ExpandVars(command));
                                    }
                                    else if (command.StartsWith("exit") || command.StartsWith("quit"))
                                    {
                                        message = new Commands.Exit().Run(null, Settings.ExpandVars(command));
                                    }
                                    else if (command.StartsWith("help"))
                                    {
                                        ConsoleIO.WriteLineFormatted("§8MCC: " + (Settings.internalCmdChar == ' ' ? "" : "" + Settings.internalCmdChar) + new Commands.Reco().CMDDesc);
                                        ConsoleIO.WriteLineFormatted("§8MCC: " + (Settings.internalCmdChar == ' ' ? "" : "" + Settings.internalCmdChar) + new Commands.Connect().CMDDesc);
                                    }
                                    else ConsoleIO.WriteLineFormatted("§8Comando desconhecido -> '" + command.Split(' ')[0] + "'.");

                                    if (message != "")
                                        ConsoleIO.WriteLineFormatted("§8MCC: " + message);
                                }
                            }
                        }));
                        offlinePrompt.Start();
                    }
                }

            }
            else
            {
                McTcpClient.ReconnectionAttemptsLeft = 999;
                Program.Restart(2);
                vars.loggedIn = false;
            }
        }

        /// <summary>
        /// Detect if the user is running Minecraft Console Client through Mono
        /// </summary>
        public static bool isUsingMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }

        /// <summary>
        /// Enumerate types in namespace through reflection
        /// </summary>
        /// <param name="nameSpace">Namespace to process</param>
        /// <param name="assembly">Assembly to use. Default is Assembly.GetExecutingAssembly()</param>
        /// <returns></returns>
        public static Type[] GetTypesInNamespace(string nameSpace, Assembly assembly = null)
        {
            if (assembly == null) { assembly = Assembly.GetExecutingAssembly(); }
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        /// <summary>
        /// Static initialization of build information, read from assembly information
        /// </summary>
        static Program()
        {
            AssemblyConfigurationAttribute attribute
             = typeof(Program)
                .Assembly
                .GetCustomAttributes(typeof(System.Reflection.AssemblyConfigurationAttribute), false)
                .FirstOrDefault() as AssemblyConfigurationAttribute;
            if (attribute != null)
                BuildInfo = attribute.Configuration;
        }
    }
}
