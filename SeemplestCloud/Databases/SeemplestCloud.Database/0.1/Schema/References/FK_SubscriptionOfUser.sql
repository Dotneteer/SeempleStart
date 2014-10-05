ALTER TABLE [Platform].[User] WITH CHECK
ADD CONSTRAINT [FK_SubscriptionOfUser]
FOREIGN KEY ([SubscriptionId])
REFERENCES [Platform].[Subscription] ([Id])
