CREATE TABLE [Platform].[User]
(
	[Id] uniqueidentifier NOT NULL,
	[SubscriptionId] int NULL,
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
    CONSTRAINT [PK_User] PRIMARY KEY ([Id]), 
    CONSTRAINT [AK_UserName] UNIQUE ([UserName]),
    CONSTRAINT [AK_Email] UNIQUE ([Email])
)
