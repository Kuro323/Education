using education;

/// <summary>
/// Главный класс программы, представляющий собой консольный интерфейс пользователя (UI).
/// Обеспечивает интерактивный выбор технологии доступа к данным (EF Core или ADO.NET) 
/// и управление сущностями базы данных.
/// </summary>
/// <remarks>
/// На данный момент интерфейс демонстрирует работу на примере сущности "Позиции" (Positions),
/// однако заложенная архитектура и флаг выбора технологии <c>useEFCore</c> являются универсальными.
/// По аналогии с кейсами для Positions, это меню может быть расширено для вызова CRUD-методов 
/// других сущностей предметной области:
/// <list type="bullet">
///     <item><description>Команды (Teams)</description></item>
///     <item><description>Игроки (Players)</description></item>
///     <item><description>Матчи (Matches)</description></item>
///     <item><description>Статистика (PlayerMatchStats)</description></item>
/// </list>
/// </remarks>
class Program
{
    /// <summary>
    /// Точка входа в приложение. Управляет жизненным циклом главного меню и переключением репозиториев.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    static void Main(string[] args)
    {
        int numberMenu;

        // Переменная-флаг, определяющая, какой репозиторий будет вызываться ниже.
        // Это применима ко всем сущностям (не только к positions, но и к игрокам, матчам и т.д.)
        bool useEFCore = true;

        Console.WriteLine("========================================");
        Console.WriteLine("       Выберите технологию работы       ");
        Console.WriteLine("========================================");
        Console.WriteLine("1. Entity Framework Core (EF Core)");
        Console.WriteLine("2. ADO.NET");
        Console.Write("Выбор (по умолчанию 1): ");

        string techChoice = Console.ReadLine();

        if (techChoice == "2")
        {
            useEFCore = false;
        }

        do
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($"   Управление БД ({(useEFCore ? "EF Core" : "ADO.NET")})   ");
            Console.WriteLine("========================================");

            // В перспективе здесь могут быть добавлены пункты меню для управления командами, 
            // игроками и матчами, использующие тот же подход с флагом useEFCore
            Console.WriteLine("1. Добавить новую позицию (Create)");
            Console.WriteLine("2. Вывести список позиций (Read)");
            Console.WriteLine("3. Изменить позицию (Update)");
            Console.WriteLine("4. Удалить позицию (Delete)");
            Console.WriteLine("0. Выйти из программы");
            Console.Write("Введите номер команды: ");

            if (!int.TryParse(Console.ReadLine(), out numberMenu))
            {
                Console.WriteLine("Ошибка. Введено не число. Нажмите любую клавишу, чтобы продолжить.");
                Console.ReadKey();
                numberMenu = -1;
                continue;
            }

            switch (numberMenu)
            {
                case 1:
                    {
                        Console.Write("Введите название новой позиции: ");
                        string name = Console.ReadLine();

                        // Универсальное ветвление: аналогичный вызов через if (useEFCore) 
                        // можно сделать, например, для EFCoreRepository.CreatePlayer() или AdoBasketballRepository.CreatePlayer()
                        if (useEFCore)
                            EFCoreRepository.CreatePosition(name);
                        else
                            AdoBasketballRepository.CreatePosition(name);

                        Console.WriteLine("Позиция успешно добавлена!");
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("\n--- Список позиций ---");
                        List<string> positions;

                        if (useEFCore)
                            positions = EFCoreRepository.ReadPositions();
                        else
                            positions = AdoBasketballRepository.ReadPositions();

                        foreach (var pos in positions)
                        {
                            Console.WriteLine(pos);
                        }
                        break;
                    }
                case 3:
                    {
                        Console.Write("Введите ID позиции для изменения: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            Console.Write("Введите новое название позиции: ");
                            string newName = Console.ReadLine();

                            if (useEFCore)
                                EFCoreRepository.UpdatePosition(id, newName);
                            else
                                AdoBasketballRepository.UpdatePosition(id, newName);

                            Console.WriteLine("Позиция успешно обновлена!");
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;
                    }
                case 4:
                    {
                        Console.Write("Введите ID позиции для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            if (useEFCore)
                                EFCoreRepository.DeletePosition(id);
                            else
                                AdoBasketballRepository.DeletePosition(id);

                            Console.WriteLine("Позиция успешно удалена!");
                        }
                        else
                        {
                            Console.WriteLine("Неверный ID.");
                        }
                        break;
                    }
                case 0:
                    {
                        Console.WriteLine("Завершение работы. Выход..");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Неизвестная команда. Выберите пункт 0-4.");
                        break;
                    }
            }

            if (numberMenu != 0)
            {
                Console.WriteLine("\nНажмите любую клавишу для возврата в меню.");
                Console.ReadKey();
            }

        }
        while (numberMenu != 0);
    }
}