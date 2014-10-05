ALTER TABLE [Platform].[SubscriptionOwner] WITH CHECK
ADD CONSTRAINT [FK_UserOfOwner]
FOREIGN KEY ([UserId])
REFERENCES [Platform].[User] ([Id])
