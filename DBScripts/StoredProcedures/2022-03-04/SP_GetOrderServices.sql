SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 25, 2022
-- Description: Get All OrderServices
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetOrderServices]
    @OrderId INT = NULL,
    @ServiceId INT = NULL,
    @ReferrerId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT *, S.Name AS [ServiceName], R.Name AS [ReferrerName] FROM OrderService OS
    LEFT JOIN [Service] S ON OS.ServiceId=S.Id
    LEFT JOIN Referrer R ON OS.ReferrerId=R.Id
        WHERE 
            (@OrderId IS NULL OR OrderId=@OrderId) AND
            (@ServiceId IS NULL OR ServiceId=@ServiceId) AND
            (@ReferrerId IS NULL OR ReferrerId=@ReferrerId) AND
            (@IsFOC IS NULL OR IsFOC=@IsFOC)
        ORDER BY SortOrder
END
GO
