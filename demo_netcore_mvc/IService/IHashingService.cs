namespace demo_netcore_mvc.IService
{
    public interface IHashingService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
