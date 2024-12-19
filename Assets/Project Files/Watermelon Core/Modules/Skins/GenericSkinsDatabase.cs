using System;
using UnityEngine;

namespace Watermelon
{
    /// <example>
    /// [CreateAssetMenu(fileName = "Type Skins Database", menuName = "Data/Skins/Type Skins Database")]
    /// </example>
    public abstract class GenericSkinsDatabase<T> : AbstractSkinsDatabase where T : ISkinData
    {
        [SerializeField] T[] skins;
        public T[] Skins => skins;

        public override int SkinsCount => skins.Length;

        public override Type SkinType => typeof(T);

        public override void Init()
        {
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].Initialise(this);
            }
        }

        public override ISkinData GetSkinData(int index)
        {
            return skins[index];
        }

        public override ISkinData GetSkinData(string id)
        {
            for (int i = 0; i < skins.Length; i++)
            {
                ISkinData data = skins[i];

                if (data.ID == id) return data;
            }

            return null;
        }
    }
}