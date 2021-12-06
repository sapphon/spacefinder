using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipUI : MonoBehaviour
{

    public Ship shipToTrack;
    protected SpriteRenderer renderer;
    protected Tilemap shipMap;

    void Awake()
    {
        this.shipMap = FindObjectOfType<Tilemap>();
        this.renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Transform transform = this.transform;
        transform.position = shipMap.CellToWorld(shipToTrack.gridPosition);
        transform.rotation = Quaternion.AngleAxis(shipToTrack.rotation, Vector3.forward);
        renderer.color = shipToTrack.affiliation == Affiliation.Player ? Color.green : Color.red;
    }
}
