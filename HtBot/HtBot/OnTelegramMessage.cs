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

        public void onTelegramMessage(string chat, string Suser, string text, bool mention, int usermention)
        {
            int user = int.Parse(Suser);

            if (!chat.Equals(Suser))
            {

                if (text.Equals("ajuda"))
                {
                    response.sendHelp();
                }

                if (Regex.IsMatch(text, "^ajuda ([1-9])$", RegexOptions.IgnoreCase))
                {
                    response.sendHelp(Regex.Match(text, "^ajuda ([1-9])$", RegexOptions.IgnoreCase).Groups[1].Value);
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

                if (Regex.IsMatch(text, "^add (.+)", RegexOptions.IgnoreCase))
                {
                    Match nick = Regex.Match(text, "^add (.+)", RegexOptions.IgnoreCase);
                    Telegram.SendTypingStatus();

                    if (nick.Groups[1].Value.Contains(" "))
                    {
                        response.sendNickSpace();
                        return;
                    }
                    List<Account> accs = data.GetAccountList(user);
                    foreach (Account acc in accs)
                    {
                        if (acc.getNick().ToLower().Equals(nick.Groups[1].Value.ToLower()))
                        {
                            response.sendAccountAlreadyAdded();
                            return;
                        }
                    }
                    data.addNewAccount(user, nick.Groups[1].Value);
                    ConsoleIO.WriteLineFormatted("§d[Banco de dados] §2Adicionando o nick " + nick.Groups[1].Value + " id(" + user + ") Ao banco de dados!");
                }

                /*if (Regex.IsMatch(text, "^tadd ([0-9]+) (.+)"))
                {
                    Match nick = Regex.Match(text, "^tadd ([0-9]+) (.+)");
                    data.addTreasure(int.Parse(nick.Groups[1].Value), nick.Groups[2].Value);
                }*/

                if (Regex.IsMatch(text, "^notificar (.+) (.+)", RegexOptions.IgnoreCase))
                {
                    Match notify = Regex.Match(text, "^notificar (.+) (.+)", RegexOptions.IgnoreCase);

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

                if (Regex.IsMatch(text, "^del (.+)", RegexOptions.IgnoreCase))
                {
                    Match nick = Regex.Match(text, "^del (.+)", RegexOptions.IgnoreCase);
                    Telegram.SendTypingStatus();

                    if (nick.Groups[1].Value.Contains(" "))
                    {
                        response.sendNickSpace();
                        return;
                    }
                    data.removeAccount(user, nick.Groups[1].Value);
                    ConsoleIO.WriteLineFormatted("§d[Banco de dados] §2Removendo o nick " + nick.Groups[1].Value + " id(" + user + ") Do banco de dados!");
                }

                if (Regex.IsMatch(text, "^cargo (.+) (user|moderador|admin)$", RegexOptions.IgnoreCase))
                {
                    Match match = Regex.Match(text, "^cargo (.+) (user|moderador|admin)$", RegexOptions.IgnoreCase);
                    string name = match.Groups[1].Value;
                    string cargo = match.Groups[2].Value;

                    bool ok = false;

                    Telegram.SendTypingStatus();

                    if (!mention)
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Mencione alguem para usar este comando!");
                    }

                    if (Telegram.isAdmin(user))
                    {
                        Admin adm = Telegram.getAdmin(user);
                        if (adm.canPromoteMembers())
                        {
                            ok = Telegram.promoteUser(usermention, cargo);
                        }
                        else
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Desculpe, você nao pode alterar cargos!");
                        }
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Apenas Admins podem usar este comando!");
                    }


                    if (ok)
                    {
                        Telegram.SendHtmlMessage(vars.emjok + " Cargo de " + name + " Definido para " + cargo);
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Erro ao processar a solicitação");
                    }

                }

                if (Regex.IsMatch(text, "^contas (.+)$", RegexOptions.IgnoreCase))
                {
                    Match match = Regex.Match(text, "^contas (.+)$", RegexOptions.IgnoreCase);
                    string name = match.Groups[1].Value;

                    bool ok = false;

                    Telegram.SendTypingStatus();

                    if (!mention)
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Mencione alguem para usar este comando!");
                        return;
                    }

                    if (Telegram.isAdmin(user))
                    {
                        Admin adm = Telegram.getAdmin(user);
                        if (adm.canPromoteMembers())
                        {
                            response.sendNicknames(usermention);
                        }
                        else
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Apenas Admins podem usar este comando!");
                        }
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Apenas Admins podem usar este comando!");
                    }

                }

                if (Regex.IsMatch(text, "^reconectar$", RegexOptions.IgnoreCase))
                {
                    Match match = Regex.Match(text, "^reconectar$", RegexOptions.IgnoreCase);

                    Telegram.SendTypingStatus();

                    if (Telegram.isAdmin(user))
                    {
                        Admin adm = Telegram.getAdmin(user);
                        if (adm.canPromoteMembers())
                        {
                            Program.Restart();
                            Telegram.SendHtmlMessage(vars.emjok + " Reconectando ao servidor!");
                        }
                        else
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Desculpe, você não tem permissão para isso!");
                        }
                    }
                    else
                    {
                        Telegram.SendHtmlMessage(vars.emjerror + " Apenas Admins podem usar este comando!");
                    }
                }

                if (text.ToLower().Equals("contas"))
                {
                    response.sendNicknames(user);
                }

                if (vars.loggedIn)
                {
                    if (Regex.IsMatch(text, "^online ([a-zA-Z0-9-_]+)$", RegexOptions.IgnoreCase))
                    {
                        Match nick = Regex.Match(text, "^(?:online|o|a) ([a-zA-Z0-9-_]+)(?: (?:ta|tá|esta|está) on.*|$)", RegexOptions.IgnoreCase);
                        bool isOnline = false;
                        Telegram.SendTypingStatus();

                        foreach (string player in Program.Client.GetOnlinePlayers())
                        {

                            if (player.ToLower().Equals(nick.Groups[1].Value.ToLower()))
                            {
                                isOnline = true;
                                response.sendOnline(isOnline, player);

                            }

                        }

                        if (!isOnline)
                        {
                            foreach (string player in Program.Client.GetOnlinePlayers())
                            {

                                if (player.ToLower().Contains(nick.Groups[1].Value.ToLower()))
                                {
                                    isOnline = true;
                                    response.sendOtherOnline(player);
                                    break;
                                }
                            }

                            if (!isOnline)
                                response.sendOnline(isOnline, nick.Groups[1].Value);
                        }

                    }

                    if (Regex.IsMatch(text, "^money (.+)", RegexOptions.IgnoreCase))
                    {
                        Match nick = Regex.Match(text, "^money (.+)", RegexOptions.IgnoreCase);
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

                    if (Regex.IsMatch(text, "^inspect (.+)", RegexOptions.IgnoreCase))
                    {
                        Match nick = Regex.Match(text, "^inspect (.+)", RegexOptions.IgnoreCase);
                        Telegram.SendTypingStatus();

                        if (nick.Groups[1].Value.Contains(" "))
                        {
                            response.sendNickSpace();
                            return;
                        }

                        vars.checkSkills = true;
                        Program.Client.SendText("/inspect " + nick.Groups[1].Value);

                    }

                    if (Regex.IsMatch(text, "^mctop (.+)", RegexOptions.IgnoreCase))
                    {
                        Match skill = Regex.Match(text, "^mctop (.+)", RegexOptions.IgnoreCase);
                        Telegram.SendTypingStatus();

                        vars.checkmctop = true;
                        Program.Client.SendText("/mctop " + skill.Groups[1].Value);
                        vars.mctopskill = skill.Groups[1].Value;

                    }

                    if (Regex.IsMatch(text, "^mctop$", RegexOptions.IgnoreCase))
                    {
                        Match skill = Regex.Match(text, "^mctop$", RegexOptions.IgnoreCase);
                        Telegram.SendTypingStatus();

                        vars.checkmctop = true;
                        Program.Client.SendText("/mctop");
                        vars.mctopskill = "Geral";

                    }

                    if (Regex.IsMatch(text, "^inspect$", RegexOptions.IgnoreCase))
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

                    if (Regex.IsMatch(text, "^money$", RegexOptions.IgnoreCase))
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

                    if (Regex.IsMatch(text, "^online$", RegexOptions.IgnoreCase))
                    {
                        Telegram.SendTypingStatus();

                        response.sendOnlineNicknames(user);

                    }

                    if (Regex.IsMatch(text, "^verificar (.+)$", RegexOptions.IgnoreCase))
                    {

                        Match match = Regex.Match(text, "^verificar (.+)$", RegexOptions.IgnoreCase);
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
                                if (Telegram.SendPrivateMessage(user, vars.emjok + " Para verificar sua conta " + nick + ", envie um tell <b>dela</b>%0AAssim: <code>/tell htbot verificar " + Token + "</code>","Verificação de conta",true))
                                {
                                    Telegram.SendHtmlMessage(vars.emjok + " Te enviei uma mensagem privada.%0AVerifique-a para confirmar a posse da sua conta!");
                                }
                            }

                        }
                        else
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                        }

                    }

                    if (Regex.IsMatch(text, "^tell ([A-z0-9_-]{1,16}) (.+)$", RegexOptions.IgnoreCase))
                    {

                        Match tell = Regex.Match(text, "^tell ([A-z0-9_-]{1,16}) (.+)$", RegexOptions.IgnoreCase);
                        string nick = tell.Groups[1].Value;
                        string msg = tell.Groups[2].Value;
                        List<Account> accounts = Telegram.data.GetAccountList(user);
                        Telegram.SendTypingStatus();

                        if (accounts.Count > 0)
                        {
                            bool found = false;
                            bool verifyed = false;
                            int Token = 0;

                            bool isOnline = false;

                            foreach (var player in Program.Client.GetOnlinePlayers())
                            {

                                if (player.ToLower().Equals(nick))
                                {
                                    isOnline = true;
                                }

                            }

                            foreach (Account account in accounts)
                            {

                                if (account.getNick().ToLower().Equals(nick.ToLower()))
                                {
                                    found = true;
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

                            if (!verifyed)
                            {
                                Telegram.SendHtmlMessage(vars.emjerror + "Desculpe, Você não tem permissão pra isso!%0AEssa conta é sua? " + vars.emjthinking);
                                return;
                            }

                            if (!isOnline)
                            {
                                Telegram.SendHtmlMessage(vars.emjerror + " Conta Offline!");
                                return;
                            }

                            Program.Client.SendText("/tell " + nick + " 0 " + Token + " " + msg);
                            Telegram.data.ResponseLimit(Token, true);
                            Telegram.SendHtmlMessage(vars.emjok + " Mensagem enviada!");


                        }
                        else
                        {
                            Telegram.SendHtmlMessage(vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                        }

                    }

                    if (text.ToLower().Equals("wm"))
                    {
                        vars.sendWM = true;
                        Program.Client.SendText("/wm");
                    }

                }
                else
                {

                }
            }
            else
            {
                if (Regex.IsMatch(text, "^tell ([A-z0-9_-]{1,16}) (.+)$", RegexOptions.IgnoreCase))
                {

                    Match tell = Regex.Match(text, "^tell ([A-z0-9_-]{1,16}) (.+)$", RegexOptions.IgnoreCase);
                    string nick = tell.Groups[1].Value;
                    string msg = tell.Groups[2].Value;
                    List<Account> accounts = Telegram.data.GetAccountList(user);
                    Telegram.SendTypingStatus(user.ToString());

                    if (accounts.Count > 0)
                    {
                        bool found = false;
                        bool verifyed = false;
                        int Token = 0;

                        bool isOnline = false;

                        foreach (var player in Program.Client.GetOnlinePlayers())
                        {

                            if (player.ToLower().Equals(nick))
                            {
                                isOnline = true;
                            }

                        }

                        foreach (Account account in accounts)
                        {

                            if (account.getNick().ToLower().Equals(nick.ToLower()))
                            {
                                found = true;
                                verifyed = account.getVerify();
                                Token = account.getToken();
                                break;
                            }
                        }

                        if (!found)
                        {
                            Telegram.SendPrivateMessage(user, vars.emjerror + " Conta nao encontrada!");
                            return;
                        }

                        if (!verifyed)
                        {
                            Telegram.SendPrivateMessage(user, vars.emjerror + "Desculpe, Você não tem permissão pra isso!%0AEssa conta é sua? " + vars.emjthinking);
                            return;
                        }

                        if (!isOnline)
                        {
                            Telegram.SendPrivateMessage(user, vars.emjerror + " Conta Offline!");
                            return;
                        }

                        Program.Client.SendText("/tell " + nick + " 1 " + Token + " " + msg);
                        Telegram.data.ResponseLimit(Token, true);
                        Telegram.SendPrivateMessage(user, vars.emjok + " Mensagem enviada!");


                    }
                    else
                    {
                        Telegram.SendPrivateMessage(user, vars.emjerror + " Antes de usar esse comando %0AUse /add <code>nick</code> para adicionar suas contas!");
                    }

                }
                else if (text.ToLower().Equals("start"))
                {
                    Telegram.SendPrivateMessage(user, vars.emjok + " Olá, se você veio aqui pela verificação de conta%0APode voltar ao grupo e concluir a verificação%0AVocê tbm pode me mandar o comando <b>/tell <nick> <mensagem></b> por aqui");
                }
                else
                {
                    Telegram.SendPrivateMessage(user, vars.emjerror + " Por favor, use o bot no grupo!");
                }
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
