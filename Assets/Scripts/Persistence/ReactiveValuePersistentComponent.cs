using Honeylab.Utils.Persistence;
using Newtonsoft.Json;
using UniRx;


namespace Honeylab.Persistence
{
    public class ReactiveValuePersistentComponent<T> : PersistentComponent
    {
        private T _value;
        private ReactiveProperty<T> _valueProperty;


        [JsonProperty("V")]
        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                if (_valueProperty != null)
                {
                    _valueProperty.Value = _value;
                }
            }
        }


        [JsonIgnore] public IReadOnlyReactiveProperty<T> ValueProperty =>
            _valueProperty ??= new ReactiveProperty<T>(_value);
    }
}
