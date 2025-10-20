using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    // Модель для хранения отзыва
    class Review
    {
        public string Text { get; set; }
        public string Sentiment { get; set; }
    }

    // Класс для классификации тональности
    class SentimentClassifier
    {
        private Dictionary<string, int> positiveWords = new Dictionary<string, int>();
        private Dictionary<string, int> negativeWords = new Dictionary<string, int>();

        public void Train(List<Review> reviews)
        {
            Console.WriteLine("Обучение модели...");

            foreach (var review in reviews)
            {
                var words = PreprocessText(review.Text).Split(' ');

                foreach (var word in words)
                {
                    if (string.IsNullOrWhiteSpace(word)) continue;

                    if (review.Sentiment == "positive")
                    {
                        positiveWords[word] = positiveWords.GetValueOrDefault(word, 0) + 1;
                    }
                    else if (review.Sentiment == "negative")
                    {
                        negativeWords[word] = negativeWords.GetValueOrDefault(word, 0) + 1;
                    }
                }
            }

            Console.WriteLine($"Найдено положительных слов: {positiveWords.Count}");
            Console.WriteLine($"Найдено отрицательных слов: {negativeWords.Count}");
        }

        public string Predict(string text)
        {
            var words = PreprocessText(text).Split(' ');
            int positiveScore = 0;
            int negativeScore = 0;

            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                positiveScore += positiveWords.GetValueOrDefault(word, 0);
                negativeScore += negativeWords.GetValueOrDefault(word, 0);
            }

            if (positiveScore > negativeScore) return "positive";
            if (negativeScore > positiveScore) return "negative";
            return "neutral";
        }

        private string PreprocessText(string text)
        {
            // Приведение к нижнему регистру
            text = text.ToLower();

            // Удаление специальных символов
            text = Regex.Replace(text, @"[^а-яёa-z\s]", " ");

            // Удаление лишних пробелов
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("=== КЛАССИФИКАТОР ТЕКСТОВЫХ ОТЗЫВОВ ===\n");

        // 1. Создание тестовых данных (в реальном проекте загружаем из CSV)
        var reviews = CreateSampleData();

        // 2. Создание и обучение классификатора
        var classifier = new SentimentClassifier();
        classifier.Train(reviews);

        // 3. Тестирование
        TestClassifier(classifier);

        // 4. Интерактивный режим
        InteractiveMode(classifier);
    }

    static List<Review> CreateSampleData()
    {
        return new List<Review>
        {
            new Review { Text = "Отличный товар! Очень доволен покупкой.", Sentiment = "positive" },
            new Review { Text = "Прекрасное качество, быстрая доставка.", Sentiment = "positive" },
            new Review { Text = "Очень хороший продукт, рекомендую!", Sentiment = "positive" },
            new Review { Text = "Отлично работает, спасибо!", Sentiment = "positive" },
            new Review { Text = "Превосходное качество за свои деньги", Sentiment = "positive" },

            new Review { Text = "Ужасное качество, не рекомендую", Sentiment = "negative" },
            new Review { Text = "Очень плохой товар, деньги на ветер", Sentiment = "negative" },
            new Review { Text = "Не работает, полный разочарование", Sentiment = "negative" },
            new Review { Text = "Отвратительный сервис", Sentiment = "negative" },
            new Review { Text = "Худшая покупка в моей жизни", Sentiment = "negative" },

            new Review { Text = "Товар как товар, ничего особенного", Sentiment = "neutral" },
            new Review { Text = "Обычный продукт за обычные деньги", Sentiment = "neutral" },
            new Review { Text = "Соответствует описанию", Sentiment = "neutral" },
            new Review { Text = "Нормально, но есть лучшие варианты", Sentiment = "neutral" }
        };
    }

    static void TestClassifier(SentimentClassifier classifier)
    {
        Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ===");

        var testReviews = new[]
        {
            "Отличный товар! Очень рад что купил",
            "Ужасное качество, никогда больше",
            "Нормальный товар за свои деньги",
            "Плохая работа, не доволен",
            "Хороший продукт, советую"
        };

        foreach (var review in testReviews)
        {
            var sentiment = classifier.Predict(review);
            Console.WriteLine($"Отзыв: {review}");
            Console.WriteLine($"Тональность: {sentiment}\n");
        }
    }

    static void InteractiveMode(SentimentClassifier classifier)
    {
        Console.WriteLine("=== ИНТЕРАКТИВНЫЙ РЕЖИМ ===");
        Console.WriteLine("Введите текст отзыва (или 'выход' для завершения):");

        while (true)
        {
            Console.Write("\n> ");
            string input = Console.ReadLine();

            if (input.ToLower() == "выход")
                break;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var sentiment = classifier.Predict(input);
            Console.WriteLine($"Результат: {sentiment}");
        }
    }
}