using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using HtBot.HtBot;
using System.Collections.Generic;

namespace MinecraftClient.HtBot
{
    class Telegram
    {
        
        private const string telegram = "http://api.telegram.org/bot";
        private const string token = "656836377:AAFWhZ8NzU7vRowm2uioJuTbQNzb4EHNWOM";
        private const string chatid = "-1001182153789";
        private static string json = null;
        private static OnTelegramMessage telegramEvent = new OnTelegramMessage();
        private static WebClient Connection = new WebClient();
        public static DataManager data = new DataManager();
        public static string BotID;
        public static string BotName;
        public static string BotUserName;

        public static void Start() {

            data.init();

            string jsonMe = Connection.DownloadString(telegram + token + "/getMe");
            JObject jsonObject = JObject.Parse(jsonMe);

            JToken ok = jsonObject.GetValue("ok");
            JObject result = JObject.Parse(jsonObject.GetValue("result").ToString());

            JToken ID = result.GetValue("id");
            JToken IsBot = result.GetValue("is_bot");
            JToken FirstName = result.GetValue("first_name");
            JToken UserName = result.GetValue("username");

            BotID = ID.ToString();
            BotName = FirstName.ToString();
            BotUserName = "@" + UserName.ToString();

            ConsoleIO.WriteLineFormatted("§6[Telegram] §e Obtendo dados do bot!");
            ConsoleIO.WriteLineFormatted("§6[Telegram] §3Bot Username: " + BotUserName);
            ConsoleIO.WriteLineFormatted("§6[Telegram] §3Bot Name: " + BotName);
            ConsoleIO.WriteLineFormatted("§6[Telegram] §3Bot ID: " + BotID);
            ConsoleIO.WriteLineFormatted("§6[Telegram] §3Bot : " + IsBot);

            Thread t = new Thread(Thread);
            t.Start();
        }

        public static bool SendMessage(string text, string type = "Resposta de comando")
        {
            string message = text.Replace(",", "%2C").Replace(".", "%2E");

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(telegram + token + "/sendMessage?chat_id=" + chatid + "&text=" + message);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))


