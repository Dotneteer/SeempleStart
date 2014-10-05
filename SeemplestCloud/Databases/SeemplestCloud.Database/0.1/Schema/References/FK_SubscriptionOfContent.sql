ALTER TABLE [Platform].[SubscriptionContent] WITH CHECK
ADD CONSTRAINT [FK_SubscriptionOfContent]
FOREIGN KEY ([SubscriptionId])
REFERENCES [Platform].[Subscription] ([Id])
