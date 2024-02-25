using System;
using System.Collections.Generic;
using System.Text;

namespace RamathSoft
{
    public interface ITrigonalisable
    {
        List<Triangle> GetTriangles();
        List<Point> GetPoints();
    }
}
