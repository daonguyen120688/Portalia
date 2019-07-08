USE [Portalia]
GO

ALTER TABLE [dbo].[WorkContracts]
ADD AdminAddress nvarchar(1000) NULL

GO

UPDATE FieldDetails
SET FieldName='AdminAddress'
WHERE FieldId=283

GO