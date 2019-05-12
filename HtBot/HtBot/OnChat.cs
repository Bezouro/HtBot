using MinecraftClient.Protocol;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MinecraftClient.HtBot
{

    public class OnChat : ChatBot
    {
        Response response = new Response();
        public void onchat(string text, bool isJson)
        {

            List<string> links = new List<string>();
            string json = null;
            if (isJson)
            {
                json = text;
                text = ChatParser.ParseText(json, links);
            }

            String chat = text;
            String chatclean = GetVerbatim(text);

            if (chatclean.Equals("»Bem vindo de volta. Por favor digite /login sua-senha."))
            {
                ConsoleIO.WriteLine("[Auto-Login] Logando no servidor");
                Program.Client.SendText("/login 59185555");
                vars.loggedIn = true;
            }

            if (Regex.IsMatch(chatclean, "^Saldo de (.+): (.+) Coins.$"))
            {
                Match match = Regex.Match(chatclean, "^Saldo de (.+): (.+) Coins.$");
                string moneynick = match.Groups[1].Value;
                string money = match.Groups[2].Value;


                if (!vars.checkMultipleMoney)
                {
                    if (vars.tmoney)
                    {
                        response.sendMoney(moneynick, money);
                        vars.tmoney = false;
                    }
                }
                else
                {
                    vars.multiplemoney.Add(moneynick + " <b>" + money + "</b>");

                    if (vars.multiplemoney.Count == vars.multiplemoneycheck)
                    {
                        response.sendMultipleMoney(vars.multiplemoney);
                    }
                }
            }

            if (Regex.IsMatch(chatclean, "^(.+): ([0-9,]+) XP\\((.+)\\/(.+)\\)$"))
            {
                Match match = Regex.Match(chatclean, "(.+): ([0-9,]+) XP\\((.+)\\/(.+)\\)$");
                string skill = match.Groups[1].Value;
                string level = match.Groups[2].Value;
                string xp1 = match.Groups[3].Value;
                string xp2 = match.Groups[4].Value;

                switch (skill)
                {
                    case "Acrobacia": skill = "🤸🏻‍♂️ (" + skill; break;
                    case "Reparaçao": skill = "▫️ (" + skill; break;
                    case "Machado": skill = "⚔️ (" + skill; break;
                    case "Arqueiro": skill = "🏹 (" + skill; break;
                    case "Espadas": skill = "⚔️ (" + skill; break;
                    case "Domar": skill = "🦴 (" + skill; break;
                    case "Desarmado": skill = "👊🏻 (" + skill; break;
                    case "Escavaçao": skill = "🥄 (" + skill; break;
                    case "Pescador": skill = "🐟 (" + skill; break;
                    case "Herbalismo": skill = "🌿 (" + skill; break;
                    case "Mineraçao": skill = "⛏ (" + skill; break;
                    case "Lenhador": skill = "🌳 (" + skill; break;
                }

                if ((vars.checkSkills)||(vars.checkMultipleSkills))
                {

                    vars.skills.Add(skill + ")" + " <b>" + level + "</b> [<code>" + xp1 + "/" + xp2 + "</code>]%0A");
                    vars.skillsList++;
                    if (!vars.checkMultipleSkills)
                    {
                        if (vars.skillsList == 10)
                        {
                            response.sendSkills(vars.skills);
                        }
                    }
                    else
                    {
                        if ((vars.checkedskillscount == vars.multipleskillscheck)&&(vars.skillsList == 10))
                        {
                            response.sendSkills(vars.skills);
                        }
                    }
                }
            }

            if (Regex.IsMatch(chatclean, "^(\\d{2,5})\\. (.+) - (\\d{1,5})$"))
            {
                Match match = Regex.Match(chatclean, "^(\\d{2,5})\\. (.+) - (\\d{1,5})$");
                int pos = int.Parse(match.Groups[1].Value);
                string nick = match.Groups[2].Value;
                int level = int.Parse(match.Groups[3].Value);
                string strpos;

                int dif;

                switch (pos)
                {
                    case 1: strpos = "🥇"; break;
                    case 2: strpos = "🥈"; break;
                    case 3: strpos = "🥉"; break;
                    case 4: strpos = " ➃"; break;
                    case 5: strpos = " ➄"; break;
                    case 6: strpos = " ➅"; break;
                    case 7: strpos = " ➆"; break;
                    case 8: strpos = " ➇"; break;
                    case 9: strpos = " ➈"; break;
                    default: strpos = "(" + pos + ")";  break;
                }

                if (pos == 1) {
                    vars.firstlevel = level;
                }

                dif = vars.firstlevel - level;

                if (vars.checkmctop)
                {
                    if (dif > 0)
                    {
                        vars.mctop.Add(strpos + " <b>" + nick + "</b> (<code>" + level + "</code>) - " + dif + "%0A");
                    }
                    else
                    {
                        vars.mctop.Add(strpos + " <b>" + nick + "</b> (<code>" + level + "</code>)%0A");
                    }

                    vars.mcTopList++;
                    if (vars.mcTopList == 10)
                    {
                        response.sendmctop(vars.mctop);
                    }
                }
            }

            if (Regex.IsMatch(chatclean, "^Skills do player (.+)$"))
            {
                Match match = Regex.Match(chatclean, "^Skills do player (.+)$");

                if (vars.checkSkills)
                {
                    vars.skills.Clear();
                    vars.skillsList = 0;
                    vars.skills.Add("Essas são as skills de <code>" + match.Groups[1].Value + "</code> :%0A════════════════════%0A");
                }
                if (vars.checkMultipleSkills) {
                    vars.checkedskillscount++;
                    vars.skillsList = 0;
                    if (vars.checkedskillscount > 1) {
                        vars.skills.Add("%0A");
                    }
                    vars.skills.Add("Essas são as skills de <code>" + match.Groups[1].Value + "</code> :%0A════════════════════%0A");
                }
            }

            if (Regex.IsMatch(chatclean, "^--CraftLandia--$"))
            {
                if (vars.checkmctop)
                {
                    vars.mctop.Clear();
                    vars.mcTopList = 0;
                    vars.mctop.Add("Ranking da skill <code>" + vars.mctopskill + "</code> :%0A════════════════════%0A");
                }
            }

            if (Regex.IsMatch(chatclean, "^([0-9]+)\\) (.+) \\(([0-9,.]+) Coins\\)$"))
            {
                Match match = Regex.Match(chatclean, "([0-9]+)\\) (.+) \\(([0-9,.]+) Coins\\)$");
                int pos = int.Parse(match.Groups[1].Value);
                string nick = match.Groups[2].Value;
                string money = match.Groups[3].Value;
                int arraypos = pos - 1;

                if (vars.tmoney)
                {

                    vars.moneytop.Insert(arraypos, "[" + pos + "]" + " <b>" + nick + "</b> <code>" + money + "</code>%0A");
                    vars.moneyTopList++;
                    if (vars.moneyTopList == 15)
                    {
                        response.sendMoneyTop(vars.moneytop);
                    }
                }
            }

            if (Regex.IsMatch(chatclean, "^\\[Tesouro\\] (.+) encontrou um livro: Tesouro Nível (\\d{1,2})$"))
            {
                Match match = Regex.Match(chatclean, "^\\[Tesouro\\] (.+) encontrou um livro: Tesouro Nível (\\d{1,2})$");
                string nick = match.Groups[1].Value;
                int level = int.Parse(match.Groups[2].Value);

                Telegram.data.addTreasure(level, nick);
            }

            if (chatclean.Equals("» TOP 15 jogadores mais ricos:"))
            {
                if (vars.tmoney)
                {
                    vars.moneytop.Clear();
                    vars.moneyTopList = 0;
                }
            }

            if (Regex.IsMatch(chatclean, "(.+) alcançou nível (\\d{1,4}) na skill (.+)!"))
            {
                Match notificação = Regex.Match(chatclean, "(.+) alcançou nível (\\d{1,4}) na skill (.+)!");
                string nick = notificação.Groups[1].Value;
                int nivel = int.Parse(notificação.Groups[2].Value);
                string skill = notificação.Groups[3].Value;

                
                bool success = Telegram.data.addNotification(nick, "<i>Alcançou</i> <code>" + nivel + "</code> <i>na skill:</i> <code>" + skill + "</code>");

                if (success)
                {
                    Telegram.SendHtmlMessage("🎉A conta <code>" + nick + "</code> Alcançou <code>" + nivel + "</code> <i>na skill:</i> <code>" + skill + "</code>🎉");
                    Program.Client.SendText("/g [Bezouro Bot] " + nick + " usa o Bot e ja sabe que alcançou " + nivel + " " + skill);

                }

            }

            if (Regex.IsMatch(chatclean, "^(.+) é o (\\d{1,5}). mais rico do servidor.$"))
            {
                if (vars.checkMoneyRank)
                {
                    vars.checkMoneyRank = false;
                    Match notificação = Regex.Match(chatclean, "^(.+) é o (\\d{1,5}). mais rico do servidor.$");
                    string nick = notificação.Groups[1].Value;
                    int pos = int.Parse(notificação.Groups[2].Value);

                    Telegram.SendHtmlMessage("<code>" + pos + "º</code>) <b>" + nick + "</b>");
                }

            }

            if (Regex.IsMatch(chatclean, "^\\(Mensagem de (.+)\\): verificar (\\d\\d\\d\\d)$"))
            {
                Match match = Regex.Match(chatclean, "^\\(Mensagem de (.+)\\): verificar (\\d\\d\\d\\d)$");
                string Nick = match.Groups[1].Value;
                int Token = int.Parse(match.Groups[2].Value);

                Program.Client.SendText("/r Verificando codigo!");

                bool success = Telegram.data.Verify(Nick, Token);

                wait(4000);

                if (success)
                {
                    Program.Client.SendText("/r Conta verificada com sucesso!");
                }
                else
                {
                    Program.Client.SendText("/r Houve um erro ao verificar sua conta!");
                }

            }

            if (Regex.IsMatch(chatclean, "^» Players online: (\\d{1,4})$"))
            {
                Match match = Regex.Match(chatclean, "^» Players online: (\\d{1,4})$");
                if (!vars.sendWM)
                {
                    return;
                }
                else
                {
                    response.sendWM(int.Parse(match.Groups[1].Value));
                }

            }

            if (Regex.IsMatch(chatclean, "^\\[Server\\] REINICIANDO O SERVIDOR\\.$"))
            {
                Telegram.SendHtmlMessage(vars.emjinfo + "O servidor está reiniciando!");
            }

            if (Regex.IsMatch(chatclean, "^\\(Mensagem de (.+)\\): ([0-1]) (\\d{4}) (.+)$"))
            {
                Match resposta = Regex.Match(chatclean, "^\\(Mensagem de (.+)\\): ([0-1]) (\\d{4}) (.+)$");
                string Sender = resposta.Groups[1].Value;
                bool privado = Convert.ToBoolean(int.Parse(resposta.Groups[2].Value));
                int Token = int.Parse(resposta.Groups[3].Value);
                string Message = resposta.Groups[4].Value;
                

                bool mathToken = Telegram.data.mathToken(Sender, Token);
                bool waiting = Telegram.data.ResponseLimit(Token);


                if (mathToken)
                {
                    if (privado)
                    {
                        Telegram.SendPrivateMessage(Telegram.data.getIdFromToken(Token), vars.replaceEmoji(Message));
                    }
                    else
                    {
                        if (waiting)
                        {
                            string response = "Mensagem de " + Sender + ":%0A════════════════════%0A" + vars.replaceEmoji(Message);
                            Telegram.SendHtmlMessage(response);
                        }
                    }
                }
                else
                {
                    if (mathToken)
                    {
                        Program.Client.SendText("/tell " + Sender + " Desculpe, Tempo de resposta (3s) excedido");
                    }
                    else
                    {
                        Program.Client.SendText("/tell " + Sender + " Token invalido");
                    }
                }

            }

            if (Regex.IsMatch(chatclean, "^\\(Mensagem de (.+)\\): 2 (\\d{4}) (.+)$"))
            {
                Match resposta = Regex.Match(chatclean, "^\\(Mensagem de (.+)\\): 2 (\\d{4}) (.+)$");
                string Sender = resposta.Groups[1].Value;
                int Token = int.Parse(resposta.Groups[2].Value);
                string Message = resposta.Groups[3].Value;


                bool mathToken = Telegram.data.mathToken(Sender, Token);


                if (mathToken)
                {
                    Telegram.data.addNotification(Sender, Message, true, Token);
                    Program.Client.SendText("/tell " + Sender + " Mensagem salva!");
                }
                else
                {
                    Program.Client.SendText("/tell " + Sender + " Token invalido");
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
