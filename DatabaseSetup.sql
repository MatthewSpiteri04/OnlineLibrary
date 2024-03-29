USE [master];

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'OnlineLibrary')
BEGIN
    ALTER DATABASE [OnlineLibrary] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [OnlineLibrary];
END

CREATE DATABASE [OnlineLibrary];

USE [OnlineLibrary];

CREATE TABLE Roles(
	Id INT IDENTITY(1, 1) PRIMARY KEY,
	[Description] NVARCHAR(30),
	AcademicUser BIT,
	ManageCategories BIT 
);

CREATE TABLE Users(
	Id INT IDENTITY(1, 1) PRIMARY KEY,
	FirstName NVARCHAR(50),
	LastName NVARCHAR(50),
	Username NVARCHAR(100),
	Email NVARCHAR(128),
	[Password] NVARCHAR(100),
	RoleId INT
	FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE DocumentTypes(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	[Type] VARCHAR(50)
);

CREATE TABLE Documents(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	TypeId INT,
	Title NVARCHAR(100),
	[Language] NVARCHAR(30),
	UploadDate DATETIME,
	PublicAccess BIT,
	ImageLocation NVARCHAR(MAX),
	DocumentLocation NVARCHAR(MAX),
	FOREIGN KEY (TypeId) REFERENCES DocumentTypes(Id)
);

CREATE TABLE Favorites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    DocumentId INT
	FOREIGN KEY (UserId) REFERENCES Users(Id),
	FOREIGN KEY (DocumentId) REFERENCES Documents(Id)
);

CREATE TABLE AttributeTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [TypeName] NVARCHAR(50)
);

CREATE TABLE Attributes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50),
	[TypeId] INT
	FOREIGN KEY (TypeId) REFERENCES AttributeTypes(Id)
);

CREATE TABLE AttributeValues (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AttributeID INT,
    [Value] NVARCHAR(MAX),
    DocumentID INT,
	FOREIGN KEY (DocumentId) REFERENCES Documents(Id),
	FOREIGN KEY (AttributeId) REFERENCES Attributes(Id)
);

CREATE TABLE Contributors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Type NVARCHAR(50),
    Email NVARCHAR(100)
);

CREATE TABLE ContributorToDocument (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ContributorID INT,
    DocumentID INT
	FOREIGN KEY (DocumentId) REFERENCES Documents(Id),
	FOREIGN KEY (ContributorId) REFERENCES Contributors(Id)
);

INSERT INTO Roles VALUES ('Public', 0, 0), ('Student', 1, 0), ('Librarian', 1, 1);

INSERT INTO AttributeTypes VALUES ('Number'), ('Text'), ('True/False'), ('Date');