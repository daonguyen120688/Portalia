Use Portalia
GO

CREATE TABLE TrackingAction
(
	TrackingActionId UNIQUEIDENTIFIER DEFAULT (NEWID()) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	CONSTRAINT PK_TrackingAction PRIMARY KEY (TrackingActionId)
)

GO