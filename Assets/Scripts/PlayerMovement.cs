using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float timeToMove = 0.2f;
    private bool _isMoving;
    private Vector3 _origPos, _targetPos;
    
    // Update is called once per frame
    private void Update()
    {
        if (_isMoving || !TurnManager.Instance.CanMove()) return;
        if (Input.GetKey(KeyCode.W))
            StartCoroutine(MovePlayer(Vector3.up));
        if (Input.GetKey(KeyCode.A))
            StartCoroutine(MovePlayer(Vector3.left));
        if (Input.GetKey(KeyCode.S))
            StartCoroutine(MovePlayer(Vector3.down));
        if (Input.GetKey(KeyCode.D))
            StartCoroutine(MovePlayer(Vector3.right));
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {

        _isMoving = true;

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
        
        _isMoving = false;
    }
}