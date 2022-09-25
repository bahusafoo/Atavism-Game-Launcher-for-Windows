using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    public bool working = false;
    public GameObject LoadingIconObject;
    private void Start()
    {
        working = true;
    }
    private void Update()
    {
        if (working == true)
        {
            LoadingIconObject.transform.Rotate(new Vector3(0, 0, -360) * Time.deltaTime, Space.Self);
        } else
        {
            this.gameObject.SetActive(false);
        }
    }
}
