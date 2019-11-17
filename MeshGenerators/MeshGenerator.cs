using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MeshGenerator {
	Mesh Process(Chunk chunk, int levelOfDetail);
}
