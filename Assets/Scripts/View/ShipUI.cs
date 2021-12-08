using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipUI : MonoBehaviour
{

    public Ship shipToTrack;
    protected SpriteRenderer spriteRenderer;
    protected Tilemap shipMap;

    void Awake()
    {
        this.shipMap = FindObjectOfType<Tilemap>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Transform transform = this.transform;
        transform.position = shipMap.CellToWorld(shipToTrack.gridPosition);
        transform.rotation = Quaternion.AngleAxis((int)shipToTrack.facing, Vector3.forward);
        spriteRenderer.color = shipToTrack.affiliation == Affiliation.Player ? Color.green : Color.red;
    }
}
