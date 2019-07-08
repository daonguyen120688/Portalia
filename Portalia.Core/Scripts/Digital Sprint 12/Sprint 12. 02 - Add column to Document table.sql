USE [Portalia]
GO

ALTER TABLE [dbo].[Document]
ADD IsActive BIT NOT NULL
CONSTRAINT DF_Document_IsActive DEFAULT 1

GO

ALTER TABLE [dbo].[WorkContracts]
ADD DocumentId INT NULL

GO

ALTER TABLE [dbo].[WorkContracts]
ADD CONSTRAINT FK_WorkContracts_Document
FOREIGN KEY (DocumentId)
REFERENCES [dbo].[Document](DocumentId)

GO








