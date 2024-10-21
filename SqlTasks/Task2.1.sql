SELECT
    c."ClientName",
    COUNT(cc."Id") AS ContactCount
FROM
    "clients" c
LEFT JOIN
    "ClientContacts" cc ON c."Id" = cc."ClientId"
GROUP BY
    c."ClientName"
ORDER BY
    c."ClientName";

