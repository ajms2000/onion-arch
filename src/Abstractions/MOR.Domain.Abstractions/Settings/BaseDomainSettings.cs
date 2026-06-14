namespace MOR.Settings
{
    public abstract class BaseDomainSettings : IDomainSettings
    {
        public virtual void Process()
        {
            // Do nothing. Derived class extend as necessary.
        }
    }
}
