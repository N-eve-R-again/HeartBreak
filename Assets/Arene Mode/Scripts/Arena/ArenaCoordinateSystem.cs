using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ArenaCoordinateSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ArenaRing[] arenaRings;
    public static ArenaCoordinateSystem instance;
    [SerializeField] private Transform center;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(instance == null) { instance = this; }
    }

    private int2 ClampTargetPos(int2 _targetPos)
    {
        int2 result = _targetPos;
        result.y = ClampTargetRing(result.y);
        result.x = ClampXByRing(result);
        return result;
    }
    private int ClampTargetRing(int _targetRing)
    {
        return Mathf.Clamp(_targetRing, 0, GetMaxRingIndex());
    }

    private int ClampXByRing(int2 _targetPos)
    {
        int _xClamp = GetArenaRing(_targetPos.y).ClampX(_targetPos.x);
        return _xClamp;
    }

    private bool IsTransitionValid(int2 _origin, bool _toInner)
    {
        return GetArenaRing(_origin.y).GetTransition(_toInner).SolveValidMove(_origin.x);
    }

    private int SolveNewX(int2 _origin, bool _toInner)
    {
        return GetArenaRing(_origin.y).GetTransition(_toInner).SolveNewX(_origin.x);
    }

    private bool IsTargetRingValid(int _targetRing)
    {
        if(_targetRing < 0) return false;
        if(_targetRing > GetMaxRingIndex()) return false;
        return true;
    }
    private int GetMaxRingIndex()
    {
        return arenaRings.Length - 1;
    }

    private bool GetTransitionComplexity(int _currentRing, bool _toInner)
    {
        
        return GetArenaRing(_currentRing).GetTransition(_toInner).IsComplex();
    }

    private ArenaRing GetArenaRing(int index)
    {
        return arenaRings[index];
    }

    public Vector3 GetPositionInWorld(int2 _position)
    {
        if (_position.y < 0 || _position.y > GetMaxRingIndex()) return Vector3.zero;

        return GetArenaRing(_position.y).GetSpacePos(_position.x);
    }

    public Vector3 GetCaseAxisInWorld(int2 _position, int2 _dir)
    {
        if (_position.y < 0 || _position.y > GetMaxRingIndex()) return Vector3.zero;

        return GetArenaRing(_position.y).GetSpaceAxis(_position.x, _dir);
    }

    public void SolveMove(ref MoveData _data)
    {
        int2 origin = _data.origin; //acces facile à l'origine
        int2 direction = _data.direction; // acces facile à la direction
        int2 orientation;
        //Setup des valeurs finales
        bool isValid = true;

        int targetRing = origin.y + direction.y;

        if (targetRing != origin.y) //si on detecte un changement de ring
        {
            
            bool toInner = ToInner(direction.y); //est ce que on va vers l'interieur ou l'exterieur de l'arène

            isValid = IsTransitionValid(origin, toInner) && IsTargetRingValid(targetRing); //Et ce que la transition est valide et le ring de direction aussi

            if (!isValid)//si le changement n'est pas valide
            {
                _data.Finalize(isValid, origin);
                return;
            }

            int ajustedOriginX = SolveNewX(origin, toInner);//le ring a changé, il faut ajuster la position x d'origine
            origin.x = ajustedOriginX;
        }

        int targetX = origin.x + direction.x;//bouger sur l'axe x

        int2 targetPos = new int2(targetX, targetRing); //int de la position finale


        targetPos = ClampTargetPos(targetPos); //clamp pour que la position soit valide

        isValid = GetAvailability(targetPos) && IsTargetRingValid(targetRing); //verification de la validité finale du move
        Vector3 toDir = GetPositionInWorld(targetPos) - GetPositionInWorld(_data.origin);
        toDir.Normalize();
        _data.orientation = toDir;
        _data.Finalize(isValid, targetPos); //finaliser le data

    }

    public bool IsValidSpace(int2 _wantedPosition)
    {
        return GetAvailability(_wantedPosition);
    }

    public bool IsMoveComplex(int _currentRing, int _directionY)
    {
        if (_directionY == 0) return false;
        return GetTransitionComplexity(_currentRing,ToInner(_directionY));
    }
    private bool GetAvailability(int2 _position)
    {
        return arenaRings[_position.y].IsSpaceAvailable(_position.x);
    }

    public static bool ToInner(int _directionY)
    {
        return (_directionY < 0);
    }
    public float DirToAngle(int2 _position, Vector3 _orientation)
    {


        Vector3 forward = GetCaseAxisInWorld(_position, new int2(0,-1));

        //Vector3 lookaxis = GetCaseAxisInWorld(_position, _direction);
        Debug.DrawRay(GetPositionInWorld(_position), forward, Color.red, 10f);
        Debug.DrawRay(GetPositionInWorld(_position), _orientation, Color.yellow, 10f);
        return Quaternion.LookRotation(GetPositionInWorld(_position) + _orientation, Vector3.up).eulerAngles.y;
    }

    public void SetWarning(bool _state, int2 _pos)
    {
        GetArenaRing(_pos.y).SetSpaceDanger(_pos.x, _state);
    }

    public void TriggerTeleraph(int2 _pos)
    {
        GetArenaRing(_pos.y).TriggerTelegraph(_pos.x);
    }

    public int GetRingLength(int _ring)
    {
        return GetArenaRing(_ring).GetRingMax();
    }
}

