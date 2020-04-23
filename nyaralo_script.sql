DECLARE @dbname nvarchar(128)
SET @dbname = N'Berles'

IF (NOT EXISTS (SELECT name 
			FROM master.dbo.sysdatabases 
			WHERE ('[' + name + ']' = @dbname 
			OR name = @dbname)))
	CREATE DATABASE Berles 
GO

USE Berles;  
GO  

IF OBJECT_ID (N'Berlesek', N'U') IS NOT NULL  
	DROP TABLE Berlesek;
IF OBJECT_ID (N'Berlok', N'U') IS NOT NULL  
	DROP TABLE Berlok;
IF OBJECT_ID (N'Felszerelesei', N'U') IS NOT NULL  
DROP TABLE Felszerelesei;
IF OBJECT_ID (N'Felszerelesek', N'U') IS NOT NULL  
	DROP TABLE Felszerelesek;
IF OBJECT_ID (N'Nyaralok', N'U') IS NOT NULL  
	DROP TABLE Nyaralok;
IF OBJECT_ID (N'Tulajdonosok', N'U') IS NOT NULL 
	DROP TABLE Tulajdonosok;
IF OBJECT_ID (N'Orszagok', N'U') IS NOT NULL 
	DROP TABLE Orszagok;
GO  

CREATE TABLE Tulajdonosok(
	TulajID INT IDENTITY NOT NULL,
	Nev VARCHAR(30),
	Cim VARCHAR(30),
	EmailCim VARCHAR(20)
);
ALTER TABLE Tulajdonosok
	ADD CONSTRAINT TulajID_PK PRIMARY KEY(TulajID)
ALTER TABLE Tulajdonosok
	ADD CONSTRAINT Email_unique UNIQUE(EmailCim)

CREATE TABLE Orszagok(
	OrszagID INT IDENTITY NOT NULL,
	OrszagNev VARCHAR(30),
);
ALTER TABLE Orszagok
	ADD CONSTRAINT OrszagID_PK PRIMARY KEY(OrszagID)
ALTER TABLE Orszagok
	ADD CONSTRAINT OrszagNev_unique UNIQUE(OrszagNev)

CREATE TABLE Nyaralok(
	NyaraloID INT IDENTITY NOT NULL,
	NyaraloNev VARCHAR(30),
	NyaraloCim VARCHAR (30),
	OrszagID INT,
	TulajID INT,
	Ferohely INT,
	Ar INT,
	Besorolas INT
);
ALTER TABLE Nyaralok
	ADD CONSTRAINT NyaraloID_PK PRIMARY KEY(NyaraloID)
ALTER TABLE Nyaralok
	ADD CONSTRAINT OrszagID_FK FOREIGN KEY(OrszagID) REFERENCES Orszagok (OrszagID)
ALTER TABLE Nyaralok
	ADD CONSTRAINT TulajID_FK FOREIGN KEY(TulajID) REFERENCES Tulajdonosok (TulajID)
ALTER TABLE Nyaralok
	ADD CONSTRAINT NyaraloNev_UNIQUE UNIQUE(NyaraloNev)
ALTER TABLE Nyaralok
	ADD CONSTRAINT Besorolas_CHECK CHECK (Besorolas BETWEEN 0 AND 5)
ALTER TABLE Nyaralok
	ADD CONSTRAINT Besorolas_DEFAULT DEFAULT 0 FOR Besorolas
ALTER TABLE Nyaralok
	ADD CONSTRAINT Ferohely_CHECK CHECK (Ferohely>=0)
ALTER TABLE Nyaralok
	ADD CONSTRAINT Ar_CHECK CHECK (Ar>=0)

CREATE TABLE Felszerelesek(
	FelszerelesID INT IDENTITY NOT NULL,
	FelszerelesNev VARCHAR(30),
);
ALTER TABLE Felszerelesek
	ADD CONSTRAINT FelszerelesID_PK PRIMARY KEY(FelszerelesID)
ALTER TABLE Felszerelesek
	ADD CONSTRAINT FelszerelesNev_UNIQUE UNIQUE(FelszerelesNev)

