ALTER TABLE [Platform].[UserAccount] WITH CHECK
ADD CONSTRAINT [FK_UserOfAccount]
FOREIGN KEY ([UserId])
REFERENCES [Platform].[User] ([Id])
