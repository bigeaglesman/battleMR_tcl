using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;

        // Storage container object to hold game object subsystems
        Context context;

        //상태 갱신
        UnitStats unitStatsVisual;
        UnitState unitStateNetwork;

        Node previousNode = null;

        //실행 여부
        bool isTreeRunning = false;

        // 외부 UI 버튼에서 호출
        public void StartTree()
        {
            isTreeRunning = true;
            //Debug.Log("[BehaviourTreeRunner] 트리 실행 시작됨");
        }

        // Start is called before the first frame update
        void Start() {
            unitStatsVisual = GetComponent<UnitStats>(); //상태 갱신
            unitStateNetwork = GetComponent<UnitState>(); //상태 갱신

            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
        }

        // Update is called once per frame
        void Update() {
            if (tree && isTreeRunning) {
                tree.Update();

                // 상태 갱신
                Node currentNode = context.currentExecutingNode;

                if (currentNode != previousNode)
                {
                    previousNode = currentNode;
                    OnNodeExecutionChanged(currentNode);
                }
            }
        }


        void OnNodeExecutionChanged(Node node)
        {
            //if (unitStateNetwork == null || unitStatsVisual == null || node == null) return;
            //if (unitStatsVisual == null || node == null) return;
            if (node == null) return;

            string nodeName = node.GetType().Name;
            //Debug.Log($"[BT 상태체크] 현재 실행 중 노드: {nodeName}");
            //Debug.Log($"[BT 상태전환] node: {nodeName}, 이전 상태: {unitStatsVisual.unitStats}");

            if (nodeName == "MoveToTarget")
            {
                if (unitStatsVisual != null) 
                    unitStatsVisual.unitStats = UnitStats.Stats.Run;
                if (unitStateNetwork != null) 
                    unitStateNetwork.unitState = UnitState.State.Run;

            }
            else if (nodeName == "Attack" || nodeName == "WarriorAttack")
            {
                if (unitStatsVisual != null) 
                    unitStatsVisual.unitStats = UnitStats.Stats.Attack;
                if (unitStateNetwork != null) 
                    unitStateNetwork.unitState = UnitState.State.Attack;

            }
            /*
            else
            {
                unitStats.unitStats = UnitStats.Stats.Idle;
                //unitStats.unitStats = UnitStats.Stats.Idle;
            }*/
            //Debug.Log($"[BT 상태] 현재 실행 중인 노드: {node.GetType().Name}");
        }

        /*
        void OnNodeExecutionChanged(Node node)
        {
            if (unitState == null || node == null) return;

            string nodeName = node.GetType().Name;
            unitState.unitState = nodeName;

            Debug.Log($"[UnitState] 현재 실행 중인 노드: {nodeName}");
        }
        */


        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}