using System;

namespace education
{
    /// <summary>
    /// Класс для представления прямоугольника с заданными координатами и размерами.
    /// </summary>
    public class Rectangle
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;

        /// <summary>
        /// Иницилизация нового экземпляра класса <see cref="Rectangle"/> без параметров со стандартными значениями.
        /// </summary>
        public Rectangle()
        {
            _x = 0;
            _y = 0;
            _width = 10;
            _height = 10;
        }

        /// <summary>
        /// Иницилизация нового экземпляра класса <see cref="Rectangle"/> c параметрами.
        /// </summary>
        /// <param name="x">Координата X левого верхнего угла.</param>
        /// <param name="y">Координата Y левого верхнего угла.</param>
        /// <param name="width">Ширина прямоугольника (должна быть больше 0).</param>
        /// <param name="height">Высота прямоугольника (должна быть больше 0).</param>
        /// <exception cref="ArgumentException">Исключение генерируется, если высота или ширина меньше или равна 0.</exception>
        public Rectangle(double x, double y, double width, double height)
        {
            if(width >= 0 && height >= 0)
            {
                _x = x;
                _y = y;
                _width = width;
                _height = height;
            }
            else
            {
                throw new ArgumentException("Длина и ширина должны быть больше 0");
            }
        }

        /// <summary>
        /// Получение периметра прямоугльника.
        /// </summary>
        public double Perimeter { get { return 2 * (_width + _height); } }

        /// <summary>
        /// Получение площади прямоугольника.
        /// </summary>
        public double Area { get { return _width * _height; } }
    }
}
