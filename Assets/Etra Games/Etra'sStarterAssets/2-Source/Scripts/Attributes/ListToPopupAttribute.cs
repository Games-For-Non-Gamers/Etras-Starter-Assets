using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class ListToPopupAttribute : PropertyAttribute
    {
        public Type myType;
        public string propertyName;

        public ListToPopupAttribute(Type _myType, string _propertyName)
        {
            myType = _myType;
            propertyName = _propertyName;
        }
    }
}
