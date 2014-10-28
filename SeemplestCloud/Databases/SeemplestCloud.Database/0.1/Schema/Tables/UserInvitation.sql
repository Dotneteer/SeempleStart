CREATE TABLE [Platform].[UserInvitation]
(
	[Id] int identity NOT NULL,
	[UserId] uniqueidentifier NOT NULL,
	[SubscriptionId] int NULL,
	[InvitedEmail] nvarchar(255) NOT NULL,
	[InvitedUserName] nvarchar(64) not null,
	[InvitationCode] nvarchar(1024) NOT NULL,
	[ExpirationDateUtc] datetimeoffset NULL,
	[State] varchar(16) NOT NULL,
	[Type] varchar(16) NOT NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_UserInvitation] PRIMARY KEY NONCLUSTERED ([Id])
)

CREATE UNIQUE INDEX [IX_UserInvitationOnCode] ON [Platform].[UserInvitation] ([InvitationCode])
CREATE INDEX [IX_UserInvitationOnEmail] ON [Platform].[UserInvitation] ([InvitedEmail])
CREATE CLUSTERED INDEX [IX_UserInvitationOnSubscriptionUser] ON [Platform].[UserInvitation] ([SubscriptionId], [InvitedUserName])

