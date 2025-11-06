-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE SP_GetDoctorHistory
	-- Add the parameters for the stored procedure here
	 @BranchId INT = NULL,
     @StartDate DATETIME2(7) = NULL,
     @EndDate DATETIME2(7) = NULL,   
     @Status INT = NULL

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from (SELECT Count(V.PatientId) As BookPatient,null As CompletedPatient,null As Status, D.Name AS [DoctorName]

    FROM Visit V
        LEFT JOIN VisitType VT ON V.VisitTypeId=VT.Id
        LEFT JOIN Patient P ON V.PatientId=P.Id
        LEFT JOIN Doctor D ON V.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR V.BranchId=@BranchId)AND       
        (@StartDate IS NULL OR Convert(Date,[Date])>=Convert(Date,@StartDate))AND
        (@EndDate IS NULL OR Convert(Date,[Date])<=Convert(Date,@EndDate))AND
        (@Status IS NULL OR V.Status=@Status) AND
        V.IsDelete = 0 and (V.Status=1 )
		group by D.Name
   ) as T1
union
select * from (SELECT null as BookPatient,Count(V.PatientId) As CompletedPatient,null  As Status, D.Name AS [DoctorName]

    FROM Visit V
        LEFT JOIN VisitType VT ON V.VisitTypeId=VT.Id
        LEFT JOIN Patient P ON V.PatientId=P.Id
        LEFT JOIN Doctor D ON V.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR V.BranchId=@BranchId)AND       
        (@StartDate IS NULL OR Convert(Date,[Date])>=Convert(Date,@StartDate))AND
        (@EndDate IS NULL OR Convert(Date,[Date])<=Convert(Date,@EndDate))AND
        (@Status IS NULL OR V.Status=@Status) AND
        V.IsDelete = 0 and (V.Status=3)
		group by D.Name
   ) T2
END
GO
