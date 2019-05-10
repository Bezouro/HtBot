using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtBot.HtBot
{
    class Account
    {
        private bool Verifyed;
        private string Nick;
        private int Token;

        public Account(string Nick, bool Verifyed,int Token)
        {
            this.Verifyed = Verifyed;
            this.Nick = Nick;
            this.Token = Token;
        }

        public string getNick()
        {
            return this.Nick;
        }
        public bool getVerify()
        {
            return this.Verifyed;
        }

        public int getToken()
        {
            return this.Token;
        }


    }
}
