SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetVisits]
    @BranchId INT = NULL,
    @VisitId INT = NULL,
    @VisitNo NVARCHAR(MAX) = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL,
    @PatientId INT = NULL,
    @DoctorId INT = NULL,
    @VisitTypeId INT = NULL,
    @Status INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT V.*, VT.[Type] AS [VisitTypeDesc], P.Name AS [PatientName], D.Name AS [DoctorName]
    FROM Visit V
        LEFT JOIN VisitType VT ON V.VisitTypeId=VT.Id
        LEFT JOIN Patient P ON V.PatientId=P.Id
        LEFT JOIN Doctor D ON V.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR V.BranchId=@BranchId)AND
        (@VisitId IS NULL OR V.Id=@VisitId)AND
        (@VisitNo IS NULL OR dbo.IncludeInEachOther([VisitNo], @VisitNo)=1)AND
        (@Date IS NULL OR [Date]=@Date)AND
        (@StartDate IS NULL OR [Date]>=@StartDate)AND
        (@EndDate IS NULL OR [Date]<=@EndDate)AND
        (@PatientId IS NULL OR PatientId=@PatientId) AND
        (@DoctorId IS NULL OR V.DoctorId=@DoctorId) AND
        (@VisitTypeId IS NULL OR VisitTypeId=@VisitTypeId) AND
        (@Status IS NULL OR V.Status=@Status) AND
        V.IsDelete = 0
    ORDER BY [Date], [CreatedAt] DESC
END
GO
