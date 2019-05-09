using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MinecraftClient.HtBot
{
    class DataManager
    {
        private Response response = new Response();
        public string dataFile = "DataBase.htbot";
        static JObject data = new JObject();
        static JArray users = new JArray();
        JArray accounts;
        public bool dataLoaded = false;

        public void init()
        {
            if (System.IO.File.Exists("DataBase.htbot")) LoadData();
            else WriteDefaultData();
            if (data != null) dataLoaded = true;
        }

        public void WriteDefaultData()
        {
            ConsoleIO.WriteLineFormatted("§d[Banco de dados] §aGerando arquivo de banco de dados");
            data.Add("users", users);
            System.IO.File.WriteAllText(dataFile, data.ToString());
        }

        public void WriteData()
        {
            System.IO.File.WriteAllText(dataFile, data.ToString());
        }

        public void LoadData()
        {
            ConsoleIO.WriteLineFormatted("§d[Banco de dados] §aInicializando banco de dados");
            data = JObject.Parse(File.ReadAllText(dataFile));
            users = (JArray)data["users"];
        }

        public void addNewBotUser(int user, string name)
        {
            ConsoleIO.WriteLineFormatted("§d[Banco de dados] §2Adicionando o usuario " + name + " id(" + user + ") Ao banco de dados!");
            JObject usuario = new JObject();
            JToken id = user;
            accounts = new JArray();
            usuario.Add("user_id", id);
            usuario.Add("user_accounts", accounts);
            users.Add(usuario);
            WriteData();
        }

        public void stuff()
        {
            JObject json = new JObject();

            JArray users = new JArray();
            JObject usersj = new JObject();

            JToken id = 001;
            usersj.Add("user_id", id);

            JObject jsona = new JObject();
            JToken nick = "htbruno";
            jsona.Add("nick", nick);

            JArray treasures = new JArray();
            JObject treasuresArray = new JObject();
            treasures.Add(treasuresArray);
            jsona.Add("treasures", treasures);

            JArray array = new JArray();
            array.Add(jsona);

            usersj.Add("user_accounts", array);
            users.Add(usersj);
            json.Add("users", users);

            System.IO.File.WriteAllText("stuff.txt", json.ToString());
        }

        public void verifyBotUser(int user, string name)
        {
            bool added = false;
            foreach (var theUser in users)
            {
                JObject parsingUser = JObject.Parse(theUser.ToString());
                if ((int)parsingUser.GetValue("user_id") == user)
                {
                    added = true;
                    break;
                }
                else added = false;
            }
            if (!added)
            {
                addNewBotUser(user, name);
                Telegram.SendMessage(vars.emjinfo + "Hey " + name + ", estou te cadastrando no meu banco de dados %0AAgora você pode registrar seus nicks e aproveitar melhor o bot" + vars.emjsmile, "Registrando usuario");
            }
        }

        public void addNewAccount(int user, string name)
        {
            try
            {
                int count = 0;
                foreach (var zzz in users)
                {
                    JObject parsingUser = (JObject)users[count];
                    if ((int)parsingUser.GetValue("user_id") == user)
                    {
                        break;
                    }
                    count++;
                }

                JObject theUser = (JObject)users[count];
                JArray accounts = (JArray)theUser["user_accounts"];

                JObject jsona = new JObject();
                JToken nick = name;
                jsona.Add("nick", nick);

                JArray treasures = new JArray();
                jsona.Add("treasures", treasures);
                JArray notifications = new JArray();
                jsona.Add("notifications", notifications);

                accounts.Add(jsona);
                WriteData();
                response.sendAccountAdded();
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
            }
        }

        public void removeAccount(int user, string name)
        {
            try
            {
                int count = 0;
                int count2 = 0;
                foreach (var zzz in users)
                {
                    JObject parsingUser = (JObject)users[count];
                    if ((int)parsingUser.GetValue("user_id") == user)
                    {
                        break;
                    }
                    count++;
                }

                JObject theUser = (JObject)users[count];
                JArray accounts = (JArray)theUser["user_accounts"];

                if (name.Equals("all"))
                {
                    accounts.RemoveAll();
                    WriteData();
                    response.sendAccountCleared();
                    return;
                }
                
                foreach (var zzz in accounts)
                {
                    JObject parsingUser = (JObject)accounts[count2];
                    string nick = parsingUser["nick"].ToString();
                    ConsoleIO.WriteLineFormatted(name + "->" + nick);

                    if (nick.ToLower().Equals(name.ToLower()))
                    {
                        parsingUser.Remove();
                        WriteData();
                        response.sendAccountRemoved();
                        break;
                    }
                    count2++;
                }

                
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
            }
        }

        public string GetAccounts(int user)
        {
            try
            {
                bool found = false;
                string reply = null;

                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        foreach (JToken acc in accounts)
                        {
                            string conta = (string)acc["nick"];

                            if (!found)
                            {
                                if (conta != null)
                                {
                                    found = true;
                                }
                            }

                            string nick = "<code>" + conta + "</code>";
                            reply = reply + nick + "%0A";
                        }

                    }

                }

                if (found)
                {
                    return reply;
                }
                else
                {
                    return "-1";
                }
            }
            catch(Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return "-1";
            }

        }

        public string GetOnlineAccounts(int user)
        {
            try
            {
                bool found = false;
                string reply = null;

                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        foreach (JToken acc in accounts)
                        {
                            string conta = (string)acc["nick"];
                            bool isOnline = false;

                            if (!found)
                            {
                                if (conta != null)
                                {
                                    found = true;
                                }
                            }


                            foreach (var player in Program.Client.GetOnlinePlayers())
                            {

                                if (player.ToLower().Equals(conta.ToLower()))
                                {
                                    isOnline = true;
                                }

                            }

                            string nick;

                            if (isOnline)
                            {
                                nick = vars.emjok + " <code>" + conta + "</code>";
                            }
                            else
                            {
                                nick = vars.emjcl + " <code>" + conta + "</code>";
                            }

                            
                            reply = reply + nick + "%0A";
                        }

                    }

                }

                if (found)
                {
                    return reply;
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return "-1";
            }

        }

        public List<string> GetAccountList(int user)
        {
            try
            {
                bool found = false;
                List<string> contas = new List<string>();

                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        foreach (JToken acc in accounts)
                        {
                            string conta = (string)acc["nick"];

                            if (!found)
                            {
                                if (conta != null)
                                {
                                    found = true;
                                }
                            }

                            contas.Add(conta);

                        }

                    }

                }

                if (found)
                {
                    return contas;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return null;
            }

        }

        public void addTreasure(int lvl, string nick)
        {

            DateTime dt = DateTime.Now;
            string date = DateTime.Today.ToString("dd/MM");
            string Time = dt.ToLongTimeString();

            try
            {
                int count = 0;
                foreach (var zzz in users)
                {
                    JObject theUser = (JObject)users[count];
                    JArray accounts = (JArray)theUser["user_accounts"];
                    int count2 = 0;

                    foreach (var aaa in accounts)
                    {
                        JObject parsingAccount = (JObject)accounts[count2];
                        string Nick = parsingAccount["nick"].ToString();
                        //ConsoleIO.WriteLineFormatted(nick + "->" + Nick);

                        if (Nick.ToLower().Equals(nick.ToLower()))
                        {
                            JArray treasures = (JArray) parsingAccount["treasures"];
                            treasures.Add("[" + date + " " + Time + "] " + lvl);
                            Telegram.SendHtmlMessage("🎉A conta <code>" + nick + "</code> Acaba de pegar um <b>Tesouro Nível " + lvl + "</b>🎉");
                            Program.Client.SendText("/g [BezouroBot] " + nick + " usa o BezouroBot e ja sabe que achou um tesouro " + lvl);
                            WriteData();
                            break;
                        }
                        count2++;
                    }
                    count++;
                }

            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
            }
        }

        public void addNotification(string nick, string notify)
        {

            DateTime dt = DateTime.Now;
            string date = DateTime.Today.ToString("dd/MM");
            string Time = dt.ToLongTimeString();

            try
            {
                int count = 0;
                foreach (var zzz in users)
                {
                    JObject theUser = (JObject)users[count];
                    JArray accounts = (JArray)theUser["user_accounts"];
                    int count2 = 0;

                    foreach (var aaa in accounts)
                    {
                        JObject parsingAccount = (JObject)accounts[count2];
                        string Nick = parsingAccount["nick"].ToString();
                        //ConsoleIO.WriteLineFormatted(nick + "->" + Nick);

                        if (Nick.ToLower().Equals(nick.ToLower()))
                        {
                            JArray notifications = (JArray)parsingAccount["notifications"];
                            notifications.Add("<b>[" + date + " " + Time + "]</b> " + notify);
                            WriteData();
                            break;
                        }
                        count2++;
                    }
                    count++;
                }

            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
            }
        }

        public string getTreasures(int user)
        {
            try
            {
                bool found = false;
                string reply = null;

                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        bool foundtreasure;

                        foreach (JToken acc in accounts)
                        {
                            string conta = (string)acc["nick"];
                            JArray treasures = (JArray)acc["treasures"];
                            foundtreasure = false;
                            string accTsrs = "<code>" + conta + "</code>: %0A";

                            foreach (JValue lvl in treasures)
                            {
                                if (Regex.IsMatch(lvl.ToString(), "^(\\[\\d\\d/\\d\\d \\d\\d:\\d\\d:\\d\\d\\]) (\\d{1,2})$"))
                                {
                                    Match match = Regex.Match(lvl.ToString(), "^(\\[\\d\\d/\\d\\d \\d\\d:\\d\\d:\\d\\d\\]) (\\d{1,2})$");
                                    string time = match.Groups[1].Value;
                                    int level = int.Parse(match.Groups[2].Value);

                                    if (!found)
                                    {
                                        if (lvl != null)
                                        {
                                            found = true;
                                            foundtreasure = true;
                                        }
                                    }
                                    else
                                    {
                                        if (lvl != null)
                                        {
                                            foundtreasure = true;
                                        }
                                    }
                                    string book = null;

                                    if (level > 0 && level < 7)
                                    {
                                        book = "📗";
                                    }

                                    if (level > 6 && level < 10)
                                    {
                                        book = "📘";
                                    }

                                    if (level == 10)
                                    {
                                        book = "📙";
                                    }
                                    if (level == 11)
                                    {
                                        book = "📕";
                                    }
                                    if (level > 11)
                                    {
                                        book = "📓";
                                    }

                                    accTsrs = accTsrs + "   <b>" + time + "</b> " + book + "Tesouro Nivel (" + level + ")%0A";
                                }
                            }

                            if (foundtreasure)
                            {
                                reply = reply + accTsrs;
                            }

                        }

                    }

                }

                if (found)
                {
                    return reply;
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return "-1";
            }

        }

        public string getNotifications(int user)
        {
            try
            {
                bool found = false;
                string reply = null;

                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        bool foundmessage;

                        foreach (JToken acc in accounts)
                        {
                            string conta = (string)acc["nick"];
                            JArray notifications = (JArray)acc["notifications"];
                            foundmessage = false;
                            string accMsgs = "<code>" + conta + "</code>: %0A";


                            foreach (JValue notification in notifications)
                            {
                                string Notification = notification.ToString();
                                if (!found)
                                {
                                    if (Notification != null)
                                    {
                                        found = true;
                                        foundmessage = true;
                                    }
                                }
                                else {
                                    if (Notification != null)
                                    {
                                        foundmessage = true;
                                    }
                                }
                                string type = null;

                                if (true)
                                {
                                    type = "  ✅ ";
                                }

                                accMsgs = accMsgs + "   " + notification + "%0A";

                            }

                            if (foundmessage) {
                                reply = reply + accMsgs;
                            }

                        }

                    }

                }

                if (found)
                {
                    return reply;
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return "-1";
            }

        }

        public int getTreasuresCount(int user)
        {
            try
            {
                int found = 0;
                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());
                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        foreach (JToken acc in accounts)
                        {
                            JArray treasures = (JArray)acc["treasures"];
                            foreach (JValue lvl in treasures) { found++; }
                        }

                    }

                }

                return found;
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return 0;
            }

        }

        public int getNotificationsCount(int user)
        {
            try
            {
                int found = 0;
                foreach (var theUser in users)
                {
                    JObject User = JObject.Parse(theUser.ToString());
                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User.GetValue("user_accounts");
                        foreach (JToken acc in accounts)
                        {
                            JArray notifications = (JArray)acc["notifications"];
                            foreach (JValue msg in notifications) { found++; }
                        }

                    }

                }

                return found;
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return 0;
            }

        }

        public bool clearTreasures(int user)
        {
            try
            {
                bool found = false;
                int count = 0;
                int count2 = 0;

                foreach (var theUser in users)
                {
                    JObject User = (JObject)users[count];

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User["user_accounts"];
                        foreach (var zzz in accounts)
                        {
                            JToken acc = accounts[count2];
                            string conta = (string)acc["nick"];
                            JArray treasures = (JArray)acc["treasures"];
                            foreach (JValue lvl in treasures){if (!found){if (lvl != null){found = true;}}}
                            treasures.RemoveAll();
                            count2++;
                        }

                    }

                    count++;

                }

                if (found)
                {
                    WriteData();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return false;
            }

        }

        public bool clearNotifications(int user)
        {
            try
            {
                bool found = false;
                int count = 0;
                int count2 = 0;

                foreach (var theUser in users)
                {
                    JObject User = (JObject)users[count];

                    if ((int)User.GetValue("user_id") == user)
                    {
                        JArray accounts = (JArray)User["user_accounts"];
                        foreach (var zzz in accounts)
                        {
                            JToken acc = accounts[count2];
                            string conta = (string)acc["nick"];
                            JArray notifications = (JArray)acc["notifications"];
                            foreach (JValue msg in notifications) { if (!found) { if (msg != null) { found = true; } } }
                            notifications.RemoveAll();
                            count2++;
                        }

                    }

                    count++;

                }

                if (found)
                {
                    WriteData();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ConsoleIO.WriteLineFormatted(e.ToString());
                return false;
            }

        }

    }
}
