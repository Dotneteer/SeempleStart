CREATE TABLE [Security].[UserAccount]
(
	[UserId] varchar(64) NOT NULL,
	[Provider] varchar(64) NOT NULL,
	[ProviderData] nvarchar(255) NOT NULL, 
    CONSTRAINT [PK_UserAccount]  PRIMARY KEY NONCLUSTERED ([UserId], [Provider]), 
    CONSTRAINT [AK_UserAccountOnProviderData] UNIQUE CLUSTERED ([Provider], [ProviderData]), 
    CONSTRAINT [FK_UserOfAccount] FOREIGN KEY ([UserId]) REFERENCES [Security].[User]([Id])
)
