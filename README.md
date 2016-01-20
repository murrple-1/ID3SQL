# ID3SQL
A command-line SQL-like interface to search and update your ID3-tagged files

ID3SQL supports a subset of the SQL standard in order to do batch search and updates to ID3-tagged files, such as your MP3 collection.

This searching mechanism is much more comprehensive than many media managers, like iTunes or Windows Media Player, because it allows searching and updating of more niche tags, like Composer or Disc Count.

# Usage

    ID3SQL.exe [-f|--fileRegex FILEREGEX] [-d|--directory DIRECTORY] [-v|--verbose] [-r|--recycle] [-y|--dryRun] [-s|--stringLiteralSeparator STRINGLITERALSEPARATOR] [-l|--ignoreCaseRegex] [-c|--columnNames] [-l|--columnSeparator COLUMNSEPARATOR] SQLSTATEMENT
    ID3SQL.exe [-h|--help]

```
  -f, --fileRegex                 .NET-style Regex to determine whether file is to be evaluated. Evaluates the whole file path. Defaults to /.*\.(wma|mp3|m4a)/

  -d, --directory                 Directory to recursively search for tag files. Defaults to current users "My Music" directory

  -v, --verbose                   (Default: False) Prints more information to STDOUT as the process runs

  -r, --recycle                   (Default: False) When running DELETE statements, determine whether to fully delete files or just put them in Windows Recycle Bin

  -y, --dryRun                    (Default: False) When running UPDATE or DELETE statements, flags whether to actually make changes to the files. If true, no changes will be made, but they will log via the verbose (-v, --verbose) flag as if they were actually updated/deleted

  -s, --stringLiteralSeparator    (Default: ;) When comparing a string against a string array, sets the delimiter to use to split the string

  -l, --ignoreCaseRegex           (Default: True) Sets whether the where-clause "LIKE" regex should ignore case

  -c, ----columnNames             (Default: True) When running SELECT statements, flags whether to print the column names as the first line

  -l, ----columnSeparator         (Default:     |       ) When running SELECT statements, the string given here will be the glue between each column. Usually a variation of the pipe-character

  --help                          Display the help screen.
```

# SQL Examples

ID3SQL supports 3 types of SQL-like actions: SELECT, UPDATE and DELETE

Here are some example statements that give an idea of how the statements are structured.

    SELECT *
Selects all tags from all files

    SELECT * WHERE Album = 'Hotel California'
Selects all tags from files where the Album is 'Hotel California'

    SELECT Album, FirstAlbumArtist WHERE 'Gorillaz' IN AlbumArtists

Select Album and First Album Artist tags where 'Gorillaz' is one of the Album Artists

    DELETE

Delete all files (why? Recommend turning on Recycle flag)

    DELETE WHERE Year = 1995

Delete files where the Year is 1995

    UPDATE SET Disc = 1, DiscCount = 2

Updates all files, setting Disc to 1 and Disc Count to 2

    UPDATE SET Genres = 'Rap;Gangsta Rap' WHERE Lyrics LIKE '.* rhythm .*' AND Year >= 2000

Updates files where the Lyrics contain the phrase 'rhythm' and the Year is 2000 or higher, setting the Grouping to 'Rap' and 'Gangsta Rap'

### Note
UPDATE statements support updating a subset of the available tags. Namely, any of the tags of the form 'First...' cannot be updated, since they are just convience methods accessing an array underneith
