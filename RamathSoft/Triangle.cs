using System;
using System.Collections.Generic;
using System.Text;

namespace RamathSoft
{
    /// <summary>
    /// Représente un triangle
    /// </summary>
    public class Triangle
    {
        public Point[] Points { get; private set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="p1">Sommet 1</param>
        /// <param name="p2">Sommet 2</param>
        /// <param name="p3">Sommet 3</param>
        internal Triangle(Point p1, Point p2, Point p3)
        {
            Points = new Point[]
            {
                p1,
                p2,
                p3
            };
        }


        /// <summary>
        /// Formatte le triangle en chaine de caractères lisible
        /// </summary>
        /// <returns>Le triangle formatté</returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Points[0], Points[1], Points[2]);
        }
    }
}
