using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.Base
{
    public abstract class BranchEntity : Entity
    {
        [Required(ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }
        [SkipProperty]
        public Branch Branch { get; set; }
    }
}
