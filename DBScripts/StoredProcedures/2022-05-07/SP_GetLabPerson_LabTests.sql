SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 07, 2022
-- Description: Get All LabPerson_LabTests
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabPerson_LabTests]
    @LabPersonId INT = NULL,
    @LabTestId INT = NULL,
    @LabPerson_LabTestId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT PT.*, P.Name AS [LabPersonName], P.[Type] AS [LabPersonType], L.Name AS [LabTestName]
    FROM LabPerson_LabTest PT
        LEFT JOIN [LabPerson] P ON P.Id=PT.LabPersonId
        LEFT JOIN [LabTest] L ON L.Id=PT.LabTestId
    WHERE 
        (@LabPersonId IS NULL OR LabPersonId=@LabPersonId) AND
        (@LabTestId IS NULL OR LabTestId=@LabTestId) AND
        (@LabPerson_LabTestId IS NULL OR PT.Id=@LabPerson_LabTestId)
    ORDER BY SortOrder
END
GO
