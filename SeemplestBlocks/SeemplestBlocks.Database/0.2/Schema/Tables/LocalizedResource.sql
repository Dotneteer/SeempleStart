CREATE TABLE [Config].[LocalizedResource]
(
    [Locale] VARCHAR(5) NOT NULL, 
    [Category] NVARCHAR(128) NOT NULL, 
    [ResourceKey] NVARCHAR(128) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_LocalizedResource] PRIMARY KEY ([Locale], [Category], [ResourceKey])
)
