using System;

namespace education
{
    public class Program
    {
        /// <summary>
        /// Точка входа в программу. Демонстрация создания объекта класса Rectangle.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        public static void Main(string[] args)
        {
            //1. Демонстрация работы конструктора по умолчанию (создает прямоугольник 10х10 в координатах 0,0)
            Rectangle rectangle1 = new Rectangle();

            Console.WriteLine($"Периметр первого пямоугольника = {rectangle1.Perimeter} \nПлощадь первого прямогульника = {rectangle1.Area}");

            //2. Демонстрация работы конструктора c параметрами (создает прямоугольник 40х25 в координатах 5,-3)
            Rectangle rectangle2 = new Rectangle(5, -3, 40, 25);

            Console.WriteLine($"Периметр второго пямоугольника = {rectangle2.Perimeter} \nПлощадь второго прямогульника = {rectangle2.Area}");

            //3. Тестирование защиты объекта от некорректного состояния
            try
            {
                Console.WriteLine("Попытка создания некорректрного прямогульника:");

                //Передача отрицательного параметра ширины. Код должен прерваться и уйти в блок catch
                Rectangle rectangle3 = new Rectangle(2, 0.3, -5, 10);

                Console.WriteLine($"Периметр третьего пямоугольника = {rectangle3.Perimeter} \nПлощадь третьего прямогульника = {rectangle3.Area}");
            }
            catch (ArgumentException ex)
            {
                //Выводим сообщение об ошибке
                Console.WriteLine($"Ошибка валидации: {ex.Message}");
            }
        }
    }
}
