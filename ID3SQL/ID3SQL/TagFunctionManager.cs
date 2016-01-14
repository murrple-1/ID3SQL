using System;
using System.Collections.Generic;

using TagLib;

namespace ID3SQL
{
    public static class TagFunctionManager
    {
        public static Func<File, string, object> GetFunction(string propertyName)
        {
            Func<File, string, object> fn;
            if (_GetFunctions.TryGetValue(propertyName, out fn))
            {
                return fn;
            }
            else
            {
                return null;
            }
        }

        private static readonly IDictionary<string, Func<File, string, object>> _GetFunctions = new Dictionary<string, Func<File, string, object>>()
        {
            {
                "FilePath",
                (file, filePath) =>
                {
                    return filePath;
                }
            },
            {
                "Album",
                (file, filePath) =>
                {
                    return file.Tag.Album;
                }
            },
            {
                "DiscCount",
                (file, filePath) =>
                {
                    return file.Tag.DiscCount;
                }
            }
        };

        public static Action<object, File> SetFunction(string propertyName)
        {
            Action<object, File> fn;
            if(_SetFunctions.TryGetValue(propertyName, out fn))
            {
                return fn;
            }
            else
            {
                return null;
            }
        }

        private static readonly IDictionary<string, Action<object, File>> _SetFunctions = new Dictionary<string, Action<object, File>>()
        {
            {
                "Album",
                (value, file) =>
                {
                    string _value = value as string;
                    if(_value != null)
                    {
                        file.Tag.Album = _value;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment to Album: {0}", value));
                    }
                }
            }
        };
    }
}
