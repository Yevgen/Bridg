using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridg
{
    public class Player
    {
        #region Приватні поля
        private int number;
        private List<int> walks;
        private List<Card> cards;
        private Place place;
        private int quantityPutJacks;

        #endregion

        #region Властивості
        public string Name { set; get; }
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
        public int score { set; get; }
        public bool TwoWalk { set; get; }
        public int SuidOfJeck { set; get; }
        public List<Card> Cards { get { return cards; } }
        public Card OpenDeckC { set; get; }
        //private int head;
        //public int count
        //{
        //    get
        //    {
        //        return walk.Count;
        //    }
        //}
        public List<int> Walks 
        {
            get
            {
                return walks;
            }
        }

        public Place Place { get { return place; } }
        #endregion

        public Card this[int numb]
        {
            get
            {
                if (cards.Count == 0)
                    return null;
                else if (numb == cards.Count)
                    return cards[numb - 1];
                return cards[numb];
            }
        }

        public Player(int number, Place place)
        {
            this.place = place;
            this.number = number;
            cards = new List<Card>();
            TwoWalk = false;
            walks = new List<int>();
            score = 0;
        }
        public Player(Player player)
        {
            number =  player.number;
            //head =  player.head;
            cards = new List<Card>() ;
            OpenDeckC = new Card(player.OpenDeckC);
            TwoWalk = player.TwoWalk;
            SuidOfJeck = player.SuidOfJeck;
            for (int i = 0; i < player.cards.Count; i++)
            {
                cards.Add(new Card(player.cards[i]));
            }
            walks = new List<int>();
            for (int i = 0; i < player.walks.Count; i++)
            {
                //walks[i] = player.walks[i];
                walks.Add(player.walks[i]);
            }
            //headWalk = player.headWalk;
        }

        /// <summary>
        /// Додавання заданої карти в руки гравця
        /// </summary>
        /// <param name="card">Карта яку потрібно додати в руки гравця</param>
        public void PushCard(Card card)
        {
            cards.Add(card);
            SortCards();
        }

        /// <summary>
        /// Взяття карти за заданим номером з рук гравця з видаленням її з рук
        /// </summary>
        /// <param name="numb">Номер карти яку потрібно взяти</param>
        /// <returns>Повертається видалена карта</returns>
        public Card PopCard(int numb)
        {
            Card res = cards[numb];
            cards.RemoveAt(numb);
            if (res.Priority() == 11)
                quantityPutJacks++;
            else quantityPutJacks = 0;
            return res;
        }

        public bool Walking(int numb)
        {
            if (TwoWalk && cards[numb].Priority() == OpenDeckC.Priority())
            {
                OpenDeckC = cards[numb];
                cards.RemoveAt(numb);

                walks.Add(numb);
                //Walk[headWalk] = numb;
                //headWalk++;
                return true;
            }
            if (numb < cards.Count &&
                !TwoWalk &&
                (((cards[numb].Suid() == OpenDeckC.Suid() ||
                cards[numb].Priority() == OpenDeckC.Priority() ||
                cards[numb].Priority() == 11) &&
                OpenDeckC.Priority() != 11) ||
                ((cards[numb].Priority() == 11 || cards[numb].Suid() == SuidOfJeck) && OpenDeckC.Priority() == 11)))
            {
                if (cards[numb].Priority() != 6)
                    TwoWalk = true;
                OpenDeckC = cards[numb];

                cards.RemoveAt(numb);

                walks.Add(numb);
                //Walk[headWalk] = numb;
                //headWalk++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Прибавляє до набраних раніше очків набрані за цей розклад
        /// </summary>
        public void AddPoints(int multiplier)
        {
            score += NumberOfPoints(multiplier);// *multiplier;
            //score *= multiplier;
        }

        /// <summary>
        /// Підраховує очки в даному розкладі
        /// </summary>
        /// <returns></returns>
        public int NumberOfPoints(int multiplier)
        {
            int res = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Priority() == 1)
                {
                    res += 15;
                }
                if (cards[i].Priority() == 10 || cards[i].Priority() == 12 || cards[i].Priority() == 13)
                {
                    res += 10;
                }
                if (cards[i].Priority() == 11)
                {
                    res += 20;
                }
                if (cards[i].NumberOfCard == 17)
                {
                    res += 40;
                }
            }
            if (cards.Count == 0)
                res = quantityPutJacks * -20;
            return res * multiplier;
        }

        /// <summary>
        /// Очищає карти гравця
        /// </summary>
        public void ClearCards()
        {
            cards.Clear();
        }

        public void SortCards()
        {
            for (int w = cards.Count - 1; w > 0; w --)
            {
                for (int i = 0; i < w; i++)
                {
                    if (cards[i].NumberOfCard > cards[i+1].NumberOfCard)
                    {
                        Card card = cards[i];
                        cards[i] = cards[i + 1];
                        cards[i + 1] = card;
                    }
                }
            }
        }

        public void SetPlaceCards(int widthCards, bool isOpen)
        {
            for (int w = 0; w < cards.Count; w++)
            {
                if (!cards[w].Animation)
                {
                    if (place.IsHorizont)
                    {
                        cards[w].X = place.X + (w * (0.25 * widthCards));
                        cards[w].Y = place.Y;
                    }
                    else
                    {
                        cards[w].X = place.X;
                        cards[w].Y = place.Y + (w * (0.3 * widthCards));
                    }
                    cards[w].open = isOpen;
                }
            }
        }
    }
}
