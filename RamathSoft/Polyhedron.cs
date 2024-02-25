using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamathSoft
{
    /// <summary>
    /// Réprésente un polyèdre
    /// </summary>
    public class Polyhedron : ITrigonalisable
    {
        private List<Polygon> _Polygons;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="polygons">Polygones de chaque face du polyèdre</param>
        internal Polyhedron(List<Polygon> polygons)
        {
            _Polygons = new List<Polygon>(polygons);
        }


        /// <summary>
        /// Obtient la liste des points du polyèdre
        /// </summary>
        /// <returns>Points du polyèdre</returns>
        public List<Point> GetPoints()
        {
            List<Point> points = new List<Point>();
            foreach (Polygon polygon in _Polygons)
            {
                points.AddRange(polygon.GetPoints());
            }
            return points.Distinct().ToList();
        }

        /// <summary>
        /// Calcule la liste des triangles du polyèdre
        /// </summary>
        /// <returns>Triangles du polyèdre</returns>
        public List<Triangle> GetTriangles()
        {
            List<Triangle> triangles = new List<Triangle>();
            foreach (Polygon polygon in _Polygons)
            {
                triangles.AddRange(polygon.GetTriangles());
            }
            return triangles;
        }
    }
}
