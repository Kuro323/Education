USE BasketballDB;
GO

-- Заполняем позиции
INSERT INTO Positions(PositionName) VALUES 
(N'Разыгрывающий защитник'),
(N'Атакующий защитник'),
(N'Легкий форвард'),
(N'Тяжелый форвард'),
(N'Центровой');

-- Заполняем команды
DECLARE @Team1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Team2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Team3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Teams(TeamGuid, TeamName, Budget, FoundedYear) VALUES 
(@Team1, N'ЦСКА', 15000000.00, 1923),
(@Team2, N'Зенит', 12000000.50, 2014),
(@Team3, N'УНИКС', 9500000.00, 1991);

-- Заполняем игроков
INSERT INTO Players(FirstName, LastName, BirthDate, IsActive, TeamGuid, PositionId) VALUES 
(N'Алексей', N'Швед', '1988-12-16', 1, @Team1, 2), -- ЦСКА, Атакующий защитник
(N'Никита', N'Курбанов', '1986-10-05', 1, @Team1, 3), -- ЦСКА, Легкий форвард
(N'Андрей', N'Зубков', '1991-06-29', 1, @Team2, 4), -- Зенит, Тяжелый форвард
(N'Сергей', N'Карасев', '1993-10-26', 1, @Team2, 2), -- Зенит, Атакующий защитник
(N'Дмитрий', N'Кулагин', '1992-07-01', 1, @Team3, 1), -- УНИКС, Разыгрывающий
(N'Иван', N'Лазарев', '1991-01-31', 0, @Team3, 5); -- УНИКС, Завершил карьеру (IsActive = 0)

-- Заполняем матчи
INSERT INTO Matches(MatchDate, ArenaName, HomeTeamGuid, AwayTeamGuid) VALUES 
('2026-05-10 19:00:00', N'Мегаспорт Арена', @Team1, @Team2), -- ЦСКА против Зенита
('2026-05-15 18:30:00', N'Сибур Арена', @Team2, @Team3),    -- Зенит против УНИКСа
('2026-05-20 19:30:00', N'Баскет-холл', @Team3, @Team1);     -- УНИКС против ЦСКА

-- Заполняем статистику игроков в матчах
INSERT INTO PlayerMatchStats(PlayerId, MatchId, PointsScored, FoulsCount) VALUES 
(1, 1, 25, 2), -- Швед в матче 1 набрал 25 очков
(2, 1, 12, 4), -- Курбанов в матче 1 набрал 12 очков
(3, 1, 14, 3), -- Зубков в матче 1 набрал 14 очков
(4, 1, 18, 1), -- Карасев в матче 1 набрал 18 очков
(3, 2, 10, 2), -- Зубков в матче 2 набрал 10 очков
(5, 2, 22, 1); -- Кулагин в матче 2 набрал 22 очка
GO