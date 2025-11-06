SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 07, 2022
-- Description: Get All LabResultDetails
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabResultDetails]
    @LabResultId INT = NULL,
    @LabResultDetailId INT = NULL,
    @IsTitle BIT = NULL,
    @IsPrinted BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT D.*, L.ResultNo AS [LabResultNo]
    FROM LabResultDetail D
        LEFT JOIN [LabResult] L ON L.Id=D.LabResultId
    WHERE 
        (@LabResultId IS NULL OR LabResultId=@LabResultId) AND
        (@LabResultDetailId IS NULL OR D.Id=@LabResultDetailId) AND
        (@IsTitle IS NULL OR IsTitle=@IsTitle) AND
        (@IsPrinted IS NULL OR IsPrinted=@IsPrinted)
    ORDER BY SortOrder
END
GO
