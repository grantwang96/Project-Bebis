using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public abstract class SubActionData : ScriptableObject {

        public abstract void PerformAction(SubActionInitializationData subActionData);
        public abstract SubActionInitializationData CreateInitData(ICharacter user);
    }

    public class SubActionInitializationData {
        public SubActionData Data;
    }
}
