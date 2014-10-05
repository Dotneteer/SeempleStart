ALTER TABLE [Platform].[PackageTemplateParameter] WITH CHECK
ADD CONSTRAINT [FK_PackageOfTemplateParameter]
FOREIGN KEY ([PackageCode])
REFERENCES [Platform].[PackageTemplate] ([Code])
