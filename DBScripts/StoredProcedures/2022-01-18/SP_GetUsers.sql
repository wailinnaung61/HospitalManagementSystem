SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 18, 2022
-- Description: Get All Users
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUsers]
    @UserId NVARCHAR(450) = NULL,
    @RoleId NVARCHAR(450) = NULL,
    @BranchId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT U.*, R.Id AS [RoleId], R.Name AS [RoleName], B.Name AS [BranchName] FROM AspNetUsers U
        LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
        LEFT JOIN AspNetRoles R ON R.Id = UR.RoleId
        LEFT JOIN Branch B ON B.Id = U.BranchId
        WHERE 
            (@UserId IS NOT NULL AND U.Id=@UserId) OR 
            (@UserId IS NULL AND U.Id IS NOT NULL) AND

            ((@RoleId IS NOT NULL AND R.Id=@RoleId) OR 
            (@RoleId IS NULL AND U.Id IS NOT NULL)) AND

            ((@BranchId IS NOT NULL AND U.BranchId=@BranchId) OR 
            (@BranchId IS NULL AND U.Id IS NOT NULL)) 
        ORDER BY B.Id, R.Name, U.UserName
END
GO
