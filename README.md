MongoDB C# Driver Extension for Atlas Search
============================================

This library is an extension to the MongoDB C# driver providing support for the
`$search` stage used with Atlas Search. You can gain access to the extension
methods for Atlas search by adding a reference to the library in your project
and using the `MongoDB.Labs.Search` namespace.

**This repository is NOT a supported MongoDB product.**

You can get the latest release from the [NuGet feed](https://www.nuget.org/packages/MongoDB.Labs.Search)
or from the [GitHub releases page](https://github.com/mongodb-labs/mongo-csharp-search/releases).

Examples
--------

```C#
public class HistoricalDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("body")]
    public string Body { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("highlights")]
    public List<Highlight> Highlights { get; set; }

    [BsonElement("score")]
    public double Score { get; set; }
}

List<HistoricalDocument> results = coll.Aggregate()
    .Search(
        SearchBuilders<HistoricalDocument>.Search
            .Phrase("life, liberty, and the pursuit of happiness", x => x.Body, 5),
        SearchBuilders<HistoricalDocument>.Highlight
            .Options(x => x.Body))
    .Limit(10)
    .Project<HistoricalDocument>(
        Builders<HistoricalDocument>.Projection
            .Include(x => x.Title)
            .Include(x => x.Body)
            .MetaSearchHighlights("highlights")
            .MetaSearchScore("score"))
    .ToList();
```

### Autocomplete Operator

```C#
List<HistoricalDocument> results = coll.Aggregate()
    .Search(
         SearchBuilders<HistoricalDocument>.Search
              .Autocomplete("life, liber", x => x.Body, AutocompleteTokenOrder.Sequential))
    .ToList();
```

### Compound Operator

```C#
List<HistoricalDocument> results = coll.Aggregate()
    .Search(
        SearchBuilders<HistoricalDocument>.Search
            .Compound()
            .Must(
                SearchBuilders<HistoricalDocument>.Search
                    .Text("life", x => x.Body),
                SearchBuilders<HistoricalDocument>.Search
                    .Text("liberty", x => x.Body))
            .MustNot(
                SearchBuilders<HistoricalDocument>.Search
                    .Text("property", x => x.Body)))
    .ToList();
```

### Equals Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .Eq(x => x.Retired, true))
    .ToList();
```

### Exists Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .Exists(x => x.MiddleName))
    .ToList();
```

### Near Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .Near(x => x.Age, 18, 1))
    .ToList();
```

### Phrase Operator

```C#
List<HistoricalDocument> results = coll.Aggregate()
    .Search(
        SearchBuilders<HistoricalDocument>.Search
            .Phrase("life, liberty, and the pursuit of happiness", x => x.Body))
    .ToList();
```

### Query String Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .QueryString(x => x.FirstName, "firstName:john lastName:doe"))
    .ToList();
```

### Range Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .RangeInt32(x => x.Age).Gte(18).Lt(21))
    .ToList();
```

The search operator functions `RangeInt64`, `RangeDouble`, and `RangeDateTime` are also available
to perform range searches for their respective data types. Every range search must include a lower
bound using `Gt` (greater than) or `Gte` (greater than or equal to) and an upper bound using `Lt`
(less than) or `Lte` (less than or equal to). The order of the bounds does not matter.

### Regular Expression Operator

```C#
List<Person> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .Regex("joh?n", x => x.FirstName))
    .ToList();
```

Regular expressions must be specified using the
[Lucene syntax](https://www.mongodb.com/docs/atlas/atlas-search/regex/#lucene-regular-expression-behavior).

### Text Operator

```C#
List<HistoricalDocument> results = coll.Aggregate()
    .Search(
        SearchBuilders<Person>.Search
            .Text(x => x.Body, "life, liberty, and the pursuit of happiness"))
    .ToList();
```

### Wildcard Operator

```C#
List<HistoricalDocument> results = coll.Aggregate()
    .Search(
        SearchBuilders<HistoricalDocument>.Search
            .Wildcard("happ*", x => x.Body))
    .ToList();
```
