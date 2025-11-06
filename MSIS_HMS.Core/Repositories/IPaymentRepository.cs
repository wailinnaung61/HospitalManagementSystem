using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IPaymentRepository : IRepository<IPDPayment>
    {
        PaymentCommonDTO GetRecordByRecordId(int? RecordId);
        List<PaymentDetailDTO> GetPaymentDetail(int? RecordId);
        IPDRecordDetailDTO GetIPDRecordDetailByRecordId(int RecordId, DateTime date);
        List<IPDRecordDetailReportDTO> GetIPDRecordDetailForReport(int RecordId, DateTime date);
        List<PaymentAmountDTO> GetPaymentAmount(int ipdRecordId);
    }
}
