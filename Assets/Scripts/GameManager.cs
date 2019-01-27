using UnityEngine;
using UnityTools.DataManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Player playerPrefab;
    [SerializeField]
    private Settings settings;
    [SerializeField]
    private LayerMask mapLayerMask;
    [SerializeField]
    private Bounds mapBounds;

    [SerializeField]
    private KeyBindings[] allPlayersKeyBindings;
    [SerializeField]
    private FloatValue[] allPlayersHealthValues;
    [SerializeField]
    private FloatValue[] allPlayersShellValues;
    [SerializeField]
    private IntValue[] allPlayersRoundWonValues;
    [SerializeField]
    private Material[] allPlayersBaseMaterials;
    [SerializeField]
    private RectTransform[] allPlayersHealthUI;

    [SerializeField]
    private Transform[] twoPlayersSpawnPositions;
    [SerializeField]
    private Transform[] threePlayersSpawnPositions;
    [SerializeField]
    private Transform[] fourPlayersSpawnPositions;

    private static GameManager _instance;

    private Camera mainCamera;

    private List<Player> players;
    private List<Shell> shells;
    private List<Transform> props;

    private bool isPlayingRound = false;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("There should be only one GameManager in a scene.");
            DestroyImmediate(gameObject);
        }

        Debug.Log("Connected controllers:");
        foreach(var name in Input.GetJoystickNames())
        {
            Debug.Log("Controller: " + name);
        }

        mainCamera = Camera.main;

        foreach (var r in allPlayersRoundWonValues)
        {
            r.SetValue(0);
        }

        //SetupMapBounds();

        PlacePlayers();
        PlaceShells();
        PlaceProps();

        FindObjectOfType<StartRoundCountDown>().StartCountDown();
    }

    public static void StartRound()
    {
        foreach (var player in _instance.players)
        {
            player.EnableControls();
        }

        _instance.isPlayingRound = true;
    }

    public static void PlayerDied()
    {
        List<Player> alivePlayers = _instance.players.Where(pl => pl.IsAlive).ToList();

        if(_instance.isPlayingRound &&  alivePlayers.Count == 1)
        {
            Player winner = alivePlayers[0];

            IntValue winnerRoundsWon = _instance.allPlayersRoundWonValues[alivePlayers.IndexOf(winner)];
            winnerRoundsWon.SetValue(winnerRoundsWon.Value + 1);

            if (winnerRoundsWon.Value == 2)
            {
                // TODO: win 
                Debug.Log("WIN");
                return;
            }

            Destroy(winner.gameObject);

            for (int i = 0; i < _instance.shells.Count; i++)
            {
                if (_instance.shells[i] != null)
                {
                    Destroy(_instance.shells[i].gameObject);
                }
            }

            //for (int i = 0; i < _instance.props.Count; i++)
            //{
            //    Destroy(_instance.props[i].gameObject);
            //}

            _instance.PlacePlayers();
            _instance.PlaceShells();
            _instance.PlaceProps();

            _instance.isPlayingRound = false;
            FindObjectOfType<StartRoundCountDown>().StartCountDown();
        }
    }

    private void PlacePlayers()
    {
        players = new List<Player>();

        Transform[] spawnPositions = (settings.NumberOfPlayers == 2) ? twoPlayersSpawnPositions :
                                        (settings.NumberOfPlayers == 3) ? threePlayersSpawnPositions : fourPlayersSpawnPositions;

        for (int i = 0; i < settings.NumberOfPlayers; i++)
        {
            Player player = Instantiate(playerPrefab, spawnPositions[i].position, Quaternion.identity);
            player.SetKeyBindings(allPlayersKeyBindings[i]);
            player.SetHealthValue(allPlayersHealthValues[i]);
            player.SetShellValue(allPlayersShellValues[i]);
            player.SetBaseMaterial(allPlayersBaseMaterials[i]);
            player.SetID(i + 1);
            players.Add(player);
        }

        for (int i = 0; i < allPlayersHealthUI.Length; i++)
        {
            allPlayersHealthUI[i].gameObject.SetActive(i < settings.NumberOfPlayers);
        }
    }

    private void PlaceProps()
    {

    }

    private void PlaceShells()
    {
        shells = new List<Shell>();

        int numberOfShells = settings.NumberOfPlayers * 2;
        Shell[] allShells = ConstantsManager.AllShellPrefabs;
        for (int i = 0; i < numberOfShells; i++)
        {
            Vector3 position = RandomPoint();
            Quaternion rotation = Quaternion.identity;
            Shell shell = Instantiate(allShells[Random.Range(0, allShells.Length)], position, rotation);
            shells.Add(shell);
        }
    }

    private void SetupMapBounds()
    {
        Ray bottomLeftRay = mainCamera.ScreenPointToRay(new Vector2(0, 0));
        Ray bottomRightRay = mainCamera.ScreenPointToRay(new Vector2(Screen.width, 0));
        Ray topLeftRay = mainCamera.ScreenPointToRay(new Vector2(0, Screen.height));
        Ray topRightRay = mainCamera.ScreenPointToRay(new Vector2(Screen.width, Screen.height));

        RaycastHit hitInfo;
        Physics.Raycast(bottomLeftRay, out hitInfo, float.MaxValue, mapLayerMask);
        Vector3 bottomLeftPoint = hitInfo.point;
        Physics.Raycast(bottomRightRay, out hitInfo, float.MaxValue, mapLayerMask);
        Vector3 bottomRightPoint = hitInfo.point;
        Physics.Raycast(topLeftRay, out hitInfo, float.MaxValue, mapLayerMask);
        Vector3 topLeftPoint = hitInfo.point;
        Physics.Raycast(topRightRay, out hitInfo, float.MaxValue, mapLayerMask);
        Vector3 topRightPoint = hitInfo.point;

        void SetupBoundGameObject(Vector3 a, Vector3 b)
        {
            GameObject bound = new GameObject();
            bound.name = "Bound";
            bound.transform.SetParent(transform);
            bound.transform.rotation = Quaternion.Euler(0, Vector3.Angle(a - b, Vector3.forward), 0);
            bound.transform.position = (a + b) / 2;
            BoxCollider collider = bound.AddComponent<BoxCollider>();
            collider.size = new Vector3(1, 100, (a - b).magnitude);
        }

        SetupBoundGameObject(bottomLeftPoint, topLeftPoint);
        SetupBoundGameObject(topRightPoint, bottomRightPoint);
        SetupBoundGameObject(topRightPoint, topLeftPoint);
        SetupBoundGameObject(bottomRightPoint, bottomLeftPoint);
    }

    private Vector3 RandomPoint()
    {
        Vector3 p = new Vector3(Random.Range(mapBounds.min.x, mapBounds.max.x), 0, Random.Range(mapBounds.min.z, mapBounds.max.z));

        float closestPlayerDistance = float.MaxValue;
        if(players != null && players.Count > 0)
        {
            closestPlayerDistance = players.Select(pl => (pl.transform.position - p).sqrMagnitude).Min();
        }

        float closestShellDistance = float.MaxValue;
        if (shells != null && shells.Count > 0)
        {
            closestShellDistance = shells.Select(s => (s.transform.position - p).sqrMagnitude).Min();
        }

        float closestPropDistance = float.MaxValue;
        if (props != null && props.Count > 0)
        {
            closestPropDistance = props.Select(pr => (pr.position - p).sqrMagnitude).Min();
        }

        float minDist = ConstantsManager.MinDistanceBetweenMapElements;
        minDist *= minDist;

        if(closestPlayerDistance > minDist && closestShellDistance > minDist && closestPropDistance > minDist)
        {
            return p;
        }
        else
        {
            return RandomPoint();
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color baseColor = Gizmos.color;
        Gizmos.color = Color.red;

        Vector3 bottomLeft = mapBounds.min;
        Vector3 bottomRight = mapBounds.min;
        bottomRight.x += mapBounds.size.x;
        Vector3 topLeft = mapBounds.max;
        topLeft.x -= mapBounds.size.x;
        Vector3 topRight = mapBounds.max;

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        Gizmos.color = baseColor;
    }
    #endif
}
