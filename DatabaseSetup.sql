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
	[Description] NVARCHAR(30)
);

CREATE TABLE Users(
	Id INT IDENTITY(1, 1) PRIMARY KEY,
	FirstName NVARCHAR(50),
	LastName NVARCHAR(50),
	Username NVARCHAR(100) UNIQUE,
	Email NVARCHAR(128) UNIQUE,
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

CREATE TABLE Privileges (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	[Description] NVARCHAR(30),
);

CREATE TABLE RolesToPrivileges (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	RoleId INT,
	PrivilegeId INT
	FOREIGN KEY (RoleId) REFERENCES Roles(Id),
	FOREIGN KEY (PrivilegeId) REFERENCES Privileges(Id)
);

INSERT INTO Roles VALUES ('Public'), ('AcademicUser'), ('Librarian');
INSERT INTO Privileges VALUES ('Manage Categories'), ('Academic User');
INSERT INTO RolesToPrivileges VALUES (2, 2), (3, 2), (3, 1);

INSERT INTO AttributeTypes VALUES ('Number'), ('Text'), ('True/False'), ('Date');

INSERT INTO Users VALUES ('Matthew', 'Spiteri', 'Spim04', 'matthewspiteri@gmail.com', 'matt04', 1);
INSERT INTO Users VALUES ('Gorg', 'Borg', 'Gborg', 'gorgborg@gmail.com', 'gb05', 2);
INSERT INTO Users VALUES ('Chris', 'Calleja', 'Cc04', 'chriscalleja@gmail.com', 'chris04', 3);