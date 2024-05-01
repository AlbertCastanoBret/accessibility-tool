using UnityEngine;

namespace TFG_Videojocs.ACC_Sound
{
    public class ACC_AudioManager: MonoBehaviour
    {
        [SerializeField] private ACC_SerializableDictiornary<string, AudioClip> audioSourcesSounds = new();
    }
}