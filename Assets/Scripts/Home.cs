using UnityEngine;

public class Home : MonoBehaviour
{
    public GameObject frog;

    private void OnEnable()
    {
        this.frog.SetActive(true);
    }

    private void OnDisable()
    {
        this.frog.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            this.enabled = true;

            FindObjectOfType<GameManager>().HomeReached();
        }
    }

}
