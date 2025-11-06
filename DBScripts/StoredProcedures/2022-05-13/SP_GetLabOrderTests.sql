SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 13, 2022
-- Description: Get All LabOrderTests
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabOrderTests]
    @LabOrderId INT = NULL,
    @LabTestId INT = NULL,
    @ReferrerId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT *, S.Name AS [LabTestName], R.Name AS [ReferrerName] FROM LabOrderTest OS
    LEFT JOIN [LabTest] S ON OS.LabTestId=S.Id
    LEFT JOIN Referrer R ON OS.ReferrerId=R.Id
        WHERE 
            (@LabOrderId IS NULL OR LabOrderId=@LabOrderId) AND
            (@LabTestId IS NULL OR LabTestId=@LabTestId) AND
            (@ReferrerId IS NULL OR ReferrerId=@ReferrerId)
        ORDER BY SortOrder
END
GO
