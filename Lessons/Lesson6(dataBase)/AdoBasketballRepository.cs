using Microsoft.Data.SqlClient;

namespace education
{
    /// <summary>
    /// Репозиторий для работы с базой данных баскетбольной лиги через чистый ADO.NET.
    /// Обеспечивает выполнение CRUD-операций без использования ORM.
    /// </summary>
    static class AdoBasketballRepository
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        private static readonly string _connectionString;

        /// <summary>
        /// Конструктор для класса <see cref="AdoBasketballRepository"/>.
        /// Инициализирует строку подключения через конфигурационный хелпер.
        /// </summary>
        static AdoBasketballRepository()
        {
            _connectionString = ConfigurationHelper.GetConnectionString();
        }

        /// <summary>
        /// Добавляет новую игровую позицию в базу данных.
        /// </summary>
        /// <param name="name">Название позиции (например, "Центровой").</param>
        public static void CreatePosition(string name)
        {
            string query = "INSERT INTO Positions (PositionName) VALUES (@name)";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Создает новую команду в базе данных с автоматической генерацией уникального GUID.
        /// </summary>
        /// <param name="teamName">Название баскетбольной команды.</param>
        /// <param name="budget">Бюджет команды.</param>
        /// <param name="foundedYear">Год основания команды.</param>
        public static void CreateTeams(string teamName, float budget, int foundedYear)
        {
            string query = "INSERT INTO Teams (TeamGuid, TeamName, Budget, FoundedYear) VALUES (@guid, @teamName, @budget, @foundedYear)";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@guid", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@teamName", teamName);
                    cmd.Parameters.AddWithValue("@budget", Convert.ToDecimal(budget));
                    cmd.Parameters.AddWithValue("@foundedYear", foundedYear);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Добавляет нового игрока в базу данных с предварительной валидацией команды и позиции.
        /// </summary>
        /// <param name="firstName">Имя игрока.</param>
        /// <param name="lastName">Фамилия игрока.</param>
        /// <param name="birthDate">Дата рождения игрока.</param>
        /// <param name="isActive">Статус активности игрока (в заявке или нет).</param>
        /// <param name="teamName">Название команды, к которой привязывается игрок.</param>
        /// <param name="positionId">Идентификатор позиции игрока.</param>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <exception cref="Exception">
        /// Выбрасывается, если указанная команда не найдена, 
        /// отсутствуют позиции в БД или передан некорректный ID позиции.
        /// </exception>
        public static void CreatePlayers(string firstName, string lastName, DateTime birthDate, bool isActive, string teamName, int positionId)
        {
            string getGuidTeamQuery = "SELECT TeamGuid FROM Teams WHERE TeamName = @teamName";
            string insertPlayerQuery = "INSERT INTO Players (FirstName, LastName, BirthDate, IsActive, TeamGuid, PositionID) VALUES (@firstName, @lastName, @birthDate, @isActive, @teamGuid, @positionId)";
            string getCountPositionQuery = "SELECT COUNT(*) FROM Positions";

            string teamGuid = null;
            int countPosition = 0;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Проверка существования команды и получение её GUID.
                using (var cmdGetGuidTeam = new SqlCommand(getGuidTeamQuery, conn))
                {
                    cmdGetGuidTeam.Parameters.AddWithValue("@teamName", teamName);

                    var res = cmdGetGuidTeam.ExecuteScalar();

                    if (res == null || res == DBNull.Value)
                    {
                        throw new Exception($"Команда с названием {teamName} не найдена в бд.");
                    }

                    teamGuid = res.ToString();
                }

                // Проверка общего количества позиций.
                using (var cmdGetCountPosition = new SqlCommand(getCountPositionQuery, conn))
                {
                    countPosition = Convert.ToInt32(cmdGetCountPosition.ExecuteScalar());
                    if (countPosition == 0)
                    {
                        throw new Exception("Нет позиций для игроков. Перед добавлением игрока добавьте позицию.");
                    }
                }

                // Валидация: проверка корректности переданного ID позиции.
                if (positionId > countPosition || positionId <= 0)
                {
                    throw new Exception("Введенной позиции не существует в бд.");
                }

                // Вставка записи игрока.
                using (var cmdInsertPlayer = new SqlCommand(insertPlayerQuery, conn))
                {
                    cmdInsertPlayer.Parameters.AddWithValue("@firstName", firstName);
                    cmdInsertPlayer.Parameters.AddWithValue("@lastName", lastName);
                    cmdInsertPlayer.Parameters.AddWithValue("@birthDate", birthDate);
                    cmdInsertPlayer.Parameters.AddWithValue("@isActive", isActive);
                    cmdInsertPlayer.Parameters.AddWithValue("@teamGuid", teamGuid);
                    cmdInsertPlayer.Parameters.AddWithValue("@positionId", positionId);

                    cmdInsertPlayer.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Создает запись о проведении матча между двумя командами.
        /// </summary>
        /// <param name="matchDate">Дата и время проведения матча.</param>
        /// <param name="arenaName">Название спортивной арены.</param>
        /// <param name="teamNameHome">Название домашней команды.</param>
        /// <param name="teamNameAway">Название гостевой команды.</param>
        /// <exception cref="Exception">
        /// Выбрасывается, если названия команд совпадают или одна из команд не найдена в БД.
        /// </exception>
        public static void CreateMatches(DateTime matchDate, string arenaName, string teamNameHome, string teamNameAway)
        {
            // Валидация: команда не может играть против самой себя.
            if (teamNameAway == teamNameHome)
            {
                throw new Exception("Команда не может играть сама с собой. Введите разные названия команд.");
            }

            string getGuidTeamHome = "SELECT TeamGuid FROM Teams WHERE TeamName = @teamNameHome";
            string getGuidTeamAway = "SELECT TeamGuid FROM Teams WHERE TeamName = @teamNameAway";
            string insertMatch = "INSERT INTO Matches (MatchDate, ArenaName, HomeTeamGuid, AwayTeamGuid) VALUES (@matchDate, @arenaName, @homeTeamGuid, @awayTeamGuid)";

            string homeTeamGuid = null;
            string awayTeamGuid = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Получение GUID для домашней команды.
                using (var cmdGetGuidTeamHome = new SqlCommand(getGuidTeamHome, conn))
                {
                    cmdGetGuidTeamHome.Parameters.AddWithValue("@teamNameHome", teamNameHome);

                    var res = cmdGetGuidTeamHome.ExecuteScalar();

                    if (res == null || res == DBNull.Value)
                    {
                        throw new Exception("Домашней команды с таким именем не найдено в бд.");
                    }

                    homeTeamGuid = res.ToString();
                }

                // Получение GUID для гостевой команды.
                using (var cmdGetGuidTeamAway = new SqlCommand(getGuidTeamAway, conn))
                {
                    cmdGetGuidTeamAway.Parameters.AddWithValue("@teamNameAway", teamNameAway);

                    var res = cmdGetGuidTeamAway.ExecuteScalar();

                    if (res == null || res == DBNull.Value)
                    {
                        throw new Exception("Команды гостей с таким именем не найдено в бд.");
                    }

                    awayTeamGuid = res.ToString();
                }

                // Вставка записи матча.
                using (var cmdInsertMatches = new SqlCommand(insertMatch, conn))
                {
                    cmdInsertMatches.Parameters.AddWithValue("@matchDate", matchDate);
                    cmdInsertMatches.Parameters.AddWithValue("@arenaName", arenaName);
                    cmdInsertMatches.Parameters.AddWithValue("@homeTeamGuid", homeTeamGuid);
                    cmdInsertMatches.Parameters.AddWithValue("@awayTeamGuid", awayTeamGuid);

                    cmdInsertMatches.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Добавляет статистику игрока за конкретный матч.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <param name="pointsScored">Количество набранных очков.</param>
        /// <param name="foulsCount">Количество фолов.</param>
        /// <exception cref="Exception">
        /// Выбрасывается, если показатели отрицательные, либо если указанный игрок или матч не существуют.
        /// </exception>
        public static void CreatePlayerMatchStats(int playerId, int matchId, int pointsScored, int foulsCount)
        {
            // Валидация: проверка на отрицательные спортивные показатели.
            if (pointsScored < 0 || foulsCount < 0)
            {
                throw new Exception("Показатели очков и фолов не могут быть отрицательными.");
            }

            string checkPlayerQuery = "SELECT COUNT(*) FROM Players WHERE Id = @playerId";
            string checkMatchQuery = "SELECT COUNT(*) FROM Matches WHERE Id = @matchId";
            string insertStatsQuery = "INSERT INTO PlayerMatchStats (PlayerID, MatchID, PointsScored, FoulsCount) VALUES (@playerId, @matchId, @pointsScored, @foulsCount)";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Валидация: проверка существования игрока.
                using (var cmdCheckPlayer = new SqlCommand(checkPlayerQuery, conn))
                {
                    cmdCheckPlayer.Parameters.AddWithValue("@playerId", playerId);
                    if (Convert.ToInt32(cmdCheckPlayer.ExecuteScalar()) == 0)
                    {
                        throw new Exception($"Игрок с ID = {playerId} не найден в бд.");
                    }
                }

                // Валидация: проверка существования матча.
                using (var cmdCheckMatch = new SqlCommand(checkMatchQuery, conn))
                {
                    cmdCheckMatch.Parameters.AddWithValue("@matchId", matchId);
                    if (Convert.ToInt32(cmdCheckMatch.ExecuteScalar()) == 0)
                    {
                        throw new Exception($"Матч с ID = {matchId} не найден в бд.");
                    }
                }

                // Вставка записи статистики.
                using (var cmdInsert = new SqlCommand(insertStatsQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@playerId", playerId);
                    cmdInsert.Parameters.AddWithValue("@matchId", matchId);
                    cmdInsert.Parameters.AddWithValue("@pointsScored", pointsScored);
                    cmdInsert.Parameters.AddWithValue("@foulsCount", foulsCount);

                    cmdInsert.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновляет название существующей позиции по её идентификатору.
        /// </summary>
        /// <param name="positionId">Идентификатор позиции.</param>
        /// <param name="positionName">Новое название позиции.</param>
        /// <exception cref="Exception">Выбрасывается, если позиция с указанным ID не найдена.</exception>
        public static void UpdatePosition(int positionId, string positionName)
        {
            string query = "UPDATE Positions SET PositionName = @positionName WHERE PositionId = @positionId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@positionName", positionName);
                    cmd.Parameters.AddWithValue("@positionId", positionId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Позиция с id {positionId} не найдена в бд.");
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет параметры команды по её уникальному GUID.
        /// </summary>
        /// <param name="teamName">Новое название команды.</param>
        /// <param name="budget">Новый бюджет команды.</param>
        /// <param name="foundedYear">Новый год основания.</param>
        /// <param name="teamGuid">Уникальный GUID команды для поиска записи.</param>
        /// <exception cref="Exception">Выбрасывается, если команда с указанным GUID не найдена.</exception>
        public static void UpdateTeams(string teamName, float budget, int foundedYear, string teamGuid)
        {
            string query = "UPDATE Teams SET TeamName = @teamName, Budget = @budget, FoundedYear = @foundedYear WHERE TeamGuid = @teamGuid";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@teamName", teamName);
                    cmd.Parameters.AddWithValue("@budget", Convert.ToDecimal(budget));
                    cmd.Parameters.AddWithValue("@foundedYear", foundedYear);
                    cmd.Parameters.AddWithValue("@teamGuid", teamGuid);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Команда с guid = {teamGuid} не найдена в бд.");
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет персональные и регистрационные данные игрока.
        /// </summary>
        /// <param name="firstName">Новое имя.</param>
        /// <param name="lastName">Новая фамилия.</param>
        /// <param name="birthDate">Новая дата рождения.</param>
        /// <param name="isActive">Новый статус активности.</param>
        /// <param name="teamGuid">Новый GUID привязанной команды.</param>
        /// <param name="positionId">Новый ID позиции.</param>
        /// <param name="playerId">Идентификатор обновляемого игрока.</param>
        /// <exception cref="Exception">Выбрасывается, если игрок с указанным ID не найден.</exception>
        public static void UpdatePlayers(string firstName, string lastName, DateTime birthDate, bool isActive, string teamGuid, int positionId, int playerId)
        {
            string query = "UPDATE Players SET FirstName = @firstName, LastName = @lastName, BirthDate = @birthDate, IsActive = @isActive, TeamGuid = @teamGuid, PositionID = @positionId WHERE PlayerID = @playerId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@birthDate", birthDate);
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@teamGuid", teamGuid);
                    cmd.Parameters.AddWithValue("@positionId", positionId);
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Игрок с id = {playerId} не найден в бд.");
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет статистические показатели игрока в конкретном матче.
        /// </summary>
        /// <param name="pointsScored">Новое количество очков.</param>
        /// <param name="foulsCount">Новое количество фолов.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <exception cref="Exception">Выбрасывается, если запись статистики по данной паре ID не найдена.</exception>
        public static void UpdatePlayerMatchStats(int pointsScored, int foulsCount, int matchId, int playerId)
        {
            string query = "UPDATE PlayerMatchStats SET PointsScored = @pointsScored, FoulsCount = @foulsCount WHERE MatchID = @matchId AND PlayerID = @playerId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pointsScored", pointsScored);
                    cmd.Parameters.AddWithValue("@foulsCount", foulsCount);
                    cmd.Parameters.AddWithValue("@matchId", matchId);
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Игрок с id = {playerId} участвующий в матче с id = {matchId} не найден в бд.");
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет информацию о проведении матча.
        /// </summary>
        /// <param name="matchDate">Новая дата матча.</param>
        /// <param name="arenaName">Новое название арены.</param>
        /// <param name="homeTeamGuid">Новый GUID домашней команды.</param>
        /// <param name="awayTeamGuid">Новый GUID гостевой команды.</param>
        /// <param name="matchId">Идентификатор обновляемого матча.</param>
        /// <exception cref="Exception">Выбрасывается, если GUID команд совпадают или матч не найден.</exception>
        public static void UpdateMatches(DateTime matchDate, string arenaName, string homeTeamGuid, string awayTeamGuid, int matchId)
        {
            if (awayTeamGuid == homeTeamGuid)
            {
                throw new Exception("Команда не может играть сама с собой");
            }

            string query = "UPDATE Matches SET MatchDate = @matchDate, ArenaName = @arenaName, HomeTeamGuid = @homeTeamGuid, AwayTeamGuid = @awayTeamGuid WHERE MatchID = @matchId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@matchDate", matchDate);
                    cmd.Parameters.AddWithValue("@arenaName", arenaName);
                    cmd.Parameters.AddWithValue("@homeTeamGuid", homeTeamGuid);
                    cmd.Parameters.AddWithValue("@awayTeamGuid", awayTeamGuid);
                    cmd.Parameters.AddWithValue("@matchId", matchId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Матч с id = {matchId} не найден в бд.");
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает название позиции по её идентификатору.
        /// </summary>
        /// <param name="positionId">Идентификатор позиции.</param>
        /// <returns>Название позиции из БД.</returns>
        /// <exception cref="Exception">Выбрасывается, если позиция с указанным ID не существует.</exception>
        public static string ReadPosition(int positionId)
        {
            string query = "SELECT PositionName FROM Positions WHERE PositionID = @positionId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@positionId", positionId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception($"Позиция с id = {positionId} не найдена в бд.");
                        }

                        return reader["PositionName"].ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Получает список названий всех позиций, зарегистрированных в базе данных.
        /// </summary>
        /// <returns>Список строк с названиями позиций.</returns>
        public static List<string> ReadPositions()
        {
            string query = "SELECT PositionName FROM Positions";
            List<string> positions = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            positions.Add(reader["PositionName"].ToString());
                        }
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// Формирует структурированную строку с информацией о команде по её GUID.
        /// </summary>
        /// <param name="teamGuid">Уникальный GUID команды.</param>
        /// <returns>Текстовое досье команды.</returns>
        /// <exception cref="Exception">Выбрасывается, если команда не найдена.</exception>
        public static string ReadTeam(string teamGuid)
        {
            string query = "SELECT TeamName, Budget, FoundedYear FROM Teams WHERE TeamGuid = @teamGuid";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@teamGuid", teamGuid);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception($"Команда с guid = {teamGuid} не найден в бд");
                        }

                        return $"Команда с guid {teamGuid}:\nНазвание - {reader["TeamName"]}\nБюджет - {reader["Budget"]}\nГод основания - {reader["FoundedYear"]}";
                    }
                }
            }
        }

        /// <summary>
        /// Формирует текстовый список всех команд, находящихся в базе данных.
        /// </summary>
        /// <returns>Список строк с описанием каждой команды.</returns>
        public static List<string> ReadTeams()
        {
            string query = "SELECT TeamGuid, TeamName, Budget, FoundedYear FROM Teams";
            List<string> teams = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teams.Add($"Команда с guid = {reader["TeamGuid"]}: Название - {reader["TeamName"]}, Бюджет - {reader["Budget"]}, Год основания - {reader["FoundedYear"]}");
                        }
                    }
                }
            }

            return teams;
        }

        /// <summary>
        /// Получает подробное текстовое описание профиля игрока по его ID.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <returns>Строка с полными данными игрока.</returns>
        /// <exception cref="Exception">Выбрасывается, если игрок не найден.</exception>
        public static string ReadPlayer(int playerId)
        {
            string query = "SELECT FirstName, LastName, BirthDate, IsActive, TeamGuid, PositionID FROM Players WHERE PlayerID = @playerId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception($"Игрок с id = {playerId} не найден в бд.");
                        }

                        return $"Игрок с id = {playerId}: Имя - {reader["FirstName"]}\nФамилия - {reader["LastName"]}\nДень рождения - {reader["BirthDate"]}\nВ заявке - {reader["IsActive"]}\nGuid команды - {reader["TeamGuid"]}\nID позиции - {reader["PositionID"]}";
                    }
                }
            }
        }

        /// <summary>
        /// Получает список всех зарегистрированных игроков с их подробными анкетами.
        /// </summary>
        /// <returns>Список строк с описанием каждого игрока.</returns>
        public static List<string> ReadPlayers()
        {
            string query = "SELECT FirstName, LastName, BirthDate, IsActive, TeamGuid, PositionID, PlayerID FROM Players";
            List<string> players = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            players.Add($"Игрок с id = {reader["PlayerID"]}: Имя - {reader["FirstName"]}, Фамилия - {reader["LastName"]}, День рождения - {reader["BirthDate"]}, В заявке - {reader["IsActive"]}, Guid команды - {reader["TeamGuid"]}, ID позиции - {reader["PositionID"]}");
                        }
                    }
                }
            }

            return players;
        }

        /// <summary>
        /// Получает сведения о конкретном матче по его уникальному ID.
        /// </summary>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <returns>Строка с параметрами матча (дата, арена, GUID команд).</returns>
        /// <exception cref="Exception">Выбрасывается, если матч с таким ID не найден.</exception>
        public static string ReadMatch(int matchId)
        {
            string query = "SELECT MatchDate, ArenaName, HomeTeamGuid, AwayTeamGuid FROM Matches WHERE MatchID = @matchId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@matchId", matchId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception($"Матч с id = {matchId} не найден в бд.");
                        }

                        return $"Матч с id = {matchId}: Дата - {reader["MatchDate"]}\nАрена - {reader["ArenaName"]}\nХозяева (Guid) - {reader["HomeTeamGuid"]}\nГости (Guid) - {reader["AwayTeamGuid"]}";
                    }
                }
            }
        }

        /// <summary>
        /// Считывает список всех проведенных и запланированных матчей из базы данных.
        /// </summary>
        /// <returns>Список строк с информацией по каждому матчу.</returns>
        public static List<string> ReadMatches()
        {
            const string query = "SELECT MatchID, MatchDate, ArenaName, HomeTeamGuid, AwayTeamGuid FROM Matches";
            List<string> matches = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matches.Add($"Матч с id = {reader["MatchID"]}: Дата - {reader["MatchDate"]}, Арена - {reader["ArenaName"]}, Хозяева (Guid) - {reader["HomeTeamGuid"]}, Гости (Guid) - {reader["AwayTeamGuid"]}");
                        }
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Получает персональные результаты игрока (очки, фолы) в конкретном матче.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <returns>Строка с результатами результативности и нарушений.</returns>
        /// <exception cref="Exception">Выбрасывается, если статистика для данной пары ID отсутствует.</exception>
        public static string ReadPlayerMatchStat(int playerId, int matchId)
        {
            const string query = "SELECT PointsScored, FoulsCount FROM PlayerMatchStats WHERE PlayerID = @playerId AND MatchID = @matchId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.Parameters.AddWithValue("@matchId", matchId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception($"Статистика для игрока с id = {playerId} в матче с id = {matchId} не найдена.");
                        }

                        return $"Статистика игрока {playerId} в матче {matchId}: Очки - {reader["PointsScored"]}\nФолы - {reader["FoulsCount"]}";
                    }
                }
            }
        }

        /// <summary>
        /// Выводит общую сводную статистику по всем игрокам и всем матчам лиги.
        /// </summary>
        /// <returns>Список строк, содержащих информацию о перформансе игроков.</returns>
        public static List<string> ReadPlayerMatchStats()
        {
            const string query = "SELECT PlayerID, MatchID, PointsScored, FoulsCount FROM PlayerMatchStats";
            List<string> stats = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stats.Add($"Игрок ID = {reader["PlayerID"]} в матче ID = {reader["MatchID"]}: Очки - {reader["PointsScored"]}, Фолы - {reader["FoulsCount"]}");
                        }
                    }
                }
            }

            return stats;
        }

        /// <summary>
        /// Удаляет игровую позицию из базы данных по её уникальному идентификатору.
        /// </summary>
        /// <param name="positionId">Идентификатор удаляемой позиции.</param>
        /// <exception cref="Exception">Выбрасывается, если позиция с указанным ID не найдена.</exception>
        public static void DeletePosition(int positionId)
        {
            const string query = "DELETE FROM Positions WHERE PositionID = @positionId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@positionId", positionId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Позиция с id = {positionId} не найдена для удаления.");
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет команду из базы данных по её уникальному GUID.
        /// </summary>
        /// <param name="teamGuid">Уникальный GUID команды.</param>
        /// <exception cref="Exception">Выбрасывается, если команда с указанным GUID отсутствует.</exception>
        public static void DeleteTeam(string teamGuid)
        {
            const string query = "DELETE FROM Teams WHERE TeamGuid = @teamGuid";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@teamGuid", teamGuid);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Команда с guid = {teamGuid} не найдена для удаления.");
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет игрока из базы данных по его идентификатору.
        /// </summary>
        /// <param name="playerId">Идентификатор удаляемого игрока.</param>
        /// <exception cref="Exception">Выбрасывается, если игрок с указанным ID не найден.</exception>
        public static void DeletePlayer(int playerId)
        {
            const string query = "DELETE FROM Players WHERE PlayerID = @playerId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Игрок с id = {playerId} не найден для удаления.");
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет запись о проведенном матче по его идентификатору.
        /// </summary>
        /// <param name="matchId">Идентификатор удаляемого матча.</param>
        /// <exception cref="Exception">Выбрасывается, если матч с указанным ID не найден в системе.</exception>
        public static void DeleteMatch(int matchId)
        {
            const string query = "DELETE FROM Matches WHERE MatchID = @matchId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@matchId", matchId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Матч с id = {matchId} не найден для удаления.");
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет статистическую сводку игрока в определенном матче.
        /// </summary>
        /// <param name="playerId">Идентификатор игрока.</param>
        /// <param name="matchId">Идентификатор матча.</param>
        /// <exception cref="Exception">Выбрасывается, если запись статистики для данной пары ID не найдена.</exception>
        public static void DeletePlayerMatchStat(int playerId, int matchId)
        {
            const string query = "DELETE FROM PlayerMatchStats WHERE PlayerID = @playerId AND MatchID = @matchId";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.Parameters.AddWithValue("@matchId", matchId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Статистика игрока ID = {playerId} в матче ID = {matchId} не найдена для удаления.");
                    }
                }
            }
        }
    }
}