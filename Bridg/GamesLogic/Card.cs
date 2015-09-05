using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bridg
{
    public class Card
    {
        private int _numberOfCard;

        public int NumberOfCard
        {
            get
            {
                return _numberOfCard;
            }
        }

        public bool open { get; set; }

        public double X
        { get; set; }
        public double Y
        { get; set; }

        public bool Animation { get; set; }


        public Card(int number)
        {
            _numberOfCard = number;

            X = 0; Y = 0; 
            open = false;
            Animation = false;
        }

        public Card(int number, double x, double y )
        {
            _numberOfCard = number;
            this.X = x; this.Y = y;
            open = false;
        }

        public Card(Card C)
        {
            _numberOfCard = C._numberOfCard;
            open = C.open;
            X = C.X;
            Y = C.Y;
        }

        /// <summary>
        /// Повертає масть, 0 - чирва, 1 - піка, 2 - буба, 3 хреста
        /// </summary>
        /// <returns></returns>
        public int Suid()
        {
            return (_numberOfCard - 1) / 9;
        }

        public int Priority()
        {
            if ((_numberOfCard - 1) % 9 > 0)
            {
                return (_numberOfCard - 1) % 9 + 5;
            }
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (NumberOfCard == ((Card)obj).NumberOfCard)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return NumberOfCard;
        }
    }
}
