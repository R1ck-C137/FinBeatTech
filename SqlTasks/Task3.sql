WITH Intervals AS(
SELECT
  "Id",
  "Dt" AS Sd,
LEAD("Dt") OVER (PARTITION BY "Id" ORDER BY "Dt") AS Ed
FROM
  task3
)
SELECT
  "Id",
  Sd,
  Ed
FROM
  Intervals
WHERE
  Ed IS NOT NULL
ORDER BY
  "Id", Sd;