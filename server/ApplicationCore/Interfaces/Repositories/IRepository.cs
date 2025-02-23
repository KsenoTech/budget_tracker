namespace server.ApplicationCore.Interfaces.Repositories
{
    /// <summary>
    /// Generic репозиторий CRUD
    /// </summary>
    /// <typeparam name="T">Класс</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <returns>Список сущностей типа T</returns>
        Task<List<T>> GetListAsync();

        #region AsyncCRUD
            /// <summary>
            /// Создать сущность
            /// </summary>
            /// <param name="entity">Сущность для создания</param>
            /// <returns>True, если успешно</returns>
            Task<bool> CreateAsync(T entity);

            /// <summary>
            /// Получить сущность по идентификатору
            /// </summary>
            /// <typeparam name="TId">Тип идентификатора (int, string и т.д.)</typeparam>
            /// <param name="id">Идентификатор сущности</param>
            /// <returns>Сущность типа T</returns>
            Task<T> GetByIdAsync<TId>(TId id);

            /// <summary>
            /// Обновить сущность
            /// </summary>
            /// <param name="entity">Сущность для обновления</param>
            /// <returns>True, если успешно</returns>        
            Task<bool> UpdateAsync(T request);

            /// <summary>
            /// Удалить сущность по идентификатору
            /// </summary>
            /// <typeparam name="TId">Тип идентификатора (int, string и т.д.)</typeparam>
            /// <param name="id">Идентификатор сущности</param>
            /// <returns>True, если успешно</returns>
            Task<bool> DeleteAsync<TId>(TId id);
        #endregion

    }
}
