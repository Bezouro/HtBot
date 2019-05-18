using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.HtBot
{
    class vars
    {
        public static long logInTimestamp = 0;
        public static bool loggedIn = false;




        public static bool tmoney;
        public static bool checkSkills;
        public static bool checkMultipleSkills;
        public static bool checkMultipleMoney;
        public static bool checkmctop;
        public static bool checkmcrank;
        public static bool checkMoneyRank;
        public static bool sendWM;
        public static bool singleSkillCheck;

        public const bool debug = false;

        public static int atualUser = 0;
        public static int skill_count = 0;
        public static int skill_notfound = 0;
        public static int firstlevel = 0;
        public static int multipleskillscheck = 0;
        public static int multiplemoneycheck = 0;
        public static int checkedNicksCount = 0;
        public static int checkedmoneycount = 0;

        public const string emjthinking = "🤔";
        public const string emjneutralface = "😐";
        public const string emjthumbsup = "👍";
        public const string emjmoney = "💰";
        public const string emjinfo = "ℹ";
        public const string emjsmile = "😃";
        public const string emjerror = "❌";
        public const string emjok = "✅";
        public const string emjcl = "🆑";

        public static string mctopskill;
        public static string chatID;
        public static string atualNick;
        public static string checkingSkill;


        public static List<string> moneytop = new List<string>();
        public static int moneyTopList = 0;

        public static List<string> multiplemoney = new List<string>();

        public static List<string> skills = new List<string>();
        public static int skillsList = 0;

        public static List<string> mctop = new List<string>();
        public static int mcTopList = 0;

        public static List<string> mcrank = new List<string>();

        public static void resetVars()
        {
            tmoney = false;
            checkSkills = false;
            checkMultipleSkills = false;
            checkMultipleMoney = false;
            checkmctop = false;
            checkMoneyRank = false;
            sendWM = false;
            singleSkillCheck = false;
            atualUser = 0;
            skill_count = 0;
            skill_notfound = 0;
            multipleskillscheck = 0;
            multiplemoneycheck = 0;
            checkedNicksCount = 0;
            checkedmoneycount = 0;
            atualNick = "";
            checkingSkill = "";
            skills = new List<string>();
            skillsList = 0;
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
        public static string replaceEmoji(string text)
        {
            text = text.Replace("emjthinking",emjthinking);
            text = text.Replace("emjneutralface",emjneutralface);
            text = text.Replace("emjthumbsup",emjthumbsup);
            text = text.Replace("emjmoney",emjmoney);
            text = text.Replace("emjinfo",emjinfo);
            text = text.Replace("emjsmile",emjsmile);
            text = text.Replace("emjok",emjok);
            text = text.Replace("emjcl",emjcl);
            text = text.Replace("emjerror",emjerror);

            return text;
        }

    }

    



}
