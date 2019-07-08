USE [Portalia]
GO

ALTER TABLE [dbo].[WorkContractStatus]
ADD HasSentReminder BIT NOT NULL
CONSTRAINT DF_WorkContractStatus_HasSentReminder DEFAULT 0