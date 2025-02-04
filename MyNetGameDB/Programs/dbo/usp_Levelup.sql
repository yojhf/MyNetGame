CREATE PROCEDURE [dbo].[usp_Levelup]
	@userId varchar(20)
AS
BEGIN
	-- 레벨업 성공시 레벨업한 최종 레벨값 반환, 실패시 0 반환
	declare @t_result int
	set @t_result = 0

	if exists (select userId from userTbl where userId = @userId)
	begin
		update userTbl set level = level + 1 where userId = @userId
		select level from userTbl where userId = @userId
	end
	else
	begin
		select @t_result
	end
END
