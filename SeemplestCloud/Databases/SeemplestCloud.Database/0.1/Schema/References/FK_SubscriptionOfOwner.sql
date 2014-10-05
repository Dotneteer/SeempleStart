ALTER TABLE [Platform].[SubscriptionOwner] WITH CHECK
ADD CONSTRAINT [FK_SubscriptionOfOwner]
FOREIGN KEY ([SubscriptionId])
REFERENCES [Platform].[Subscription] ([Id])
