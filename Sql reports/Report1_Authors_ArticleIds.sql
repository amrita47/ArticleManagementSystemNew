
SELECT
 u.Id AS AuthorId, u.Username AS AuthorName,
COALESCE('[' + STRING_AGG(CAST(x.ArticleId AS VARCHAR(MAX)), ',') + ']',  '[]'  ) AS ArticleIds
FROM Users u
LEFT JOIN (SELECT DISTINCT Author, ArticleId FROM Contents) x ON x.Author = u.Id
GROUP BY u.Id, u.Username
ORDER BY u.Username;