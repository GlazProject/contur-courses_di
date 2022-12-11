﻿using System.Globalization;
using TagCloud.App.WordPreprocessorDriver.WordsPreprocessor.BoringWords;
using TagCloud.App.WordPreprocessorDriver.WordsPreprocessor.Words;

namespace TagCloud.App.WordPreprocessorDriver.WordsPreprocessor;

/// <summary>
/// Класс, предоставляющий возможности базовой обработки слов.
/// Приводит все слова к нижнему регистру, подсчитывает их количество и tf индекс, изюавляется от "скучных" слов,
/// таких как предлоги, союзы, местоимения
/// </summary>
public class DefaultWordsPreprocessor : IWordsPreprocessor
{
    private readonly CultureInfo cultureInfo;
    private HashSet<IWord> wordsSet;

    public DefaultWordsPreprocessor(CultureInfo cultureInfo)
    {
        this.cultureInfo = cultureInfo;
        wordsSet = new HashSet<IWord>();
    }

    /// <summary>
    /// Позволяет получить обработанные неповторяющиеся слова с посчитанным индексом tf
    /// </summary>
    /// <param name="words">Список всех слов</param>
    /// <param name="boringWords">Сущность, которая позволит определять, скучное ли слово</param>
    /// <returns>Обработанные неповторяющиеся слова без скучных слов</returns>
    public ISet<IWord> GetProcessedWords(List<string> words, IReadOnlyCollection<IBoringWords> boringWords)
    {
        wordsSet = CreateWordsSet(words);
        CalculateTfIndexes(wordsSet, words.Count);
        return wordsSet
            .Where(word => 
                boringWords.All(checker => !checker.IsBoring(word)))
            .ToHashSet();
    }

    /// <summary>
    /// Метод, который позволяет получить tf индекс
    /// </summary>
    /// <param name="wordCount">Количество раз, которое слова встречается в текста</param>
    /// <param name="totalWordsCount">Общее количество слов в тексте</param>
    /// <returns>Значение tf индекса</returns>
    private static double GetTfIndex(int wordCount, int totalWordsCount)
    {
        return 1d * totalWordsCount / wordCount;
    }
        
    /// <summary>
    /// Метод, который зааполняет свойства tf у каждого слова
    /// </summary>
    /// <param name="words">Список неповторяющихся слов текста</param>
    /// <param name="totalWordsCount">Общее количество слов в текста</param>
    private static void CalculateTfIndexes(IEnumerable<IWord> words, int totalWordsCount)
    {
        foreach (var word in words)
        {
            word.Tf = GetTfIndex(word.Count, totalWordsCount);
        }
    }
        
    /// <summary>
    /// Метод, который аозволяет сформировать набор уникальных слов текста, применив к каждому слову некоторый
    /// алгоритм предварительной обработки (приведение к начальной форме, к lowercase и прочие...)
    /// </summary>
    /// <param name="words">Все слова в тексте</param>
    /// <returns>Set уникальных слов в тексте</returns>
    private HashSet<IWord> CreateWordsSet(IEnumerable<string> words)
    {
        var set = new HashSet<IWord>();
        foreach (var word in words.Select(wordValue => new Word(wordValue.ToLower(cultureInfo))))
        {
            if (set.TryGetValue(word, out var containedWord))
                containedWord.Count++;
            else 
                set.Add(word);
        }

        return set;
    }
}