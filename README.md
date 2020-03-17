# Introduction

The goal of this exercise is to implemenent the `DuplicateFileFinder` class so
that it returns a list of duplicate files that are found in the given base
directory.

Each entry in the list consists of the base-64 encoded hash of all the files
in a group along with the file size and the relative paths to each file.

```csharp
public class DuplicateFileResult
{
  public string Sha1HashBase64 { get; set;  }
  public long FileSize { get; set; }
  public List<string> FileRelativePaths { get; set; }
}
```

# Criteria

Your implementation must return the correct set of duplicate files
or it will be disqualified.

All correct implementations will then be run against a set of test folders to
determine which implementation is the fastest.
