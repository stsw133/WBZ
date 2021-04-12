using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WBZ.Modules._base
{
    public class D : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;

            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            if (!(selectorExpression.Body is MemberExpression body))
                throw new ArgumentException("The body must be a member expression");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(body.Member.Name));
            return true;
        }
    }
}
