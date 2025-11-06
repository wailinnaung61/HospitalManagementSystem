SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 07, 2022
-- Description: Get All LabTests
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabTests]
    @BranchId INT = NULL,
    @LabTestId INT = NULL,
    @Name NVARCHAR(MAX) = NULL,
    @Code NVARCHAR(MAX) = NULL,
    @IsLabReport BIT = NULL,
    @Category INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM [LabTest]
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId) AND
            (@LabTestId IS NULL OR Id=@LabTestId) AND
            (@Name IS NULL OR dbo.IncludeInEachOther([Name], @Name)=1) AND
            (@Code IS NULL OR dbo.IncludeInEachOther([Code], @Code)=1) AND
            (@IsLabReport IS NULL OR IsLabReport=@IsLabReport) AND
            (@Category IS NULL OR Category=@Category) AND
            IsDelete = 0
        ORDER BY [Name]
END
GO
