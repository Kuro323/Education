using education.Models;

namespace education
{
    /// <summary>
    /// Репозиторий для работы с базой данных баскетбольной лиги через Entity Framework Core.
    /// Реализует CRUD-операции с использованием контекста <see cref="BasketballDbContext"/>.
    /// </summary>
    public static class EFCoreRepository
    {
        /// <summary>
        /// Добавляет новую игровую позицию в базу данных.
        /// </summary>
        /// <param name="positionName">Название позиции (например, "Разыгрывающий защитник").</param>
        public static void CreatePosition(string positionName)
        {
            using (var db = new BasketballDbContext())
            {
                var position = new Position { PositionName = positionName };
                db.Positions.Add(position);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Возвращает текстовую информацию о позиции по её идентификатору.
        /// </summary>
        /// <param name="positionId">Идентификатор позиции.</param>
        /// <returns>Строка с ID и названием позиции.</returns>
        /// <exception cref="Exception">Выбрасывается, если позиция с указанным ID не найдена в БД.</exception>
        public static string ReadPosition(int positionId)
        {
            using (var db = new BasketballDbContext())
            {
                var position = db.Positions.Find(positionId);

                if (position == null)
                {
                    throw new Exception($"Позиция с id = {positionId} не найдена в бд.");
                }

                return $"Позиция с id = {position.PositionId}: Название - {position.PositionName}";
            }
        }

        /// <summary>
        /// Получает список всех позиций, зарегистрированных в базе данных.
        /// </summary>
        /// <returns>Список форматированных строк с данными позиций.</returns>
        public static List<string> ReadPositions()
        {
            using (var db = new BasketballDbContext())
            {
                var positions = new List<string>();

                foreach (var pos in db.Positions)
                {
                    positions.Add($"Позиция с id = {pos.PositionId}: Название - {pos.PositionName}");
                }

                return positions;
            }
        }

        /// <summary>
        /// Обновляет название существующей позиции.
        /// </summary>
        /// <param name="positionId">Идентификатор обновляемой позиции.</param>
        /// <param name="positionName">Новое название позиции.</param>
        public static void UpdatePosition(int positionId, string positionName)
        {
            using (var db = new BasketballDbContext())
            {
                var position = db.Positions.Find(positionId);
                if (position != null)
                {
                    position.PositionName = positionName;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет позицию из базы данных по её идентификатору.
        /// </summary>
        /// <param name="positionId">Идентификатор удаляемой позиции.</param>
        public static void DeletePosition(int positionId)
        {
            using (var db = new BasketballDbContext())
            {
                var position = db.Positions.Find(positionId);
                if (position != null)
                {
                    db.Positions.Remove(position);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Создает новую команду с указанными параметрами.
        /// </summary>
        /// <param name="teamGuid">Уникальный строковый GUID команды.</param>
        /// <param name="teamName">Название команды.</param>
        /// <param name="budget">Бюджет команды.</param>
        /// <param name="foundedYear">Год основания команды.</param>
        public static void CreateTeam(string teamGuid, string teamName, decimal budget, int foundedYear)
        {
            using (var db = new BasketballDbContext())
            {
                var team = new Team
                {
                    TeamGuid = Guid.Parse(teamGuid),
                    TeamName = teamName,
                    Budget = budget,
                    FoundedYear = foundedYear
                };
                db.Teams.Add(team);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Получает информацию о команде по её уникальному GUID.
        /// </summary>
        /// <param name="teamGuid">Строковое представление GUID команды.</param>
        /// <returns>Строка с подробным описанием команды.</returns>
        /// <exception cref="Exception">Выбрасывается, если команда с указанным GUID не найдена.</exception>
        public static string ReadTeam(string teamGuid)
        {
            using (var db = new BasketballDbContext())
            {
                var team = db.Teams.Find(teamGuid);

                if (team == null)
                {
                    throw new Exception($"Команда с guid = {teamGuid} не найдена в бд.");
                }

                return $"Команда с guid = {team.TeamGuid}: Название - {team.TeamName}\nБюджет - {team.Budget}\nГод основания - {team.FoundedYear}";
            }
        }

        /// <summary>
        /// Получает список всех команд в базе данных.
        /// </summary>
        /// <returns>Список строк с описанием каждой команды.</returns>
        public static List<string> ReadTeams()
        {
            using (var db = new BasketballDbContext())
            {
                var teams = new List<string>();

                foreach (var team in db.Teams)
                {
                    teams.Add($"Команда с guid = {team.TeamGuid}: Название - {team.TeamName}, Бюджет - {team.Budget}, Год основания - {team.FoundedYear}");
                }

                return teams;
            }
        }

        /// <summary>
        /// Обновляет данные существующей команды по её GUID.
        /// </summary>
        /// <param name="teamGuid">Строковое представление GUID команды.</param>
        /// <param name="teamName">Новое название команды.</param>
        /// <param name="budget">Новый бюджет.</param>
        /// <param name="foundedYear">Новый год основания.</param>
        public static void UpdateTeam(string teamGuid, string teamName, decimal budget, int foundedYear)
        {
            using (var db = new BasketballDbContext())
            {
                var team = db.Teams.Find(teamGuid);
                if (team != null)
                {
                    team.TeamName = teamName;
                    team.Budget = budget;
                    team.FoundedYear = foundedYear;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет команду по её уникальному GUID.
        /// </summary>
        /// <param name="teamGuid">Строковое представление GUID удаляемой команды.</param>
        public static void DeleteTeam(string teamGuid)
        {
            using (var db = new BasketballDbContext())
            {
                var team = db.Teams.Find(teamGuid);
                if (team != null)
                {
                    db.Teams.Remove(team);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Добавляет нового игрока в базу данных.
        /// </summary>
        /// <param name="firstName">Имя игрока.</param>
        /// <param name="lastName">Фамилия игрока.</param>
        /// <param name="birthDate">Дата рождения.</param>
        /// <param name="isActive">Статус активности игрока (в составе/активен).</param>
        /// <param name="teamGuid">Строковое представление GUID команды игрока.</param>
        /// <param name="positionId">Идентификатор позиции игрока.</param>
        public static void CreatePlayer(string firstName, string lastName, DateTime birthDate, bool isActive, string teamGuid, int positionId)
        {
            using (var db = new BasketballDbContext())
            {
                var player = new Player
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate,
                    IsActive = isActive,
                    TeamGuid = Guid.Parse(teamGuid),
                    PositionId = positionId
                };
                db.Players.Add(player);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Возвращает анкетные данные игрока по его уникальному идентификатору.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <returns>Текстовая строка с подробной информацией об игроке.</returns>
        /// <exception cref="Exception">Выбрасывается, если игрок с указанным ID отсутствует в БД.</exception>
        public static string ReadPlayer(int playerId)
        {
            using (var db = new BasketballDbContext())
            {
                var player = db.Players.Find(playerId);

                if (player == null)
                {
                    throw new Exception($"Игрок с id = {playerId} не найден в бд.");
                }

                return $"Игрок с id = {player.PlayerId}: Имя - {player.FirstName}\nФамилия - {player.LastName}\nДень рождения - {player.BirthDate}\nВ заявке - {player.IsActive}\nGuid команды - {player.TeamGuid}\nID позиции - {player.PositionId}";
            }
        }

        /// <summary>
        /// Получает сводный список всех зарегистрированных игроков.
        /// </summary>
        /// <returns>Список форматированных строк с профилями игроков.</returns>
        public static List<string> ReadPlayers()
        {
            using (var db = new BasketballDbContext())
            {
                var players = new List<string>();

                foreach (var player in db.Players)
                {
                    players.Add($"Игрок с id = {player.PlayerId}: Имя - {player.FirstName}, Фамилия - {player.LastName}, День рождения - {player.BirthDate}, В заявке - {player.IsActive}, Guid команды - {player.TeamGuid}, ID позиции - {player.PositionId}");
                }

                return players;
            }
        }

        /// <summary>
        /// Обновляет персональные, контрактные и игровые данные игрока.
        /// </summary>
        /// <param name="firstName">Новое имя.</param>
        /// <param name="lastName">Новая фамилия.</param>
        /// <param name="birthDate">Новая дата рождения.</param>
        /// <param name="isActive">Новый статус активности.</param>
        /// <param name="teamGuid">Новый GUID привязанной команды.</param>
        /// <param name="positionId">Новый ID позиции.</param>
        /// <param name="playerId">Идентификатор обновляемого игрока.</param>
        public static void UpdatePlayers(string firstName, string lastName, DateTime birthDate, bool isActive, string teamGuid, int positionId, int playerId)
        {
            using (var db = new BasketballDbContext())
            {
                var player = db.Players.Find(playerId);
                if (player != null)
                {
                    player.FirstName = firstName;
                    player.LastName = lastName;
                    player.BirthDate = birthDate;
                    player.IsActive = isActive;
                    player.TeamGuid = Guid.Parse(teamGuid);
                    player.PositionId = positionId;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет игрока из базы данных по его идентификатору.
        /// </summary>
        /// <param name="playerId">Идентификатор удаляемого игрока.</param>
        public static void DeletePlayer(int playerId)
        {
            using (var db = new BasketballDbContext())
            {
                var player = db.Players.Find(playerId);
                if (player != null)
                {
                    db.Players.Remove(player);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Создает запись о матче между двумя командами.
        /// </summary>
        /// <param name="matchDate">Дата и время начала матча.</param>
        /// <param name="arenaName">Название стадиона / арены.</param>
        /// <param name="homeTeamGuid">Строковое представление GUID домашней команды.</param>
        /// <param name="awayTeamGuid">Строковое представление GUID гостевой команды.</param>
        public static void CreateMatch(DateTime matchDate, string arenaName, string homeTeamGuid, string awayTeamGuid)
        {
            using (var db = new BasketballDbContext())
            {
                var match = new Match
                {
                    MatchDate = matchDate,
                    ArenaName = arenaName,
                    HomeTeamGuid = Guid.Parse(homeTeamGuid),
                    AwayTeamGuid = Guid.Parse(awayTeamGuid)
                };
                db.Matches.Add(match);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Возвращает информацию о матче по его идентификатору.
        /// </summary>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <returns>Строка с информацией о дате, месте проведения и командах.</returns>
        /// <exception cref="Exception">Выбрасывается, если матч с указанным ID не найден.</exception>
        public static string ReadMatch(int matchId)
        {
            using (var db = new BasketballDbContext())
            {
                var match = db.Matches.Find(matchId);

                if (match == null)
                {
                    throw new Exception($"Матч с id = {matchId} не найден в бд.");
                }

                return $"Матч с id = {match.MatchId}: Дата - {match.MatchDate}\nАрена - {match.ArenaName}\nХозяева (Guid) - {match.HomeTeamGuid}\nГости (Guid) - {match.AwayTeamGuid}";
            }
        }

        /// <summary>
        /// Получает список всех запланированных или прошедших матчей.
        /// </summary>
        /// <returns>Список строк с описанием параметров каждого матча.</returns>
        public static List<string> ReadMatches()
        {
            using (var db = new BasketballDbContext())
            {
                var matches = new List<string>();

                foreach (var match in db.Matches)
                {
                    matches.Add($"Матч с id = {match.MatchId}: Дата - {match.MatchDate}, Арена - {match.ArenaName}, Хозяева (Guid) - {match.HomeTeamGuid}, Гости (Guid) - {match.AwayTeamGuid}");
                }

                return matches;
            }
        }

        /// <summary>
        /// Обновляет параметры проведения существующего матча.
        /// </summary>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <param name="matchDate">Новая дата и время матча.</param>
        /// <param name="arenaName">Новое название арены.</param>
        /// <param name="homeTeamGuid">Новый GUID домашней команды.</param>
        /// <param name="awayTeamGuid">Новый GUID гостевой команды.</param>
        public static void UpdateMatch(int matchId, DateTime matchDate, string arenaName, string homeTeamGuid, string awayTeamGuid)
        {
            using (var db = new BasketballDbContext())
            {
                var match = db.Matches.Find(matchId);
                if (match != null)
                {
                    match.MatchDate = matchDate;
                    match.ArenaName = arenaName;
                    match.HomeTeamGuid = Guid.Parse(homeTeamGuid);
                    match.AwayTeamGuid = Guid.Parse(awayTeamGuid);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет матч по его идентификатору.
        /// </summary>
        /// <param name="matchId">Идентификатор удаляемого матча.</param>
        public static void DeleteMatch(int matchId)
        {
            using (var db = new BasketballDbContext())
            {
                var match = db.Matches.Find(matchId);
                if (match != null)
                {
                    db.Matches.Remove(match);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Вносит статистические результаты (очки, фолы) игрока за определенный матч.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <param name="pointsScored">Количество набранных очков.</param>
        /// <param name="foulsCount">Количество совершенных фолов.</param>
        public static void CreatePlayerMatchStat(int playerId, int matchId, int pointsScored, int foulsCount)
        {
            using (var db = new BasketballDbContext())
            {
                var stat = new PlayerMatchStat
                {
                    PlayerId = playerId,
                    MatchId = matchId,
                    PointsScored = pointsScored,
                    FoulsCount = foulsCount
                };
                db.PlayerMatchStats.Add(stat);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Извлекает статистику конкретного игрока в конкретном матче по составному ключу.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <returns>Строка с результатами (очки, фолы).</returns>
        /// <exception cref="Exception">Выбрасывается, если запись статистики не найдена для указанных ID.</exception>
        public static string ReadPlayerMatchStat(int playerId, int matchId)
        {
            using (var db = new BasketballDbContext())
            {
                var stat = db.PlayerMatchStats.Find(playerId, matchId);

                if (stat == null)
                {
                    throw new Exception($"Статистика для игрока с id = {playerId} в матче с id = {matchId} не найдена.");
                }

                return $"Статистика игрока {stat.PlayerId} в матче {stat.MatchId}: Очки - {stat.PointsScored}\nФолы - {stat.FoulsCount}";
            }
        }

        /// <summary>
        /// Возвращает общую игровую статистику по всем матчам и игрокам.
        /// </summary>
        /// <returns>Список форматированных строк со сводной статистикой.</returns>
        public static List<string> ReadPlayerMatchStats()
        {
            using (var db = new BasketballDbContext())
            {
                var stats = new List<string>();

                foreach (var stat in db.PlayerMatchStats)
                {
                    stats.Add($"Игрок ID = {stat.PlayerId} в матче ID = {stat.MatchId}: Очки - {stat.PointsScored}, Фолы - {stat.FoulsCount}");
                }

                return stats;
            }
        }

        /// <summary>
        /// Обновляет набранные очки и фолы для существующей записи статистики.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <param name="pointsScored">Новое количество очков.</param>
        /// <param name="foulsCount">Новое количество фолов.</param>
        public static void UpdatePlayerMatchStat(int playerId, int matchId, int pointsScored, int foulsCount)
        {
            using (var db = new BasketballDbContext())
            {
                var stat = db.PlayerMatchStats.Find(playerId, matchId);
                if (stat != null)
                {
                    stat.PointsScored = pointsScored;
                    stat.FoulsCount = foulsCount;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет статистическую запись игрока за конкретный матч.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        public static void DeletePlayerMatchStat(int playerId, int matchId)
        {
            using (var db = new BasketballDbContext())
            {
                var stat = db.PlayerMatchStats.Find(playerId, matchId);
                if (stat != null)
                {
                    db.PlayerMatchStats.Remove(stat);
                    db.SaveChanges();
                }
            }
        }
    }
}