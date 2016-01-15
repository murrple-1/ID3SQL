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
                return _GetFunctions.Values;
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

        private static readonly IDictionary<string, Action<object, File, ExecutionPlanOptions>> _SetFunctions = new Dictionary<string, Action<object, File, ExecutionPlanOptions>>()
        {
            {
                "Album",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Album = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "AlbumArtists",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.AlbumArtists = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.AlbumArtists = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "AlbumArtistsSort",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.AlbumArtistsSort = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.AlbumArtistsSort = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "AlbumSort",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Album = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "BeatsPerMinute",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.BeatsPerMinute = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Comment",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Comment = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Composers",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.Composers = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.Composers = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "ComposersSort",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.ComposersSort = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.ComposersSort = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Conductor",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Conductor = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Copyright",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Copyright = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Disc",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.Disc = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "DiscCount",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.DiscCount = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Genres",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.Genres = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.Genres = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Grouping",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Grouping = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Lyrics",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Lyrics = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Performers",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.Performers = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.Performers = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "PerformersSort",
                (value, file, executionPlanOptions) =>
                {
                    if(value is IEnumerable<string>)
                    {
                        file.Tag.PerformersSort = (value as IEnumerable<string>).ToArray();
                    }
                    else if(value is string)
                    {
                        file.Tag.PerformersSort = (value as string).Split(executionPlanOptions.StringArraySeparator);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Title",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.Title = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "TitleSort",
                (value, file, executionPlanOptions) =>
                {
                    if(value is string)
                    {
                        file.Tag.TitleSort = value as string;
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Track",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.Track = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "TrackCount",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.TrackCount = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            },
            {
                "Year",
                (value, file, executionPlanOptions) =>
                {
                    if(IsNumeric(value))
                    {
                        file.Tag.Year = Convert.ToUInt32(value);
                    }
                    else
                    {
                        throw new ID3SQLException(string.Format("Invalid Assignment: {0}", value));
                    }
                }
            }
        };

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
