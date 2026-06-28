using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace education
{
    /// <summary>
    /// Класс представляющий структуру данных - умного стека, на базе массива.
    /// </summary>
    /// <typeparam name="T">Тип элементов, хранящихся в стеке.</typeparam>
    public class SmartStack<T> : IEnumerable<T>
    {
        private int _size = 0;
        private readonly T[] _items;

        /// <summary>
        /// Возвращает кол-во элементов в стеке.
        /// </summary>
        public int Count { get { return _size; } }

        /// <summary>
        /// Возвращает максимальную ёмкость стека (массива).
        /// </summary>
        public int Capacity { get { return _items.Length; } }

        /// <summary>
        /// Конструктор без параметра, инициализирует пустой стек ёмкостью в 4 элемента.
        /// </summary>
        public SmartStack()
        {
            _items = new T[4];
        }

        /// <summary>
        /// Конструктор с параметром, инициализирует стек с максимальной ёмкостью заданной параметром length.
        /// </summary>
        /// <param name="length">Максимальная ёмкость стека.</param>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если размер меньше или равен нулю.</exception>
        public SmartStack(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("Размер стека должен быть больше 0");

            _items = new T[length];
        }

        /// <summary>
        /// Конструктор с параметром, инициализирует стек на основе существующей коллекции.
        /// </summary>
        /// <param name="items">Коллекция, элементы которой будут скопированы в стек.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если коллекция равна null.</exception>
        public SmartStack(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("Список элементов не может быть null");

            _items = items.ToArray();
            _size = _items.Length;
        }

        /// <summary>
        /// Добавляет новый элемент на вершину стека.
        /// </summary>
        /// <param name="item">Элемент для добавления.</param>
        /// <exception cref="InvalidOperationException">Выбрасывается, если стек заполнен.</exception>
        public void Push(T item)
        {
            if (_size >= _items.Length)
                throw new InvalidOperationException("Стек переполнен. Невозможно добавить новый элемент");

            _items[_size] = item;
            _size++;
        }

        /// <summary>
        /// Добавляет новые элементы из существующей коллекции в обратном порядке.
        /// </summary>
        /// <param name="items">Коллекция элементов для добавления.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если коллекция null.</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается, если кол-во добавляемых элементов больше, чем свободных мест в стеке.</exception>
        public void PushRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("Список элементов не может быть null");

            T[] tempArray = items.Reverse().ToArray();

            if (_size + tempArray.Length > _items.Length)
                throw new InvalidOperationException("Невозможно добавить новые элементы, стек переполнился");

            foreach (T item in tempArray)
                _items[_size++] = item;
        }

        /// <summary>
        /// Удаляет и Возвращает элемент из вершины стека.
        /// </summary>
        /// <returns>Удаленный элемент с вершины стека.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если стек пуст и достать элемент не возможно.</exception>
        public T Pop()
        {
            if (_size <= 0)
                throw new InvalidOperationException("Стек пуст. Достать элемент невозможно");

            T targetItem = _items[--_size];

            _items[_size] = default;

            return targetItem;
        }

        /// <summary>
        /// Возвращает элемент из вершины стека.
        /// </summary>
        /// <returns>Элемент из вершины стека.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если стек пуст.</exception>
        public T Peek()
        {
            if (_size <= 0)
                throw new InvalidOperationException("Стек пуст. Достать элемент невозможно");

            return _items[_size - 1];
        }

        /// <summary>
        /// Проверяет содержится ли элемент в стеке.
        /// </summary>
        /// <param name="item">Элемент для поиска.</param>
        /// <returns>true, если элемент найдет, иначе false.</returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < _size; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Возвращает перечислитель, выполняющий обход элементов стека от вершины к основанию.
        /// </summary>
        /// <returns>Итератор для последовательного чтения элементов.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _size - 1; i >= 0; i--)
                yield return _items[i];
        }

        /// <summary>
        /// Возвращает перечислитель старого стиля (для обратной совместимости .NET).
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Предоставляет доступ к элементам стека по глубине(0 - вершина, count - 1 - основание).
        /// </summary>
        /// <param name="index">Индекс глубины элемента в стеке.</param>
        /// <returns>Элемент на нужной глубине.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если индекс выходит за границы стека.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("Индекс выходит за границу стека.");

                return _items[_size - 1 - index];
            }
        }
    }
}   
