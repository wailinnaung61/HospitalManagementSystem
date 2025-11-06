SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 02, 2022
-- Description: Get All Patients
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPatients]
    @BranchId INT = NULL,
    @PatientId INT = NULL,
    @RegDate DATETIME2(7) = NULL,
    @StartRegDate DATETIME2(7) = NULL,
    @EndRegDate DATETIME2(7) = NULL,
    @RegNo NVARCHAR(MAX) = NULL,
    @Name NVARCHAR(MAX) = NULL,
    @NRC NVARCHAR(MAX) = NULL,
    @Guardian NVARCHAR(MAX) = NULL,
    @DateOfBirth DATETIME2(7) = NULL,
    @StartDateOfBirth DATETIME2(7) = NULL,
    @EndDateOfBirth DATETIME2(7) = NULL,
    @Phone NVARCHAR(MAX) = NULL,
    @BloodType NVARCHAR(MAX) = NULL,
    @ReferredBy NVARCHAR(MAX) = NULL,
    @ReferrerId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM Patient 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId) AND
            (@PatientId IS NULL OR Id=@PatientId) AND
            (@RegDate IS NULL OR RegDate=@RegDate) AND
            (@StartRegDate IS NULL OR RegDate>=@StartRegDate) AND
            (@EndRegDate IS NULL OR RegDate<=@EndRegDate) AND
            (@RegNo IS NULL OR dbo.IncludeInEachOther(RegNo, @RegNo)=1) AND
            (@Name IS NULL OR dbo.IncludeInEachOther([Name], @Name)=1) AND
            (@NRC IS NULL OR dbo.IncludeInEachOther(NRC, @NRC)=1) AND
            (@Guardian IS NULL OR dbo.IncludeInEachOther(Guardian, @Guardian)=1) AND
            (@DateOfBirth IS NULL OR DateOfBirth=@DateOfBirth) AND
            (@StartDateOfBirth IS NULL OR DateOfBirth>=@StartDateOfBirth) AND
            (@EndDateOfBirth IS NULL OR DateOfBirth<=@EndDateOfBirth) AND
            (@Phone IS NULL OR dbo.IncludeInEachOther(Phone, @Phone)=1) AND
            (@BloodType IS NULL OR dbo.IncludeInEachOther(BloodType, @BloodType)=1) AND
            (@ReferredBy IS NULL OR dbo.IncludeInEachOther(ReferredBy, @ReferredBy)=1) AND
            (@ReferrerId IS NULL OR ReferrerId=@ReferrerId) AND
            (@IsActive IS NULL OR IsActive=@IsActive) AND
            IsDelete=0
        ORDER BY Name
END
GO
