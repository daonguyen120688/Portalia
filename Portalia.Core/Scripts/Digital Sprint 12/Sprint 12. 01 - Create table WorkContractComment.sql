USE [Portalia]
GO

CREATE TABLE [dbo].[WorkContractComment]
(
 [WCCommentId] int IDENTITY (1, 1) NOT NULL ,
 [ContractId]  int NOT NULL ,
 [Comment]     ntext NOT NULL ,
 [CreatedDate] datetime2(7) NOT NULL ,
 [CreatedBy]   nvarchar(128) NOT NULL ,

 CONSTRAINT [PK_WorkContractComment] PRIMARY KEY CLUSTERED ([WCCommentId] ASC),
 CONSTRAINT [FK_282] FOREIGN KEY ([ContractId])  REFERENCES [dbo].[WorkContracts]([ContractId])
);
GO


CREATE NONCLUSTERED INDEX [fkIdx_WorkContractCommentContractId] ON [dbo].[WorkContractComment] 
 (
  [ContractId] ASC
 )

GO