﻿namespace MongoDB.Driver.Search
{
    public static class ProjectionDefinitionExtensions
    {
        public static ProjectionDefinition<TDocument> MetaTextScore<TDocument>(
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
