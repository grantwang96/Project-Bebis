using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public abstract class SubActionDataV2 : ScriptableObject
    {

        public abstract void PerformAction(SubActionInitializationDataV2 subActionData);
        public abstract SubActionInitializationDataV2 CreateInitData(ICharacterV2 user);
    }
    public class SubActionInitializationDataV2
    {
        public SubActionDataV2 Data;
    }
}
