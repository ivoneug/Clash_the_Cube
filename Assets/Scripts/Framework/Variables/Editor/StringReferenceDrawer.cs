using UnityEditor;
using UnityEngine;

namespace Framework.Variables
{
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : ReferenceDrawer
    { }
}