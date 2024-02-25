using RamathSoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionTriangulation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("You must specify parameters in this order : <input_filename> <output_filename> <extrusion_length>");
                return;
            }
            string inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("File {0} not found", inputFile);
                return;
            }
            string outputFile = args[1];
            if (!Decimal.TryParse(args[2], out decimal extrusion) || extrusion < Constants.MIN_VALUE || extrusion > Constants.MAX_VALUE)
            {
                Console.WriteLine("Extrusion value {0} is not valid", args[2]);
                return;
            }

            try
            {
                // Récupère les points depuis le fichier source
                Point[] points = ReadPointsFromFile(inputFile).ToArray();

                // Crée le polygone
                Polygon polygon = Polygon.Create(points);

                // Effectue l'extrusion
                Polyhedron polyhedron = polygon.Extrude(10);

                // Récupère les triangles
                List<Triangle> triangles = polyhedron.GetTriangles();

                // Sérialise dans le fichier de destination
                WriteTrianglesToFile(triangles, outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        /// <summary>
        /// Lit une liste de points depuis un fichier.
        /// Un point par ligne.
        /// </summary>
        /// <param name="filename">Nom du fichier source</param>
        /// <returns>La liste de points</returns>
        static public List<Point> ReadPointsFromFile(string filename)
        {
            List<Point> points = new List<Point>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string value = sr.ReadLine();
                while(!String.IsNullOrEmpty(value))
                {
                    points.Add(Point.TryParse(value));
                    value = sr.ReadLine();
                }
            }
            return points;
        }

        /// <summary>
        /// Ecrit la liste des triangles dans un fichier.
        /// Un triangle par ligne.
        /// </summary>
        /// <param name="triangles">Triangles à sérialiser</param>
        /// <param name="filename">Nom du fichier de sortie</param>
        static public void WriteTrianglesToFile(List<Triangle> triangles, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (Triangle triangle in triangles)
                {
                    sw.WriteLine(triangle);
                }
            }
        }
    }
}
