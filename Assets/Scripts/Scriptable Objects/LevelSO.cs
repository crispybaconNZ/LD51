using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Scriptable Objects/New Level")]
public class LevelSO : ScriptableObject {
    public string levelName;
    public int level;
    public List<EnemySO> spawnRoster;    // list of things the crystal can spawn on this level
    public EnemySO boss;

    public Sprite backgroundImage;

    public LevelSO nextLevel;   // null means final level

    public bool isBossLevel() {
        return boss != null;
    }
}
