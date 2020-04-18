using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct MeshLod : IComparable<MeshLod> {
    public static readonly int maxLod = 0;
    public static readonly int minLod = 6;
    private readonly int value;

    public MeshLod(int value) {
        if (value < maxLod || value > minLod) {
            throw new Exception("Mesh level of detail (lod) must be between " + maxLod + " and " + minLod + ".");
        }
        this.value = value;
    }

    public int GetInterval() {
        return value == 0 ? 1 : value * 2; // 1,2,4,6,8,10,12
    }

    public int CompareTo(MeshLod other) {
        return value.CompareTo(other);
    }

    public static implicit operator int(MeshLod meshLod) => meshLod.value;
}
