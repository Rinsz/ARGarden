using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    public class StateMachine<TStateKey, TState>
        where TStateKey : Enum
        where TState : class
    {
        private readonly IReadOnlyDictionary<TStateKey, TState> states;

        public TState CurrentState { get; private set; }

        public StateMachine(IReadOnlyDictionary<TStateKey, TState> states, TStateKey initialStateKey)
        {
            this.states = states;
            SetState(initialStateKey);
        }

        public void SetState(TStateKey key)
        {
            if (!states.ContainsKey(key)) throw new InvalidEnumArgumentException();
            CurrentState = states[key];
        }
    }
}