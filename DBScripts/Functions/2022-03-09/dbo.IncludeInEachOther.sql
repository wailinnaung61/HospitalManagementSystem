CREATE FUNCTION dbo.IncludeInEachOther (@Str1 NVARCHAR(MAX), @Str2 NVARCHAR(MAX))
RETURNS BIT 
AS
BEGIN
    DECLARE @ReturnValue BIT = 0;
    IF (@Str1 LIKE '%' + @Str2 + '%' OR @Str2 LIKE '%' + @Str1 + '%')
    BEGIN
        SET @ReturnValue = 1
    END
    RETURN @ReturnValue
END