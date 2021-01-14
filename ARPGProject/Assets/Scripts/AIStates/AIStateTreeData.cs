using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "AI Tree")]
    public class AIStateTreeData : ScriptableObject
    {
        [SerializeField] private AIStateData _aiStateRoot;

        public AIStateData AIStateRoot => _aiStateRoot;
    }

    [System.Serializable]
    public class AIStateData
    {
        [SerializeField] private string _aiStateName;
        [SerializeField] private string _aiStateDetails;
        [SerializeField] private List<AIStateData> _childrenData = new List<AIStateData>();
        [SerializeField] private List<SerializedStringString> _transitionDatas = new List<SerializedStringString>();
        [SerializeField] private List<SerializedStringFloat> _floatDatas = new List<SerializedStringFloat>();
        [SerializeField] private List<SerializedStringInt> _intDatas = new List<SerializedStringInt>();
        [SerializeField] private List<SerializedStringBool> _boolDatas = new List<SerializedStringBool>();
        [SerializeField] private List<SerializedStringString> _stringDatas = new List<SerializedStringString>();
        [SerializeField] private List<SerializedAIKeyAnimationData> _animationDatas = new List<SerializedAIKeyAnimationData>();

        public string AIStateName => _aiStateName;
        public string AIStateDetails => _aiStateDetails;
        public List<AIStateData> ChildrenData => _childrenData;
        public List<SerializedStringString> TransitionDatas => _transitionDatas;

        public List<SerializedStringFloat> FloatDatas => _floatDatas;
        public List<SerializedStringInt> IntDatas => _intDatas;
        public List<SerializedStringBool> BoolDatas => _boolDatas;
        public List<SerializedStringString> StringDatas => _stringDatas;
        public List<SerializedAIKeyAnimationData> AnimationDatas => _animationDatas;
    }
}
