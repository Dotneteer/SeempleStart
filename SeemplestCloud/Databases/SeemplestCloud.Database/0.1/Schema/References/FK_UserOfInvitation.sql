ALTER TABLE [Platform].[UserInvitation] WITH CHECK
ADD CONSTRAINT [FK_UserOfInvitation]
FOREIGN KEY ([UserId])
REFERENCES [Platform].[User] ([Id])
