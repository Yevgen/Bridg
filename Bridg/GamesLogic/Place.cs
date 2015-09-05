using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bridg
{
    /// <summary>
    /// Клас для вказання місця на столі
    /// </summary>
    public class Place
    {
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// Визначає направлення карт в руці (true горизонтальне, false вертикальне)
        /// </summary>
        public bool IsHorizont { get; set; }

        public Place(int x, int y, bool horizont)
        {
            this.X = x;
            this.Y = y;
            this.IsHorizont = horizont;
        }
    }
}
