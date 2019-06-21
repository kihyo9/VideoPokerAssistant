using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VideoPokerAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            poker();
        }

        public static void poker()
        {
            Poker poker = new Poker();
            poker.start();
        }
    }
}