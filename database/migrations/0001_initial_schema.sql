CREATE SCHEMA learning;
GO

CREATE TABLE learning.Mentor
(
    MentorId        UNIQUEIDENTIFIER    NOT NULL CONSTRAINT PK_Mentor PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DisplayName     NVARCHAR(120)       NOT NULL,
    Headline        NVARCHAR(400)       NULL,
    HourlyRate      DECIMAL(10, 2)      NOT NULL,
    Currency        CHAR(3)             NOT NULL CONSTRAINT DF_Mentor_Currency DEFAULT 'GBP',
    MeetingType     TINYINT             NOT NULL,
    Line1           NVARCHAR(200)       NOT NULL,
    City            NVARCHAR(100)       NOT NULL,
    Postcode        NVARCHAR(16)        NOT NULL,
    Latitude        FLOAT               NOT NULL,
    Longitude       FLOAT               NOT NULL,
    IsPublished     BIT                 NOT NULL CONSTRAINT DF_Mentor_Published DEFAULT 0,
    CreatedAtUtc    DATETIME2(3)        NOT NULL CONSTRAINT DF_Mentor_Created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Mentor_Rate      CHECK (HourlyRate >= 0),
    CONSTRAINT CK_Mentor_Latitude  CHECK (Latitude BETWEEN -90 AND 90),
    CONSTRAINT CK_Mentor_Longitude CHECK (Longitude BETWEEN -180 AND 180)
);
GO

CREATE TABLE learning.MentorSubject
(
    MentorId    UNIQUEIDENTIFIER NOT NULL,
    Subject     TINYINT          NOT NULL,

    CONSTRAINT PK_MentorSubject PRIMARY KEY (MentorId, Subject),
    CONSTRAINT FK_MentorSubject_Mentor FOREIGN KEY (MentorId)
        REFERENCES learning.Mentor (MentorId) ON DELETE CASCADE
);
GO

CREATE TABLE learning.Student
(
    StudentId       UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Student PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DisplayName     NVARCHAR(120)    NOT NULL,
    Email           NVARCHAR(256)    NOT NULL,
    YearGroup       TINYINT          NOT NULL,
    CreatedAtUtc    DATETIME2(3)     NOT NULL CONSTRAINT DF_Student_Created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_Student_Email UNIQUE (Email),
    CONSTRAINT CK_Student_Year  CHECK (YearGroup BETWEEN 7 AND 13)
);
GO

CREATE INDEX IX_Mentor_Published_Rate
    ON learning.Mentor (IsPublished, HourlyRate)
    INCLUDE (DisplayName, Currency, City);
GO
