using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface IInteractable {
        void OnInteractStart(ICharacter character);
        void OnInteractHold(ICharacter character);
        void OnInteractRelease(ICharacter character);
    }
}
