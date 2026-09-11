SELECT
    u.FirstName + ' ' + u.LastName AS Participant,
    e.EventName,
    c.CategoryName,
    en.Status
FROM dbo.Enrolment en
INNER JOIN dbo.[User] u
    ON en.ParticipantID = u.UserID
INNER JOIN dbo.[Event] e
    ON en.EventID = e.EventID
INNER JOIN dbo.Category c
    ON en.EventID = c.EventID
    AND en.CategoryID = c.CategoryID;