using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamathSoft
{
    public static class Extenders
    {
        /// <summary>
        /// Obtient l'axe du plan 2D d'un ensemble de points
        /// </summary>
        /// <param name="points">Ensemble de points</param>
        /// <returns>L'axe du plan 2D</returns>
        static public EAxe GetPlanAxe(this Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            if (points.Length < 3)
                throw new ArgumentException("You must specify at least 3 points to get the plan.");

            Dictionary<EAxe, bool> axes = new Dictionary<EAxe, bool>()
            {
                { EAxe.X, true },
                { EAxe.Y, true },
                { EAxe.Z, true },
            };
            for (int i = 0; i < points.Length - 1; i++)
            {
                if (points[i].X != points[i + 1].X)
                    axes[EAxe.X] = false;

                if (points[i].Y != points[i + 1].Y)
                    axes[EAxe.Y] = false;

                if (points[i].Z != points[i + 1].Z)
                    axes[EAxe.Z] = false;
            }

            if (axes.Count(kvp => kvp.Value) > 1)
                throw new Exception("Multiple plans possible");

            if (axes.Count(kvp => kvp.Value) == 0)
                throw new Exception("Impossible to determine the plan");

            return axes.First(kvp => kvp.Value).Key;
        }
        
        /// <summary>
        /// Calcule les points ayant la plus petite coordonnée sur un axe parmis un ensemble de points
        /// </summary>
        /// <param name="points">Ensemble de points</param>
        /// <param name="axe">Axe à analyser</param>
        /// <returns>Les points ayant la coordonnée la plus petite sur l'axe demandé</returns>
        static public Point[] GetMins(this Point[] points, EAxe axe)
        {
            if (points == null)
                throw new ArgumentNullException();

            if (points.Length < 1)
                throw new ArgumentException("You must specify at least 1 point to get minimas.");

            List<Point> list = new List<Point>();
            decimal minValue = Constants.MAX_VALUE;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i][axe] < minValue)
                {
                    minValue = points[i][axe];
                    list.Clear();
                    list.Add(points[i]);
                }
                else if (points[i][axe] == minValue)
                {
                    list.Add(points[i]);
                }
            }

            return list.ToArray();
        }
        /// <summary>
        /// Calcule les points ayant la plus grande coordonnée sur un axe parmis un ensemble de points
        /// </summary>
        /// <param name="points">Ensemble de points</param>
        /// <param name="axe">Axe à analyser</param>
        /// <returns>Les points ayant la coordonnée la plus grande sur l'axe demandé</returns>
        static public Point[] GetMaxs(this Point[] points, EAxe axe)
        {
            if (points == null)
                throw new ArgumentNullException();

            if (points.Length < 2)
                throw new ArgumentException("You must specify at least 2 points to get minimas.");

            List<Point> list = new List<Point>();
            decimal maxValue = Constants.MIN_VALUE;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i][axe] > maxValue)
                {
                    maxValue = points[i][axe];
                    list.Clear();
                    list.Add(points[i]);
                }
                else if (points[i][axe] == maxValue)
                {
                    list.Add(points[i]);
                }
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Ordonne une liste de points suivant un axe principal (celui de la base) et l'axe de la découpe
        /// </summary>
        /// <param name="points">Ensemble de points à ordonner</param>
        /// <param name="mainAxe">Axe principal</param>
        /// <param name="secondaryAxe">Axe de découpe</param>
        /// <returns>Ensemble de points ordonnés</returns>
        static public Point[] OrderBy(this IEnumerable<Point> points, EAxe mainAxe, EAxe secondaryAxe)
        {
            List<Point> orderedList = points.OrderBy(p => p[mainAxe]).ToList();
            for (int i = 1; i < orderedList.Count - 1; i++)
            {
                if (orderedList[i][mainAxe] == orderedList[i + 1][mainAxe])
                {
                    if (orderedList[i - 1][secondaryAxe] != orderedList[i][secondaryAxe])
                    {
                        // Inverse les points pour rester en angle droit
                        Point tmp = orderedList[i];
                        orderedList[i] = orderedList[i + 1];
                        orderedList[i + 1] = tmp;
                    }
                }
            }
            return orderedList.ToArray();
        }

        /// <summary>
        /// Calcule l'angle intérieur formé par 3 points
        /// </summary>
        /// <param name="p1">Point central</param>
        /// <param name="p2">Point externe</param>
        /// <param name="p3">Point externe</param>
        /// <returns>L'angle formé exprimé en degré</returns>
        static public double Angle(this Point p1, Point p2, Point p3)
        {
            double a = p1.Length(p2);
            double b = p1.Length(p3);
            double c = p2.Length(p3);

            // Loi des cosinus : cos(C) = (a² + b² - c²) / (2ab)
            double radian = Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));

            // Converti en degré
            return (180 / Math.PI) * radian;
        }
        /// <summary>
        /// Indique si l'angle intérieur formé par 3 points est un angle droit
        /// </summary>
        /// <param name="p1">Point central</param>
        /// <param name="p2">Point externe</param>
        /// <param name="p3">Point externe</param>
        /// <returns>True si l'angle est droit</returns>
        static public bool IsRightAngle(this Point p1, Point p2, Point p3)
        {
            double angle = p1.Angle(p2, p3);
            return Math.Round(angle) == 90;
        }
        /// <summary>
        /// Obtient la distance entre 2 points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Distance entre les 2 points</returns>
        static public double Length(this Point p1, Point p2)
        {
            Point deltaPoint = p1 - p2;
            return Math.Sqrt(Math.Pow(Convert.ToDouble(deltaPoint.X), 2) + Math.Pow(Convert.ToDouble(deltaPoint.Y), 2) + Math.Pow(Convert.ToDouble(deltaPoint.Z), 2));
        }
    }
}
