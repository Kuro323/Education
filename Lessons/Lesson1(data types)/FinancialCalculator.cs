using System;
using System.Collections.Generic;
using System.Text;

namespace education
{
    /// <summary>
    /// Класс для проведения финансовых расчетов.
    /// </summary>
    public class FinancialCalculator
    {
        public static string CalculatePercentage(double initial_deposit, int years, double interest_rate)
        {
            // Валидация входных данных.
            if(initial_deposit <= 0 || years <= 0 || interest_rate <= 0)
                throw new ArgumentException("Все числа должны быть положительными.");

            StringBuilder res = new StringBuilder();
            double currentBalance = initial_deposit;

            // Перевод процентов в коэффициент (10% - 0.1).
            double rateFactor = interest_rate / 100;

            for(int i = 1; i <= years; i++)
            {
                // Начисление процентов (сложный процент).
                currentBalance += rateFactor * currentBalance;

                // Добавление строки в ответ, с форматировнием числа до 2-ух знаков после запятой.
                res.AppendLine($"Год {years}: {currentBalance:F2} руб.");
            }

            // Возращение полученой строки, с удалением последнего пустого переноса.
            return res.ToString().TrimEnd();
        }
    }
}
