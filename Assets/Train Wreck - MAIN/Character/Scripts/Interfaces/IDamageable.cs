public interface IDamageable
{
    bool IsFullyCured { get; }
    void Damage(DamageInfo damageInfo);
}

