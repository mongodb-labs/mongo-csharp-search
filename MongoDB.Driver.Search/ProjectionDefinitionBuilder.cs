using MongoDB.Driver;

namespace MongoDB.Labs.Search
{
    public static class ProjectionDefinitionExtensions
    {
        public static ProjectionDefinition<TDocument> MetaSearchScore<TDocument>(
            this ProjectionDefinition<TDocument> projection,
            string field)
        {
            var builder = Builders<TDocument>.Projection;
            return builder.Combine(projection, builder.MetaSearchScore(field));
        }
    }

    public static class ProjectionDefinitionBuilderExtensions
    {
        public static ProjectionDefinition<TSource> MetaSearchScore<TSource>(
            this ProjectionDefinitionBuilder<TSource> builder,
            string field)
        {
            return builder.Meta(field, "searchScore");
        }
    }
}
