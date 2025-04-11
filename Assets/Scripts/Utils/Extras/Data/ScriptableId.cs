using Honeylab.Utils.Extensions;
using System;
using UnityEngine;


namespace Honeylab.Utils.Data
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(ScriptableId),
        menuName = DataUtil.MenuNamePrefix + "Scriptable ID")]
    public class ScriptableId : ScriptableObject, ISerializationCallbackReceiver, IEquatable<ScriptableId>
    {
        [SerializeField] private string _stringValue;

        private Guid _guid;


        public void GenerateNewGuid() => SetGuid(Guid.NewGuid());


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (string.IsNullOrEmpty(_stringValue))
            {
                GenerateNewGuid();
            }
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!Guid.TryParse(_stringValue, out _guid))
            {
                this.SelfLogWarning($"{nameof(ScriptableId)} deserialization failed.");
            }
        }


        public bool Equals(ScriptableId other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || _guid.Equals(other._guid);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ScriptableId)obj);
        }


        public override int GetHashCode() => _guid.GetHashCode();


        public override string ToString() => _stringValue;


        protected void SetGuid(Guid guid)
        {
            _guid = guid;
            _stringValue = guid.ToString();
        }
    }
}
