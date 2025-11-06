SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetPatientSymptoms]
    @MedicalRecordId INT = NULL,
    @SymptomId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT PS.*
    FROM PatientSymptom PS
        LEFT JOIN Symptom S ON PS.SymptomId=S.Id
    WHERE 
        (@MedicalRecordId IS NULL OR PS.MedicalRecordId=@MedicalRecordId)AND
        (@SymptomId IS NULL OR PS.SymptomId=@SymptomId)
    ORDER BY [SortOrder] DESC
END
GO
