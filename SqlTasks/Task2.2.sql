SELECT
    c."ClientName",
    COUNT(cc."Id") AS ContactCount
FROM
    "clients" c
LEFT JOIN
    "ClientContacts" cc ON c."Id" = cc."ClientId"
GROUP BY
    c."ClientName"
HAVING
    COUNT(cc."Id") > 2
ORDER BY
    c."ClientName";