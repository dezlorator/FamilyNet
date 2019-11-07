use FamilyNetLogs

GO
Create Table [Log](
	Id INT PRIMARY KEY IDENTITY (1, 1),
	Logged Datetime default GETDATE(),
    [Level] VARCHAR (256),
    [Message] VARCHAR (256),
    Logger NVARCHAR(256),
	CallSite NVARCHAR(256),
	Exception TEXT
);
GO

ALTER TABLE [Log] ADD UserId NVARCHAR(50)
ALTER TABLE [Log] ADD JSON text