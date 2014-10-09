CREATE TABLE [Email].[Template]
(
    [Id] VARCHAR(64) NOT NULL, 
    [Subject] NVARCHAR(256) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_EmailTemplate] PRIMARY KEY ([Id])
)