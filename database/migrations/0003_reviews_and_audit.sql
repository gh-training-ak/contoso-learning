CREATE TABLE learning.Review
(
    ReviewId        UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Review PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    MentorId        UNIQUEIDENTIFIER NOT NULL,
    StudentId       UNIQUEIDENTIFIER NOT NULL,
    Rating          TINYINT          NOT NULL,
    Comment         NVARCHAR(2000)   NULL,
    SubmittedAtUtc  DATETIME2(3)     NOT NULL CONSTRAINT DF_Review_Submitted DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Review_Mentor  FOREIGN KEY (MentorId)  REFERENCES learning.Mentor (MentorId),
    CONSTRAINT FK_Review_Student FOREIGN KEY (StudentId) REFERENCES learning.Student (StudentId),
    CONSTRAINT CK_Review_Rating  CHECK (Rating BETWEEN 1 AND 5)
);
GO

CREATE OR ALTER VIEW learning.MentorRating
AS
SELECT
    m.MentorId,
    m.DisplayName,
    COUNT(r.ReviewId)              AS ReviewCount,
    AVG(CAST(r.Rating AS DECIMAL(4, 2))) AS AverageRating
FROM learning.Mentor AS m
LEFT JOIN learning.Review AS r ON r.MentorId = m.MentorId
GROUP BY m.MentorId, m.DisplayName;
GO
