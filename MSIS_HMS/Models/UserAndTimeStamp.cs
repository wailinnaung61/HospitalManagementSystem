using System;
namespace MSIS_HMS.Models
{
    public class UserAndTimeStamp
    {
        public UserAndTimeStamp()
        {
        }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}
