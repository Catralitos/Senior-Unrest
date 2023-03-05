using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float timeToMove = 0.2f;
        public bool IsMoving  {get; private set; }
        private Vector3 _origPos, _targetPos;
    
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
            TurnManager.Instance.ProcessTurn(_targetPos);

            while(elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(_origPos, _targetPos, (elapsedTime / timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = _targetPos;
        
            IsMoving = false;
        }
    }
}