CREATE TABLE [Config].[ConfigurationValue]
(
    [VersionId] int NOT NULL,
    [Category] varchar(64) NOT NULL, 
    [ConfigKey] VARCHAR(64) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_ConfigurationValue] PRIMARY KEY ([VersionId], [Category], [ConfigKey])
)
