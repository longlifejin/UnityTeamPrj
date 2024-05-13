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
    private readonly System.Random _generator; //���� ���� ����

    private readonly Sprite[] _tileSprites;
    private readonly GameObject _gridBG; //Ÿ���� ��ġ�� ĵ����
    private readonly SpriteRenderer[,] _renderer; //�� Ÿ���� �׷��� ǥ���� ���� SpriteRenderer ������Ʈ�� �����ϴ� 2���� �迭
    private readonly int[,] _value; //���� �׸����� Ÿ�� ���� �����ϴ� 2���� �迭
    private int[,] _lastValue; //�������� ������Ʈ�� Ÿ�� �� (Ÿ�� ���� ����Ǿ����� üũ�Ϸ��� ���� ����)
    public readonly int _size; //����(����)�� ũ�⸦ ��Ÿ���� ����

    public int maxValue;
    public int emptyCount;

    public bool isGridFull = false;

    private CardDeck cardDeck;

    private void InitTile(int x, int y) //������ ��ġ�� �� Ÿ���� �ʱ�ȭ�ϰ� ����
    {
        _value[y, x] = Empty;
        var newTile = new GameObject("Tile[" + y + "," + x + "]");
        newTile.transform.parent = _gridBG.transform; //gridCanvas�� �ڽ� ������Ʈ�� �ֱ�
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
                if (_value[y, x] != Empty && _lastValue[y, x] != _value[y, x]) //Ÿ�� ���� ������� �ʰ� ����Ǿ��ٸ�
                {
                    _renderer[y, x].sprite = _tileSprites[_value[y, x]]; //sprite�� ������Ʈ ���ֱ�
                }
            }
        }
        _lastValue = (int[,])_value.Clone(); //������ ���� ���� ���� �����ؼ� �Ҵ��Ͽ� ��������

    }

    public bool Full()  //���� ���尡 ���� á���� �ƴ��� Ȯ���ϴ� bool ��ȯ�� �޼ҵ�
    {
        return _value.Cast<int>().All(x => x != Empty); //_value �迭�� ��ȸ�ϸ鼭 �� ĭ�� �ִ��� �˻�
    }

    private void AddRandom() //������ ��ġ�� ���ο� Ÿ���� �߰�
    {
        if (Full()) //���� á����
        {
            return;
        }

        int x, y;
        do
        {
            x = _generator.Next() % _size; //0,1,2,3 �� �ϳ� ��ȯ
            y = _generator.Next() % _size;
        } while (_value[y, x] != Empty); //��ĭ�� �ƴϸ� �ٽ� �ݺ�

        if (cardDeck.playerDeck != null && cardDeck.playerDeck.Count > 0)
        {
            var random = UnityEngine.Random.Range(0, cardDeck.playerDeck.Count);
            _value[y, x] = cardDeck.playerDeck[random];
            cardDeck.playerDeck.RemoveAt(random);
        }
        else
        {
            _value[y, x] = _generator.Next() % 2; //�� ĭ�� 0 Ȥ�� 1 �� �Ҵ� 
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
            result |= MoveBuffer(buffer); //Ÿ���� �� �������� �о� �ִ´�. �̵��� ������ true ����
            result |= MergeBuffer(buffer); //������ ������ Ÿ���� �����Ѵ�. ������ �̷������ true ����
            result |= MoveBuffer(buffer); //���� �� ���� �� ������ �ٽ� �о� �־� ����, �������� ������ true ����
            //�� 3���� ���� �� �ϳ��� true�� ������ result�� true�� �ȴ�.
            SaveBuffer(buffer, i, direction);
        }
        
        return result; //������ �Ͽ����� true����, ���������� �ʰ� ���յ� �������� false ����
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
