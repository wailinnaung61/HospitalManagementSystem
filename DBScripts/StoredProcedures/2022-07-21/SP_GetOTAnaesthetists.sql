/****** Object:  StoredProcedure [dbo].[SP_GetOTDoctors]    Script Date: 7/21/2022 4:33:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: July 21, 2022
-- Description:	Get OT Aneasthetist by OperationTreaterId
-- =============================================
Create PROCEDURE [dbo].[SP_GetOTAnaesthetists]
	@OperationTreaterId int,
	@DoctorId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select OTD.*,D.Name as DoctorName from OT_Anaesthetist OTD join Doctor D on OTD.DoctorId=D.Id
	where OperationTreaterId=@OperationTreaterId AND
	 (@DoctorId IS NULL OR DoctorId=@DoctorId) 
END
