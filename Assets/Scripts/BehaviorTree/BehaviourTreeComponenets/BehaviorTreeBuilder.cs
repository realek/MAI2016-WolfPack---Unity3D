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
    /// Decorator type node that automatically fails if its condition returns false, else the node it contains is also executed,
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
    /// Decorator type node that automatically fails if its condition returns false, else the node it contains is also executed,
    /// this method automatically finishes the decorator node.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="condition">contained action execution condition</param>
    /// <param name="routine">contained action's delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddCondition(string name, Func<bool> condition, Func<RoutineState> executableAction)
    {
        Action a = new Action();
        a.LoadAction(executableAction);
        Condition cond = new Condition();
        cond.name = name;
        cond.LoadCondition(condition);
        cond.AddChild(a);
        m_routineStack.Peek().AddChild(cond);
        return this;
    }

    /// <summary>
    /// Decorator type node that returns the oposite of the current output Success -> Failiure and vice versa.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="executableAction">action delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddInverter(string name, Func<RoutineState> executableAction)
    {
        Action a = new Action();
        a.LoadAction(executableAction);
        Inverter inv = new Inverter();
        inv.name = name;
        inv.AddChild(a);
        m_routineStack.Peek().AddChild(inv);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginInverter(string name)
    {
        Inverter inv = new Inverter();
        inv.name = name;
        m_routineStack.Peek().AddChild(inv);
        m_routineStack.Push(inv);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="executableAction"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddRepeater(string name, Func<RoutineState> executableAction,int repeatCount)
    {
        Action a = new Action();
        a.LoadAction(executableAction);
        Repeater reap = new Repeater();
        reap.name = name;
        reap.numberOfRepeats = repeatCount;
        reap.AddChild(a);
        m_routineStack.Peek().AddChild(reap);
        return this;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginRepeater(string name,int repeatCount)
    {
        Repeater reap = new Repeater();
        reap.name = name;
        if(m_routineStack.Count>0)
            m_routineStack.Peek().AddChild(reap);
        m_routineStack.Push(reap);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginSequence(string name)
    {
        Sequence seq = new Sequence();
        seq.name = name;

        if(m_routineStack.Count > 0)
            m_routineStack.Peek().AddChild(seq);
        m_routineStack.Push(seq);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginSelector(string name)
    {
        Selector sel = new Selector();
        sel.name = name;
        if(m_routineStack.Count>0)
            m_routineStack.Peek().AddChild(sel);
        m_routineStack.Push(sel);
        return this;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="treeRoutine"></param>
    /// <returns></returns>
    public BehaviorTreeBuilder AttachTree(BaseRoutine treeRoutine)
    {
        m_routineStack.Peek().AddChild(treeRoutine);
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
