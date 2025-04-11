using Honeylab.Utils;
using Honeylab.Utils.Persistence;
using Newtonsoft.Json;
using System;
using UniRx;


namespace Honeylab.Persistence
{
    public class ObservableValuePersistentComponent : PersistentComponent
    {
        [JsonProperty("V")]
        private string _value;

        [JsonIgnore]
        private readonly ISubject<int> _subject = new Subject<int>();

        [JsonIgnore]
        public int Value
        {
            get => Cipher.DecryptInt(_value);
            set
            {
                _value = Cipher.EncryptInt(value);
                _subject.OnNext(value);
            }
        }


        public IObservable<int> OnValueChangeAsObservable() => _subject.AsObservable();
    }
}
