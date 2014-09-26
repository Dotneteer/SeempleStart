CREATE TABLE [Config].[ConfigurationVersion]
(
  [Id] INT identity NOT NULL, 
  [Label] NVARCHAR(128) NULL, 
  [Created] DATETIME NOT NULL, 
  CONSTRAINT [PK_ConfigurationVersion] PRIMARY KEY ([Id])
)
