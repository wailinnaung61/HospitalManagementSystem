SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetSchedules]
    @BranchId INT = NULL,
    @ScheduleId INT = NULL,
    @DepartmentId INT = NULL,
    @DepartmentType INT = NULL,
    @DoctorId INT = NULL,
    @SpecialityId INT = NULL,
    @DayOfWeek INT = NULL,
    @Time TIME(7) = NULL,
    @FromTime TIME(7) = NULL,
    @ToTime TIME(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT S.*, D.Name AS [DoctorName], DP.Name AS [DepartmentName]
    FROM Schedule S
        LEFT JOIN Doctor D ON S.DoctorId=D.Id
        LEFT JOIN Department DP ON S.DepartmentId=DP.Id
    WHERE 
        (@BranchId IS NULL OR S.BranchId=@BranchId)AND
        (@ScheduleId IS NULL OR S.Id=@ScheduleId)AND
        (@DepartmentId IS NULL OR S.DepartmentId=@DepartmentId)AND
        (@DepartmentType IS NULL OR DP.Type=@DepartmentType)AND
        (@DoctorId IS NULL OR S.DoctorId=@DoctorId)AND
        (@SpecialityId IS NULL OR D.SpecialityId=@SpecialityId)AND
        (@Time IS NULL OR (@Time >= FromTime AND @Time <=ToTime))AND
        (@FromTime IS NULL OR (@FromTime >= FromTime AND @FromTime <=ToTime))AND
        (@ToTime IS NULL OR (@ToTime >= FromTime AND @ToTime <=ToTime))AND
        S.IsDelete = 0
    ORDER BY [DayOfWeek], [FromTime], [ToTime] DESC
END
GO
