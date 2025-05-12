USE MASTER;
if exists (select * from sys.databases where name = 'EventEaseDB')
DROP DATABASE EventEaseDB;
CREATE DATABASE EventEaseDB;
USE EventEaseDB;

DROP TABLE IF EXISTS Venues;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS Bookings;

CREATE TABLE Venues (
    VenueId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
	ImageUrl NVARCHAR(MAX)
);

CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
	Description NVARCHAR(MAX) NOT NULL,
    VenueId INT FOREIGN KEY REFERENCES Venues(VenueId) 
);

CREATE TABLE Bookings (
    BookingsId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    BookingDate DATE NOT NULL,
	EventId INT NOT NULL,
    VenueId INT NOT NULL
	CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Bookings_Venues FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
); 


INSERT INTO Venues (VenueName, Location, Capacity, ImageUrl)
VALUES ('Conference Hall A', 'City Center', 200, 'image.jpg'),
       ('Outdoor Park', 'Main Street', 500, 'image2.jpg');

INSERT INTO Events(EventName, EventDate, Description, VenueId)
VALUES ('Tech Summit 2025', '2025-06-15','description example', 1),
       ('Music Festival', '2025-07-20','description example2', 2);

INSERT INTO Bookings (BookingDate,VenueId, EventId)
VALUES ('2024-05-16',1, 1),
       ('2025-01-20',2, 2);
	   
SELECT * FROM Venues;
SELECT * FROM Events;
SELECT * FROM Bookings;

