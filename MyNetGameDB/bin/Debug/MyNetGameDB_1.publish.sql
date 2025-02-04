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
PRINT N'[dbo].[userTbl] 테이블 다시 빌드 시작...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_userTbl] (
    [userId]   VARCHAR (20) NOT NULL,
    [password] VARCHAR (20) NOT NULL,
    [mobile]   CHAR (11)    NULL,
    [level]    SMALLINT     NOT NULL,
    [health]   INT          NOT NULL,
    [gold]     INT          NOT NULL,
    [mDate]    DATE         NULL,
    PRIMARY KEY CLUSTERED ([userId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[userTbl])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_userTbl] ([userId], [password], [mobile], [level], [health], [gold], [mDate])
        SELECT   [userId],
                 [password],
                 [mobile],
                 [level],
                 [health],
                 [gold],
                 [mDate]
        FROM     [dbo].[userTbl]
        ORDER BY [userId] ASC;
    END

DROP TABLE [dbo].[userTbl];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_userTbl]', N'userTbl';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'업데이트가 완료되었습니다.';


GO