CREATE TABLE Felszerelesei(
	NyaraloID INT NOT NULL,
	FelszerelesID INT NOT NULL
);
ALTER TABLE Felszerelesei
	ADD CONSTRAINT NyaraloID_FK FOREIGN KEY(NyaraloID) REFERENCES Nyaralok(NyaraloID)
ALTER TABLE Felszerelesei
	ADD CONSTRAINT FelszerelesID_FK FOREIGN KEY(FelszerelesID) REFERENCES Felszerelesek (FelszerelesID)
ALTER TABLE Felszerelesei
	ADD CONSTRAINT FNID_PK PRIMARY KEY(NyaraloID,FelszerelesID)

CREATE TABLE Berlok(
	BerloID INT IDENTITY NOT NULL,
	BerloNev VARCHAR(30),
	TfSzam NCHAR(13)
);
ALTER TABLE Berlok
	ADD CONSTRAINT BerloID_FK PRIMARY KEY (BerloID)

CREATE TABLE Berlesek(
	BerlesID INT IDENTITY NOT NULL,
	NyaraloID INT NOT NULL,
	BerloID INT NOT NULL,
	KezdoDatum DATETIME,
	IdoTartam INT,
	Ertek INT 
);
ALTER TABLE Berlesek
	ADD CONSTRAINT BerlesID PRIMARY KEY (BerlesID)
ALTER TABLE Berlesek
	ADD CONSTRAINT NyaraloID_B_FK FOREIGN KEY(NyaraloID) REFERENCES Nyaralok(NyaraloID)
ALTER TABLE Berlesek
	ADD CONSTRAINT BerloID_B_FK FOREIGN KEY(BerloID) REFERENCES Berlok(BerloID)
ALTER TABLE Berlesek
	ADD CONSTRAINT Ertek_Default DEFAULT 0 FOR Ertek
GO


INSERT INTO Orszagok VALUES ('Romania'),
							('Magyarorszag'),
							('Ausztria'),
							('Olaszorszag'),
							('Belgium'),
							('Malta')

INSERT INTO Tulajdonosok VALUES ('Antal Attila', NULL, 'attila@email.com'),
								('Kelemen Melinda', NULL, 'melinda@email.com'),
								('Gergely Emese', NULL, 'emese@email.com'),
								('Kovacs Annamaria', NULL, 'anna@email.com'),
								('Horvath Levente', NULL, 'levente@email.com'),
								('Bereczki Gergely', NULL, 'gergely@email.com'),
								('Incze Istvan', NULL, 'istvan@email.com')

INSERT INTO Felszerelesek VALUES ('szauna'),
								 ('medence'),
								 ('jacuzzi'),
								 ('tv'),
								 ('terasz'),
								 ('teniszpalya'),
								 ('grillsuto')

INSERT INTO Nyaralok VALUES ('Wolfgangsee', null, 3, 2, 4, 25, 5),
							('Lous Escartoun', null, 4, 2, 2, 20, 3),
							('Budapest Holidays Fashion', null, 2, 4, 4, 31, 5),
							('Hi5 Apartments', null, 2, 1, 3, 20, 5),
							('Apartmenthaus Alpen Diamond', null, 3, 1, 4, 12, 2),
							('Green Resort Bran', null, 1, 5, 12, 100, 3),
							('Tulip Inn Venice', null, 4, 3, 4, 12, 5),
							('Pike Inn Dunavatul de Sus', null, 1, 5, 2, 20, 4),
							('Nola Naples', null, 4, 3, 4, 12, 1),
							('Aurius Apartments Szeged', null, 2, 2, 5, 30, 3),
							('Medieval Inn', null, 1, 1, 6, 25, 2),
							('Berekside', null, 2, 1, 8, 35, 2),
							('Herrenhaus Gut Bliestorf', null, 3, 1, 2, 10, 4),
							('Vila de la mare', null, 1, 1, 16, 50, 1),
							('Maas & Mechelen', null, 5, 1, 20, 50, 5),
							('Valetta Inn', null, 6, 1, 4, 15, 3)

INSERT INTO Felszerelesei VALUES(1, 2),	(1, 1), (1, 3),
								(2, 1),
								(3, 2),
								(4, 1), (4, 2), (4, 3), (4, 4), (4, 5), (4, 6), (4, 7),
								(5, 5),
								(6, 3)