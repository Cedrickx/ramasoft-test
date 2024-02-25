using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamathSoft
{
    static public class Helpers
    {
        /// <summary>
        /// Trie de façon aléatoire un ensemble de points
        /// </summary>
        /// <param name="points">Ensemble de points à trier</param>
        /// <returns>L'ensemble des points trié de manière aléatoire</returns>
        static public Point[] Shuffle(Point[] points)
        {
            Random rnd = new Random();
            return points.OrderBy(p => rnd.Next()).ToArray();
        }

        /// <summary>
        /// Génère de façon mi-aléatoire un ensemble de points
        /// </summary>
        /// <param name="count">Nombre de points à générer</param>
        /// <param name="mainAxe">Axe principal (base du polygone)</param>
        /// <param name="secondaryAxe">Axe de découpe</param>
        /// <returns>L'ensemble de points générés</returns>
        static public Point[] Generate(int count, EAxe mainAxe, EAxe secondaryAxe)
        {
            Random rnd = new Random();

            Point[] points = new Point[count];

            // Crée la base de manière arbitraire
            Point basePoint1 = new Point();
            basePoint1[mainAxe] = 0;
            basePoint1[secondaryAxe] = 0;

            Point basePoint2 = new Point();
            basePoint2[mainAxe] = count - 2;
            basePoint2[secondaryAxe] = 0;

            points[0] = basePoint1;
            points[count - 1] = basePoint2;

            for (int i = 0; i < count - 2; i++)
            {
                Point point = new Point();
                if (i % 2 == 0)
                {
                    point[mainAxe] = points[i][mainAxe];
                    point[secondaryAxe] = rnd.Next(1, 100);
                }
                else
                {
                    point[mainAxe] = i + 1;
                    point[secondaryAxe] = points[i][secondaryAxe];
                }

                points[i + 1] = point;
            }

            return points;
        }
    }
}
