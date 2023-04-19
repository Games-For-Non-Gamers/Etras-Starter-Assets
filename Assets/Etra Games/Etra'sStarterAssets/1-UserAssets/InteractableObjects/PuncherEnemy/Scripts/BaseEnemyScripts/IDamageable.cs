//Create an interface with a shared function that can be called
//By multiple objects
namespace EtrasStarterAssets{
    public interface IDamageable<T>
    {
        void TakeDamage(T damage);
    }
}