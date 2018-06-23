using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {

    // Единственный экземпляр класса
    static public BoidSpawner S;

    // Эти поля позволяют регулировать боиды как группу
    public int numBoids = 100;
    public GameObject boidPrefab;
    public float spawnRadius = 100f;
    public float spawnVelocity = 10f;
    public float minVelocity = 0f;
    public float maxVelocity = 30f;
    public float nearDist = 30f;
    public float collisionDist = 5f;
    public float velocityMathingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float collisionAvoidanceAmt = -0.5f;
    public float mouseAttractionAmt = 0.01f;
    public float mouseAvoidanceAmt = 0.75f;
    public float mouseAvoidanceDist = 15f;
    public float velocityLerpAmt = 0.25f;

    public bool _______________;

    public Vector3 mousePos;

	// Use this for initialization
	void Start () {
        // Задаём синглтон
        S = this;
        // Создаём боидов
        for (int i = 0; i < numBoids; i++) {
            Instantiate(boidPrefab);
        }
	}

    void LateUpdate() {
        // Отслеживаем позицию мышки
        Vector3 mousePos2d = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.y);
        mousePos = this.GetComponent<Camera>().ScreenToWorldPoint(mousePos2d);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
