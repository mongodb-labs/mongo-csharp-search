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
    /// Object that specifies the bottom left and top right GeoJSON points of a box to
    /// search within.
    /// </summary>
    /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
    public class GeoWithinBox<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        private readonly GeoJsonPoint<TCoordinates> _bottomLeft;
        private readonly GeoJsonPoint<TCoordinates> _topRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoWithinBox{TCoordinates}"/> class.
        /// </summary>
        /// <param name="bottomLeft">The bottom left GeoJSON point.</param>
        /// <param name="topRight">The top right GeoJSON point.</param>
        public GeoWithinBox(GeoJsonPoint<TCoordinates> bottomLeft, GeoJsonPoint<TCoordinates> topRight)
        {
            _bottomLeft = Ensure.IsNotNull(bottomLeft, nameof(bottomLeft));
            _topRight = Ensure.IsNotNull(topRight, nameof(topRight));
        }

        /// <summary>
        /// Gets the bottom left GeoJSON point.
        /// </summary>
        public GeoJsonPoint<TCoordinates> BottomLeft => _bottomLeft;

        /// <summary>
        /// Gets the top right GeoJSON point.
        /// </summary>
        public GeoJsonPoint<TCoordinates> TopRight => _topRight;

        internal BsonDocument Render()
        {
            BsonDocument document = new BsonDocument();
            document.Add("bottomLeft", _bottomLeft.ToBsonDocument());
            document.Add("topRight", _topRight.ToBsonDocument());
            return document;
        }
    }
}
