SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 07, 2022
-- Description: Get All LabTestDetails
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabTestDetails]
    @LabTestId INT = NULL,
    @LabTestDetailId INT = NULL,
    @IsTitle BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT D.*, L.Name AS [LabTestName]
    FROM LabTestDetail D
        LEFT JOIN [LabTest] L ON L.Id=D.LabTestId
    WHERE 
        (@LabTestId IS NULL OR LabTestId=@LabTestId) AND
        (@LabTestDetailId IS NULL OR D.Id=@LabTestDetailId) AND
        (@IsTitle IS NULL OR IsTitle=@IsTitle)
    ORDER BY SortOrder
END
GO
