using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.ValueOrError.Action
{
    public class ErrorMatcher<T>
    {
        private readonly DelegateAction<DelegateAction<string>> _addAction;
        private readonly ValueOrErrorMatcher<T> _matcher;
        private readonly bool _isError;

        public ErrorMatcher(ValueOrErrorMatcher<T> matcher,
            DelegateAction<DelegateAction<string>> addAction, bool isError)
        {
            _addAction = addAction;
            _matcher = matcher;
            _isError = isError;
        }

        public ValueOrErrorMatcher<T> Do(DelegateAction<string> action)
        {
            if (_isError)
            {
                _addAction(action);
            }
            return _matcher;
        }
    }
}