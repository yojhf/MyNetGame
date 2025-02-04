CREATE PROCEDURE [dbo].[usp_UserInfo]
	@userId varchar(20)
AS
BEGIN	
	if exists (select userId from userTbl where userId = @userId)
	begin
		select * from userTbl where userId = @userId
	end
	else
	begin
		select null
	end
END
