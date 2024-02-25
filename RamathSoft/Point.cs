using System;
using System.Collections.Generic;
using System.Text;

namespace RamathSoft
{
    /// <summary>
    /// Représente un point en 3 dimensions
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// Obtient ou définit la coordonnée d'un axe
        /// </summary>
        /// <param name="axe">Axe</param>
        /// <returns>La coordonée du point sur l'axe</returns>
        public decimal this[EAxe axe]
        {
            get
            {
                switch (axe)
                {
                    case EAxe.X: return X;
                    case EAxe.Y: return Y;
                    case EAxe.Z: return Z;
                }
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                switch (axe)
                {
                    case EAxe.X:
                        X = value;
                        break;
                    case EAxe.Y:
                        Y = value;
                        break;
                    case EAxe.Z:
                        Z = value;
                        break;
                }
            }
        }

        public decimal X;
        public decimal Y;
        public decimal Z;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="x">Coordonnée X</param>
        /// <param name="y">Coordonnée Y</param>
        /// <param name="z">Coordonnée Z</param>
        public Point(decimal x, decimal y, decimal z)
        {
            if (x < Constants.MIN_VALUE || x > Constants.MAX_VALUE || y < Constants.MIN_VALUE || y > Constants.MAX_VALUE || z < Constants.MIN_VALUE || z > Constants.MAX_VALUE)
                throw new ArgumentOutOfRangeException();

            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// Formatte le point en chaine de caractères lisible
        /// </summary>
        /// <returns>Le point formatté</returns>
        public override string ToString()
        {
            return String.Format("{0};{1};{2}", X, Y, Z);
        }

        /// <summary>
        /// Transforme une chaine de caractères en Point
        /// </summary>
        /// <param name="value">Chaine de caractère à transformer</param>
        /// <returns>Point correspondant à la chaine de caractères</returns>
        static public Point TryParse(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException();

            string[] values = value.Split(';');
            if (values.Length != 3)
                throw new ArgumentException("Invalid format");

            decimal x;
            decimal y;
            decimal z;

            if (!Decimal.TryParse(values[0], out x))
                throw new ArgumentException("Invalid format of X coordonate");
            if (!Decimal.TryParse(values[1], out y))
                throw new ArgumentException("Invalid format of Y coordonate");
            if (!Decimal.TryParse(values[2], out z))
                throw new ArgumentException("Invalid format of Z coordonate");

            return new Point(x, y, z);
        }


    
    
    
        static public Point operator+(Point p1, Point p2)
        {
            return new Point()
            {
                X = p1.X + p2.X,
                Y = p1.Y + p2.Y,
                Z = p1.Z + p2.Z,
            };
        }
        static public Point operator-(Point p1, Point p2)
        {
            return new Point()
            {
                X = p1.X - p2.X,
                Y = p1.Y - p2.Y,
                Z = p1.Z - p2.Z,
            };
        }
    }
}
