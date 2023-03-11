using System.Collections;
using Managers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public bool IsMoving  {get; private set; }
        private Vector3 _origPos, _targetPos;
        public LayerMask obstacles;
        
        private void Update()
        {
            if (!TurnManager.Instance.CanMove()) return;
            if (Input.GetKeyDown(KeyCode.W))
                StartCoroutine(MovePlayer(Vector3.up));
            if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(MovePlayer(Vector3.left));
            if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(MovePlayer(Vector3.down));
            if (Input.GetKeyDown(KeyCode.D))
                StartCoroutine(MovePlayer(Vector3.right));
        }

        private IEnumerator MovePlayer(Vector3 direction)
        {

            IsMoving = true;

            float elapsedTime = 0.0f;

            _origPos = transform.position;
            _targetPos = _origPos + direction;

            if (Physics2D.OverlapBox(_targetPos, new Vector2(0.5f, 0.5f), 0, obstacles) != null)
            {
                IsMoving = false;              

                yield break;
            }

            TurnManager.Instance.ProcessTurn(_targetPos);

            //audio do Nuno :)
            PlayerEntity.Instance.audioManager.Play("Moving");

            while (elapsedTime < TurnManager.Instance.unitTimeToMove)
            {
                transform.position = Vector3.Lerp(_origPos, _targetPos, (elapsedTime / TurnManager.Instance.unitTimeToMove));
                elapsedTime += Time.deltaTime;                               

                yield return null;

            }

            

            transform.position = _targetPos;
            IsMoving = false;
        }
    }
}