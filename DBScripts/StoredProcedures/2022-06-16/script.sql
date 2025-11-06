declare @BranchId INT = 1
  declare   @StartDate DATETIME2(7) = '2022-06-16'
  declare   @EndDate DATETIME2(7) = '2022-06-16'   
  declare   @Status INT = NULL

select * from (SELECT Count(V.PatientId) As BookPatient,null As CompletedPatient, D.Name AS [DoctorName]

    FROM Visit V
        LEFT JOIN VisitType VT ON V.VisitTypeId=VT.Id
        LEFT JOIN Patient P ON V.PatientId=P.Id
        LEFT JOIN Doctor D ON V.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR V.BranchId=@BranchId)AND       
        (@StartDate IS NULL OR Convert(Date,[Date])>=Convert(Date,@StartDate))AND
        (@EndDate IS NULL OR Convert(Date,[Date])<=Convert(Date,@EndDate))AND
        (@Status IS NULL OR V.Status=@Status) AND
        V.IsDelete = 0 and (V.Status=1 )
		group by V.status,D.Name
   ) as T1
union
select * from (SELECT null as BookPatient,Count(V.PatientId) As CompletedPatient, D.Name AS [DoctorName]

    FROM Visit V
        LEFT JOIN VisitType VT ON V.VisitTypeId=VT.Id
        LEFT JOIN Patient P ON V.PatientId=P.Id
        LEFT JOIN Doctor D ON V.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR V.BranchId=@BranchId)AND       
        (@StartDate IS NULL OR Convert(Date,[Date])>=Convert(Date,@StartDate))AND
        (@EndDate IS NULL OR Convert(Date,[Date])<=Convert(Date,@EndDate))AND
        (@Status IS NULL OR V.Status=@Status) AND
        V.IsDelete = 0 and (V.Status=3)
		group by V.status,D.Name
   ) T2
   group by DoctorName,CompletedPatient,BookPatient
