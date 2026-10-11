
USE ArticleManagementDB;
GO

DECLARE @CurrentDate DATETIME2 = GETDATE();
DECLARE @ArticleStartDate DATETIME2 =
 DATEADD(MONTH, -3, @CurrentDate);
DECLARE @UserStartDate DATETIME2 =
DATEADD(MONTH, -4, @CurrentDate);
DECLARE @Language VARCHAR(20) = 'English';

SELECT
 a.Id AS ArticleId, a.Status AS ArticleStatus, a.CreatedAt AS ArticleCreatedOn, c.Title, c.Language, u.Id AS AuthorId,u.Username AS AuthorName, u.CreatedAt AS UserCreatedOn
FROM Articles a
INNER JOIN dbo.Contents c
 ON c.ArticleId = a.Id
INNER JOIN Users u
 ON u.Id = c.Author
WHERE
 a.CreatedAt >= @ArticleStartDate
AND a.CreatedAt <= @CurrentDate
AND u.CreatedAt >= @UserStartDate
AND u.CreatedAt <= @CurrentDate
AND c.Language = @Language
ORDER BY a.CreatedAt DESC;
