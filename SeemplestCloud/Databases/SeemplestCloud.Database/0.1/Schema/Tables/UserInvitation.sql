CREATE TABLE [Platform].[UserInvitation]
(
	[UserId] uniqueidentifier NOT NULL,
	[SubscriptionId] int NULL,
	[InvitedEmail] nvarchar(255) NOT NULL,
	[InvitationCode] nvarchar(1024) NOT NULL,
	[ExpirationDateUtc] datetimeoffset NULL,
	[State] varchar(16) NOT NULL,
	[Type] varchar(16) NOT NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_UserInvitation] PRIMARY KEY ([UserId]), 
    CONSTRAINT [AK_InvitedEmail] UNIQUE ([InvitedEmail])
)
