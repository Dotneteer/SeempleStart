ALTER TABLE [Platform].[SubscriptionParameterValue] WITH CHECK
ADD CONSTRAINT [FK_SubscriptionOfParameterValue]
FOREIGN KEY ([SubscriptionId])
REFERENCES [Platform].[Subscription] ([Id])
