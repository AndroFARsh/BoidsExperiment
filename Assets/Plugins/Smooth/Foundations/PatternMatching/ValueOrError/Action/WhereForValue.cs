using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.ValueOrError.Action
{
    public sealed class WhereForValue<T>
    {
        private readonly DelegateFunc<T, bool> _predicate;
        private readonly DelegateAction<DelegateFunc<T, bool>, DelegateAction<T>> _addPredicateAndAction;
        private readonly ValueOrErrorMatcher<T> _matcher;
        private readonly bool _isUseless;

        public static WhereForValue<T> Useless(ValueOrErrorMatcher<T> matcher)
        {
            return new WhereForValue<T>(null, null, matcher, true);
        }

        internal WhereForValue(DelegateFunc<T, bool> predicate,
            DelegateAction<DelegateFunc<T, bool>, DelegateAction<T>> addPredicateAndAction,
            ValueOrErrorMatcher<T> matcher, bool isUseless = false)
        {
            _predicate = predicate;
            _addPredicateAndAction = addPredicateAndAction;
            _matcher = matcher;
            _isUseless = isUseless;
        }

        public ValueOrErrorMatcher<T> Do(DelegateAction<T> action)
        {
            if (!_isUseless)
            {
                var predicate = _predicate;
                _addPredicateAndAction(o => predicate(o), action);
            }
            return _matcher;
        }
    }
}