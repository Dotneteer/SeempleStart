ALTER TABLE [Platform].[ServiceUser] WITH CHECK
ADD CONSTRAINT [FK_ServiceUser]
FOREIGN KEY ([Id])
REFERENCES [Platform].[User] ([Id])
