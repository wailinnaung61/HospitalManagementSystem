using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("UserAccessMenu")]
    public class UserAccessMenu
    {
        public UserAccessMenu()
        {
        }

        //[Key]
        //[ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        //[Key]
        //[ForeignKey("Menu")]
        public int MenuId { get; set; }

        [NotMapped]
        [SkipProperty]
        public bool Selected { get; set; }

        [SkipProperty]
        public ApplicationUser User { get; set; }
        [SkipProperty]
        public Menu Menu { get; set; }
    }
}
