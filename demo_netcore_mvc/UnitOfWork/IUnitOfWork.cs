using demo_netcore_mvc.IRepositories;

namespace demo_netcore_mvc.UnitOfWork
{
    public interface IUnitOfWork
    {
        IDetaiRepository DeTaiRepository { get; }
        IGiangVienRepository GiangVienRepository { get; }
        IHuongDanRepository HuongDanRepository { get; }
        IKhoaRepository KhoaRepository { get; }
        ISinhVienRepository SinhVienRepository { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Disposes of the resources used by this unit of work.
        /// </summary>
        void Dispose();
    }
}
