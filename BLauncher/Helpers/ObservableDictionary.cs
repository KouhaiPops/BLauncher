using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLauncher.Helpers
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly Dictionary<TKey, TValue> innerDict = new Dictionary<TKey, TValue>();
        public ICollection<TKey> Keys => innerDict.Keys;

        public ICollection<TValue> Values => innerDict.Values;

        public int Count => innerDict.Count;

        public bool IsReadOnly => false;

        public TValue this[TKey key] 
        { 
            get => innerDict[key];
            set
            {
                innerDict[key] = value;
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
            }
        }


        public void Add(TKey key, TValue value)
        {
            innerDict.Add(key, value);
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }

        public bool ContainsKey(TKey key) => innerDict.ContainsKey(key);

        public bool Remove(TKey key)
        {
            var result = innerDict.Remove(key);
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
            return result;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            => innerDict.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            innerDict.Clear();
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => innerDict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => innerDict.GetEnumerator();
    }
}
