USE Portalia
GO

CREATE TABLE [dbo].[WorkContracts]
(
 [ContractId]         int IDENTITY (1, 1) NOT NULL ,
 [UserId]             nvarchar(128) NOT NULL ,
 [Type]               int NOT NULL ,
 [Title]              nvarchar(100) NULL ,
 [FirstName]          nvarchar(100) NULL ,
 [LastName]           nvarchar(100) NULL ,
 [DateOfBirth]        datetime2(7) NULL ,
 [CountryOfBirth]     nvarchar(250) NULL ,
 [Nationality]        nvarchar(250) NULL ,
 [Address]            nvarchar(500) NULL ,
 [PostCode]           nvarchar(50) NULL ,
 [City]               nvarchar(100) NULL ,
 [Country]            nvarchar(150) NULL ,
 [SSN]                nvarchar(150) NULL ,
 [VisaPermitNo]       nvarchar(100) NULL ,
 [ContractStartDate]  datetime2(7) NULL ,
 [ContractEndDate]    datetime2(7) NULL ,
 [ProjectDescription] nvarchar(700) NULL ,
 [Skills]             nvarchar(1000) NULL ,
 [ClientNameByCandidate] nvarchar(250) NULL ,
 [ClientName]         nvarchar(200) NULL ,
 [ClientAddress]      nvarchar(500) NULL ,
 [Allowances]         decimal(16,4) NULL ,
 [Currency]           nvarchar(50) NULL ,
 [Basic]              nvarchar(250) NULL ,
 [CandidateId]        nvarchar(150) NULL ,
 [CreatedDate]        datetime2(7) NOT NULL ,
 [CreatedBy]          nvarchar(128) NOT NULL ,
 [UpdatedDate]        datetime2(7) NULL ,
 [UpdatedBy]          nvarchar(128) NULL ,
 [HighlightedFields]     nvarchar(1000) NULL ,

 CONSTRAINT [PK_WorkContract] PRIMARY KEY CLUSTERED ([ContractId] ASC),
 CONSTRAINT [FK_UerWorkContract] FOREIGN KEY ([UserId])  REFERENCES [dbo].[AspNetUsers]([Id])
);
GO








-- ************************************** [dbo].[StatusMapping]

CREATE TABLE [dbo].[StatusMapping]
(
 [Id]         int IDENTITY (1, 1) NOT NULL ,
 [StatusName] nvarchar(200) NOT NULL ,

 CONSTRAINT [PK_StatusMapping] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO








-- ************************************** [dbo].[RefData]

CREATE TABLE [dbo].[RefData]
(
 [Id]    int IDENTITY (1, 1) NOT NULL ,
 [Key]   nvarchar(50) NOT NULL ,
 [Value] nvarchar(500) NULL ,
 [Code]  nvarchar(50) NOT NULL ,

 CONSTRAINT [PK_RefData] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO








-- ************************************** [dbo].[FieldDetails]

CREATE TABLE [dbo].[FieldDetails]
(
 [FieldDetailId] int IDENTITY (1, 1) NOT NULL ,
 [FieldId]       int NOT NULL ,
 [FieldName]     nvarchar(250) NOT NULL ,
 [Order]         int NULL ,

 CONSTRAINT [PK_FieldDetails] PRIMARY KEY CLUSTERED ([FieldDetailId] ASC)
);
GO








-- ************************************** [dbo].[WorkContractStatus]

CREATE TABLE [dbo].[WorkContractStatus]
(
 [WorkContractStatusId] int IDENTITY (1, 1) NOT NULL ,
 [ContractId]           int NOT NULL ,
 [StatusId]             int NOT NULL ,
 [CreatedDate]          datetime2(7) NOT NULL ,
 [CreatedBy]            nvarchar(128) NOT NULL ,

 CONSTRAINT [PK_WorkContractStatus] PRIMARY KEY CLUSTERED ([WorkContractStatusId] ASC),
 CONSTRAINT [FK_171] FOREIGN KEY ([ContractId])  REFERENCES [dbo].[WorkContracts]([ContractId]),
 CONSTRAINT [FK_179] FOREIGN KEY ([StatusId])  REFERENCES [dbo].[StatusMapping]([Id])
);
GO


CREATE NONCLUSTERED INDEX [fkIdx_171] ON [dbo].[WorkContractStatus] 
 (
  [ContractId] ASC
 )

GO

CREATE NONCLUSTERED INDEX [fkIdx_179] ON [dbo].[WorkContractStatus] 
 (
  [StatusId] ASC
 )

GO







-- ************************************** [dbo].[TrackingChanges]

CREATE TABLE [dbo].[TrackingChanges]
(
 [TrackingChangeId] int IDENTITY (1, 1) NOT NULL ,
 [ContractId]       int NOT NULL ,
 [FieldName]        nvarchar(250) NOT NULL ,
 [OldValue]         nvarchar(500) NULL ,
 [NewValue]         nvarchar(500) NULL ,
 [CreatedDate]      datetime2(7) NOT NULL ,
 [CreatedBy]        nvarchar(128) NOT NULL ,

 CONSTRAINT [PK_TrackingChanges] PRIMARY KEY CLUSTERED ([TrackingChangeId] ASC),
 CONSTRAINT [FK_230] FOREIGN KEY ([ContractId])  REFERENCES [dbo].[WorkContracts]([ContractId])
);
GO


CREATE NONCLUSTERED INDEX [fkIdx_230] ON [dbo].[TrackingChanges] 
 (
  [ContractId] ASC
 )

GO







-- ************************************** [dbo].[DataFields]

CREATE TABLE [dbo].[DataFields]
(
 [DataFieldId] int IDENTITY (1, 1) NOT NULL ,
 [ContractId]  int NOT NULL ,
 [FieldId]     int NOT NULL ,
 [Value]       nvarchar(1000) NULL ,
 [CreatedDate] datetime2(7) NOT NULL ,

 CONSTRAINT [PK_DataFields] PRIMARY KEY CLUSTERED ([DataFieldId] ASC),
 CONSTRAINT [FK_213] FOREIGN KEY ([ContractId])  REFERENCES [dbo].[WorkContracts]([ContractId])
);
GO


CREATE NONCLUSTERED INDEX [fkIdx_213] ON [dbo].[DataFields] 
 (
  [ContractId] ASC
 )

GO







