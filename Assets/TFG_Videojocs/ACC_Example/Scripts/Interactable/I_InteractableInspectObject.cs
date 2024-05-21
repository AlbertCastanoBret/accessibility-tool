using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_InteractableInspectObject
{
    bool IsInspecting { get; set; }
    void Inspect();
    void StopInspect();
}
