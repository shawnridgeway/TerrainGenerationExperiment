using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MeshGenerator {
	GameObject Process(Chunk chunk, int levelOfDetail);
}
