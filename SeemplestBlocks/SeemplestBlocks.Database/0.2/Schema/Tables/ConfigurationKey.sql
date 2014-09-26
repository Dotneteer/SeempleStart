CREATE TABLE [Config].[ConfigurationKey]
(
    [Id] VARCHAR(64) NOT NULL, 
    [CategoryId] VARCHAR(64) NOT NULL,
    [Description] NVARCHAR(1024) NULL, 
    [IsActive] BIT NOT NULL, 
    [LastModified] DATETIME NOT NULL, 
    CONSTRAINT [PK_ConfigurationKey] PRIMARY KEY ([Id]),
)