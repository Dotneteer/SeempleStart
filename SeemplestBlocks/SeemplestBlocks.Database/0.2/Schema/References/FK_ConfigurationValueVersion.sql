ALTER TABLE [Config].[ConfigurationValue] WITH CHECK
ADD CONSTRAINT [FK_ConfigurationValueVersion]
FOREIGN KEY ([VersionId])
REFERENCES [Config].[ConfigurationVersion] ([Id])
