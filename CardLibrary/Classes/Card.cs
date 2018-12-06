using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS225CardLibrary
{
    public class Card : ICloneable, IComparable<Card>
    {
        public readonly Ranks rank;
        public readonly Suits suit;

        private Card()
        {
        }

        public Card(Suits newSuit, Ranks newRank)
        {
            suit = newSuit;
            rank = newRank;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return "The " + rank + " of " + suit;
        }

        public string GetRankAsString()
        {
            switch ((int)rank)
            {
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
                case 14: return "A";
                default: return Convert.ToString((int)rank);
            }
        }

        public string GetSuitAsString()
        {
            switch ((int)suit)
            {
                case 0: return "\u2663";
                case 1: return "\u2666";
                case 2: return "\u2665";
                case 3: return "\u2660";
                default: return Convert.ToString((int)suit);
            }
        }

        public int CompareTo(Card otherCard)
        {
            if (this.rank < otherCard.rank)
            {
                return -1;
            }
            else if (this.rank > otherCard.rank)
            {
                return 1;
            }

            return 0;
        }
    }
}
