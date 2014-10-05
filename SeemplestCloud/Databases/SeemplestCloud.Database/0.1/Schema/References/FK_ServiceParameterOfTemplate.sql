ALTER TABLE [Platform].[PackageTemplateParameter] WITH CHECK
ADD CONSTRAINT [FK_ServiceParameterOfTemplate]
FOREIGN KEY ([ServiceCode], [ParameterCode])
REFERENCES [Platform].[ServiceParameter] ([ServiceCode], [ParameterCode])
