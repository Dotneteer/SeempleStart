ALTER TABLE [Platform].[ServiceRole] WITH CHECK
ADD CONSTRAINT [FK_DefinitionOfServiceRole]
FOREIGN KEY ([ServiceCode])
REFERENCES [Platform].[ServiceDefinition] ([Code])
