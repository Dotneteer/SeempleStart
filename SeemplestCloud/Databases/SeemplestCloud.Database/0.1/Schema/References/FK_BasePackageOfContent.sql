ALTER TABLE [Platform].[SubscriptionContent] WITH CHECK
ADD CONSTRAINT [FK_BasePackageOfContent]
FOREIGN KEY ([BasePackageCode])
REFERENCES [Platform].[PackageTemplate] ([Code])
