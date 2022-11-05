using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STPO_Lab1.Model
{
    public class ParameterValue
    {
        public decimal LeftBorder { get; set; }
        public decimal RightBorder { get; set; }
        public string CoeffString { get; set; }
        public decimal AllowableEPS { get; set; }
        public int TestCaseQuantity { get; set; }
        public decimal StarterStep { get; set; }
        public decimal Increment { get; set; }
    }
}
