USE BasketballDB;
GO

-- Выборка с фильтрацией(Игрок действующий и родился после 1 января 1990 года) и сортировкой(по фамилии)
SELECT FirstName, LastName, BirthDate FROM Players WHERE IsActive = 1 AND BirthDate > '1990-01-01' ORDER BY LastName ASC;

-- Изменение данных(увеличение бюджета клуба)
UPDATE Teams SET Budget = Budget + 500000.00 WHERE TeamName = N'УНИКС';

--Удаление данных из статистики матчей, где у игрока фолов больше 3
DELETE FROM PlayerMatchStats WHERE FoulsCount > 3;

--Выбор данных с группировкой
SELECT PlayerId, SUM(PointsScored) AS TotalPoints, AVG(CAST(FoulsCount AS FLOAT)) AS AverageFouls FROM PlayerMatchStats GROUP BY PlayerId;

--Вывод всех игроков и названия их игровых позиций.
SELECT p.FirstName, p.LastName, pos.PositionName FROM Players p LEFT JOIN Positions pos ON p.PositionId = pos.PositionId;

--Вывод всех существующих позиций из справочника и игроков, которые на них играют.
SELECT pos.PositionName, p.FirstName, p.LastName FROM Players p RIGHT JOIN Positions pos ON p.PositionId = pos.PositionId;

--Вывод полной связанной информацию: Имя игрока, Название его команды и Название позиции.
SELECT p.FirstName, p.LastName, t.TeamName, pos.PositionName FROM Players p INNER JOIN Teams t ON p.TeamGuid = t.TeamGuid INNER JOIN Positions pos ON p.PositionId = pos.PositionId;