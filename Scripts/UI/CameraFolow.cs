using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public class CameraFolow : MonoBehaviour 
{

    [SerializeField] private float _followSpeed;
    [SerializeField] private Tilemap tilemap;                     
    [SerializeField] private Color highlightColor; // Цвет подсветки
    [SerializeField] private Transform _target;
    [SerializeField] private LayerMask _maskPlayer;

    private Vector3Int[] areaPlayer; 
    private Vector3Int previousCell;             // Предыдущая ячейка
    private Vector3 previousLocation;
    
    private void Start()
    {
        areaPlayer = new Vector3Int[9];
        
    }

    private void Update()
    {       
        Vector3Int cellPosition = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (cellPosition != previousCell)
        {
            RestorePreviousTile();
            SetTileColour(highlightColor, cellPosition, tilemap);
            previousCell = cellPosition;
        }
        if (previousLocation != _target.position)
        {
            RestorArea();
            MapPlayerToMove();
            previousLocation = _target.position;
        }
    }
    /// <summary>
    /// Установка цвета для Tile
    /// </summary>
    /// <param name="colour"></param>
    /// <param name="position"></param>
    /// <param name="tilemap"></param>
    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {        
        tilemap.SetTileFlags(position, TileFlags.None);
                
        tilemap.SetColor(position, colour);
    }
    /// <summary>
    /// 
    /// </summary>
    private void RestorePreviousTile()
    {
        if (previousCell != transform.position)
        {            
            SetTileColour(Color.white, previousCell, tilemap);
            if (areaPlayer.Contains(previousCell))
            {
                SetTileColour(Color.yellow, previousCell, tilemap);
            }
        }
    }
   
    public bool IsCellCollider(Vector3 target) {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target,~_maskPlayer);
        if (hit.collider != null )
        {
            return false;
        }
        return true;
    }

    private void RestorArea()
    {
        if (areaPlayer.Length == 0) return;
        foreach (Vector3Int tile in areaPlayer) {
            SetTileColour(Color.white, tile, tilemap);
        }
    }

    private void MapPlayerToMove() {
        int index = 0;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                Vector3 vector = new Vector3(transform.position.x + x, transform.position.y + y);
                Vector3Int cellPosition = tilemap.WorldToCell(vector);
                if (IsCellCollider(vector)) {                   
                    areaPlayer[index] = cellPosition;
                    SetTileColour(Color.yellow, cellPosition, tilemap);
                }
                index++;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_target)
        {
            Vector3 currentPosition = Vector3.Lerp(transform.position, new Vector3(_target.position.x, _target.position.y, transform.position.z) , _followSpeed * Time.fixedDeltaTime);
            transform.position = currentPosition;            
        }
    }
   
}
