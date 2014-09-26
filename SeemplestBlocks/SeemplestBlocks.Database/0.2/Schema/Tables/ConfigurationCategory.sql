CREATE TABLE [Config].[ConfigurationCategory]
(
    [Id] VARCHAR(64) NOT NULL, 
    [Description] NVARCHAR(1024) NULL, 
    [IsActive] BIT NOT NULL, 
    [LastModified] DATETIME NOT NULL, 
    CONSTRAINT [PK_ConfigurationCategory] PRIMARY KEY ([Id])
)
