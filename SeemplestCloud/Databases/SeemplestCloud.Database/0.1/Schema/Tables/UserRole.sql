CREATE TABLE [Platform].[UserRole]
(
	[UserId] uniqueidentifier NOT NULL,
	[ServiceCode] varchar(32) NOT NULL,
	[RoleCode] varchar(32) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([UserId], [ServiceCode], [RoleCode])
)
