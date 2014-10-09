CREATE TABLE [Email].[EmailSent]
(
	[Id] int NOT NULL,
	[QueuedUtc] datetimeoffset NOT NULL,
	[FromAddr] nvarchar(256) NOT NULL,
	[FromName] nvarchar(256) NULL,
	[ToAddr] nvarchar(256) NOT NULL,
	[Subject] nvarchar(256) NOT NULL,
	[Message] nvarchar(MAX) NOT NULL,
	[Appointment] nvarchar(MAX) NULL,
	[SentUtc] datetimeoffset NOT NULL
    CONSTRAINT [PK_EmailSent] PRIMARY KEY NONCLUSTERED ([Id])
)

GO

CREATE CLUSTERED INDEX [IX_EmailSent] ON [Email].[EmailSent] ([SentUtc])
