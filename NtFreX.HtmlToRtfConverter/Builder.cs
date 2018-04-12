using System.Collections.Generic;

namespace NtFreX.HtmlToRtfConverter
{
    public abstract class Builder<TSubject> where TSubject : new()
    {
        protected readonly TSubject Subject = new TSubject();

        // ReSharper disable once RedundantAssignment
        protected TBuilder Apply<TBuilder, TValue>(ref TValue field, TValue newValue)
            where TBuilder : Builder<TSubject>
        {
            field = newValue;
            return (TBuilder)this;
        }

        protected TBuilder InsertOrUpdate<TBuilder, TKey, TValue>(ref Dictionary<TKey, TValue> field, TKey key, TValue value)
            where TBuilder : Builder<TSubject>
        {
            if (field.ContainsKey(key))
                field[key] = value;
            else
                field.Add(key, value);
            return (TBuilder)this;
        }

        public virtual TSubject Build()
            => Subject;
    }
}