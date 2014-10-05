CREATE TABLE [Platform].[SubscriptionOwner]
(
	[SubscriptionId] int NOT NULL,
	[UserId] uniqueidentifier NOT NULL
    CONSTRAINT [PK_SubscriptionOwner] PRIMARY KEY ([SubscriptionId], [UserId])
)
