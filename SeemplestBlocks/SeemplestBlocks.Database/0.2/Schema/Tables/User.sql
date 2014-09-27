CREATE TABLE [Security].[User]
(
	[Id] varchar(64) NOT NULL,
	[UserName] nvarchar(255) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[SecurityStamp] nvarchar(255) NOT NULL,
	[EmailConfirmed] bit NOT NULL,
	[PasswordHash] nvarchar(255) NULL,
	[PhoneNumber] nvarchar(255) NULL,
	[PhoneNumberConfirmed] bit NOT NULL,
	[LockOutEndDateUtc] datetimeoffset NULL,
	[AccessFailedCount] int NOT NULL,
	[Active] bit NOT NULL, 
    [Created] DATETIME NOT NULL, 
    [LastModified] DATETIME NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id]), 
    CONSTRAINT [AK_UserName] UNIQUE ([UserName]),
    CONSTRAINT [AK_UserEmail] UNIQUE ([Email])
)
