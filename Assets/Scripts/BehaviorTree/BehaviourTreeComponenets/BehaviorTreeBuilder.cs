using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Fluent builder to create behavoir trees
/// </summary>
public class BehaviorTreeBuilder {


    //Last created node
    private BaseParentRoutine m_currentRoutine = null;
    private Stack<BaseComposite> m_routineStack;
    public BehaviorTreeBuilder()
    {
        m_routineStack = new Stack<BaseComposite>();
    }

    /// <summary>
    /// Adds a action to the current non-leaf node, setting finish parent to true will end the current composite node 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="executableAction"></param>
    /// <param name="finishParent"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddAction(string name, Func<RoutineState> executableAction, bool finishParent=false)
    {
        if(m_routineStack.Count == 0)
        {
            Debug.LogError("Unable to add Action node before adding a container node, eg: Sequence");
            return this;
        }
        Action a = new Action();
        a.name = name;
        a.LoadAction(executableAction);
        m_routineStack.Peek().AddChild(a);
        return this;
    }

    public BehaviorTreeBuilder AddCondition()
    {
        return this;
    }

    public BehaviorTreeBuilder BeginSequence()
    {

        return this;
    }

    public BehaviorTreeBuilder BeginSelector()
    {

        return this;
    }




    /// <summary>
    /// Used to end composit Nodes ( eg: Selector,Sequence)
    /// </summary>
    /// <returns></returns>
    public BehaviorTreeBuilder EndComposite()
    {
        return this;
    }



    //Conversion operator
    public static implicit operator BaseRoutine(BehaviorTreeBuilder builder)
    {
        return new Action(); // Test Line
    }

}
