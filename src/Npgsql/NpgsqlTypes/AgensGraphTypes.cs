#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using JetBrains.Annotations;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace AgensGraphTypes
{
    public struct GraphId : IEquatable<GraphId>
    {
        ushort _labelId;
        ulong _locationId;

        public GraphId(ushort labelId, ulong locationId)
        {
            _labelId = labelId;
            _locationId = locationId;
        }

        public ushort LabelId => _labelId;
        public ulong LocationId => _locationId;

        public bool Equals([CanBeNull] GraphId other)
            => !ReferenceEquals(other, null) && other.LocationId == LocationId && other.LabelId == LabelId;

        public override bool Equals([CanBeNull] object obj) => obj is GraphId && Equals((GraphId)obj);

        public static bool operator ==([CanBeNull] GraphId x, [CanBeNull] GraphId y)
            => x.Equals(y);

        public static bool operator !=(GraphId x, GraphId y) => !(x == y);

        public override int GetHashCode() {
           int hash = 17;
           hash = hash * 31 + _labelId.GetHashCode();
           hash = hash * 31 + _locationId.GetHashCode();
           return hash;
        }

        public override string ToString() =>  $"{LabelId}.{LocationId}";
        
    }

    public class Vertex  {
        public GraphId Id { get; set; }
        public string Properties { get; set; }

        public override string ToString() => $"[{Id}]{Properties}";
    }

    public class Edge {
        public GraphId Id { get; set; }
        public GraphId Start { get; set; }
        
        public GraphId End { get; set; }
        public string Properties { get; set; }

        public override string ToString () => $"[{Id}]{Properties}";

    }

    public class Path {
        public Vertex[] Vertices { get; set; }
        public Edge[] Edges { get; set; }
    }

    public static class AgensGraphTypesRegistry {
        public static void RegisterGraphTypes () {
            var defaultNameTranslator = new Npgsql.NpgsqlSnakeCaseNameTranslator();
            Npgsql.TypeHandlerRegistry.MapCompositeGlobally<Vertex>("vertex", defaultNameTranslator);
            Npgsql.TypeHandlerRegistry.MapCompositeGlobally<Path>("graphpath", defaultNameTranslator);
            Npgsql.TypeHandlerRegistry.MapCompositeGlobally<Edge>("edge", defaultNameTranslator);
        }
    }
}