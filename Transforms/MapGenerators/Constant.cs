using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : TerrainTransform {
    private readonly ConstantOptions options;

    public Constant(ConstantOptions options) {
        this.options = options;
    }

    protected override float Evaluate(Point point) {
        return options.constant;
    }

    public override TerrainInformation GetTerrainInformation() {
        return new TerrainInformation(options.constant, options.constant);
    }
}

public class ConstantOptions {
    public readonly float constant;

    public ConstantOptions(float constant = 0) {
        this.constant = constant;
    }
}