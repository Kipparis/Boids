using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    // Список который содержит все боиды
    static public List<Boid> boids;

    // Заметка: этот код не использует скелет, он обрабатывает скорость напрямую
    public Vector3 velocity;
    public Vector3 newVelocity;
    public Vector3 newPosition;

    public List<Boid> neighbors;    // Просто соседи
    public List<Boid> collisionRisks;   // Слишком близкие соседи
    public Boid closest;    // Самый близкий челик

    void Awake() {
        // Создаём список если его ещё нет
        if (boids == null) boids = new List<Boid>();
        boids.Add(this);

        // Задаём рандомную позицию и скорость
        Vector3 randPos = Random.insideUnitSphere * BoidSpawner.S.spawnRadius;
        randPos.y = 0;
        this.transform.position = randPos;
        velocity = Random.onUnitSphere * BoidSpawner.S.spawnVelocity;
        velocity.y = 0; //////////////////////// mine

        // Создаём два списка
        neighbors = new List<Boid>();
        collisionRisks = new List<Boid>();

        // Делаем этот объекст зависимым от Boids
        this.transform.parent = GameObject.Find("Boids").transform;

        // Даём боиду рандомный цвет, но не слишком тёмный
        Color randColor = Color.black;
        while (randColor.r + randColor.g + randColor.b < 1.0f) {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>(); 
        foreach (Renderer r in rends) r.material.color = randColor;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Находим список ближайших боидов
        List<Boid> neighbors = GetNeighbors(this);

        // Создаём новую скорость и новую позицию для текущих значений
        newVelocity = velocity;
        newPosition = this.transform.position;

        // Усреднение скорости: даёт скорость схожую с рядом летящими челобасами
        Vector3 neighborVel = GetAverageVelocity(neighbors);
        newVelocity += neighborVel * BoidSpawner.S.velocityMathingAmt;

        // Кучкование: стремление к центру
        Vector3 neighborCenterOffset = GetAveragePosition(neighbors) - this.transform.position;
        newVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;

        // Избегание столкновений
        Vector3 dist;
        if(collisionRisks.Count > 0) {
            Vector3 collisionAveragePos = GetAveragePosition(collisionRisks);
            dist = collisionAveragePos - this.transform.position;
            newVelocity += dist * BoidSpawner.S.collisionAvoidanceAmt;
        }

        // Приближение к мыши
        dist = BoidSpawner.S.mousePos - this.transform.position;
        if (dist.magnitude > BoidSpawner.S.mouseAvoidanceDist) {
            newVelocity += dist * BoidSpawner.S.mouseAttractionAmt;
        } else {
            newVelocity -= dist.normalized * BoidSpawner.S.mouseAvoidanceDist * BoidSpawner.S.mouseAvoidanceAmt;
        }
        // Новая скорость и позиция готовы, но позволим сначала всем боидам просчитать это
	}

    void LateUpdate() {
        velocity = (1 - BoidSpawner.S.velocityLerpAmt) * velocity + BoidSpawner.S.velocityLerpAmt * newVelocity;
        // Убеждаемся что скорость в нужных пределах
        if (velocity.magnitude > BoidSpawner.S.maxVelocity) {
            velocity = velocity.normalized * BoidSpawner.S.maxVelocity;
        }
        if (velocity.magnitude < BoidSpawner.S.minVelocity) {
            velocity = velocity.normalized * BoidSpawner.S.minVelocity;
        }

        // Выбираем новую позицию
        newPosition = this.transform.position + velocity * Time.deltaTime;
        // Удерживаем всё в плоскости XZ
        newPosition.y = 0;
        // Боид смотрит в сторону новой позиции
        this.transform.LookAt(newPosition);
        // Перемещаем боид 
        this.transform.position = newPosition;
    }

    // Находит и соседей, и опасные риски, и ближайшого боида
    // бои - боид оф интерест
    public List<Boid>GetNeighbors(Boid boi) {
        float closestDist = float.MaxValue;
        Vector3 delta;
        float dist;
        neighbors.Clear();
        collisionRisks.Clear();

        foreach (Boid b in boids) {
            if (b == boi) continue;
            delta = b.transform.position - boi.transform.position;
            dist = delta.magnitude;
            if(dist < closestDist) {
                closestDist = dist;
                closest = b;
            }
            if (dist < BoidSpawner.S.nearDist) neighbors.Add(b);
            if (dist < BoidSpawner.S.collisionDist) collisionRisks.Add(b);
            if (neighbors.Count == 0) neighbors.Add(closest);
            return (neighbors);
        }
        return neighbors;
    }

    public Vector3 GetAveragePosition(List<Boid> someBoids) {
        Vector3 sum = Vector3.zero;
        foreach(Boid b in someBoids) {
            sum += b.transform.position;
        }
        Vector3 center = sum / someBoids.Count;
        return (center);
    }

    public Vector3 GetAverageVelocity(List<Boid> someBoids) {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids) {
            sum += b.velocity;
        }
        Vector3 avg = sum / someBoids.Count;
        return (avg);
    }
}
