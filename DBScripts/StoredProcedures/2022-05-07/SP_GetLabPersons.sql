SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetLabPersons]
    @BranchId INT = NULL,
    @DepartmentId INT = NULL,
    @LabPersonId INT = NULL,
    @Name NVARCHAR(MAX) = NULL,
    @Code NVARCHAR(MAX) = NULL,
    @Type INT = NULL,
    @DoctorId INT = NULL
AS  
BEGIN
    SET NOCOUNT ON

    SELECT P.*, DP.Name AS [DepartmentName], D.Name AS [DoctorName]
    FROM LabPerson P
    LEFT JOIN Department DP ON P.DepartmentId=DP.Id
    LEFT JOIN Doctor D ON P.DoctorId=D.Id
    WHERE   
        (@BranchId IS NULL OR P.BranchId=@BranchId) AND
        (@DepartmentId IS NULL OR DepartmentId=@DepartmentId) AND
        (@LabPersonId IS NULL OR P.Id=@LabPersonId) and
        (@Name IS NULL OR dbo.IncludeInEachOther(P.[Name], @Name)=1)AND
        (@Code IS NULL OR dbo.IncludeInEachOther(P.[Code], @Code)=1)AND
        (@Type IS NULL OR P.Type=@Type) and
        (@DoctorId IS NULL OR DoctorId=@DoctorId) and
        P.IsDelete=0
    ORDER BY P.[Name]
END 
GO
