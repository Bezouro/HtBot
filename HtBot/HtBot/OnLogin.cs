using MinecraftClient;
using MinecraftClient.HtBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HtBot.HtBot
{
    public class OnLogin
    {
        public static void Protect(string nick)
        {
            if (Telegram.data.Protect(nick))
            {
                new Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(5000);
                    Telegram.data.login(nick, true, true);
                    Program.Client.SendText("/tell " + nick + " [Protect] Ola " + nick + ", voce tem 10s para enviar seu token");
                    System.Threading.Thread.Sleep(11000);
                    if (!Telegram.data.login(nick))
                    {
                        Telegram.data.login(nick, true, false);
                        Program.Client.SendText("/tell " + nick + " [Protect] Acesso suspeito alertado!");
                        List<int> tokens = Telegram.data.getTokenProtected(nick);
                        foreach (int token in tokens)
                        {
                            Telegram.data.addNotification(nick, "Acesso suspeito identificado", true, token);
                        }

                    }
                })).Start();
            }
        }
    }
}
