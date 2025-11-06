SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 18, 2022
-- Description: Get All Users
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetUsers]
    @UserId NVARCHAR(450) = NULL,
    @RoleId NVARCHAR(450) = NULL,
    @BranchId INT = NULL,
    @OutletId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT U.*, R.Id AS [RoleId], R.Name AS [RoleName], B.Name AS [BranchName], O.Name AS [OutletName] FROM AspNetUsers U
        LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
        LEFT JOIN AspNetRoles R ON R.Id = UR.RoleId
        LEFT JOIN Branch B ON B.Id = U.BranchId
        LEFT JOIN Outlet O ON O.Id = U.OutletId
        WHERE 
            (@UserId IS NULL OR U.Id=@UserId) AND
            (@RoleId IS NULL OR R.Id=@RoleId) AND
            (@BranchId IS NULL OR U.BranchId=@BranchId) AND
            (@OutletId IS NULL OR U.OutletId=@OutletId) 
        ORDER BY B.Id, R.Name, U.UserName
END
GO
