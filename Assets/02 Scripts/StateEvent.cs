using System;
using System.Collections.Generic;

// Warning! 
// This is somewhat fragile Event pattern implementation. Recommended they aren't used outside of the state machine
// 
namespace MonsterLove.StateMachine
{
    public class StateEvent
    {
        private Func<int> getStateInt;
        private Func<bool> isInvokeAllowed;
        private Action[] routingTable;

        public StateEvent(Func<bool> isInvokeAllowed, Func<int> stateProvider, int capacity)
        {
            this.isInvokeAllowed = isInvokeAllowed;
            this.getStateInt = stateProvider;
            routingTable = new Action[capacity];
        }

        internal void AddListener(int stateInt, Action listener)
        {
            routingTable[stateInt] = listener;
        }

        public void Invoke()
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }

            Action call = routingTable[getStateInt()];
            if (call != null)
            {
                call();
                return;
            }
        }
    }

    public class StateEvent<T>
    {
        private Func<int> getStateInt;
        private Func<bool> isInvokeAllowed;
        private Action<T>[] routingTable;

        public StateEvent(Func<bool> isInvokeAllowed, Func<int> stateProvider, int capacity)
        {
            this.isInvokeAllowed = isInvokeAllowed;
            this.getStateInt = stateProvider;
            routingTable = new Action<T>[capacity];
        }

        internal void AddListener(int stateInt, Action<T> listener)
        {
            routingTable[stateInt] = listener;
        }

        public void Invoke(T param)
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }

            Action<T> call = routingTable[getStateInt()];
            if (call != null)
            {
                call(param);
                return;
            }
        }
    }

    public class StateEvent<T1, T2>
    {
        private Func<int> getStateInt;
        private Func<bool> isInvokeAllowed;
        private Action<T1, T2>[] routingTable;

        public StateEvent(Func<bool> isInvokeAllowed, Func<int> stateProvider, int capacity)
        {
            this.isInvokeAllowed = isInvokeAllowed;
            this.getStateInt = stateProvider;
            routingTable = new Action<T1, T2>[capacity];
        }

        internal void AddListener(int stateInt, Action<T1, T2> listener)
        {
            routingTable[stateInt] = listener;
        }

        public void Invoke(T1 param1, T2 param2)
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }

            Action<T1, T2> call = routingTable[getStateInt()];
            if (call != null)
            {
                call(param1, param2);
                return;
            }
        }
    }
}