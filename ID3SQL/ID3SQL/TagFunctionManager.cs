using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<string> AllGetFunctionPropertyNames(Comparison<string> comparison = null)
        {
            if (comparison != null)
            {
                List<string> keys = _GetFunctions.Keys.ToList();
                keys.Sort(comparison);

                return keys;
            }
            else
            {
                return _GetFunctions.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key);
            }
        }

        public static IEnumerable<Func<File, string, object>> AllGetFunctions(Comparison<string> comparison = null)
        {
            if(comparison != null)
            {
                List<string> keys = _GetFunctions.Keys.ToList();
                keys.Sort(comparison);

                ICollection<Func<File, string, object>> getFns = new List<Func<File, string, object>>();
                foreach(string key in keys)
                {
                    getFns.Add(_GetFunctions[key]);
                }
                return getFns;
            }
            else
            {
                return _GetFunctions.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
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
                "AlbumArtists",
                (file, filePath) =>
                {
                    return file.Tag.AlbumArtists;
                }
            },
            {
                "AlbumArtistsSort",
                (file, filePath) =>
                {
                    return file.Tag.AlbumArtistsSort;
                }
            },
            {
                "AlbumSort",
                (file, filePath) =>
                {
                    return file.Tag.AlbumSort;
                }
            },
            {
                "BeatsPerMinute",
                (file, filePath) =>
                {
                    return file.Tag.BeatsPerMinute;
                }
            },
            {
                "Comment",
                (file, filePath) =>
                {
                    return file.Tag.Comment;
                }
            },
            {
                "Composers",
                (file, filePath) =>
                {
                    return file.Tag.Composers;
                }
            },
            {
                "ComposersSort",
                (file, filePath) =>
                {
                    return file.Tag.ComposersSort;
                }
            },
            {
                "Conductor",
                (file, filePath) =>
                {
                    return file.Tag.Conductor;
                }
            },
            {
                "Copyright",
                (file, filePath) =>
                {
                    return file.Tag.Copyright;
                }
            },
            {
                "Disc",
                (file, filePath) =>
                {
                    return file.Tag.Disc;
                }
            },
            {
                "DiscCount",
                (file, filePath) =>
                {
                    return file.Tag.DiscCount;
                }
            },
            {
                "FirstAlbumArtist",
                (file, filePath) =>
                {
                    return file.Tag.FirstAlbumArtist;
                }
            },
            {
                "FirstAlbumArtistSort",
                (file, filePath) =>
                {
                    return file.Tag.FirstAlbumArtistSort;
                }
            },
            {
                "FirstComposer",
                (file, filePath) =>
                {
                    return file.Tag.FirstComposer;
                }
            },
            {
                "FirstComposerSort",
                (file, filePath) =>
                {
                    return file.Tag.FirstComposerSort;
                }
            },
            {
                "FirstGenre",
                (file, filePath) =>
                {
                    return file.Tag.FirstGenre;
                }
            },
            {
                "FirstPerformer",
                (file, filePath) =>
                {
                    return file.Tag.FirstPerformer;
                }
            },
            {
                "FirstPerformerSort",
                (file, filePath) =>
                {
                    return file.Tag.FirstPerformerSort;
                }
            },
            {
                "Genres",
                (file, filePath) =>
                {
                    return file.Tag.Genres;
                }
            },
            {
                "Grouping",
                (file, filePath) =>
                {
                    return file.Tag.Grouping;
                }
            },
            {
                "Lyrics",
                (file, filePath) =>
                {
                    return file.Tag.Lyrics;
                }
            },
            {
                "Performers",
                (file, filePath) =>
                {
                    return file.Tag.Performers;
                }
            },
            {
                "PerformersSort",
                (file, filePath) =>
                {
                    return file.Tag.PerformersSort;
                }
            },
            {
                "Title",
                (file, filePath) =>
                {
                    return file.Tag.Title;
                }
            },
            {
                "TitleSort",
                (file, filePath) =>
                {
                    return file.Tag.TitleSort;
                }
            },
            {
                "Track",
                (file, filePath) =>
                {
                    return file.Tag.Track;
                }
            },
            {
                "TrackCount",
                (file, filePath) =>
                {
                    return file.Tag.TrackCount;
                }
            },
            {
                "Year",
                (file, filePath) =>
                {
                    return file.Tag.Year;
                }
            }
        };

        public static Action<object, File, ExecutionPlanOptions> SetFunction(string propertyName)
        {
            Action<object, File, ExecutionPlanOptions> fn;
            if(_SetFunctions.TryGetValue(propertyName, out fn))
            {
                return fn;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<string> AllSetFunctionPropertyNames(Comparison<string> comparison = null)
        {
            if (comparison != null)
            {
                List<string> keys = _SetFunctions.Keys.ToList();
                keys.Sort(comparison);

                return keys;
            }
            else
            {
                return _SetFunctions.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key);
            }
        }

        private static readonly IDictionary<string, Action<object, File, ExecutionPlanOptions>> _SetFunctions = new Dictionary<string, Action<object, File, ExecutionPlanOptions>>()
        {
            {
                "Album",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Album = ValueToString(value);
                }
            },
            {
                "AlbumArtists",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.AlbumArtists = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "AlbumArtistsSort",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.AlbumArtistsSort = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "AlbumSort",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.AlbumSort = ValueToString(value);
                }
            },
            {
                "BeatsPerMinute",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.BeatsPerMinute = ValueToUInt(value);
                }
            },
            {
                "Comment",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Comment = ValueToString(value);
                }
            },
            {
                "Composers",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Composers = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "ComposersSort",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.ComposersSort = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "Conductor",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Conductor = ValueToString(value);
                }
            },
            {
                "Copyright",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Copyright = ValueToString(value);
                }
            },
            {
                "Disc",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Disc = ValueToUInt(value);
                }
            },
            {
                "DiscCount",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.DiscCount = ValueToUInt(value);
                }
            },
            {
                "Genres",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Genres = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "Grouping",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Grouping = ValueToString(value);
                }
            },
            {
                "Lyrics",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Lyrics = ValueToString(value);
                }
            },
            {
                "Performers",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Performers = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "PerformersSort",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.PerformersSort = ValueToStringArray(value, executionPlanOptions);
                }
            },
            {
                "Title",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Title = ValueToString(value);
                }
            },
            {
                "TitleSort",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.TitleSort = ValueToString(value);
                }
            },
            {
                "Track",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Track = ValueToUInt(value);
                }
            },
            {
                "TrackCount",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.TrackCount = ValueToUInt(value);
                }
            },
            {
                "Year",
                (value, file, executionPlanOptions) =>
                {
                    file.Tag.Year = ValueToUInt(value);
                }
            }
        };

        private static string ValueToString(object value)
        {
            if (value is string)
            {
                return value as string;
            }
            else
            {
                throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
            }
        }

        private static string[] ValueToStringArray(object value, ExecutionPlanOptions executionPlanOptions)
        {
            if (value is IEnumerable<string>)
            {
                return (value as IEnumerable<string>).ToArray();
            }
            else if (value is string)
            {
                return (value as string).Split(executionPlanOptions.StringArraySeparator);
            }
            else
            {
                throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
            }
        }

        private static uint ValueToUInt(object value)
        {
            if (IsNumeric(value))
            {
                return Convert.ToUInt32(value);
            }
            else
            {
                throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
            }
        }

        private static bool IsNumeric(object value)
        {
            return value is sbyte
            || value is byte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong
            || value is float
            || value is double
            || value is decimal;
        }
    }
}
