using System;
using System.Linq;
using System.Reflection;
using TFG_Videojocs.ACC_Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TFG_Videojocs.ACC_Subtitles
{
    public class ACC_SubtitleEditorWindowController: ACC_FloatingWindowController
    {
        public override void ConfigureJson()
        {
            ACC_SubtitleData subtitleData = new ACC_SubtitleData();
        }
    }
}