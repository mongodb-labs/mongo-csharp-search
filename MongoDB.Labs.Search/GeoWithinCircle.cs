// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// Object that specifies the center point and the radius in meters to search within.
    /// </summary>
    /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
    public class GeoWithinCircle<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        private readonly GeoJsonPoint<TCoordinates> _center;
        private readonly double _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoWithinCircle{TCoordinates}"/> class.
        /// </summary>
        /// <param name="center">Center of the circle specified as a GeoJSON point.</param>
        /// <param name="radius">Radius specified in meters.</param>
        public GeoWithinCircle(GeoJsonPoint<TCoordinates> center, double radius)
        {
            _center = Ensure.IsNotNull(center, nameof(center));
            _radius = EnsureExtensions.IsGreaterThanZero(radius, nameof(radius));
        }

        /// <summary>
        /// Gets the center of the circle specified as a GeoJSON point.
        /// </summary>
        public GeoJsonPoint<TCoordinates> Center => _center;

        /// <summary>
        /// Gets the radius specified in meters.
        /// </summary>
        public double Radius => _radius;

        internal BsonDocument Render()
        {
            return new BsonDocument
            {
                ["center"] = _center.ToBsonDocument(),
                ["radius"] = _radius
            };
        }
    }
}
