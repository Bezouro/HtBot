using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MinecraftClient.HtBot
{
    class Response
    {
        //<a href=\"tg://user?id=566959554\">ChangeLog</a> text mention
        //<code>meu money</code> texto azul

        public void sendHelp(string page = "1")
        {
            Telegram.SendTypingStatus();
            List<string> response = new List<string>();
            string msg = null;

            switch (page)
            {
                case "1":
                    response.Add("Ajuda (1/3)");
                    response.Add("Comandos disponiveis:");
                    response.Add("════════════════════");
                    response.Add("\"/contas\" mostra seus nicks");
                    response.Add("\"/add (nick)\" adiciona o nick a sua lista pessoal de nicks");
                    response.Add("\"/del (nick)\" Remove o nick da sua lista pessoal de nicks");
                    response.Add("\"/verificar (nick)\" Verifica a posse da conta");
                    response.Add("\"/ajuda\" mostra esses comandos :)");
                    response.Add("\"/money (nick)\" mostra o money do jogador");
                    response.Add("\"/money top\" mostra o ranking dos 15 jogadores mais ricos do servidor");
                    response.Add("\"/money\" verifica o money de todas as suas contas");
                    response.Add("\"/ajuda 2\" exibe a proxima pagina");
                    break;

                case "2":
                    response.Add("Ajuda (2/3)");
                    response.Add("Comandos disponiveis:");
                    response.Add("════════════════════");
                    response.Add("\"/online (nick)\" Verifica se o player ta online no servidor");
                    response.Add("\"/online\" verifica o estado das suas contas");
                    response.Add("\"/inspect (nick)\" mostra as skills de qualquer player do servidor");
                    response.Add("\"/inspect\" verifica os stats de suas contas");
                    response.Add("\"/mctop (skill)\" verifica o mctop de determinada skill");
                    response.Add("\"/mctop\" exibe a lista dos players com mais skills somadas do servidor");
                    response.Add("\"/changelog\" Exibe as mudanças do bot!");
                    response.Add("\"/ajuda 3\" exibe a proxima pagina");
                    break;

                case "3":
                    response.Add("Ajuda (3/3)");
                    response.Add("Comandos disponiveis:");
                    response.Add("════════════════════");
                    response.Add("\"/mensagens\" Exibe suas notificações");
                    response.Add("\"/tesouros\" Exibe seus tesouros");
                    response.Add("\"/limparmensagens\" Limpa suas notificações");
                    response.Add("\"/limpartesouros\" Limpa seus tesouros");
                    response.Add("\"/wm\" Exibe a quantidade de players no server");
                    response.Add("\"/ajuda 4\" Futuramente exibira a proxima pagina!");
                    break;
                default:
                    response.Add(vars.emjerror + " <b>Pagina de ajuda nao encontrada</b>");
                    break;
            }

            foreach (string content in response)
            {
                msg = msg + "%0A" + content;
            }
            Telegram.SendHtmlMessage(msg);

        }

        public void sendChangelog()
        {
            Telegram.SendTypingStatus();
            List<string> response = new List<string>();
            string msg = null;

            response.Add("Changelog");
            response.Add("════════════════════");
            response.Add("----(09/05/2019)----");
            response.Add("-  adicionado sistema de verificação%0ADe varias contas de 1x");
            response.Add("----(10/05/2019)----");
            response.Add("-  Adicionada verificação de posse das contas!");
            response.Add("-  Adicionada verificação se uma conta ja estava adicionada!");
            response.Add("-  Adicionada verificação de posse das contas!");
            response.Add("----(11/05/2019)----");
            response.Add("-  adicionado /wm");
            response.Add("-  mensagem de money atualizada");
            response.Add("-  <b>Novidades Em Breve!</b>");


            foreach (string content in response)
            {
                msg = msg + "%0A" + content;
            }
            Telegram.SendHtmlMessage(msg);

        }

        public void sendAccountRemoved()
        {
            Telegram.SendHtmlMessage(vars.emjinfo + " Conta removida com sucesso!");
        }

        public void sendAccountCleared()
        {
            Telegram.SendHtmlMessage(vars.emjinfo + " Suas contas foram removidas!");
        }

        public void sendAccountAdded()
        {
            Telegram.SendHtmlMessage(vars.emjinfo + " Conta adicionada com sucesso!");
        }
        public void sendAccountAlreadyAdded()
        {
            Telegram.SendHtmlMessage(vars.emjerror + " A conta ja estava adicionada!");
        }

        public void sendNicknames(int user)
        {
            string accs = Telegram.data.GetAccounts(user);
            int treasures = Telegram.data.getTreasuresCount(user);
            int notifications = Telegram.data.getNotificationsCount(user);
            string Notification = null;
            if ((treasures > 0) || (notifications > 0))
            {
                Notification = "═════════════";
            }

            if (treasures > 0)
            {
                Notification = Notification + "%0AVocê pegou " + treasures + " Tesouros!";
            }

            if (notifications > 0)
            {
                Notification = Notification + "%0AVocê Tem " + notifications + " Notificações!";
            }

            if (treasures > 0)
            {
                Notification = Notification + "%0AVeja seus tesouros com /tesouros ";
            }

            if (notifications > 0)
            {
                Notification = Notification + "%0AVeja suas notificações com /mensagens ";
            }

            if (accs.Equals("-1")) { Telegram.SendHtmlMessage(vars.emjerror + "<b>Você não tem contas cadastradas!</b>"); }
            else { Telegram.SendHtmlMessage("Suas contas: %0A════════════════════%0A" + accs + Notification); }
        }

        public void sendProtectedNicknames(int user)
        {
            string accs = Telegram.data.GetProtectedAccounts(user);
            if (accs.Equals("-1")) { Telegram.SendHtmlMessage(vars.emjerror + "<b>Você não tem contas cadastradas!</b>"); }
            else { Telegram.SendHtmlMessage("🛡HtBot Protect🛡%0A════════════════════%0A" + accs); }
        }

        public void sendOnlineNicknames(int user)
        {
            string accs = Telegram.data.GetOnlineAccounts(user);

            if (accs.Equals("-1")) { Telegram.SendHtmlMessage(vars.emjerror + "<b>Você não tem contas cadastradas!</b>"); }
            else { Telegram.SendHtmlMessage("Suas contas online: %0A════════════════════%0A" + accs); }
        }

        public void sendTreasures(int user)
        {
            string treasures = Telegram.data.getTreasures(user);
            if (treasures.Equals("-1")) { Telegram.SendHtmlMessage("<b>Sua lista de tesouros está vazia!</b>"); }
            else { Telegram.SendHtmlMessage("Seus tesouros: %0A════════════════════%0A" + treasures + "═════════════%0A/limpartesouros"); }
        }

        public void sendTreasuresClear(int user)
        {
            bool treasures = Telegram.data.clearTreasures(user);
            if (!treasures) { Telegram.SendHtmlMessage(vars.emjerror + "<b>Sua lista de tesouros já estava vazia!</b>"); }
            else { Telegram.SendHtmlMessage(vars.emjinfo + "<b>Sua lista de tesouros foi limpa!</b>"); }
        }


        public void sendNotifications(int user)
        {
            string messages = Telegram.data.getNotifications(user);
            if (messages.Equals("-1")) { Telegram.SendHtmlMessage("<b>Sua lista de mensagens está vazia!</b>"); }
            else { Telegram.SendHtmlMessage("Suas mensagens: %0A════════════════════%0A" + messages + "═════════════%0A/limparmensagens"); }
        }

        public void sendNotificationsClear(int user)
        {
            bool messages = Telegram.data.clearNotifications(user);
            if (!messages) { Telegram.SendHtmlMessage(vars.emjerror + "<b>Sua lista de mensagens já estava vazia!</b>"); }
            else { Telegram.SendHtmlMessage(vars.emjinfo + "<b>Sua lista de mensagens foi limpa!</b>"); }
        }

        public void sendWM(int online)
        {
            vars.sendWM = false;
            Telegram.SendHtmlMessage(vars.emjinfo + " Atualmente o servidor tem <b>" + online + "</b> Players online!");
        }

        public void sendMoney(string nick, string money)
        {
            Telegram.SendHtmlMessage(vars.emjmoney + " O money de " + nick + " é <b>" + money + "</b> Coins!");
        }
        public void sendMultipleMoney(List<string> moneyList)
        {

            string Money = "Seu money: %0A════════════════════%0A";
            double total = 0;

            foreach (string line in moneyList)
            {
                if (Regex.IsMatch(line, "^(.+) (.+)$"))
                {
                    Match match = Regex.Match(line, "^(.+) <b>(.+)</b>$");
                    string moneynick = match.Groups[1].Value;
                    string money = match.Groups[2].Value;

                    money = money.Replace(",", "");

                    total = total + double.Parse(money);
                    Money = Money + line + "%0A";
                }
            }

            if (!vars.IsRunningOnMono())
            {
                total = total / 100;
            }

            string Total = total.ToString("C").Replace("¤", vars.emjmoney + " ");

            if (moneyList.Count > 1)
            {
                Money = Money + "════════════════════%0ATotal: <b>" + Total + "</b>";
            }

            vars.checkMultipleMoney = false;
            vars.multiplemoney.Clear();
            Telegram.SendHtmlMessage(Money);
        }

        public void sendNickSpace()
        {
            Telegram.SendHtmlMessage("❌ <b>O nick nao pode conter espaço!</b>");
        }

        public void sendOnline(bool online, string nick)
        {
            if(online) Telegram.SendHtmlMessage("O player <code>" + nick + "</code> está online " + vars.emjsmile);
            else Telegram.SendHtmlMessage("O player <code>" + nick + "</code> está Offline " + vars.emjneutralface);
        }

        public void sendOtherOnline(string nick)
        {
            Telegram.SendHtmlMessage("Encontrei o player <code>" + nick + "</code> online " + vars.emjsmile);
        }

        public void sendStatus(bool status)
        {
            if(status) Telegram.SendHtmlMessage(vars.emjinfo + "Estou online no servidor <code>" + Program.connectedServer + "</code>!");
            else Telegram.SendMessage("Estou offline no momento " + vars.emjneutralface);
        }

        public void sendMoneyTop(List<string> top)
        {
            Telegram.SendTypingStatus();
            string msg = null;

            top.Insert(0, "Players mais ricos do servidor:%0A════════════════════%0A");

            foreach (string content in top)
            {
                msg = msg + content;
            }
            Telegram.SendHtmlMessage(msg);

        }

        public void sendSkills(List<string> skills)
        {
            Telegram.SendTypingStatus();
            string msg = null;

            foreach (string content in skills)
            {
                msg = msg + content;
            }

            Telegram.SendHtmlMessage(msg);

            vars.skills.Clear();
            vars.checkSkills = false;
            vars.checkMultipleSkills = false;
        }
        public void sendmctop(List<string> mctop)
        {
            Telegram.SendTypingStatus();
            string msg = null;

            foreach (string content in mctop)
            {
                msg = msg + content;
            }
            Telegram.SendHtmlMessage(msg);
            vars.checkmctop = false;
        }

        public void sendmcrank(List<string> mcrank)
        {
            Telegram.SendTypingStatus();
            string msg = null;

            foreach (string content in mcrank)
            {
                msg = msg + content;
            }
            Telegram.SendHtmlMessage(msg);
            vars.checkmcrank = false;
        }

    }
}
