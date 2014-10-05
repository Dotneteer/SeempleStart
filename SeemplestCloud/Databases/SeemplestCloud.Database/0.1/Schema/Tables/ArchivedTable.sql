CREATE TABLE [Platform].[ArchivedUser]
(
	[Id] uniqueidentifier NOT NULL,
	[UserName] nvarchar(255) NOT NULL,
	[Email] nvarchar(255) NOT NULL,
	[SecurityStamp] nvarchar(255) NOT NULL,
	[PasswordHash] nvarchar(255) NULL,
	[LastFailedAuthUtc] datetimeoffset NULL,
	[AccessFailedCount] int NOT NULL,
	[LockedOut] bit NOT NULL,
	[OwnerSuspend] bit NOT NULL,
	[PasswordResetSuspend] bit NOT NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
	[ArchivedUtc] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ArchivedUser] PRIMARY KEY ([Id])
)