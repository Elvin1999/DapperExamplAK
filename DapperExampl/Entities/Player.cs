using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExampl.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
        public bool IsStar { get; set; }
    }
}
