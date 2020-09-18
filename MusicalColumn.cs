using UnityEngine;

public class MusicalColumn : MonoBehaviour {

    private bool cloned = false;

    [SerializeField]
    private GameObject clone;

    [SerializeField]
    private Vector3 spawn;

    [SerializeField]
    private GameEvents eventSystem;

    void Awake() => transform.RandomizeSize();
    
    void Start() {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        eventSystem = GameEvents.instance;
        clone = gameObject;
        if(spawn.y == 0)
            spawn.y = -5.659f;
        spawn.x = Random.Range(-5.68f, 5.67f);
        cloned = false;
        if(Random.Range(0, 9999) == 1) {
            Debug.LogError(gameObject.name + " is now special!");
            r.sprite = eventSystem.specialSprite;
        }
        r.flipX = Random.Range(0, 2) == 0f;
    }

    void Update() {
        transform.position = new Vector3(transform.position.x, transform.position.y, 150);
        if(transform.position.y >= 4.357 && !cloned) {
            cloned = true;
            createObject();
        }
        
        if(transform.position.y >= 6.718 && cloned)
            Destroy(this.gameObject); 
        else transform.Translate(Vector3.up * Time.deltaTime * eventSystem.musicalSpeed, Space.World);
    }

    public void createObject() {
        Instantiate(clone, spawn, Quaternion.identity).name = gameObject.name;
    }
}
