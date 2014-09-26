CREATE TABLE [Config].[CurrentConfigurationVersion]
(
  [Id] int NOT NULL,
  [CurrentVersion] INT NOT NULL,
  [LastModified] datetime, 
  CONSTRAINT [PK_CurrentConfigurationVersion] PRIMARY KEY ([Id])
)
