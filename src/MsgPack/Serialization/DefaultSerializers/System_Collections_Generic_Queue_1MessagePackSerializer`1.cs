#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010-2013 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Collections.Generic;

namespace MsgPack.Serialization.DefaultSerializers
{
// ReSharper disable InconsistentNaming
	internal sealed class System_Collections_Generic_Queue_1MessagePackSerializer<TItem> : MessagePackSerializer<Queue<TItem>>
// ReSharper restore InconsistentNaming
	{
		private readonly MessagePackSerializer<TItem> _itemSerializer;

		public System_Collections_Generic_Queue_1MessagePackSerializer( SerializationContext context )
			: base( ( context ?? SerializationContext.Default ).CompatibilityOptions.PackerCompatibilityOptions )
		{
			this._itemSerializer = ( context ?? SerializationContext.Default ).GetSerializer<TItem>();
		}

		protected internal override void PackToCore( Packer packer, Queue<TItem> objectTree )
		{
			packer.PackArrayHeader( objectTree.Count );
			foreach ( var item in objectTree )
			{
				this._itemSerializer.PackTo( packer, item );
			}
		}

		protected internal override Queue<TItem> UnpackFromCore( Unpacker unpacker )
		{
			if ( !unpacker.IsArrayHeader )
			{
				throw SerializationExceptions.NewIsNotArrayHeader();
			}

			var queue = new Queue<TItem>( UnpackHelpers.GetItemsCount( unpacker ) );
			this.UnpackToCore( unpacker, queue );

			return queue;
		}

		protected internal override void UnpackToCore( Unpacker unpacker, Queue<TItem> collection )
		{
			var itemsCount = UnpackHelpers.GetItemsCount( unpacker );
			for ( int i = 0; i < itemsCount; i++ )
			{
				if ( !unpacker.Read() )
				{
					throw SerializationExceptions.NewMissingItem( i );
				}

				TItem item;
				if ( !unpacker.IsArrayHeader && !unpacker.IsMapHeader )
				{
					item = this._itemSerializer.UnpackFrom( unpacker );
				}
				else
				{
					using ( Unpacker subtreeUnpacker = unpacker.ReadSubtree() )
					{
						item = this._itemSerializer.UnpackFrom( subtreeUnpacker );
					}
				}

				collection.Enqueue( item );
			}
		}
	}
}