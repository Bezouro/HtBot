namespace MinecraftClient.HtBot
{
    class ChatFilter : ChatBot
    {

        public static bool chatFilter(string Chat)
        {
            string ChatClean = GetVerbatim(Chat);

            switch (ChatClean)
            {
                case "Aguarde 3 segundos": return true;
                case "Teleportado(a)!": return true;
                default: return false;
            }
        }

    }
}
