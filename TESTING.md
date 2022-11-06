Testing Configuration
---------------------

In order to run `AtlasSearch.Tests`, you must the `ATLAS_SEARCH_URI` environment variable to point
to an Atlas cluster initialized with the example data set. The following index must be configured
on the `sample_training.posts` collection:

```JSON
{
  "mappings": {
    "dynamic": true,
    "fields": {
      "author": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "type": "stringFacet"
        }
      ],
      "body": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "multi": {
            "english": {
              "analyzer": "lucene.english",
              "type": "string"
            }
          },
          "type": "string"
        }
      ],
      "date": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "type": "dateFacet"
        }
      ],
      "index": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "type": "numberFacet"
        }
      ],
      "title": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "type": "autocomplete"
        }
      ]
    }
  },
  "storedSource": {
    "include": [
      "body",
      "title"
    ]
  }
}
```

The following index must be configured on the `sample_geospatial.shipwrecks` collection:

```JSON
{
  "mappings": {
    "dynamic": true,
    "fields": {
      "coordinates": [
        {
          "dynamic": true,
          "type": "document"
        },
        {
          "indexShapes": true,
          "type": "geo"
        }
      ]
    }
  }
}
```
