CREATE TABLE [Config].[ListItemDefinition]
(
  [ListId] varchar(32) NOT NULL,
  [ItemId] varchar(32) NOT NULL,
  [IsSystemItem] BIT NOT NULL, 
  CONSTRAINT [PK_ListItemDefinition] PRIMARY KEY ([ListId], [ItemId])
)
