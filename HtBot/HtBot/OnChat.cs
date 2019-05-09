using MinecraftClient.Protocol;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                    vars.multiplemoney.Add(moneynick + " " + money);

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

            if (Regex.IsMatch(chatclean, "^(\\d\\d)\\. (.+) - (\\d{1,5})$"))
            {
                Match match = Regex.Match(chatclean, "^(\\d\\d)\\. (.+) - (\\d{1,5})$");
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

                Telegram.data.addNotification(nick, "Alcançou " + nivel + " na skill: " + skill);


            }

            if (Regex.IsMatch(chatclean, "^(.+) é o (\\d{1,5}). mais rico do servidor.$"))
            {
                Match notificação = Regex.Match(chatclean, "^(.+) é o (\\d{1,5}). mais rico do servidor.$");
                string nick = notificação.Groups[1].Value;
                int pos = int.Parse(notificação.Groups[2].Value);

                Telegram.SendHtmlMessage("<code>"+ pos + "º</code>) <b>" + nick + "</b>");


            }


        }

    }
}
