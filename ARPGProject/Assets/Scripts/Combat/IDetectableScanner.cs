using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface IDetectableScanner : ICharacterComponent
    {
        IReadOnlyList<IDetectable> ScanForDetectables(float radius);
        bool ScanForDetectable(IDetectable detectable);
    }
}
