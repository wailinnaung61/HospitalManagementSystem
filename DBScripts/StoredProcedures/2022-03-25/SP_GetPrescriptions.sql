SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetPrescriptions]
    @MedicalRecordId INT = NULL,
    @ItemId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT P.*
    FROM Prescription P
        LEFT JOIN Item I ON P.ItemId=I.Id
    WHERE 
        (@MedicalRecordId IS NULL OR P.Id=@MedicalRecordId)AND
        (@ItemId IS NULL OR P.ItemId=@ItemId)
    ORDER BY P.[SortOrder] DESC
END
GO
