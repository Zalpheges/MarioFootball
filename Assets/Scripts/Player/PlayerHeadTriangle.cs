using UnityEngine;

public class PlayerHeadTriangle : MonoBehaviour
{
    [SerializeField] private GameObject _triangle;
    [SerializeField] private Player _player;

    void LateUpdate()
    {
        if (_player.IsPiloted)
        {
            _triangle.SetActive(true);
            _triangle.transform.LookAt(Camera.main.transform.position);
        }
        else
        {
            _triangle.SetActive(false);
        }
    }
}


