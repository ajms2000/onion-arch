namespace System
{
    public abstract class MapperBase<TSource, TTarget>
    {
        public abstract TTarget MapSourceToTarget(TSource source);
        public abstract void MapSourceToTarget(TSource source, TTarget target);
        public abstract TSource MapTargetToSource(TTarget target);
        public abstract void MapTargetToSource(TTarget target, TSource source);
    }
}
