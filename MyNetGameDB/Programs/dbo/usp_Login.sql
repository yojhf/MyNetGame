CREATE PROCEDURE [dbo].[usp_Login]
	@userId varchar(20),
	@password varchar(20)
AS
BEGIN
	declare @t_result int

	if exists (select userId from userTbl where userId = @userId and password = @password)
	begin
		set @t_result = 0
	end
	else		
	begin
		set @t_result = 1
	end

	select @t_result result
END