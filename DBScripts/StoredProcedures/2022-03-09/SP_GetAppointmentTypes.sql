SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetAppointmentTypes]
    @BranchId INT = NULL,
    @AppointmentTypeId INT = NULL

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM AppointmentType 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId)AND
            (@AppointmentTypeId IS NULL OR Id=@AppointmentTypeId) AND
            IsDelete = 0
        ORDER BY [Type]
END
GO
