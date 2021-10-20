using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1.Filters
{
    public class Blur : MatrixFilter
    {
        public Blur()
        {
            this.rangX = 5;
            this.rangY = 5;
            this.matrix = new int[5, 5]
            {
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1}
            };
        }
    }
}
