using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    private readonly Sprite[] _tileSprites;
    private readonly SpriteRenderer[,] _renderer;
    private readonly int[,] _value;

    private int deckCount = 10;

    public List<int> playerDeck = new List<int>
        {
            7,
            8,
            9,
            10,
            7,
            7,
            7,
            7,
            7,
            7
        };
}
