SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 23, 2022
-- Description: Get All User's Access Menus By UserId or RoleId
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUserAccessMenus]
    @UserId NVARCHAR(450) = NULL,
    @MenuId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM UserAccessMenu
        WHERE 
            ((@UserId IS NOT NULL AND UserId=@UserId) OR 
            (@UserId IS NULL AND UserId IS NOT NULL)) AND
            ((@MenuId IS NOT NULL AND MenuId=@MenuId) OR 
            (@MenuId IS NULL AND UserId IS NOT NULL)) 
END
GO
