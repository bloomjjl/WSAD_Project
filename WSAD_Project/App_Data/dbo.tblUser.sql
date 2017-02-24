CREATE TABLE [dbo].[tblUser] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]    VARCHAR (400) NULL,
    [LastName]     VARCHAR (400) NULL,
    [EmailAddress] VARCHAR (400) NULL,
    [Username]     VARCHAR (400) NULL,
    [Password]     VARCHAR (400) NULL,
    [IsActive]     BIT           NULL,
    [IsAdmin]      BIT           NULL,
    [DateCreated]  DATETIME2 (7) NULL,
    [DateModified] DATETIME2 (7) NULL,
    [Gender]       VARCHAR (400) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);