using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.HtBot
{
    class vars
    {
        public static Boolean loggedIn = false;
        public static bool tmoney;
        public static bool checkSkills;
        public static bool checkMultipleSkills;
        public static bool checkMultipleMoney;
        public static bool checkmctop;
        public static bool checkMoneyRank;

        public const bool debug = false;

        public static int skill_count = 0;
        public static int skill_notfound = 0;
        public static int firstlevel = 0;
        public static int multipleskillscheck = 0;
        public static int multiplemoneycheck = 0;
        public static int checkedskillscount = 0;
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


        public static List<string> moneytop = new List<string>();
        public static int moneyTopList = 0;

        public static List<string> multiplemoney = new List<string>();

        public static List<string> skills = new List<string>();
        public static int skillsList = 0;

        public static List<string> mctop = new List<string>();
        public static int mcTopList = 0;

    }

    



}
