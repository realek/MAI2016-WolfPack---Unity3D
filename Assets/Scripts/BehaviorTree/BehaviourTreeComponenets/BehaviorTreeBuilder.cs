using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Fluent builder to create behavoir trees
/// </summary>
public class BehaviorTreeBuilder {


    //Last created node
    private BaseParentRoutine m_currentRoutine = null;
    private Stack<BaseParentRoutine> m_routineStack;
    public BehaviorTreeBuilder()
    {
        m_routineStack = new Stack<BaseParentRoutine>();
    }

    private bool m_complete
    {
        get
        {
            return m_routineStack.Count == 0;
        }
    }
    /// <summary>
    /// Adds a action to the current non-leaf node, can only be added to an decorator or composite node.
    /// </summary>
    /// <param name="name">node name.</param>
    /// <param name="executableAction">executable function delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddAction(string name, Func<RoutineState> executableAction)
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

    /// <summary>
    /// Decorator type node that automatically fails if its condition fails, else the node it contains is also executed,
    /// this method allows you to give other decorators or composite nodes to the condition node;
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="condition">contained node execution condition</param>
    /// <param name="routine">contained node</param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginCondition(string name, Func<bool> condition)
    {
        Condition cond = new Condition();
        cond.name = name;
        cond.LoadCondition(condition);
        m_routineStack.Peek().AddChild(cond);
        m_routineStack.Push(cond);
        return this;
    }


    /// <summary>
    /// Decorator type node that automatically fails if its condition fails, else the node it contains is also executed,
    /// this method automatically finishes the decorator node.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="condition">contained action execution condition</param>
    /// <param name="routine">contained action's delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddCondition(string name, Func<bool> condition, Func<RoutineState> executableAction)
    {
        Action a = new Action();
        a.name = name;
        a.LoadAction(executableAction);
        Condition cond = new Condition();
        cond.LoadCondition(condition);
        cond.AddChild(a);
        m_routineStack.Peek().AddChild(cond);
        return this;
    }


    public BehaviorTreeBuilder AddInverter()
    {
        return this;
    }

    public BehaviorTreeBuilder BeginInverter()
    {
        return this;
    }

    public BehaviorTreeBuilder AddRepeater()
    {
        return this;
    }

    public BehaviorTreeBuilder BeginRepeater()
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


    public BehaviorTreeBuilder AttachTree(BaseRoutine treeRoutine)
    {
        return this;
    }


    /// <summary>
    /// Used to end composite/decorator Nodes ( eg: Selector,Sequence)
    /// </summary>
    /// <returns></returns>
    public BehaviorTreeBuilder FinishNode()
    {
        m_currentRoutine = m_routineStack.Pop();
        return this;
    }



    //Conversion operator
    public static implicit operator BaseRoutine(BehaviorTreeBuilder builder)
    {

        if (builder.m_complete)
        {
            BaseRoutine cR = builder.m_currentRoutine;
            builder.m_currentRoutine = null;
            return cR;
        }
        else
        {
            Debug.LogError("Incomplete tree, there are decorator/composite nodes that have not been finished.");
            return null;
        }
    }

}
