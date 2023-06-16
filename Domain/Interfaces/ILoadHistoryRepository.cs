namespace Services.Interfaces;

public interface ILoadHistoryRepository
{
    /// <summary>
    ///     Возвращает время создания последнего обработанного файла из указанного источника
    /// </summary>
    /// <param name="source">Источник данных</param>
    /// <returns></returns>
    public Task<DateTime> GetLastEntryTime(string source);

    /// <summary>
    ///     Сохраняет информацию по вновь загруженному файлу
    /// </summary>
    /// <param name="createdAt">время создания файла</param>
    /// <param name="source">Источник данных</param>
    /// <param name="hash">Хэш фала</param>
    /// <returns></returns>
    public Task SaveLoadInfo(DateTime createdAt, string source, string hash);

    public Task<bool> HashAlreadyExists(string hash);
}