using CuratorApp.Models;
namespace CuratorApp.Repositories
{
    public interface ICuratorRepository
    {
        Task<Curator?> Login();
        Task<Curator> Register(Curator curator);
        Task<Curator> Update(Curator curator);   // Или свой DTO
        Task<bool> Delete(int curatorId);        // Вернуть bool, если удаление успешно
    }
}
