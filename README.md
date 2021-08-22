MongoDB C# Driver Extension for Atlas Search
============================================

This library is an extension to the MongoDB C# driver providing support for the
`$search` stage used with Atlas Search. You can gain access to the extension
methods for Atlas search by adding a reference to the library in your project
and using the `MongoDB.Labs.Search` namespace.

*This repository is NOT a supported MongoDB product.*

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
