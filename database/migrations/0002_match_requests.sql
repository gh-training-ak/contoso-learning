CREATE TABLE learning.MatchRequest
(
    MatchRequestId  UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_MatchRequest PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StudentId       UNIQUEIDENTIFIER NOT NULL,
    MentorId        UNIQUEIDENTIFIER NOT NULL,
    Subject         TINYINT          NOT NULL,
    Status          TINYINT          NOT NULL CONSTRAINT DF_MatchRequest_Status DEFAULT 0,
    Message         NVARCHAR(1000)   NULL,
    DeclineReason   NVARCHAR(400)    NULL,
    RequestedAtUtc  DATETIME2(3)     NOT NULL CONSTRAINT DF_MatchRequest_Requested DEFAULT SYSUTCDATETIME(),
    ResolvedAtUtc   DATETIME2(3)     NULL,

    CONSTRAINT FK_MatchRequest_Student FOREIGN KEY (StudentId) REFERENCES learning.Student (StudentId),
    CONSTRAINT FK_MatchRequest_Mentor  FOREIGN KEY (MentorId)  REFERENCES learning.Mentor (MentorId),
    CONSTRAINT CK_MatchRequest_Resolved CHECK (
        (Status = 0 AND ResolvedAtUtc IS NULL) OR (Status <> 0 AND ResolvedAtUtc IS NOT NULL)
    )
);
GO

-- One open request per student and mentor pair. Filtered so history is unconstrained.
CREATE UNIQUE INDEX UX_MatchRequest_OnePending
    ON learning.MatchRequest (StudentId, MentorId)
    WHERE Status = 0;
GO

CREATE INDEX IX_MatchRequest_Mentor_Status
    ON learning.MatchRequest (MentorId, Status)
    INCLUDE (RequestedAtUtc);
GO
