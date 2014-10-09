ALTER TABLE [Platform].[UserInvitation] WITH CHECK
ADD CONSTRAINT [FK_SubscriptionOfInvitation]
FOREIGN KEY ([SubscriptionId])
REFERENCES [Platform].[Subscription] ([Id])
