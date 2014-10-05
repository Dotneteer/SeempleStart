ALTER TABLE [Platform].[PackageTemplate] WITH CHECK
ADD CONSTRAINT [FK_UpgradeOfPackage]
FOREIGN KEY ([UpgradeCode])
REFERENCES [Platform].[PackageTemplate] ([Code])