                ConsoleIO.WriteLineFormatted("§6[Telegram] §aMensagem enviada ao telegram: §f" + type);
                return true;
            }
            catch (System.Exception e)
            {
                ConsoleIO.WriteLineFormatted("§6[Telegram] §cNão foi possivel enviar a mensagem: §f" + type);
                ConsoleIO.WriteLineFormatted(e.GetBaseException() + ": " + e.Message);
                return false;
            }

        }
        public static bool SendHtmlMessage(string text, string type = "Resposta de comando")
        {
            string message = text.Replace(",", "%2C").Replace(".", "%2E");

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(telegram + token + "/sendMessage?chat_id=" + chatid + "&text=" + message + "&parse_mode=html");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))


                    ConsoleIO.WriteLineFormatted("§6[Telegram] §aMensagem enviada ao telegram: §f" + type);
                return true;
            }
            catch (System.Exception e)
            {
                ConsoleIO.WriteLineFormatted("§6[Telegram] §cNão foi possivel enviar a mensagem: §f" + type);
                ConsoleIO.WriteLineFormatted(e.GetBaseException() + ": " + e.Message);
                return false;
            }

        }
        public static bool SendPrivateMessage(int id, string text, string type = "Resposta de comando", bool fromgroup = false)
        {
            string message = text.Replace(",", "%2C").Replace(".", "%2E");

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(telegram + token + "/sendMessage?chat_id=" + id + "&text=" + message + "&parse_mode=html");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadLine();
                JObject jsonObject = JObject.Parse(json);
                ConsoleIO.WriteLineFormatted("§6[Telegram] §aMensagem enviada ao telegram: §f" + type);

                return true;
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;

                if ((int)response.StatusCode == 400)
                {
                    if (fromgroup)
                    {
                        SendHtmlMessage(vars.emjerror + " Por favor antes disso me envie uma mensagem privada!%0A<a href=\"tg://user?id=656836377\">Toque aqui</a>");
                    }
                    return false;
                }
                else if ((int)response.StatusCode == 403)
                {
                    if (fromgroup)
                    {
                        SendHtmlMessage(vars.emjerror + " Por favor antes disso me envie uma mensagem privada!%0A<a href=\"tg://user?id=656836377\">Toque aqui</a>");
                    }
                    return false;
                }
                else
                { 
                    ConsoleIO.WriteLineFormatted("§6[Telegram] §cNão foi possivel enviar a mensagem: §f" + type);
                    ConsoleIO.WriteLineFormatted(e.GetBaseException() + ": " + e.Message);
                    return false;
                }
            }

        }

        public static List<Admin> getGroupAdmins()
        {

            string getAdmins = telegram + token + "/getChatAdministrators?chat_id=" + chatid;
            List<Admin> admins = new List<Admin>();

            try
            {
                json = Connection.DownloadString(getAdmins);

                JObject jsonObject = JObject.Parse(json);

                JToken ok = jsonObject.GetValue("ok");
                JToken result = jsonObject.GetValue("result");
                JArray array = JArray.Parse(result.ToString());

                foreach (JObject jAdmin in array)
                {
                    Admin adm = new Admin(jAdmin);

                    admins.Add(adm);

                }

                return admins;

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static bool SendTypingStatus(string user = chatid) {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(telegram + token + "/sendChatAction?chat_id=" + user + "&action=typing");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))

                    ConsoleIO.WriteLineFormatted("§6[Telegram] §aStatus \"Escrevendo\" enviado ao telegram!");
                return true;
            }
            catch
            {
                ConsoleIO.WriteLineFormatted("§6[Telegram] §cNão foi possivel enviar o status \"Escrevendo\" ");
                return false;
            }
        }

        public static bool isAdmin(int id)
        {
            List<Admin> admins = getGroupAdmins();

            foreach (Admin adm in admins)
            {
                if (adm.id() == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static Admin getAdmin(int id)
        {
            List<Admin> admins = getGroupAdmins();

            foreach (Admin adm in admins)
            {
                if (adm.id() == id)
                {
                    return adm;
                }
            }

            return null;
        }

        public static bool promoteUser(int user, string cargo)
        {
            string crg = "";

            if (cargo.ToLower().Equals("moderador"))
            {
                crg = "&can_delete_messages=true&can_restrict_members=true";
            }

            if (cargo.ToLower().Equals("admin"))
            {
                crg = "&can_invite_users=true&can_change_info=true&can_delete_messages=true&can_restrict_members=true&can_pin_messages=true&can_promote_members=true";
            }


            string promote = telegram + token + "/promoteChatMember?chat_id=-1001182153789&user_id=" + user + crg;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(telegram + token + "/promoteChatMember?chat_id=-1001182153789&user_id=" + user + crg);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))

                ConsoleIO.WriteLineFormatted("§6[Telegram] §aUsuario Promovido!");
                return true;
            }
            catch(Exception e)
            {
                ConsoleIO.WriteLineFormatted("§6[Telegram] §cNão foi possivel promover o usuario ");
                SendPrivateMessage(566959554, e.Message);
                SendPrivateMessage(566959554, promote);
                return false;
            }
        }

        static void Thread()
        {

            ConsoleIO.WriteLineFormatted("§6[Telegram] Conexão ao telegram iniciada!");
            SendMessage("Olá, Estou de volta e pronto para te ajudar 😃.%0Aℹ Use \"Ajuda\" Para ver os comandos disponiveis!","Inicialização do bot");
            string getUpdates = telegram + token + "/getUpdates";
            string clearOffset = telegram + token + "/getUpdates?offset=";

            bool telegramDebug = false;

            do
            {
                try
                {
                    int type = 0;

                    json = Connection.DownloadString(getUpdates);

                    JObject jsonObject = JObject.Parse(json);

                    JToken ok = jsonObject.GetValue("ok");
                    JToken result = jsonObject.GetValue("result");
                    JArray array = JArray.Parse(result.ToString());

                    foreach (var jobject in array)
                    {
                        JObject parsing = JObject.Parse(jobject.ToString());
                        JToken UpdateID = parsing.GetValue("update_id");
                        JObject message;

                        try
                        {
                            message = JObject.Parse(parsing.GetValue("message").ToString());
                        }
                        catch (Exception e)
                        {
                            message = JObject.Parse(parsing.GetValue("edited_message").ToString());
                        }
                        
                        JToken MessageID = message.GetValue("message_id");
                        JObject from = JObject.Parse(message.GetValue("from").ToString());
                        JToken FromID = from.GetValue("id");
                        JToken IsBot = from.GetValue("is_bot");
                        JToken FirstName = from.GetValue("first_name");
                        JObject chat = JObject.Parse(message.GetValue("chat").ToString());
                        JToken ChatID = chat.GetValue("id");
                        JToken ChatTitle = chat.GetValue("title");
                        JToken ChatType = chat.GetValue("type");
                        JToken MessageDate = message.GetValue("date");
                        JToken MessageText = message.GetValue("text");

                        bool hasMention = false;
                        int mentionedId = 0;
                        JToken Entities = null;
                        JToken newMemberName = null;
                        JToken leftMemberName = null;
                        JToken newMemberID = null;

                        try
                        {
                            JArray Aentities = JArray.Parse(message.GetValue("entities").ToString());
                            foreach (JObject obj in Aentities)
                            {
                                JObject jObject = JObject.Parse(obj.ToString());
                                if (jObject.GetValue("type").ToString().Equals("text_mention"))
                                {
                                    hasMention = true;
                                    JObject mentionedUser = JObject.Parse(jObject.GetValue("user").ToString());
                                    mentionedId = (int) mentionedUser.GetValue("id");

                                }

                            }

                        }
                        catch (Exception e)
                        {
                            ConsoleIO.WriteLine(e.Message);
                        }

                        if (type == 0)
                        {
                            try
                            {
                                JObject newMember = JObject.Parse(message.GetValue("new_chat_member").ToString());
                                newMemberName = newMember["first_name"];
                                newMemberID = newMember["id"];
                                type = 1;
                            }
                            catch
                            {
                                
                            }
                        }

                        if (type == 0)
                        {
                            try
                            {
                                JObject leftMember = JObject.Parse(message.GetValue("left_chat_member").ToString());
                                leftMemberName = leftMember["first_name"];
                                type = 2;
                            }
                            catch
                            {
                                
                            }
                        }

                        if (telegramDebug)
                        {
                            ConsoleIO.WriteLineFormatted("");
                            ConsoleIO.WriteLineFormatted("");
                            ConsoleIO.WriteLineFormatted("§6[Telegram Status] " + ok);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Update ID] " + UpdateID);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Message] " + MessageID);
                            ConsoleIO.WriteLineFormatted("§6[Telegram From ID] " + FromID);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Is Bot] " + IsBot);
                            ConsoleIO.WriteLineFormatted("§6[Telegram First Name] " + FirstName);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Chat ID] " + ChatID);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Chat Title] " + ChatTitle);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Chat Type] " + ChatType);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Chat Date] " + MessageDate);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Chat Text] " + MessageText);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Has Mention] " + hasMention);
                            ConsoleIO.WriteLineFormatted("§6[Telegram Mentioned User] " + mentionedId);
                        }

                        int UpdateId = int.Parse(UpdateID.ToString());
                        UpdateId++;
                        Connection.DownloadString(clearOffset + UpdateId);

                        if (type == 0)
                        {
                            ConsoleIO.WriteLineFormatted("§6[Telegram] Nova Mensagem: " + MessageText);
                            data.verifyBotUser((int)FromID, (string)FirstName);
                            telegramEvent.onTelegramMessage((string)ChatID, (string)FromID, MessageText.ToString().Replace(BotUserName, "").Replace("/", ""), hasMention, mentionedId);
                        }

                        if (type == 1)
                        {
                            ConsoleIO.WriteLineFormatted("§6[Telegram] Novo membro no grupo!");
                            SendMessage("Olá " + newMemberName + " Bem vindo ao grupo! " + vars.emjsmile, "1");
                            data.verifyBotUser((int)newMemberID, (string)newMemberName);
                        }

                        if (type == 2)
                        {
                            ConsoleIO.WriteLineFormatted("§6[Telegram] Um membro saiu do grupo!");
                            SendMessage(leftMemberName + " Saiu do grupo. " + vars.emjneutralface, "2");
                        }

                    }

                }
                catch (System.Exception e)
                {
                    if (vars.debug)
                    {
                        ConsoleIO.WriteLineFormatted(e.ToString());
                    }
                }

            } while (true);
        }

    }
}
