using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public abstract class Node : ScriptableObject {
        public enum State {
            Running,
            Failure,
            Success
        }

        [HideInInspector] public State state = State.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Context context;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string description;
        public bool drawGizmos = false;

        public State Update() {

            if (!started) {
                OnStart();
                started = true;
            }

            // ✅ 현재 실행 중인 노드를 Context에 등록
            context.currentExecutingNode = this;

            state = OnUpdate();

            if (state != State.Running) {
                OnStop();
                started = false;
            }
            if (state == State.Running)
            {
                //OnNodeExecutionChanged?.Invoke(this);
            }

            return state;
        }

        public virtual Node Clone() {
            return Instantiate(this);
        }

        public void Abort() {
            BehaviourTree.Traverse(this, (node) => {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }

        public virtual void OnDrawGizmos() { }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}