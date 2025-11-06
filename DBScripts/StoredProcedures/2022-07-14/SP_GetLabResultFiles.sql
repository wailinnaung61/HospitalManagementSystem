/****** Object:  StoredProcedure [dbo].[SP_GetLabResultDetails]    Script Date: 7/15/2022 3:27:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Zun Pwint Phyu
-- Create Date: July 15, 2022
-- Description: Get All LabResultFiles
-- =============================================
Create PROCEDURE [dbo].[SP_GetLabResultFiles]
    @LabResultId INT = NULL,
    @LabResultFileId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT D.*, L.ResultNo AS [LabResultNo]
    FROM LabResultFile D
        LEFT JOIN [LabResult] L ON L.Id=D.LabResultId
    WHERE 
        (@LabResultId IS NULL OR LabResultId=@LabResultId) AND
        (@LabResultFileId IS NULL OR D.Id=@LabResultFileId)
    ORDER BY CreatedAt Desc
END