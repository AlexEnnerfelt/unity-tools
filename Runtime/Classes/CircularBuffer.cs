namespace UnpopularOpinion.Tools {
    using System;

    public class CircularBuffer<T> {
        private T[] _buffer;
        private int _head;
        private int _tail;
        private int _size;
        private int _count;

        public CircularBuffer(int size) {
            if (size <= 0) {
                throw new ArgumentException("Size must be greater than zero.", nameof(size));
            }

            _size = size;
            _buffer = new T[size];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public void Enqueue(T item) {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % _size;
            if (_count == _size) {
                _head = (_head + 1) % _size; // Overwrite the oldest item
            }
            else {
                _count++;
            }
        }

        public T Dequeue() {
            if (_count == 0) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            T item = _buffer[_head];
            _head = (_head + 1) % _size;
            _count--;
            return item;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public bool IsFull => _count == _size;
        public T Peek() {
            if (_count == 0) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            return _buffer[_head];
        }

        public T[] PeekAt(int numberOfItems) {
            if (numberOfItems <= 0) {
                throw new ArgumentOutOfRangeException(nameof(numberOfItems), "Number of items must be greater than zero and less than or equal to the number of items in the buffer.");
            }
            if (numberOfItems > _count) {
                numberOfItems = _count;
            }

            T[] result = new T[numberOfItems];
            for (var i = 0; i < numberOfItems; i++) {
                result[i] = _buffer[(_head + i) % _size];
            }

            return result;
        }
    }
}