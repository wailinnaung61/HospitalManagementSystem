SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 25, 2022
-- Description: Get All OrderServices
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetOrderServices]
    @OrderId INT = NULL,
    @ServiceId INT = NULL,
    @ReferrerId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT *, S.Name AS [ServiceName], R.Name AS [ReferrerName] FROM OrderService OS
    JOIN [Service] S ON OS.ServiceId=S.Id
    JOIN Referrer R ON OS.ReferrerId=R.Id
        WHERE 
            (@OrderId IS NOT NULL AND OrderId=@OrderId) AND
            (@ServiceId IS NOT NULL AND ServiceId=@ServiceId) AND
            (@ReferrerId IS NOT NULL AND ReferrerId=@ReferrerId) AND
            (@IsFOC IS NOT NULL AND IsFOC=@IsFOC)
        ORDER BY SortOrder
END
GO
