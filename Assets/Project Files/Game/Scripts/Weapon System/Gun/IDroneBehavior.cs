namespace Watermelon.SquadShooter
{
    public interface IDroneBehavior
    {
        void GunUpdate();
        void Initialise(CharacterBehaviour characterBehaviour, WeaponData data);
        void OnGunUnloaded();
        void PlaceGun(BaseCharacterGraphics characterGraphics);
        void RecalculateDamage();
        void Reload();
    }
}