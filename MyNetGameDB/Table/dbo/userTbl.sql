create table userTbl
(	userId varchar(20) not null primary key,		-- 유저 아이디(이메일)
	password varchar(20) not null,					-- 패스워드
	mobile char(11),							-- 유저 전화번호
	level smallint not null,					-- 유저 레벨
	health int not null,						-- 유저 hp
	gold int not null,							-- 유저 골드
	mDate date									-- 유저 가입일
)
