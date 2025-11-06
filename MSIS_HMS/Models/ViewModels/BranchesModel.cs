using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Models.ViewModels
{
    public class BranchesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsMainBranch { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Township { get; set; }
        public string City { get; set; }
        public string Logo { get; set; }

    }
}
