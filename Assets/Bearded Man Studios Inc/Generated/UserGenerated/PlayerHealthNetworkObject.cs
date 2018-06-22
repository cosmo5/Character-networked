using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class PlayerHealthNetworkObject : NetworkObject
	{
		public const int IDENTITY = 9;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private int _CurrentHealth;
		public event FieldEvent<int> CurrentHealthChanged;
		public Interpolated<int> CurrentHealthInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int CurrentHealth
		{
			get { return _CurrentHealth; }
			set
			{
				// Don't do anything if the value is the same
				if (_CurrentHealth == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_CurrentHealth = value;
				hasDirtyFields = true;
			}
		}

		public void SetCurrentHealthDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_CurrentHealth(ulong timestep)
		{
			if (CurrentHealthChanged != null) CurrentHealthChanged(_CurrentHealth, timestep);
			if (fieldAltered != null) fieldAltered("CurrentHealth", _CurrentHealth, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			CurrentHealthInterpolation.current = CurrentHealthInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _CurrentHealth);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_CurrentHealth = UnityObjectMapper.Instance.Map<int>(payload);
			CurrentHealthInterpolation.current = _CurrentHealth;
			CurrentHealthInterpolation.target = _CurrentHealth;
			RunChange_CurrentHealth(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _CurrentHealth);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (CurrentHealthInterpolation.Enabled)
				{
					CurrentHealthInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					CurrentHealthInterpolation.Timestep = timestep;
				}
				else
				{
					_CurrentHealth = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_CurrentHealth(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (CurrentHealthInterpolation.Enabled && !CurrentHealthInterpolation.current.UnityNear(CurrentHealthInterpolation.target, 0.0015f))
			{
				_CurrentHealth = (int)CurrentHealthInterpolation.Interpolate();
				//RunChange_CurrentHealth(CurrentHealthInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerHealthNetworkObject() : base() { Initialize(); }
		public PlayerHealthNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerHealthNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
