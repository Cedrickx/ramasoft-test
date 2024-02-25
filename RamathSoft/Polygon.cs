using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamathSoft
{
    /// <summary>
    /// Représente un polygone
    /// </summary>
    public class Polygon : ITrigonalisable
    {
        private Point[] _Points;
        private Point[] _BasePoints;
        private EAxe _SecondaryAxe;
        private EAxe _PlanAxe;
        private Func<Point[], EAxe, Point[]> _Func;


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="points">Points du polygone hors base</param>
        /// <param name="basePoints">Points de la base du polygone</param>
        /// <param name="planAxe">Axe du plan 2D du polygone</param>
        /// <param name="secondaryAxe">Axe utilisé pour la découpe</param>
        /// <param name="isReversed">Indique si la découpe est inversée (rotation 180°)</param>
        private Polygon(Point[] points, Point[] basePoints, EAxe planAxe, EAxe secondaryAxe, bool isReversed)
        {
            _Points = points;
            _BasePoints = basePoints;
            _SecondaryAxe = secondaryAxe;
            _PlanAxe = planAxe;

            if (isReversed)
                _Func = Extenders.GetMaxs;
            else
                _Func = Extenders.GetMins;
        }



        /// <summary>
        /// Effectue l'extrusion du polygone en fonction du plan 2D auquel il appartient
        /// </summary>
        /// <param name="distance">Distance de l'extrusion</param>
        /// <returns>Le polyèdre correspondant</returns>
        public Polyhedron Extrude(decimal distance)
        {
            // Crée le point de direction
            Point direction = new Point();
            direction[_PlanAxe] = distance;

            // Crée la deuxième face principale du polyhèdre (la première étant ce polygone)
            Point[] points = new Point[_Points.Length];
            for (int i = 0; i < _Points.Length; i++)
                points[i] = _Points[i] + direction;

            Point[] basePoints = new Point[_BasePoints.Length];
            for (int i = 0; i < _BasePoints.Length; i++)
                basePoints[i] = _BasePoints[i] + direction;

            Polygon oppositeFace = new Polygon(points, basePoints, _PlanAxe, _SecondaryAxe, _Func == Extenders.GetMaxs);

            // Crée les 3 faces rectangulaires
            Polygon[] rectFaces = new Polygon[]
            {
                Create(new Point[] { _Points[0], points[0], _BasePoints[0], basePoints[0] }),
                Create(new Point[] { _BasePoints[0], basePoints[0], _BasePoints[1], basePoints[1] }),
                Create(new Point[] { _Points[_Points.Length - 1], points[points.Length - 1], _BasePoints[1], basePoints[1] }),
            };

            // Crée les autres faces
            Polygon[] otherFaces = new Polygon[_Points.Length - 1];
            for (int i = 0; i < _Points.Length - 1; i++)
                otherFaces[i] = Create(new Point[] { _Points[i], points[i], _Points[i + 1], points[i + 1] });

            // Ordonne les faces pour les rendre adjacentes afin d'optimiser le rendu des triangles
            List<Polygon> polygons = new List<Polygon>();
            polygons.Add(this);
            polygons.AddRange(rectFaces);
            polygons.Add(oppositeFace);
            polygons.AddRange(otherFaces);

            // Crée le polyèdre
            return new Polyhedron(polygons);
        }

        /// <summary>
        /// Obtient la liste des points du polygone
        /// </summary>
        /// <returns>Points du polygone (base comprise)</returns>
        public List<Point> GetPoints()
        {
            List<Point> points = new List<Point>();
            points.Add(_BasePoints[0]);
            points.AddRange(_Points);
            points.Add(_BasePoints[1]);
            return points;
        }
        /// <summary>
        /// Calcule la liste des triangles du polygone
        /// </summary>
        /// <returns>Triangles du polygone</returns>
        public List<Triangle> GetTriangles()
        {
            return GetTriangles(_Points, _BasePoints);
        }

        /// <summary>
        /// Calcule la liste des triangles d'un sous ensenmble de points
        /// </summary>
        /// <param name="points">Sous ensemble de points</param>
        /// <param name="basePoints">Base utilisée pour découper le sous ensemble</param>
        /// <returns>Triangles du sous ensemble</returns>
        private List<Triangle> GetTriangles(Point[] points, Point[] basePoints)
        {
            // Récupère le point extrême de l'axe secondaire le plus loin en fonction de l'orientation
            Point p1 = _Func(points, _SecondaryAxe).Last();
            Point p2 = basePoints[0];
            Point p3 = basePoints[1];

            // Récupère l'index du point extrême pour couper le polygone en deux
            int index = Array.IndexOf(points, p1);

            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(p1, p2, p3));

            // Récupère les triangles précédent
            Point[] previousPoints = points.Take(index + 1).ToArray();
            if (previousPoints.Length >= 2)
            {
                Point[] subBasePoints = new Point[] { p2, p1 };
                triangles.AddRange(GetTriangles(previousPoints.Except(subBasePoints).ToArray(), subBasePoints));
            }

            // Inverse la liste pour obtenir des triangles adjacents avec le découpage suivant
            triangles.Reverse();

            // Récupère les triangles suivant
            Point[] nextPoints = points.Skip(index).ToArray();
            if (nextPoints.Length >= 2)
            {
                Point[] subBasePoints = new Point[] { p1, p3 };
                triangles.AddRange(GetTriangles(nextPoints.Except(subBasePoints).ToArray(), subBasePoints));
            }

            return triangles;
        }



        /// <summary>
        /// Crée un polygone en fonction d'un ensemble de points
        /// </summary>
        /// <param name="points">Points du polygone</param>
        /// <returns>Le polygone formé par l'ensemble de points</returns>
        static public Polygon Create(Point[] points)
        {
            if (points == null || points.Length < 4)
                throw new Exception("A polygon must have at least 4 points");

            // Récupère le plan commun
            EAxe planAxe = points.GetPlanAxe();

            // Définit les deux axes du polygon
            EAxe[] axes = new EAxe[2];
            switch (planAxe)
            {
                case EAxe.X:
                    axes[0] = EAxe.Y;
                    axes[1] = EAxe.Z;
                    break;
                case EAxe.Y:
                    axes[0] = EAxe.X;
                    axes[1] = EAxe.Z;
                    break;
                case EAxe.Z:
                    axes[0] = EAxe.X;
                    axes[1] = EAxe.Y;
                    break;
            }

            // Récupère l'axe principal et l'axe secondaire
            EAxe mainAxe;
            EAxe secondaryAxe;
            if (points.Select(p => p[axes[0]]).Distinct().Count() == points.Length / 2)
            {
                mainAxe = axes[0];
                secondaryAxe = axes[1];
            }
            else if (points.Select(p => p[axes[1]]).Distinct().Count() == points.Length / 2)
            {
                mainAxe = axes[1];
                secondaryAxe = axes[0];
            }
            else
            {
                // Polygone impossible à créer suivant les règles définies
                throw new Exception("Impossible to create the polygon");
            }

            // Récupère les points principaux en fonction de l'axe associé à la base
            Point[] mainPoints = new Point[]
            {
                points.GetMins(mainAxe).GetMins(secondaryAxe).Single(),
                points.GetMins(mainAxe).GetMaxs(secondaryAxe).Single(),
                points.GetMaxs(mainAxe).GetMaxs(secondaryAxe).Single(),
                points.GetMaxs(mainAxe).GetMins(secondaryAxe).Single()
            };

            // Récupère la base
            Point[] basePoints = new Point[2];
            if (points.GetMins(secondaryAxe).First()[secondaryAxe] == mainPoints.GetMins(secondaryAxe).First()[secondaryAxe])
            {
                // Orientation "normale"
                basePoints[0] = mainPoints[0];
                basePoints[1] = mainPoints[3];

                // Vérifie si la base est bien composée de 2 angles droits
                if (!basePoints[0].IsRightAngle(mainPoints[1], basePoints[1]) || !basePoints[1].IsRightAngle(mainPoints[2], basePoints[0]))
                    throw new Exception("Invalid type of polygon");

                // Crée le polygone
                return new Polygon(points.Except(basePoints).OrderBy(mainAxe, secondaryAxe), basePoints, planAxe, secondaryAxe, false);
            }
            else if (points.GetMaxs(secondaryAxe).First()[secondaryAxe] == mainPoints.GetMaxs(secondaryAxe).First()[secondaryAxe])
            {
                // Orientation "inversée"
                basePoints[0] = mainPoints[1];
                basePoints[1] = mainPoints[2];

                // Vérifie si la base est bien composée de 2 angles droits
                if (!basePoints[0].IsRightAngle(mainPoints[0], basePoints[1]) || !basePoints[1].IsRightAngle(mainPoints[3], basePoints[0]))
                    throw new Exception("Invalid type of polygon");

                // Crée le polygone
                return new Polygon(points.Except(basePoints).OrderBy(mainAxe, secondaryAxe), basePoints, planAxe, secondaryAxe, true);
            }

            // Impossible de déterminer la base du polygon suivant les critères
            throw new Exception("Impossible to create the polygon");
        }
    }
}
