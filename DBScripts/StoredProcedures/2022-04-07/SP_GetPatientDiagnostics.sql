SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetPatientDiagnostics]
    @MedicalRecordId INT = NULL,
    @DiagnosticId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT PD.*
    FROM PatientDiagnostic PD
        LEFT JOIN Diagnostic D ON PD.DiagnosticId=D.Id
    WHERE 
        (@MedicalRecordId IS NULL OR PD.MedicalRecordId=@MedicalRecordId)AND
        (@DiagnosticId IS NULL OR PD.DiagnosticId=@DiagnosticId)
    ORDER BY [SortOrder] DESC
END
GO
