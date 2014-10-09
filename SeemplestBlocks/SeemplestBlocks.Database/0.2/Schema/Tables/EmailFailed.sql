CREATE TABLE [Email].[EmailFailed]
(
    [Id] INT NOT NULL, 
    [QueuedUtc] DATETIMEOFFSET NOT NULL,
    [FromAddr] NVARCHAR(256) NOT NULL, 
    [FromName] NVARCHAR(256) NULL, 
    [ToAddr] NVARCHAR(256) NOT NULL, 
    [Subject] NVARCHAR(256) NULL, 
    [Message] NVARCHAR(MAX) NULL, 
	[Appointment] nvarchar(MAX) NULL,
    [FailedUtc] DATETIMEOFFSET NOT NULL,
    [RetryCount] INT NOT NULL, 
    [LastError] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_EmailFailed] PRIMARY KEY NONCLUSTERED ([Id]) 
)

GO

CREATE CLUSTERED INDEX [IX_EmailFailed] ON [Email].[EmailFailed] ([FailedUtc])
