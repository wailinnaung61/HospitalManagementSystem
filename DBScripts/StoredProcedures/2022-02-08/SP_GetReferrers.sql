-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: 08_Feb_2022
-- Description:	Get Referrers
-- =============================================
CREATE PROCEDURE SP_GetReferrers
@ReferrerId int=null	
AS
BEGIN

	SET NOCOUNT ON;
	Select * from Referrer where IsDelete=0 and ((@ReferrerId is not null and Id=@ReferrerId) or (@ReferrerId is null and Id is not null))
  
END
GO
