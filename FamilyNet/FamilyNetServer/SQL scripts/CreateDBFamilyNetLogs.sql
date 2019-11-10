use FamilyNetLogs

GO
Create Table [Log](
	[Id] INT PRIMARY KEY IDENTITY (1, 1),
	[Logged] DATETIME DEFAULT GETDATE(),
    [Level] NVARCHAR (256),
	[Status] NVARCHAR(50),
	[UserId] NVARCHAR(50),
	[Token] TEXT,
	[Info] NVARCHAR(256),
	[JSON] TEXT,
    [Logger] NVARCHAR(256),
	[CallSite] NVARCHAR(256),
	[Exception] TEXT
)
GO