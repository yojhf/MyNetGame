/*
MyNetGameDB의 배포 스크립트

이 코드는 도구를 사용하여 생성되었습니다.
파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
변경 내용이 손실됩니다.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "MyNetGameDB"
:setvar DefaultFilePrefix "MyNetGameDB"
:setvar DefaultDataPath "C:\Users\Admin\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\Admin\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
SQLCMD 모드가 지원되지 않으면 SQLCMD 모드를 검색하고 스크립트를 실행하지 않습니다.
SQLCMD 모드를 설정한 후에 이 스크립트를 다시 사용하려면 다음을 실행합니다.
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'이 스크립트를 실행하려면 SQLCMD 모드를 사용하도록 설정해야 합니다.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'프로시저 [dbo].[usp_AddUser]을(를) 만드는 중...';


GO
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
GO
PRINT N'업데이트가 완료되었습니다.';


GO
