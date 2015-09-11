
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Simple state machine manager -- used for basic, fairly linier state flow. 
    /// </summary>
    [System.Serializable]
    public class SimpleStateMachine
    {
        // Delegate update handling functions for each game state. Passed the amount of time this state has been running.
        public delegate void    StateUpdateHandler(float stateTime);

        private StateUpdateHandler      m_currentState;                 // current state... used to control overall game flow.
        private string                  m_debugCurrentState;
        private float                   m_stateTime;                    // The amount of time in the state (0 means new state;
        private string                  m_stateMachineName;             // if not null, will print info on changes of state
        private static int              k_machineId        = 0;


		/// <summary>
		/// Initializes a new instance of state machine.  State machines can be associated with individual game objects (for example)
		/// </summary>
		/// <param name="stateMachineName">State machine name.</param>
        public SimpleStateMachine(string stateMachineName)
        {
            m_stateTime = 0.0f;
            if(stateMachineName == null)
            {
                m_stateMachineName = null;
            }
            else
            {
                m_stateMachineName = string.Format("({1}) {0}", stateMachineName, k_machineId++);
            }
        }

        /// <summary>
        /// Called from gameobject "Update" routine to update the state. On each update, the current state handler
		/// for the parent object will be called.
		/// Pass this routine the amount of time that has passed since last update.
        /// </summary>
        public void Update(float deltaTime)
        {
            DeltaTime = deltaTime;

            if(CurrentState != null)
            {
                StateUpdateHandler oldState = CurrentState;

                // call state handler
                CurrentState(m_stateTime);

                // don't update time if the update routine changed state...
                if(oldState == CurrentState)
                    m_stateTime += deltaTime;
            }
        }

        /// <summary>
        /// Change the state -- the parent object registers a state by passing a reference to the parent's state handler.
		/// ex:  "m_stateMachine.SetState (UpdateCountDown2);"  sets the current state handler to "UpdateCountdown2"
        /// </summary>
        public void SetState(StateUpdateHandler newState)
        {
            if(CurrentState != newState)
                CurrentState = newState;
        }

        /// <summary>
        /// Resets the state timer to 0.
        /// </summary>
        public void ResetTimer()
        {
            m_stateTime = 0.0f;
        }

        /// <summary>
        /// Gets or sets the state. Changing this value will reset associated timers, etc.
        /// Calling this repeatedly will reset the state time
        /// </summary>
        public StateUpdateHandler CurrentState
        {
            get
            {
                return m_currentState;
            }

            private set
            {
                if(m_currentState != value)
                {
                    m_currentState = value;

                    // print state change info to log.  This gets the name of the delegate function
                    if(m_stateMachineName != null)
                    {
                        if (m_currentState != null)
                        {
                            m_debugCurrentState = string.Empty;
                            System.Delegate[] dels = m_currentState.GetInvocationList();
                            foreach (System.Delegate del in dels)
                            {
                                m_debugCurrentState += del.Method.Name;
                                //Debug.Log(string.Format("StateMachine -- {0} Changed To: {1}", m_stateMachineName, del.Method.Name));
                            }
                        }
                        else
                        {
							//Debug.Log(string.Format("StateMachine -- {0} Changed To: NULL", m_stateMachineName));
                            m_debugCurrentState = "NULL";
                        }
                    }
                }

                ResetTimer();
            }
        }

        /// <summary>
        /// Gets or sets the delta time -- the time step that was passed to the state machine.
        /// </summary>
        public float DeltaTime {get;private set; }
    }
}
