using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Asv.TextConverter
{
    public class JsonOneFileConfiguration : IConfiguration
    {
        private readonly string _fileName;
        private readonly Dictionary<string, JToken> _values;
        private readonly ReaderWriterLockSlim _rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public string FilePath => _fileName;

        public JsonOneFileConfiguration(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            var dir = Path.GetDirectoryName(Path.GetFullPath(fileName));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            _fileName = fileName;
            if (!File.Exists(fileName))
            {
                _values = new Dictionary<string, JToken>();
                InternalSaveChanges();
            }
            _values = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(File.ReadAllText(_fileName), new StringEnumConverter()) ?? new Dictionary<string, JToken>();

        }

        private void InternalSaveChanges()
        {
            var content = JsonConvert.SerializeObject(_values, Formatting.Indented, new StringEnumConverter());
            if (File.Exists(_fileName)) File.Delete(_fileName);
            File.WriteAllText(_fileName, content);
        }

        public IEnumerable<string> AvalableParts => GetParts();

        private IEnumerable<string> GetParts()
        {
            try
            {
                _rw.EnterReadLock();
                return _values.Keys.ToArray();
            }
            finally
            {
                _rw.ExitReadLock();
            }
            
        }

        public bool Exist<TPocoType>(string key)
        {
            return _values.ContainsKey(key);
        }

        public TPocoType Get<TPocoType>(string key, TPocoType defaultValue)
        {
            try
            {
                _rw.EnterUpgradeableReadLock();
                JToken value;
                if (_values.TryGetValue(key, out value))
                {
                    var a = value.ToObject<TPocoType>();
                    return a;
                }
                else
                {
                    Set(key,defaultValue);
                    return defaultValue;
                }
            }
            finally
            {
                _rw.ExitUpgradeableReadLock();
            }
        }

        public void Set<TPocoType>(string key, TPocoType value)
        {
            try
            {
                _rw.EnterWriteLock();
                var jValue = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(value));
                if (_values.ContainsKey(key))
                {
                    _values[key] = jValue;
                }
                else
                {
                    _values.Add(key,jValue);
                }
                InternalSaveChanges();
            }
            finally
            {
                _rw.ExitWriteLock();
            }
            
            
        }

        public void Remove(string key)
        {
            try
            {
                _rw.EnterWriteLock();
                if (_values.ContainsKey(key))
                {
                    _values.Remove(key);
                    InternalSaveChanges();
                }
            }
            finally
            {
                _rw.ExitWriteLock();
            }
        }
    }
}