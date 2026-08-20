CREATE TABLE learning.ScheduleSlot
(
    ScheduleSlotId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ScheduleSlot PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    MentorId       UNIQUEIDENTIFIER NOT NULL,
    DayOfWeek      TINYINT          NOT NULL,
    StartsAt       TIME(0)          NOT NULL,
    EndsAt         TIME(0)          NOT NULL,

    CONSTRAINT FK_ScheduleSlot_Mentor FOREIGN KEY (MentorId)
        REFERENCES learning.Mentor (MentorId) ON DELETE CASCADE,
    CONSTRAINT CK_ScheduleSlot_Order CHECK (EndsAt > StartsAt),
    CONSTRAINT CK_ScheduleSlot_Day   CHECK (DayOfWeek BETWEEN 0 AND 6)
);
GO

CREATE INDEX IX_ScheduleSlot_Mentor_Day ON learning.ScheduleSlot (MentorId, DayOfWeek, StartsAt);
GO
