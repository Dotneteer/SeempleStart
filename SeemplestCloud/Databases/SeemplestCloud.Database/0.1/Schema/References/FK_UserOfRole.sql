ALTER TABLE [Platform].[UserRole] WITH CHECK
ADD CONSTRAINT [FK_UserOfRole]
FOREIGN KEY ([UserId])
REFERENCES [Platform].[User] ([Id])
