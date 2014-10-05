ALTER TABLE [Platform].[ServiceParameter] WITH CHECK
ADD CONSTRAINT [FK_DefinitionOfServiceParameter]
FOREIGN KEY ([ServiceCode])
REFERENCES [Platform].[ServiceDefinition] ([Code])
