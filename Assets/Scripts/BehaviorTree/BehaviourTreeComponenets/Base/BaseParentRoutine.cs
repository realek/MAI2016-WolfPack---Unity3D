using UnityEngine;
using System.Collections;

public abstract class BaseParentRoutine : BaseRoutine {

    public abstract void AddChild(BaseRoutine child);
    public abstract bool HasChildren();

}
