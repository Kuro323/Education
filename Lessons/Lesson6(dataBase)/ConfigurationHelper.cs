using Microsoft.Extensions.Configuration;

namespace education
{
    /// <summary>
    /// Класс для доступа к конфигурационным файлам приложения.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Содержит в себе пару ключ-значение из файла настроек.
        /// </summary>
        private static readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор для класса <see cref="ConfigurationHelper"/>
        /// Настраивает сборщик конфигурации и загружает файл настроек.
        /// </summary>
        static ConfigurationHelper()
        {
            //Иницилизируем сборщик конфигурации.
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        }

        /// <summary>
        /// Возращает строку подключения из секции ConnectionString из файла конфигурации.
        /// </summary>
        /// <param name="name">Имя строки подключения в файле конфигурации. По у молчания - DefaultConnection.</param>
        /// <returns>Строка подключения к БД.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если секция с указанным именем отсутсвует или пуста.</exception>
        public static string GetConnectionString(string name = "DefaultConnection")
        {
            //Получаем строку подключения.
            string connectionString = _configuration.GetConnectionString(name);

            //Валидация: отлов пустой строки.
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Строка подключения с именем {name} не найдета в конфиге.");
            }

            return connectionString;
        }
    }
}
