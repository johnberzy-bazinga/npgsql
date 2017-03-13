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

using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using AgensGraphTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.AgensGraphHandlers
{
    /// <summary>
    /// Type handler for the Agens graphid type.
    /// </summary>
    /// <remarks>
    /// https://github.com/bitnine-oss/agens-graph
    /// </remarks>
    [TypeMapping("graphid", NpgsqlDbType.GraphId, typeof(GraphId))]
    class GraphIdHandler : SimpleTypeHandler<GraphId>, ISimpleTypeHandler<string>
    {
        internal GraphIdHandler(PostgresType postgresType) : base(postgresType) { }

        public override GraphId Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var id = (ulong) buf.ReadInt64();
            var labId = (ushort) (id >> (32 + 16));
            var locId = id & 0x0000ffffffffffff;
            return new GraphId(labId, locId);
        }
           

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is GraphId))
                throw CreateConversionException(value.GetType());
            return 8;
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            var v = (GraphId)value;
            var graphid = (((ulong) (v.LabelId)) << (32 + 16)) | 
				 (((ulong) (v.LocationId)) & 0x0000ffffffffffff); 
            buf.WriteInt64((long)graphid);
        } 
    }
}
