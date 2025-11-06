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
-- Create date: 2022-04-25
-- Description:	Get IPDRound
-- =============================================
CREATE PROCEDURE SP_GetIPDRound
	-- Add the parameters for the stored procedure here
	@Id int =null,
	@IPDRecordId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select * from IPDRound R 
	join IPDRecord Re on Re.Id=R.IPDRecordId
	where 
	(@Id is null OR R.Id=@Id)AND
	(@IPDRecordId is null OR R.IPDRecordId=@IPDRecordId)
	

	
END
GO
