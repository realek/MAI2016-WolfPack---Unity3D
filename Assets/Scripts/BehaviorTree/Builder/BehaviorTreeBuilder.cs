﻿using UnityEngine;
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
    /// Adds a leaf node to the current non-leaf node, can only be added to an decorator or composite node.
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
    /// Decorator type non-leaf node that automatically fails if its condition returns false, else the node it contains is also executed,
    /// this method allows you to give other decorators or composite nodes to the condition node;
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="condition">contained node execution condition</param>
    /// <param name="routine">contained node</param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginCondition(string name, Func<bool> condition)
    {
        if (m_routineStack.Count == 0)
        {
            Debug.LogError("Unable to add Condition node before adding a container node, eg: Selector");
            return this;
        }
        Condition cond = new Condition();
        cond.name = name;
        cond.LoadCondition(condition);
        m_routineStack.Peek().AddChild(cond);
        m_routineStack.Push(cond);
        return this;
    }


    /// <summary>
    /// Decorator type leaf node that automatically fails if its condition returns false, else the node it contains is also executed,
    /// this method automatically finishes the decorator node.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="condition">contained action execution condition</param>
    /// <param name="routine">contained action's delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddCondition(string name, Func<bool> condition, Func<RoutineState> executableAction)
    {
        if (m_routineStack.Count == 0)
        {
            Debug.LogError("Unable to add Condition node before adding a non-leaf node, eg: Selector");
            return this;
        }
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
    /// Decorator type leaf node that returns the oposite of the current output Success -> Failiure and vice versa.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="executableAction">action delegate</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddInverter(string name, Func<RoutineState> executableAction)
    {
        if (m_routineStack.Count == 0)
        {
            Debug.LogError("Unable to add Inverter node before adding a non-leaf node, eg: Selector");
            return this;
        }
        Action a = new Action();
        a.LoadAction(executableAction);
        Inverter inv = new Inverter();
        inv.name = name;
        inv.AddChild(a);
        m_routineStack.Peek().AddChild(inv);
        return this;
    }

    /// <summary>
    /// Decorator type non-leaf node that returns the oposite of the current output Success -> Failiure and vice versa.
    /// </summary>
    /// <param name="name">node name</param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginInverter(string name)
    {
        Inverter inv = new Inverter();
        inv.name = name;
        if(m_routineStack.Count > 0)
            m_routineStack.Peek().AddChild(inv);
        m_routineStack.Push(inv);
        return this;
    }

    /// <summary>
    /// Decorator type leaf node that returns that will run for a specified number of times, regardless of its result.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="executableAction">action delelegate</param>
    /// <param name="repeatCount">number of repeats before return</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AddRepeater(string name, Func<RoutineState> executableAction,int repeatCount)
    {
        if (m_routineStack.Count == 0)
        {
            Debug.LogError("Unable to add Inverter node before adding a non-leaf node, eg: Selector");
            return this;
        }
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
    /// Decorator type non-leaf node that returns that will run for a specified number of times, regardless of its result.
    /// </summary>
    /// <param name="name">node name</param>
    /// <param name="repeatCount">number of repeats before return</param>
    /// <returns></returns>
    public BehaviorTreeBuilder BeginRepeater(string name,int repeatCount)
    {
        Repeater reap = new Repeater();
        reap.name = name;
        reap.numberOfRepeats = repeatCount;
        if(m_routineStack.Count>0)
            m_routineStack.Peek().AddChild(reap);
        m_routineStack.Push(reap);
        return this;
    }

    /// <summary>
    /// Composite type non-leaf node that will fail if any of its children fails, it will succed only when all of its children succed.
    /// </summary>
    /// <param name="name">node name</param>
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
    /// Composite type non-leaf node that will succed if any of its children succed, it will fail if all nodes fail.
    /// </summary>
    /// <param name="name">node name</param>
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
    /// Used to add a completed tree object to a composite/decorator node.
    /// </summary>
    /// <param name="treeRoutine">tree object</param>
    /// <returns></returns>
    public BehaviorTreeBuilder AttachTree(BaseRoutine treeRoutine)
    {
        if (m_routineStack.Count > 0)
            m_routineStack.Peek().AddChild(treeRoutine);
        else
            Debug.LogError("No valid node to attach to.");
        return this;
    }


    /// <summary>
    /// Used to end composite/decorator Nodes ( eg: Selector,Sequence)
    /// </summary>
    /// <returns></returns>
    public BehaviorTreeBuilder FinishNode()
    {
        if (m_routineStack.Peek().HasChildren())
            m_currentRoutine = m_routineStack.Pop();
        else
            throw new Exception("Unable to call FinishNode() on empty composite/decorator node, use the Add"+m_routineStack.Peek().GetType()+" method, if you do not wish to create children for the node");
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
