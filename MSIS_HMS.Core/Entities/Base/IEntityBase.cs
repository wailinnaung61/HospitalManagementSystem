using System;

namespace MSIS_HMS.Core.Entities.Base
{
    public interface IEntityBase<TId>
    {
        TId Id { get; }
        DateTime? CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
        bool IsDelete { get; set; }
    }
}
