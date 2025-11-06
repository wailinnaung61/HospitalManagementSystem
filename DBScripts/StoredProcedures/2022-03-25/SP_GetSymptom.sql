SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetSymptom]
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
	select Sy.*,Sp.Name as SpecialityName from Symptom Sy
	left join Speciality Sp on Sy.SpecialityId=Sp.Id
	where 
	(@Id is null OR Sy.Id=@Id) And
	(@Name is null OR Sy.Name=@Name) And
	(@SpecialityId is null OR Sy.SpecialityId=@SpecialityId) And
  (@DoctorId is null OR Sy.DoctorId=@DoctorId) And
	Sy.IsDelete=0
END
GO
