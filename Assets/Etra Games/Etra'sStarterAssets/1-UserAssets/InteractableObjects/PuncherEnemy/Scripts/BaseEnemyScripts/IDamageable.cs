namespace Etra.StarterAssets.Interactables.Enemies
{
    //This interface is the current mark for if a player can damage an object
    public interface IDamageable<T>
    {
        void TakeDamage(T damage);
    }
}