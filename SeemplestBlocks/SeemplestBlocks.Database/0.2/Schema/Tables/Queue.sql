CREATE TABLE [Messages].[Queue]
(
    [QueueId] [int] IDENTITY(1,1) NOT NULL,
    [QueueName] [nvarchar](64) NOT NULL,
    CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([QueueId] ASC), 
    CONSTRAINT [UK_QueueName] UNIQUE ([QueueName])
)
