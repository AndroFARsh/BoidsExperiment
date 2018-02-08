using System;
using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.ValueOrError.Function
{
    public class WhereForValueOrError<TMatcher, T1, TResult>
    {
        private readonly DelegateFunc<T1, bool> _expression;
        private readonly Action<DelegateFunc<T1, bool>, DelegateFunc<T1, TResult>> _recorder;
        private readonly TMatcher _matcher;

        internal WhereForValueOrError(DelegateFunc<T1, bool> expression,
            Action<DelegateFunc<T1, bool>, DelegateFunc<T1, TResult>> recorder,
            TMatcher matcher)
        {
            _expression = expression;
            _recorder = recorder;
            _matcher = matcher;
        }

        public TMatcher Return(DelegateFunc<T1, TResult> action)
        {
            _recorder(_expression, action);
            return _matcher;
        }

        public TMatcher Return(TResult value)
        {
            _recorder(_expression, x => value);
            return _matcher;
        }
    }
}