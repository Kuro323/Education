using System;
using System.Collections.Generic;
using System.Text;

namespace education
{
    /// <summary>
    /// Класс для рисования ромба.
    /// </summary>
    public class DrawDiamint
    {
        public static void PrintRhomb(int diagonal)
        {
            //Валидация входных данных.
            if (diagonal % 2 == 0 || diagonal <= 0)
                throw new ArgumentException("Число должно быть нечетным и больше 0");

            for (int i = 0; i < diagonal; i++)
            {
                //Длина пропуска с учётом строки.
                int space = (diagonal - 1) / 2 - i;
                //Длина пропуска по модулю.
                if (space < 0)
                    space *= -1;

                //Заполнение строки x с учетом позиции.
                for (int j = 0; j < diagonal; j++)
                {
                    if(space == j || diagonal - 1 - space == j)
                        Console.Write("x");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("");
            }
        }
    }
}
