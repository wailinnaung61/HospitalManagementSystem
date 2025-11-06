using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Models.ViewModels
{
    public class DepartmentCreateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BranchesModel> Branches { get; set; }
    }
}
