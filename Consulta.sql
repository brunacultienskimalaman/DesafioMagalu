﻿SELECT TOP 5 * FROM USUARIOS

SELECT TOP 5 * FROM PEDIDOS

SELECT TOP 5 * FROM PRODUTOS



SELECT * FROM USUARIOS USR
INNER JOIN PEDIDOS PED ON PED.USUARIOID = USR.USERID
INNER JOIN PRODUTOS PROD ON PROD.PEDIDOID = PED.ORDERID
ORDER BY USR.UserID,PED.ORDERID