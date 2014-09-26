CREATE TABLE [Messages].[QueueMessage]
(
    [QueueID] [int] NOT NULL,
    [VisibilityStartTime] [datetime] NOT NULL DEFAULT getdate(),
    [MessageId] [uniqueidentifier] NOT NULL,
    [ExpiryTime] [datetime] NOT NULL,
    [InsertionTime] [datetime] NOT NULL,
    [DequeueCount] [int] NOT NULL DEFAULT 0,
    [Data] [nvarchar](max) NOT NULL,
    [PopReceipt] [uniqueidentifier] NULL,
    CONSTRAINT [PK_QueueMessage] PRIMARY KEY CLUSTERED 
    (
        [QueueID] ASC,
        [VisibilityStartTime] ASC,
        [MessageId] ASC
    )
)
