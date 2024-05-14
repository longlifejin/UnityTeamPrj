using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Grid
{
    private const int Empty = -1;
    private readonly System.Random _generator; //랜덤 숫자 생성

    private readonly Sprite[] _tileSprites;
    private readonly GameObject _gridBG; //타일이 위치할 캔버스
    private readonly SpriteRenderer[,] _renderer; //각 타일의 그래픽 표현을 위한 SpriteRenderer 컴포넌트를 저장하는 2차원 배열
    private readonly int[,] _value; //현재 그리드의 타일 값을 저장하는 2차원 배열
    private int[,] _lastValue; //마지막에 업데이트된 타일 값 (타일 값이 변경되었는지 체크하려고 만든 변수)
    public readonly int _size; //격자(보드)의 크기를 나타내는 변수

    public int maxValue;
    public int emptyCount;

    public bool isGridFull = false;

    private CardDeck cardDeck;

    private void InitTile(int x, int y) //지정된 위치에 새 타일을 초기화하고 설정
    {
        _value[y, x] = Empty;
        var newTile = new GameObject("Tile[" + y + "," + x + "]");
        newTile.transform.parent = _gridBG.transform; //gridCanvas의 자식 오브젝트로 넣기
        newTile.transform.position = new Vector3(x, y, 1f);
        _renderer[y, x] = newTile.AddComponent<SpriteRenderer>();
        _renderer[y, x].sprite = _tileSprites[0];
        _renderer[y, x].enabled = false;
    }

    public void Update()
    {
        for (var y = 0; y < _size; y++)
        {
            for (var x = 0; x < _size; x++)
            {
                _renderer[y, x].enabled = _value[y, x] != Empty;
                if (_value[y, x] != Empty && _lastValue[y, x] != _value[y, x]) //타일 값이 비어있지 않고 변경되었다면
                {
                    _renderer[y, x].sprite = _tileSprites[_value[y, x]]; //sprite를 업데이트 해주기
                }
            }
        }
        _lastValue = (int[,])_value.Clone(); //마지막 값을 현재 값을 복사해서 할당하여 변경해줌

    }

    public bool Full()  //게임 보드가 가득 찼는지 아닌지 확인하는 bool 반환형 메소드
    {
        return _value.Cast<int>().All(x => x != Empty); //_value 배열을 순회하면서 빈 칸이 있는지 검사
    }

    private void AddRandom() //랜덤한 위치에 새로운 타일을 추가
    {
        if (Full()) //가득 찼으면
        {
            return;
        }

        int x, y;
        do
        {
            x = _generator.Next() % _size; //0,1,2,3 중 하나 반환
            y = _generator.Next() % _size;
        } while (_value[y, x] != Empty); //빈칸이 아니면 다시 반복

        if (cardDeck.playerDeck != null && cardDeck.playerDeck.Count > 0)
        {
            var random = UnityEngine.Random.Range(0, cardDeck.playerDeck.Count);
            _value[y, x] = cardDeck.playerDeck[random];
            cardDeck.playerDeck.RemoveAt(random);
        }
        else
        {
            _value[y, x] = _generator.Next() % 2; //빈 칸에 0 혹은 1 을 할당 
        }

        maxValue = GetMaxValue();
        emptyCount = GetEmptyTile();
        if (emptyCount == 0)
        {
            isGridFull = true;
        }
    }

    public Grid(int size, Sprite[] tileSprites, GameObject gridBG)
    {
        _generator = new System.Random();
        _tileSprites = tileSprites;
        _size = size;
        _value = new int[size, size];
        _renderer = new SpriteRenderer[size, size];
        _gridBG = gridBG;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                InitTile(y, x);
            }
        }
        Reset();
    }

    private static bool MoveBuffer([NotNull] IList<int> buffer)
    {
        if (buffer == null) throw new ArgumentNullException("buffer");
        var result = false;
        var index = 0;
        for (var i = 0; i < buffer.Count; i++)
        {
            if (buffer[i] == Empty) continue;
            if (index != i)
            {
                result = true;
                buffer[index] = buffer[i];
                buffer[i] = Empty;
            }
            index++;
        }
        return result;
    }

    private static bool MergeBuffer(IList<int> buffer)
    {
        var result = false;
        for (var i = 0; i < buffer.Count - 1; i++)
        {
            if (buffer[i] == Empty || buffer[i] != buffer[i + 1]) continue;
            result = true;
            buffer[i]++;
            buffer[i + 1] = Empty;
        }
        return result;
    }

    private void GetBuffer(int[] buffer, int x, Direction direction)
    {
        var vertical = direction.Vertical();
        for (var i = 0; i < _size; i++)
        {
            buffer[i] = vertical ? _value[i, x] : _value[x, i];
        }
        if (direction.Reversed())
        {
            Array.Reverse(buffer);
        }
    }

    private void SaveBuffer(int[] buffer, int x, Direction direction)
    {
        if (direction.Reversed())
        {
            Array.Reverse(buffer);
        }
        var vertical = direction.Vertical();
        for (var i = 0; i < _size; i++)
        {
            if (vertical)
            {
                _value[i, x] = buffer[i];
            }
            else
            {
                _value[x, i] = buffer[i];
            }
        }
    }

    private bool Move(Direction direction)
    {
        var result = false;
        var buffer = new int[_size];
        for (var i = 0; i < _size; i++)
        {
            GetBuffer(buffer, i, direction);
            result |= MoveBuffer(buffer); //타일을 한 방향으로 밀어 넣는다. 이동이 있으면 true 리턴
            result |= MergeBuffer(buffer); //인접한 동일한 타일을 병합한다. 병합이 이루어지면 true 리턴
            result |= MoveBuffer(buffer); //병합 후 남은 빈 공간을 다시 밀어 넣어 정렬, 움직임이 있으면 true 리턴
            //위 3번의 동작 중 하나라도 true가 있으면 result는 true가 된다.
            SaveBuffer(buffer, i, direction);
        }
        
        return result; //동작을 하였으면 true리턴, 움직이지도 않고 병합도 없었으면 false 리턴
    }

    public void MakeMove(Direction direction)
    {
        if (Move(direction))
        {
            AddRandom();
        }
    }

    public void Reset()
    {
        cardDeck = new CardDeck();
        for (var y = 0; y < _size; y++)
        {
            for (var x = 0; x < _size; x++)
            {
                _value[y, x] = Empty;
            }
        }
        _lastValue = (int[,])_value.Clone();
        AddRandom();
        AddRandom();
    }

    //private bool GameOver()
    //{
    //    for (var x1 = 0; x1 < _size - 1; x1++)
    //    {
    //        for (var y1 = 0; y1 < _size - 1; y1++)
    //        {
    //            if (_value[y1, x1] == _value[y1, x1 + 1] ||
    //                _value[y1, x1] == _value[y1 + 1, x1])
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}

    private int GetMaxValue()
    {
        int max = (int)Math.Pow(2, (_value.Cast<int>().Max() + 1));
        return max;
    }

    private int GetEmptyTile()
    {
        int count = 0;
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                if (_value[y, x] == Empty)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
