CREATE PROCEDURE [dbo].[usp_AddUser]
	@userId varchar(20),
	@password varchar(20),
	@mobile char(11) = null,
	@level smallInt = 1,
	@health int = 100,
	@gold int = 1000
AS
BEGIN
	declare @t_result int
	set @t_result = 0

	if exists (select userId from userTbl where userId = @userId)
	begin
		set @t_result = 1
	end
	else
	begin
		insert into userTbl (userId, password, mobile, level, health, gold, mDate)
		values (@userId, @password, @mobile, @level, @health, @gold, GETDATE())
	end

	select @t_result result
END
