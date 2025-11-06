/****** Object:  StoredProcedure [dbo].[SP_GetOrderServices]    Script Date: 5/13/2022 9:16:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Zun Pwint Phyu
-- Create Date: May 13, 2022
-- Description: Get OperationService By OperationTreaterId
-- =============================================
Create PROCEDURE [dbo].[SP_GetOperationServices]
    @OperationTreaterId INT = NULL,
    @ServiceId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT *, S.Name AS [ServiceName] FROM OperationService OS
    LEFT JOIN [Service] S ON OS.ServiceId=S.Id
        WHERE 
            (@OperationTreaterId IS NULL OR OperationTreaterId=@OperationTreaterId) AND
            (@ServiceId IS NULL OR ServiceId=@ServiceId) AND
            (@IsFOC IS NULL OR IsFOC=@IsFOC)
        ORDER BY SortOrder
END