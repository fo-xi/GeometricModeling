using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1.Filters
{
    public class Contour : MatrixFilter
    {
        public const int CONTOUR_COEFF = 3;

        public Contour()
        {
            this.matrix = new int[3, 3] 
            {
                {-1*CONTOUR_COEFF,   -1*CONTOUR_COEFF,   -1*CONTOUR_COEFF},
                {-1*CONTOUR_COEFF,   8*CONTOUR_COEFF,    -1*CONTOUR_COEFF},
                {-1*CONTOUR_COEFF,   -1*CONTOUR_COEFF,   -1*CONTOUR_COEFF}
            };

            this.rangX = 3;
            this.rangY = 3;
        }
    }
}
