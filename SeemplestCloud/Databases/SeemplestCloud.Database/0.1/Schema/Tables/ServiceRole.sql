CREATE TABLE [Platform].[ServiceRole]
(
	[ServiceCode] varchar(32) NOT NULL,
	[RoleCode] varchar(32) NOT NULL,
	[Name] varchar(64) NOT NULL,
	[Description] nvarchar(1024),
    CONSTRAINT [PK_ServiceRole] PRIMARY KEY ([ServiceCode], [RoleCode]), 
    CONSTRAINT [AK_ServiceRoleName] UNIQUE ([ServiceCode], [Name])
)
