CREATE TABLE [dbo].[Files] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [CreatedDate] DATETIME       NOT NULL,
    [FolderID]    INT            NOT NULL,
    [ContentType] NVARCHAR (255) NOT NULL,
    [Link]        NVARCHAR (255) NOT NULL,
    [Size]        INT            NULL,
    [Dimensions]  NVARCHAR (255) NULL,
    [Resolution]  NVARCHAR (255) NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([ID] ASC)
);

