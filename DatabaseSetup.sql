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
	Salt NVARCHAR(32),
	RoleId INT
	FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Categories(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	PublicAccess INT,
	[Name] VARCHAR(50)
);

CREATE TABLE AttributeTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [TypeName] NVARCHAR(50),
	[TagName] NVARCHAR(50),
);

CREATE TABLE Attributes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50),
	[TypeId] INT
	FOREIGN KEY (TypeId) REFERENCES AttributeTypes(Id)
);

CREATE TABLE CategoryAttributes(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	CategoryId INT,
	AttributeId INT,
	FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
	FOREIGN KEY (AttributeId) REFERENCES Attributes(Id)

);

CREATE TABLE Languages (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	[Language] NVARCHAR(30)
);

CREATE TABLE Documents(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT,
	CategoryId INT,
	Title NVARCHAR(100),
	LanguageId INT,
	UploadDate DATETIME,
	PublicAccess INT,
	DocumentLocation NVARCHAR(MAX),
	FileExtension NVARCHAR(10),
	FOREIGN KEY (UserId) REFERENCES Users(Id),
	FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
	FOREIGN KEY (LanguageId) REFERENCES Languages(Id)
);

CREATE TABLE DocumentAttributes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	DocumentID INT,
    AttributeID INT,
	[Value] NVARCHAR(MAX),
	FOREIGN KEY (DocumentId) REFERENCES Documents(Id),
	FOREIGN KEY (AttributeId) REFERENCES Attributes(Id)
);

CREATE TABLE Favourites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    DocumentId INT
	FOREIGN KEY (UserId) REFERENCES Users(Id),
	FOREIGN KEY (DocumentId) REFERENCES Documents(Id)
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

CREATE TABLE HelpDetails (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Question Nvarchar(500),
	AnswerText Nvarchar(500)
);


INSERT INTO Roles VALUES ('Public'), ('Student'), ('Lecturer'), ('Librarian'), ('Master Librarian');
INSERT INTO Privileges VALUES ('Manage Categories'), ('Upload Files'), ('Handle No ID Documents'), ('Delete Documents'), ('Academic User');
INSERT INTO RolesToPrivileges VALUES (2, 2), (2, 5), (4, 2), (4, 5), (4, 1), (5, 1), (5,2), (5,3), (5,4), (5, 5), (3, 2), (3, 5) ;

INSERT INTO Languages VALUES  ('Malti'), ('English'), ('Italiano'), ('Español'), ('Français'), ('Deutsch'), ('Português');
INSERT INTO AttributeTypes VALUES ('Number', 'number'), ('Text', 'text'), ('True/False', 'checkbox'), ('Date', 'date');



INSERT INTO Users VALUES ('Anakin', 'Skywalker', 'dvader', 'matthewspiteri04@gmail.com', '80457CF3A7B15AFB8F491F8AE06680DB', 5);

INSERT INTO HelpDetails VALUES ('What is the client portal and how do I use it', '1234'), ('How do I download resources', 'Test');


INSERT INTO Attributes VALUES ('ISBN', 2), ('Author', 2), ('Publish Date', 4), ('Publisher', 2), ('Genre', 2), ('Bestseller', 3), ('Edition', 1), ('Pages', 1), ('Supervisor', 2), ('Faculty', 2), ('Degree Program', 2),  ('ISSN', 2), ('Duration', 1);


INSERT INTO Categories VALUES (1, 'Book');
INSERT INTO CategoryAttributes VALUES (1,1), (1,2), (1,3), (1,4), (1,5), (1,6), (1,7), (1,8);

INSERT INTO Categories VALUES (1, 'FYP');
INSERT INTO CategoryAttributes VALUES (2,2), (2,3), (2,8), (2,9), (2,10), (2,11);

INSERT INTO Categories VALUES (1, 'Article');
INSERT INTO CategoryAttributes VALUES (3,2), (3,3), (3,4), (3,5), (3,8);

INSERT INTO Categories VALUES (1, 'Journal');
INSERT INTO CategoryAttributes VALUES (4,2), (4,3), (4,4), (4,5), (4,8), (4,12);

INSERT INTO Categories VALUES (1, 'Thesis');
INSERT INTO CategoryAttributes VALUES (5,2), (5,3), (5,8), (5,9), (5,10), (5,11);

INSERT INTO Categories VALUES (1, 'Audio Book');
INSERT INTO CategoryAttributes VALUES (6,2), (6,3), (6,4), (6,5), (6,6), (6,7), (6,13);

INSERT INTO Users VALUES ('Anakin', 'Skywalker', 'dvader', 'matthewspiteri04@gmail.com', '80457CF3A7B15AFB8F491F8AE06680DB', 5);

INSERT INTO HelpDetails VALUES ('How do I Upload Documents', 'First Sign Up or go into the My Info Page. Under your details, scroll down and there are 3 buttons. One for Student Access, one for Lecturer Access, and another one for Librarian Access. Choose accordingly depending on your status. Once approved, you will have the ability to upload documents. There will be a section named upload on the sidebar to the left. Fill in the sections accordingly and press "Upload File"');
INSERT INTO HelpDetails VALUES ('How do I Download Resources', 'In the home section search for your desired document. Press on the second button and the document will download.');
INSERT INTO HelpDetails VALUES ('How to Add a Category', 'Only Librarians can add categories. If you are a Librarian, press categories on the left sidebar, press add category, give it a name and add fields accordingly.');
INSERT INTO HelpDetails VALUES ('How to Use the Advanced Filter', 'On the home screen, next to the search bar there is a filter button. One can search by Author, Language or Category. Once the wanted filters are selected, press the advanced search button and there will be search results based on the filters given.');
INSERT INTO HelpDetails VALUES ('How to Edit Account', 'Press the security tab in the sidebar. Press on edit account and change accordingly. After the changes are made, confrim by pressing save changes. To change the password, press on change password, enter your current password, and enter the new password. Confirm the password and press save changes.');
INSERT INTO HelpDetails VALUES ('How to Edit Upload', 'Go to the My Info tab in the sidebar on the left. Scroll down and press My Uploads. Choose the desired document and press on the first button on the right hand side. Scroll down and press edit. Change the fields accordingly and press save.');

