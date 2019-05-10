using HtBot.HtBot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MinecraftClient.HtBot
{
    class OnTelegramMessage
    {
        Response response = new Response();
        DataManager data = new DataManager();

        public DataManager GetDataManager() {
            return data;
        }

        public void onTelegramMessage(int user, string text)
        {

            if (text.Equals("ajuda"))
            {
                response.sendHelp();
            }

            if (Regex.IsMatch(text, "^ajuda ([1-9])$"))
            {
                response.sendHelp(Regex.Match(text, "^ajuda ([1-9])$").Groups[1].Value);
            }

            if (text.Equals("changelog"))
            {
                response.sendChangelog();
            }

            if (text.Equals("status"))
            {
                Telegram.SendTypingStatus();
                response.sendStatus(vars.loggedIn);
            }

            if (Regex.IsMatch(text, "^add (.+)"))
            {
                Match nick = Regex.Match(text, "^add (.+)");
                Telegram.SendTypingStatus();

                if (nick.Groups[1].Value.Contains(" "))
                {
                    response.sendNickSpace();
                    return;
                }
                data.addNewAccount(user, nick.Groups[1].Value);
                ConsoleIO.WriteLineFormatted("§d[Banco de dados] §2Adicionando o nick " + nick.Groups[1].Value + " id(" + user + ") Ao banco de dados!");
            }

            /*if (Regex.IsMatch(text, "^tadd ([0-9]+) (.+)"))
            {
                Match nick = Regex.Match(text, "^tadd ([0-9]+) (.+)");
                data.addTreasure(int.Parse(nick.Groups[1].Value), nick.Groups[2].Value);
            }*/

            if (Regex.IsMatch(text, "^nadd (.+) (.+)"))
            {
                Match notify = Regex.Match(text, "^nadd (.+) (.+)");

                data.addNotification(notify.Groups[1].Value, notify.Groups[2].Value);
            }

            if (text.ToLower().Equals("tesouros"))
            {
                response.sendTreasures(user);
            }

            if (text.ToLower().Equals("mensagens"))
            {
                response.sendNotifications(user);
            }

            if (text.ToLower().Equals("limparmensagens"))
            {
                response.sendNotificationsClear(user);
            }

            if (text.ToLower().Equals("limpartesouros"))
            {
                response.sendTreasuresClear(user);
            }

            if (Regex.IsMatch(text, "^del (.+)"))
            {
                Match nick = Regex.Match(text, "^del (.+)");
                Telegram.SendTypingStatus();

                if (nick.Groups[1].Value.Contains(" "))
                {
                    response.sendNickSpace();
                    return;
                }
                data.removeAccount(user, nick.Groups[1].Value);
                ConsoleIO.WriteLineFormatted("§d[Banco de dados] §2Removendo o nick " + nick.Groups[1].Value + " id(" + user + ") Do banco de dados!");
            }

            if (text.ToLower().Equals("contas"))
            {
                response.sendNicknames(user);
            }

            if (vars.loggedIn)
            {
                if (Regex.IsMatch(text, "^(?:online|o|a) ([a-zA-Z0-9-_]+)(?: (?:ta|tá|esta|está) on.*|$)"))
                {
                    Match nick = Regex.Match(text, "^(?:online|o|a) ([a-zA-Z0-9-_]+)(?: (?:ta|tá|esta|está) on.*|$)");
                    bool isOnline = false;
                    Telegram.SendTypingStatus();

                    foreach (var player in Program.Client.GetOnlinePlayers())
                    { 

                        if (player.ToLower().Equals(nick.Groups[1].Value.ToLower()))
                        {
                            isOnline = true;
                            response.sendOnline(isOnline, player);
                            
                        }
                            
                    }

                    if (!isOnline)
                    {
                        foreach (var player in Program.Client.GetOnlinePlayers())
                        {

                            if (player.ToLower().Contains(nick.Groups[1].Value.ToLower()))
                            {
                                isOnline = true;
                                response.sendOtherOnline(player);
                                break;
                            }
                        }

                        if(!isOnline)
                            response.sendOnline(isOnline, nick.Groups[1].Value);
                    }

                }

                if (Regex.IsMatch(text, "^money (.+)"))
                {
                    Match nick = Regex.Match(text, "^money (.+)");
                    Telegram.SendTypingStatus();

                    if (nick.Groups[1].Value.Contains(" "))
                    {
                        if (nick.Groups[1].Value.Contains("rank "))
                        {
                            vars.checkMoneyRank = true;
                            Program.Client.SendText("/money " + nick.Groups[1].Value);
                            return;
                        }
                        else
                        {
                            response.sendNickSpace();
                            return;
                        }
                    }

                    if (nick.Groups[1].Value.ToLower().Equals("top"))
                    {
                        vars.tmoney = true;
                        Program.Client.SendText("/money " + nick.Groups[1].Value);
                        return;
                    }

                    vars.tmoney = true;
                    Program.Client.SendText("/money @" + nick.Groups[1].Value);

                }

                if (Regex.IsMatch(text, "^inspect (.+)"))
                {
                    Match nick = Regex.Match(text, "^inspect (.+)");
                    Telegram.SendTypingStatus();

                    if (nick.Groups[1].Value.Contains(" "))
                    {
                        response.sendNickSpace();
                        return;
                    }

                    vars.checkSkills = true;
                    Program.Client.SendText("/inspect " + nick.Groups[1].Value);

                }

                if (Regex.IsMatch(text, "^mctop (.+)"))
                {
                    Match skill = Regex.Match(text, "^mctop (.+)");
                    Telegram.SendTypingStatus();

                    vars.checkmctop = true;
                    Program.Client.SendText("/mctop " + skill.Groups[1].Value);
                    vars.mctopskill = skill.Groups[1].Value;

                }

                if (Regex.IsMatch(text, "^mctop$"))
                {
                    Match skill = Regex.Match(text, "^mctop$");
                    Telegram.SendTypingStatus();

                    vars.checkmctop = true;
                    Program.Client.SendText("/mctop");
                    vars.mctopskill = "Geral";

                }

                if (Regex.IsMatch(text, "^inspect$"))
                {
                    Telegram.SendTypingStatus();

                    vars.checkMultipleSkills = true;

                    List<Account> accounts = Telegram.data.GetAccountList(user);

                    vars.multipleskillscheck = accounts.Count;
                    vars.checkedskillscount = 0;

                    ConsoleIO.WriteLine(accounts.Count + "");

                    if (accounts.Count > 0)
                    {
                        foreach (Account account in accounts)
                        {
                            string acc = account.getNick();
                            wait(500);
                            Program.Client.SendText("/inspect " + acc);
                            wait(500);
                        }
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                    }


                }

                if (Regex.IsMatch(text, "^money$"))
                {
                    Telegram.SendTypingStatus();

                    vars.checkMultipleMoney = true;

                    List<Account> accounts = Telegram.data.GetAccountList(user);

                    vars.multiplemoneycheck = accounts.Count;
                    vars.checkedmoneycount = 0;

                    if (accounts.Count > 0)
                    {
                        foreach (Account account in accounts)
                        {
                            string acc = account.getNick();
                            wait(500);
                            Program.Client.SendText("/money @" + acc);
                            wait(500);
                        }
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                    }


                }

                if (Regex.IsMatch(text, "^online$"))
                {
                    Telegram.SendTypingStatus();

                    response.sendOnlineNicknames(user);

                }

                if (Regex.IsMatch(text, "^verificar (.+)$"))
                {

                    Match match = Regex.Match(text, "^verificar (.+)$");
                    string nick = match.Groups[1].Value;
                    List<Account> accounts = Telegram.data.GetAccountList(user);
                    Telegram.SendTypingStatus();

                    if (accounts.Count > 0)
                    {
                        bool found = false;
                        string acc = "";
                        bool verifyed = false;
                        int Token = 0;

                        foreach (Account account in accounts)
                        {
                            
                            if (account.getNick().ToLower().Equals(nick.ToLower()))
                            {
                                found = true;
                                acc = account.getNick();
                                verifyed = account.getVerify();
                                Token = account.getToken();
                                break;
                            }
                        }

                        if (!found)
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Conta nao encontrada!");
                            return;
                        }

                        if (verifyed)
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " A conta já estava verificada!");
                            return;
                        }

                        if ((found) && (!verifyed))
                        {
                            Telegram.SendHtmlMessage(vars.emjok + " Para verificar sua conta, envie um tell <b>dela</b>%0AAssim: <code>/tell htbot verificar " + Token + "</code>");
                        }

                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                    }

                }

            }
            else
            {

            }
            
        }

        public void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            //Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                //Console.WriteLine("stop wait timer");
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

    }
}
