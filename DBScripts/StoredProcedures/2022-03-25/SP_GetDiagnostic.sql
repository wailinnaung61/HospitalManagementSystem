SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetDiagnostic]
	-- Add the parameters for the stored procedure here
  @Id int=null,
  @Name nvarchar(MAX)=null,
  @SpecialityId int=null,
  @DoctorId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select D.*,Sp.Name as SpecialityName from Diagnostic D
	left join Speciality Sp on D.SpecialityId=Sp.Id
	where 
	(@Id is null OR D.Id=@Id) And
	(@Name is null OR D.Name=@Name) And
	(@SpecialityId is null OR D.SpecialityId=@SpecialityId) And
	(@DoctorId is null OR D.DoctorId=@DoctorId) And
	D.IsDelete=0
END
GO
