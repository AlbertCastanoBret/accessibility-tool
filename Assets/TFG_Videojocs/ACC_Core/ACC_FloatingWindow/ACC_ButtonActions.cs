using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace TFG_Videojocs
{
    public static class ACC_ButtonActions
    {
        public static Action<VisualElement, string> RemoveRowAction = (table, rowName) => 
        {
            var rowToDelete = table.Q(name: rowName);
            if (rowToDelete != null)
            {
                table.Remove(rowToDelete);
            }
        };

        public static Action CloneAction(List<VisualElement> visualElement)
        {
            if(visualElement[0].name == "delete-row-button")
            {
                return () => RemoveRowAction(visualElement[0].parent.parent, visualElement[0].parent.name);
            }
            return null;
        }
    }
}