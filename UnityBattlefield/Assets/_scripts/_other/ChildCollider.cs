
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Enables a child transform to pass collision events up to a parent.  The parent transform uses
/// the "helper" functions to delegate "enter" and "stay" callbacks to child transforms with
/// colliders.  Child transforms can
/// </summary>
public class ChildCollider : MonoBehaviour
{
    // delegate function for trigger entry (the parent delegates the handler to the child...)
    public delegate void OnTriggerEnterDelegate( Collider collider, Transform childTransform );
    // same function for collision entry.
    public delegate void OnTriggerStayDelegate( Collider collider, Transform childTransform );
    // same for collision stay
    public delegate void OnCollisionEnterDelegate( Collision collision, Transform childTransform );
    // same for trigger stay
    public delegate void OnCollisionStayDelegate( Collision collision, Transform childTransform );

    // lists of refereneces to delegate functions...
    public List<OnTriggerEnterDelegate> m_triggerEnterDelegates;
    public List<OnTriggerStayDelegate> m_triggerStayDelegates;
    public List<OnCollisionEnterDelegate> m_collisionEnterDelegates;
    public List<OnCollisionStayDelegate> m_collisionStayDelegates;

    private bool m_needsInit = true;

    private void Init()
    {
        if(m_needsInit)
        {
            m_triggerEnterDelegates = new List<OnTriggerEnterDelegate>();
            m_triggerStayDelegates = new List<OnTriggerStayDelegate>();
            m_collisionEnterDelegates = new List<OnCollisionEnterDelegate>();
            m_collisionStayDelegates = new List<OnCollisionStayDelegate>();
            m_needsInit = false;
        }
    }

    /// <summary>
    /// Passes trigger entry to delegate functions
    /// </summary>
    private void OnTriggerEnter (Collider collider)
    {
        foreach(OnTriggerEnterDelegate handler in m_triggerEnterDelegates)
            handler(collider, this.transform);
    }

    /// <summary>
    /// Passes trigger stay to delegate functions
    /// </summary>
    private void OnTriggerStay (Collider collider)
    {
        foreach(OnTriggerStayDelegate handler in m_triggerStayDelegates)
            handler(collider, this.transform);
    }

    /// <summary>
    /// Passes collision entry to delegate functions
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        foreach(OnCollisionEnterDelegate handler in m_collisionEnterDelegates)
            handler(collision, this.transform);
    }

    /// <summary>
    /// Passes collision stay to delegate functions
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        foreach(OnCollisionStayDelegate handler in m_collisionStayDelegates)
            handler(collision, this.transform);
    }

    // Helper functions...
    public static int AttachChildEnterDelegate(Transform parent, OnTriggerEnterDelegate triggerEnterDelegate)
    {
        return AttachChildColliders(parent, triggerEnterDelegate, null, null, null);
    }

    public static int AttachChildEnterDelegate(Transform parent, OnCollisionEnterDelegate collisionEnterDelegate)
    {
        return AttachChildColliders(parent, null, null , collisionEnterDelegate, null);
    }

    public static int AttachChildStayDelegate(Transform parent, OnTriggerStayDelegate triggerStayDelegate)
    {
        return AttachChildColliders(parent, null, triggerStayDelegate, null, null);
    }

    public static int AttachChildStayDelegate(Transform parent, OnCollisionStayDelegate collisionStayDelegate)
    {
        return AttachChildColliders(parent, null, null, null, collisionStayDelegate);
    }



    /// <summary>
    /// Attachs child colliders to a parent transform.  Searches all children for "childCollider" component
    /// and then sets (passed) delegate funcitons.  Returns count of child colliders set.
    /// Unity 4.0 has problems with namespaces + optional parameters so all parameters are necessary.
    /// </summary>
    private static int AttachChildColliders(Transform parent,
                                            OnTriggerEnterDelegate triggerEnterDelegate,
                                            OnTriggerStayDelegate triggerStayDelegate,
                                            OnCollisionEnterDelegate collisionEnterDelegate,
                                            OnCollisionStayDelegate collisionStayDelegate)
    {
        int childCount = 0;
		Debug.Assert(parent != null,"Parent transform missing in AttachChildColliders");

        // look gof all children with colliders...
        Collider[] colliders = parent.gameObject.GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            // don't add to parent.
            if(collider != parent.GetComponent<Collider>())
            {
                GameObject go = collider.gameObject;

                // add a child collider component to them if they don't have one already
                ChildCollider childCollider = go.GetComponent<ChildCollider>() as ChildCollider;
                if(childCollider == null)
                {
                    childCollider = go.AddComponent(typeof(ChildCollider)) as ChildCollider;
                    Debug.Assert(childCollider != null,"Failed to add child collider to " + go.name);
                }

                // setup lists.
                childCollider.Init();

                // attach delegate to child collider...
                // To Do:  Figure out a way to move this repeat code into subroutine...
				Debug.Assert(childCollider.m_triggerEnterDelegates != null);
                if(triggerEnterDelegate != null)
                    childCollider.m_triggerEnterDelegates.Add(triggerEnterDelegate);

                Debug.Assert(childCollider.m_triggerStayDelegates != null);
                if(triggerStayDelegate != null)
                    childCollider.m_triggerStayDelegates.Add(triggerStayDelegate);

				Debug.Assert(childCollider.m_collisionEnterDelegates != null);
                if(collisionEnterDelegate != null)
                    childCollider.m_collisionEnterDelegates.Add(collisionEnterDelegate);

				Debug.Assert(childCollider.m_collisionStayDelegates != null);
                if(collisionStayDelegate != null)
                    childCollider.m_collisionStayDelegates.Add(collisionStayDelegate);

                childCount += 1;
            }
        }

        return childCount;
    }
}


