# Porting tools

Scripts used while porting the Java-to-VB converter output in `unported/` to `src/`. They are not part
of the library build.

## Op class conversion (phase 2)

`convert_ops.py` does the mechanical part of porting an ND4J op class:
- renames namespaces and imports (`org.nd4j` to `org.nd4vb`, aliases `Nd4vbT`, `ShapeT`, `TransformsT`)
- puts the SameDiff, model-import and FlatBuffers members inside the `#If SAMEDIFF`, `#If MODEL_IMPORT`
  and `#If FLATBUFFERS` fences of [docs/PHASE2_DESIGN.md](../../docs/PHASE2_DESIGN.md)
- renames the fields that clash with methods (`xField`, `dimensionsField`, ...)
- writes out Lombok's parameterless constructors
- prints REVIEW lines for whatever still needs a hand

It ported the 173 legacy op classes. `custom_files.txt` lists the 403 custom op classes (`DynamicCustomOp`
subclasses) still in `unported/`.

```
python tools/porting/run_convert.py tools/porting/custom_files.txt --report report.txt
```

- `run_convert.py` takes each listed file's converter text from git `HEAD` (its `unported/` path), writes it
  to the mirrored path under `src/DeepLearning4VB/org/nd4vb/` and converts it there.
- `--out DIR` makes it a dry run that writes under `DIR` instead.
- Files listed in `hand_edited.txt` are never overwritten. Add a file there after fixing it by hand.
- After conversion, check the unfenced code by hand for bare `dimensions`/`keepDims`. With `Option Strict Off`
  a misuse can compile silently.
